using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using GeoAPI.Geometries;
using JiHeJiSuanTool;
using Topology.Geometries;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBColor = Autodesk.AutoCAD.Colors.Color;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using GeoCoordinate = GeoAPI.Geometries.Coordinate;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using NtsILinearRing = GeoAPI.Geometries.ILinearRing;
using NtsLine = NetTopologySuite.Geometries.LineString;
using NtsPoint = NetTopologySuite.Geometries.Point;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;
using NtsRing = NetTopologySuite.Geometries.LinearRing;

namespace NCZJDDJFZ.Tools
{
    /// <summary>
    /// 分析多段线到点的关系，求出最近点，
    /// </summary>
    class PointFX
    {
        private Point2d nNewPoint = new Point2d();
        private int nXH = -1;
        private bool nIsBianJie = true;
        private double nPointTOMinDist = 0;
        private bool nIsCH = false;//此点和多段线上的点重合
        /// <summary>
        /// 分析多段线到点的关系
        /// </summary>
        /// <param name="Buffcoordinates">多段线</param>
        /// <param name="dd">点</param>
        public PointFX(ACDBPolyline pline, Point2d dd)
        {
            double L1p1dis = 999999.00;//被割下的水沟线段的起点到缓冲区线段中的距离（求最小值）
            for (int p = 0; p < pline.NumberOfVertices; p++)
            {
                int pp = p + 1;
                if (pline.Closed == true && p == pline.NumberOfVertices - 1)
                {
                    pp = 0;
                }
                if (pline.Closed == false && p == pline.NumberOfVertices - 1)
                {
                    break;
                }
                Point2d dd1 = pline.GetPoint2dAt(p);//离缓冲区线段最近的那条线的端点
                Point2d dd2 = pline.GetPoint2dAt(pp);//离缓冲区线段最近的那条线的端点
                if (IsOnline(dd, dd1, dd2))
                {
                    nNewPoint = dd;
                    nXH = p;
                    nIsBianJie = false;//点在缓冲区边线上
                    if (dd.GetDistanceTo(dd1) < 0.00001 || dd.GetDistanceTo(dd2) < 0.00001)
                    {
                        nIsCH = true;
                    }
                    break;
                }
                else
                {

                    double angle = Tools.CADTools.GetAngle(dd1, dd2);
                    Point2d Newdd1 = Tools.CADTools.GetNewPoint(dd, 500.0, angle + Math.PI / 2.00);
                    Point2d Newdd2 = Tools.CADTools.GetNewPoint(dd, -500.0, angle + Math.PI / 2.00);
                    PanDuanL1AndL2GuanXi jd = new PanDuanL1AndL2GuanXi(new MyPoint(Newdd1.X, Newdd1.Y), new MyPoint(Newdd2.X, Newdd2.Y),
                                                                       new MyPoint(dd1.X, dd1.Y), new MyPoint(dd2.X, dd2.Y));
                    Point2d JsPoint1 = new Point2d(jd.JiaoDian1.X, jd.JiaoDian1.Y);
                    if (jd.JiaodianGeShu == 1)
                    {
                        double s1 = JsPoint1.GetDistanceTo(dd);
                        if (s1 < L1p1dis)
                        {
                            L1p1dis = s1;
                            nXH = p;
                            //nIsduandian = false;
                            nNewPoint = JsPoint1;
                        }
                    }
                    else
                    {
                        double s1 = dd.GetDistanceTo(dd1);
                        double s2 = dd.GetDistanceTo(dd2);
                        if (s1 < s2)
                        {
                            if (s1 < L1p1dis)
                            {
                                L1p1dis = s1;
                                nXH = p;
                                //nIsduandian = true;
                                nNewPoint = dd1;

                            }
                        }
                        else
                        {
                            if (s2 < L1p1dis)
                            {
                                L1p1dis = s2;
                                nXH = p;
                                //nIsduandian = true;
                                nNewPoint = dd2;

                            }
                        }
                    }
                }
            }
            nPointTOMinDist = dd.GetDistanceTo(nNewPoint);
        }
        /// <summary>
        /// 如果此点在多段线上则返回自身，如果不在多段线上则返回到此点到多段线边界最近点的坐标
        /// </summary>
        public Point2d NewPoint
        {
            get { return this.nNewPoint; }
        }
        /// <summary>
        /// 所在多段线边界上的线段的序号
        /// </summary>
        public int XH
        {
            get { return this.nXH; }
        }
        /// <summary>
        /// 此点是否和多段线上的点重合
        /// </summary>
        public bool IsCH
        {
            get { return this.nIsCH; }
        }
        /// <summary>
        /// 返回true表示点在多段线上，false表示点不在多段线上
        /// </summary>
        public bool OnBianJie
        {
            get { return !this.nIsBianJie; }
        }
        /// <summary>
        /// 点到多边形的最近距离
        /// </summary>
        public double PointTOMinDist
        {
            get { return this.nPointTOMinDist; }
        }
        private bool IsOnline(Point2d D, Point2d D1, Point2d D2)
        {
            double Ds1 = D.GetDistanceTo(D1);
            double Ds2 = D.GetDistanceTo(D2);
            double DiBianChang = D1.GetDistanceTo(D2);
            double Mj = Math.Abs(((D1.X - D.X) * (D2.Y - D.Y) - (D2.X - D.X) * (D1.Y - D.Y)) * 0.5);//面积
            double s3 = Ds1 + Ds2 - DiBianChang;
            JiheTool jh = new JiheTool();
            if (jh.MyEqual(Mj, 0d, 6) && jh.MyEqual(s3, 0d, 6))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 分析多段线到点的关系，求出最近点，
    /// </summary>
    class PointFX_FW
    {
        private Point2d nNewPoint = new Point2d();
        private int nXH = -1;
        private bool nIsBianJie = false ;
        private double nPointTOMinDist = 99999999999;
        private bool nIsCH = false;//此点和多段线上的点重合
        private bool nIsCZ_BianJie = false;//此点的垂足和多段线重合
        private bool nIsCZ_BianJie_DD = false;//此点的垂足到端点的距离是否小于容差，如果是，则返回垂足点在多段线端点上，如果大于容差，则返回垂足点
        private bool nDDCH = false;//仅仅是端点在容差内,重合
        /// <summary>
        /// 分析多段线到点的关系
        /// </summary>
        /// <param name="Buffcoordinates">多段线</param>
        /// <param name="dd">点</param>
        public PointFX_FW( ACDBPolyline pline, Point2d dd,double rc)
        {
            double L1p1dis = 999999999.00;//被割下的水沟线段的起点到缓冲区线段中的距离（求最小值）
            for (int p = 0; p < pline.NumberOfVertices ; p++)
            {
                int pp = p+1;
                if (pline.Closed == true && p==pline.NumberOfVertices-1)
                {
                    pp = 0;
                }
                if (pline.Closed == false && p == pline.NumberOfVertices - 1)
                {
                    break;
                }
                Point2d dd1 = pline.GetPoint2dAt(p);//离缓冲区线段最近的那条线的端点
                Point2d dd2 = pline.GetPoint2dAt(pp);//离缓冲区线段最近的那条线的端点
                double ss1 = dd.GetDistanceTo(dd1);
                double ss2 = dd.GetDistanceTo(dd2);
                if (ss1 < 0.00001 || ss2 < 0.00001)
                {
                    nNewPoint = dd;
                    nPointTOMinDist = 0;
                    nXH = p;
                    nIsCH = true;
                    break;
                }
                else if (ss1 < rc || ss2 < rc)
                {
                    if (ss1 < ss2)
                    {
                        nNewPoint = dd1;
                    }
                    else
                    {
                        nNewPoint = dd2;
                    }
                    nPointTOMinDist = 0;
                    nXH = p;
                    nDDCH = true;
                    break;
                }
                else if (IsOnline(dd, dd1, dd2))
                {
                    nNewPoint = dd;
                    nPointTOMinDist = dd.GetDistanceTo(nNewPoint);
                    nXH = p;
                    nIsBianJie = true;//此点和多段线重合
                    break;
                }
                else  
                {
                    double angle = Tools.CADTools.GetAngle(dd1, dd2);
                    Point2d Newdd1 = Tools.CADTools.GetNewPoint(dd, 500.0, angle + Math.PI / 2.00);
                    Point2d Newdd2 = Tools.CADTools.GetNewPoint(dd, -500.0, angle + Math.PI / 2.00);
                    PanDuanL1AndL2GuanXi jd = new PanDuanL1AndL2GuanXi(new MyPoint(Newdd1.X, Newdd1.Y), new MyPoint(Newdd2.X, Newdd2.Y),
                                                                       new MyPoint(dd1.X, dd1.Y), new MyPoint(dd2.X, dd2.Y));
                    Point2d JsPoint1 = new Point2d(jd.JiaoDian1.X, jd.JiaoDian1.Y);
                    if (jd.JiaodianGeShu == 1)
                    {
                        double s1 = JsPoint1.GetDistanceTo(dd);
                        if (s1 < L1p1dis)
                        {
                            L1p1dis = s1;
                            nPointTOMinDist = s1;
                            nXH = p;
                            nNewPoint = JsPoint1;
                            double dis1 = nNewPoint.GetDistanceTo(dd1);
                            double dis2 = nNewPoint.GetDistanceTo(dd2);

                            if (dis1 < rc)
                            {
                                nNewPoint = dd1;
                                nIsCZ_BianJie_DD = true;
                            }
                            if (dis2 < rc)
                            {
                                nNewPoint = dd2;
                                nIsCZ_BianJie_DD = true;
                            }
                             nIsCZ_BianJie = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 如果此点在多段线上则返回自身，如果不在多段线上则返回到此点到多段线边界最近点的坐标
        /// </summary>
        public Point2d NewPoint
        {
            get { return this.nNewPoint; }
        }
        /// <summary>
        /// 所在多段线边界上的线段的序号
        /// </summary>
        public int XH
        {
            get { return this.nXH; }
        }
        /// <summary>
        /// 此点是否和多段线上的点重合
        /// </summary>
        public bool IsCH
        {
            get { return this.nIsCH; }
        }
        /// <summary>
        /// 返回true表示点在多段线上，不在端点 
        /// </summary>
        public bool  OnBianJie
        {
            get { return  nIsBianJie; }
        }
        /// <summary>
        /// 点到多边形的最近距离
        /// </summary>
        public double PointTOMinDist
        {
            get { return this.nPointTOMinDist; }
        }
        /// <summary>
        /// 此点的垂足是否在边界上
        /// </summary>
        public bool point_CZ_OnBianJie
        {
            get { return this.nIsCZ_BianJie; }
        }
        /// <summary>
        /// 此点的垂足到端点的距离是否小于容差，如果是，则返回垂足点是多段线的顶点
        /// </summary>
        public bool point_CZ_OnBianJie_DD
        {
            get { return this.nIsCZ_BianJie_DD; }
        }
        /// <summary>
        /// 此点到端点的距离小于容差
        /// </summary>
        public bool point_DDCH
        {
            get { return this.nDDCH; }
        }
        private bool IsOnline(Point2d D, Point2d D1, Point2d D2)
        {
            double Ds1 = D.GetDistanceTo(D1);
            double Ds2 = D.GetDistanceTo(D2);
            double DiBianChang = D1.GetDistanceTo(D2);
            double Mj = Math.Abs(((D1.X - D.X) * (D2.Y - D.Y) - (D2.X - D.X) * (D1.Y - D.Y)) * 0.5);//面积
            double s3 = Ds1 + Ds2 - DiBianChang;
            JiheTool jh = new JiheTool();
            if (jh.MyEqual(Mj, 0d, 6) && jh.MyEqual(s3, 0d, 6))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class CADTools
    {
        /// <summary>
        /// 获取存放不同测区的报表的文件夹
        /// </summary>
        /// <returns>存放报表的文件夹</returns>
        public static string GetReportsFolder()
        {
            string AppPath = GetAppPath();
            string cq = Tools.ReadWriteReg.read_reg("测区名称");
            if (AppPath == null || cq == null)
            {
                MessageBox.Show("获取报表存放路径出现错误!");
                return string.Empty;
            }
            else
            {
                return AppPath + "报表\\" + cq + "\\";
            }
            
        }

        /// <summary>
        /// 获取存放不同测区程序配置的文件夹
        /// </summary>
        /// <returns>存放程序配置的文件夹</returns>
        public static string GetSettingsFolder()
        {
            string AppPath = GetAppPath();
            string cq = Tools.ReadWriteReg.read_reg("测区名称");
            if (AppPath == null || cq == null)
            {
                MessageBox.Show("获取程序配置文件存放路径出现错误!");
                return string.Empty;
            }
            else
            {
                return AppPath + "配置\\" + cq + "\\";
            }

        }

        /// <summary>获取安置时的路径
        /// 获取安置时的路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppPath()
        {
            string path = "";
            Microsoft.Win32.RegistryKey rsg = null;
            rsg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\不动产调查基础应用软件");
            if (rsg != null)
            {
                path = rsg.GetValue("BDCDCJIYYRJ", "").ToString();
                rsg.Close();
                // MessageBox.Show(path);
            }
            else
            {
                MessageBox.Show("没有找到安装路径!");
            }
            return path + "\\";
        }
        /// <summary>求指定应用程序名称的扩展属性集合
        /// 求指定应用程序名称的扩展属性集合
        /// </summary>
        /// <param name="res">从对象中获取的所有扩展属性集合</param>
        /// <param name="appName">应用程序名称</param>
        /// <returns>仅包含该扩展属性名称的数据集合</returns>
        public static TypedValue[] GetApp(TypedValue[] res, string appName)
        {
            if (res == null) { return null; }
            int qs = 0;//从哪一行起
            int nn = 0;//总共多少行
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i].TypeCode == 1001 && res[i].Value.ToString().ToUpper() == appName.ToUpper())
                {
                    qs = i;
                    break;
                }
            }
            for (int i = qs; i < res.Length; i++)
            {
                if (res[i].TypeCode == 1001 && res[i].Value.ToString().ToUpper() != appName.ToUpper())
                {
                    break;
                }
                nn++;
            }
            if (nn > 0)
            {
                int n = 0;
                TypedValue[] resr = new TypedValue[nn];
                for (int i = qs; i < qs + nn; i++)
                {
                    resr[n] = res[i];
                    n++;
                }
                return resr;
            }
            else
            {
                return null;
            }
        }
        /// <summary> 打开、解冻、解琐所有图层
        /// 打开、解冻、解琐所有图层
        /// </summary>
        public static  void OnAllLayer()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    LayerTable Layertable = (LayerTable)trans.GetObject(db.LayerTableId, AcadOpenMode.ForWrite);
                    foreach (ObjectId layerid in Layertable)
                    {

                        LayerTableRecord LayertableRecord = (LayerTableRecord)trans.GetObject(layerid, AcadOpenMode.ForWrite);//层记录表
                        LayertableRecord.IsOff = false;
                        LayertableRecord.IsLocked = false;
                        if (LayertableRecord.IsFrozen)
                        {
                            LayertableRecord.IsFrozen = false;
                        }
                    }
                }
                finally
                {
                    trans.Commit();
                    trans.Dispose();
                }
            }

        }

        /// <summary>计算方位角
        /// 计算方位角
        /// </summary>
        /// <param name="DX"></param>
        /// <param name="DY"></param>
        /// <returns></returns>
        public static double FWJ(double DX, double DY)
        {
            double t = 0;
            if (DX == 0.0)
            {
                t = Math.Sign(DX) * Math.PI / 2.0;
            }
            else
            {
                t = Math.Atan(Math.Abs(DY) / Math.Abs(DX));
                if (DX < 0 && DY >= 0) { t = Math.PI - t; }
                if (DX < 0 && DY < 0) { t = Math.PI + t; }
                if (DX > 0 && DY < 0) { t = Math.PI * 2 - t; }
            }
            return t;
        }
        /// <summary>在给定角度、距离和点坐标的情况下计算新点坐标
        /// 在给定角度、距离和点坐标的情况下计算新点坐标
        /// </summary>
        /// <param name="Pt1">给定的点</param>
        /// <param name="L">给定距离</param>
        /// <param name="Rangle">给定的角度</param>
        /// <returns>新点坐标</returns>
        public static Point3d GetNewPoint(Point3d Pt1, double L, double Rangle)
        {
            Point3d P3D = new Point3d(Pt1.X + Math.Cos(Rangle) * L, Pt1.Y + Math.Sin(Rangle) * L, Pt1.Z);
            return P3D;
        }
        /// <summary>在给定角度、距离和点坐标的情况下计算新点坐标
        /// 在给定角度、距离和点坐标的情况下计算新点坐标
        /// </summary>
        /// <param name="Pt1">给定的点</param>
        /// <param name="L">给定距离</param>
        /// <param name="Rangle">给定的角度</param>
        /// <returns>新点坐标</returns>
        public static Point2d GetNewPoint(Point2d Pt1, double L, double Rangle)
        {
            Point2d P2D = new Point2d(Pt1.X + Math.Cos(Rangle) * L, Pt1.Y + Math.Sin(Rangle) * L);
            return P2D;
        }
        /// <summary>将十六进制的句柄转化为.NET里使用的ObjectId
        /// 将十六进制的句柄转化为.NET里使用的ObjectId
        /// </summary>
        /// <param name="StrHandle">十六进制的句柄</param>
        /// <returns></returns>
        public static  ObjectId HandletoObjectID(String StrHandle)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            //long longhandle = long.Parse(StrHandle, System.Globalization.NumberStyles.HexNumber);
            Autodesk.AutoCAD.DatabaseServices.Handle handle = new Handle(Convert.ToInt64(StrHandle));
            ObjectId ObjectID = db.GetObjectId(false, handle, 0);
            return ObjectID;
        }
        /// <summary>将点转换为TF.net下的Point 
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static  Topology.Geometries.Point ConvertToPoint(double x, double y)
        {
            GeometryFactory geometryFactory = new GeometryFactory();
            Topology.Geometries.Point point = new Topology.Geometries.Point(null, GeometryFactory.Default);
            point = (Topology.Geometries.Point)geometryFactory.CreatePoint(new Topology.Geometries.Coordinate(x, y));
            return point;
        }
        /// <summary>将点转换为TF.net下的Point 
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Topology.Geometries.Point ConvertToPoint(Point2d point2d)
        {
            GeometryFactory geometryFactory = new GeometryFactory();
            Topology.Geometries.Point point = new Topology.Geometries.Point(null, GeometryFactory.Default);
            point = (Topology.Geometries.Point)geometryFactory.CreatePoint(new Topology.Geometries.Coordinate(point2d.X, point2d.Y));
            return point;
        }
        /// <summary>将点转换为TF.net下的Point 
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Topology.Geometries.Point ConvertToPoint(Point3d point3d)
        {
            GeometryFactory geometryFactory = new GeometryFactory();
            Topology.Geometries.Point point = new Topology.Geometries.Point(null, GeometryFactory.Default);
            point = (Topology.Geometries.Point)geometryFactory.CreatePoint(new Topology.Geometries.Coordinate(point3d.X, point3d.Y));
            return point;
        }
        /// <summary>将CAD的多段线转换为TF.net下的 LineString线  Autodesk.AutoCAD.DatabaseServices.Line
        /// 将CAD的多段线转换为TF.net下的 LineString线  Autodesk.AutoCAD.DatabaseServices.Line
        /// </summary>
        /// <param name="polyline">多段线</param>
        /// <returns>ILineString</returns>
        public static Topology.Geometries.ILineString ConvertToLineString(ACDBPolyline polyline)
        {
            CoordinateList points = new CoordinateList();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                points.Add(ReadCoordinate(polyline.GetPoint3dAt(i)));
            }
            if (polyline.Closed == true) { points.Add(ReadCoordinate(polyline.GetPoint3dAt(0))); }
            if (points.Count > 1)
            {
                PrecisionModel pre = new PrecisionModel(Topology.Geometries.PrecisionModels.Floating);
                GeometryFactory geometryFactory = new GeometryFactory(pre);
                LineString lineStringB = new LineString(null, geometryFactory);
                lineStringB = (LineString)geometryFactory.CreateLineString(points.ToCoordinateArray());
                return lineStringB;
            }
            else
            {
                return null;
            }
        }
        public static IList ConvertToIList(ACDBPolyline polyline)
        {
            IList ilist=new ArrayList() ;
            CoordinateList points = new CoordinateList();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                points.Add(ReadCoordinate(polyline.GetPoint3dAt(i)));
            }
            if (polyline.Closed == true) { points.Add(ReadCoordinate(polyline.GetPoint3dAt(0))); }
            
            for (int i = 0; i < points.Count-1; i++)
            {

                PrecisionModel pre = new PrecisionModel(Topology.Geometries.PrecisionModels.Floating);
                GeometryFactory geometryFactory = new GeometryFactory(pre);
                LineString lineStringB = new LineString(null, geometryFactory);
                Topology.Geometries.ICoordinate[] icoor = new Topology.Geometries.ICoordinate[2];
                icoor[0] = points[i];
                icoor[1] = points[i+1];
                lineStringB = (LineString)geometryFactory.CreateLineString(icoor);
                ilist.Add( lineStringB);
            }
            return ilist;
               
        }
        /// <summary>将CAD下的直线写入LineString
        ///将CAD下的直线写入LineString
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Topology.Geometries.ILineString ConvertToLineString(Autodesk.AutoCAD.DatabaseServices.Line line)
        {
            CoordinateList points = new CoordinateList();
            points.Add(ReadCoordinate(line.StartPoint));
            points.Add(ReadCoordinate(line.EndPoint));
            GeometryFactory geometryFactory = new GeometryFactory();
            LineString lineStringB = new LineString(null, GeometryFactory.Default);
            lineStringB = (LineString)geometryFactory.CreateLineString(points.ToCoordinateArray());
            return lineStringB;
        }
        private static Topology.Geometries.ICoordinate ReadCoordinate(Point3d point3d)
        {
            Topology.Geometries.Coordinate coord = new Topology.Geometries.Coordinate((long)(10000.0 * point3d.X + 0.5) / 10000.0, (long)(10000.0 * point3d.Y + 0.5) / 10000.0, 0.0);
             return coord;
        }
        /// <summary>创建一个多边形,多段线必须是封闭的
        /// 创建一个多边形,多段线必须是封闭的
        /// </summary>
        /// <param name="polyline">封闭的多段线</param>
        /// <returns>IPolygon</returns>
        public static Topology.Geometries.IPolygon ConvertToPolygon(ACDBPolyline polyline)
        {
            if (polyline.Closed == false) { return null; }
            GeometryFactory geometryFactory = new GeometryFactory();
            LinearRing linearRing = new LinearRing(null, GeometryFactory.Default);
            CoordinateList points = new CoordinateList();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                //points.Add(ReadCoordinate(polyline.GetPoint3dAt(i)));
                points.Add(new Topology.Geometries.Coordinate((long)(10000.0 * polyline.GetPoint3dAt(i).X + 0.5) / 10000.0, (long)(10000.0 * polyline.GetPoint3dAt(i).Y + 0.5) / 10000.0, 0.0));
            }
            points.Add(ReadCoordinate(polyline.GetPoint3dAt(0)));
            linearRing = (LinearRing)geometryFactory.CreateLinearRing(points.ToCoordinateArray());
            Topology.Geometries.Polygon polygon = new Topology.Geometries.Polygon(linearRing);
            return polygon;
        }
        /// <summary>将topology下的多个多边形写入DWG
        /// 将topology下的多个多边形写入DWG
        /// </summary>
        /// <param name="multiPolygons"></param>
        /// <returns></returns>
        public static ACDBPolyline[] WriteToDwg(Topology.Geometries.MultiPolygon multiPolygons)
        {
            ACDBPolyline[] Polylines = new ACDBPolyline[multiPolygons.Count];
            for (int i = 0; i < multiPolygons.Count; i++)
            {

                Topology.Geometries.Polygon polygon = (Topology.Geometries.Polygon)multiPolygons.GetGeometryN(i);
                Topology.Geometries.ICoordinate[] coordinates = polygon.Coordinates;
                ACDBPolyline polyline = new ACDBPolyline();
                int p = 0;
                foreach (Topology.Geometries.Coordinate coordinate in coordinates)
                {
                    Point2d point = new Point2d(coordinate.X, coordinate.Y);
                    polyline.AddVertexAt(p, point, 0, 0, 0);
                    p++;
                }
                polyline.RemoveVertexAt(coordinates.Length - 1);
                polyline.Closed = true;
                Polylines[i] = polyline;
            }
            return Polylines;
        }
        /// <summary>将topology下的多边形写入DWG
        /// 将topology下的多边形写入DWG
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static ACDBPolyline WriteToDwg(Topology.Geometries.Polygon polygon)
        {
            Topology.Geometries.ICoordinate[] coordinates = polygon.Coordinates;
            ACDBPolyline polyline = new ACDBPolyline();
            int i = 0;
            foreach (Topology.Geometries.Coordinate coordinate in coordinates)
            {
                Point2d point = new Point2d(coordinate.X, coordinate.Y);
                polyline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            polyline.RemoveVertexAt(coordinates.Length - 1);
            polyline.Closed = true;
            return polyline;
        }
        /// <summary>将topology下的多边形写入DWG
        /// 将topology下的多边形写入DWG
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static ACDBPolyline WriteToDwg(Topology.Geometries.LineString lineString)
        {
            Topology.Geometries.ICoordinate[] coordinates = lineString.Coordinates;
            ACDBPolyline polyline = new ACDBPolyline();
            int i = 0;
            foreach (Topology.Geometries.Coordinate coordinate in coordinates)
            {
                Point2d point = new Point2d(coordinate.X, coordinate.Y);
                polyline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            //polyline.RemoveVertexAt(coordinates.Length - 1);
            // polyline.Closed = true;
            return polyline;
        }
        /// <summary>用两点坐标求与X轴的夹角
        /// 用两点坐标求与X轴的夹角
        /// </summary>
        /// <param name="D1">第一个点</param>
        /// <param name="D2">第二个点</param>
        /// <returns>角度</returns>
        public static double GetAngle(Point2d D1, Point2d D2)
        {
            return D1.GetVectorTo(D2).Angle;
        }
        /// <summary>用两点坐标求与X轴的夹角
        /// 用两点坐标求与X轴的夹角
        /// </summary>
        /// <param name="D1">第一个点</param>
        /// <param name="D2">第二个点</param>
        /// <returns>角度</returns>
        public static double GetAngle(Point3d D1, Point3d D2)
        {
            Point2d DD1 = new Point2d(D1[0], D1[1]);
            Point2d DD2 = new Point2d(D2[0], D2[1]);
            return DD1.GetVectorTo(DD2).Angle;
        }
        /// <summary>创建一个新的图层
        /// 创建一个新的图层
        /// </summary>
        /// <param name="LayerNanm">图层名称</param>
        /// <param name="LineTypeNanm">新型名称</param>
        /// <param name="layerColor">图层颜色</param>
        /// <returns>图层ID号</returns>
        public static ObjectId AddNewLayer(string LayerNanm, string LineTypeNanm, short layerColor)
        {
            ObjectId layerid;
            ObjectId LinetypeID;
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    LayerTable CB = (LayerTable)trans.GetObject(db.LayerTableId, AcadOpenMode.ForWrite);
                    LinetypeTable XXB = (LinetypeTable)trans.GetObject(db.LinetypeTableId, AcadOpenMode.ForWrite);
                    if (XXB.Has(LineTypeNanm))
                    {
                        LinetypeID = XXB[LineTypeNanm];
                    }
                    else
                    {
                        LinetypeID = XXB["Continuous"];
                        ed.WriteMessage("\n注意：线型\"" + LineTypeNanm + "\"不存在!按 Continuous 线型处理！");

                    }
                    if (CB.Has(LayerNanm))
                    {
                        layerid = CB[LayerNanm];
                    }
                    else
                    {
                        LayerTableRecord LTR = new LayerTableRecord();
                        LTR.Name = LayerNanm;

                        //LTR.Color = AcadApplication.FromColorIndex(ColorMethod.ByLayer, layerColor);
                        LTR.Color = ACDBColor.FromColorIndex(ColorMethod.ByLayer, layerColor);
                        LTR.LinetypeObjectId = LinetypeID;
                        layerid = CB.Add(LTR);
                        trans.AddNewlyCreatedDBObject(LTR, true);
                    }
                    return layerid;
                }
                finally
                {
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }
        /// <summary>加载一个新的线型
        /// 加载一个新的线型
        /// </summary>
        /// <param name="LinetypeName">线型名称</param>
        /// <param name="LinetypeFileName">线型文件名</param>
        /// <returns>新线型的ID号</returns>
        public static ObjectId AddNewLinetype(string LinetypeName, string LinetypeFileName)
        {
            ObjectId LinetypeID = ObjectId.Null;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LinetypeTable lt = (LinetypeTable)trans.GetObject(db.LinetypeTableId, AcadOpenMode.ForWrite);
                if (lt.Has(LinetypeName))
                {
                    LinetypeID = lt[LinetypeName];
                }
                else
                {
                    try
                    {
                        db.LoadLineTypeFile(LinetypeName, LinetypeFileName);
                    }
                    catch (System.Exception ex)
                    {
                        Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                        ed.WriteMessage(ex.Message);
                        return lt["Continuous"];
                    }
                }
                trans.Commit();
                trans.Dispose();
                return LinetypeID;
            }
        }
        public void zj(string Stext, Point3d point,double zjgd)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Autodesk.AutoCAD.DatabaseServices.DBText text = new Autodesk.AutoCAD.DatabaseServices.DBText();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite, true);
                SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, AcadOpenMode.ForWrite);
                try
                {
                    text.TextStyleId = Tools.CADTools.AddNewTextStyle("幢号", "宋体", 1, 0, false);
                    text.LayerId = Tools.CADTools.AddNewLayer("幢号", "Continuous", 2);
                    text.ColorIndex = 2;
                    text.TextString = Stext;//注记内容;
                    text.Height = zjgd;//注记高度
                    text.Position = point;
                    text.WidthFactor = 0.7;
                    text.HorizontalMode = TextHorizontalMode.TextMid;//水平对齐方式
                    text.AlignmentPoint = point;//必须在mtext.HorizontalMode后
                    btr.AppendEntity(text);
                    trans.AddNewlyCreatedDBObject(text, true);
                    #region 增加扩展属性
                    TypedValue[] tv = new TypedValue[2];
                    tv[0] = new TypedValue(1001, "SOUTH");
                    tv[1] = new TypedValue(1000, "属性注记");//什么纸张
                    RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                    regAppTableRecord.Name = "SOUTH";
                    if (!symbolTable.Has("SOUTH"))
                    {
                        symbolTable.Add(regAppTableRecord);
                        trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                    }
                    if (text.IsWriteEnabled)
                    {
                        text.UpgradeOpen();
                    }
                    text.XData = new ResultBuffer(tv);
                    #endregion
                }
                finally
                {
                    trans.Commit();
                }
            }
        }
        /// <summary>附属性
        /// 附属性
        /// </summary>
        /// <param name="cm">层名</param>
        /// <param name="xx">线型</param>
        /// <param name="xk">线宽</param>
        /// <param name="ys">颜色</param>
        /// <param name="sx">属性</param>
        public void FSX(ObjectId plid, string[] sx,string cm,string xx,double  xk, int   ys,double bg,double  xxbl)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                // double bl = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");
                Database db = HostApplicationServices.WorkingDatabase;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                    ACDBPolyline PLOBJ = (ACDBPolyline)trans.GetObject(plid, AcadOpenMode.ForWrite);//"Continuous", 0.2 * bl, 8
                    
                    //AddNewLinetype(xx, NCZJDDJFZ.ATT.linetypePath);
                    //AddNewLayer(cm, xx, ys);
                    PLOBJ.Layer = cm;
                    PLOBJ.Linetype = xx;
                    PLOBJ.ConstantWidth = xk;
                    PLOBJ.ColorIndex = ys;
                    PLOBJ.Elevation = bg;
                    PLOBJ.LinetypeScale = xxbl;
                    PLOBJ.Plinegen = true;//线型生成
                    PLOBJ.Closed = true;
                    if (sx[0] == "房屋" || sx[0] == "门廊" || sx[0] == "车棚")
                    {
                        if (sx[12] == "1")
                        {
                            string jg = "";
                            if (sx[13] == "钢、钢筋混凝土结构")
                            {
                                jg = "钢砼";
                            }
                            else if (sx[13] == "钢筋混凝土结构")
                            {
                                jg = "砼";
                            }
                            else
                            {
                                jg = sx[13].Substring(0, 1);
                            }
                            string text = "(" + sx[9] + ")" + "\\" + sx[11] + "\\" + jg + "\\" + sx[17].Substring(0, 1);
                            Extents3d point = (Extents3d)PLOBJ.Bounds;
                            Point3d pot = new Point3d((point.MaxPoint.X + point.MinPoint.X) / 2.0, (point.MaxPoint.Y + point.MinPoint.Y) / 2.0, 0);
                            zj(text, pot,1.5);
                            
                        }
                    }
                    Add_FW_SX_XG(plid, sx);
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }
         
        /// <summary>增加房屋对象信息
        /// 增加房屋对象信息
       /// </summary>
       /// <param name="plineid"></param>
       /// <param name="sx"></param>
        public void Add_FW_SX(ObjectId plineid,string[] sx)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                ACDBPolyline pl = (ACDBPolyline)trans.GetObject(plineid, AcadOpenMode.ForWrite);
                SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, AcadOpenMode.ForWrite);

                #region 增加扩展属性
                TypedValue[] tv = new TypedValue[2];
                tv[0] = new TypedValue(1001, "SOUTH");
                tv[1] = new TypedValue(1000, sx[29]);//什么纸张
                RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                regAppTableRecord.Name = "SOUTH";
                if (!symbolTable.Has("SOUTH"))
                {
                    symbolTable.Add(regAppTableRecord);
                    trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                }
                if (!pl.IsWriteEnabled)
                {
                    pl.UpgradeOpen();
                }
                pl.XData = new ResultBuffer(tv);



                
                #endregion




                MapApplication mapApp = HostMapApplicationServices.Application;
                Tables tables = mapApp.ActiveProject.ODTables;
                Autodesk.Gis.Map.ObjectData.Table table;
                Autodesk.Gis.Map.ObjectData.Record record = Autodesk.Gis.Map.ObjectData.Record.Create();
                bool IsxxB = true;
                if (tables.TablesCount == 0) { IsxxB = true; }
                if (tables.TablesCount > 0)
                {
                    StringCollection tbablename = tables.GetTableNames();
                    for (int i = 0; i < tables.TablesCount; i++)
                    {
                        if ("房屋属性" == tbablename[i])
                        {
                            IsxxB = false;
                            break;
                        }
                    }
                }
                if (IsxxB == true) {Creat_FW_SX(); }

                table = tables["房屋属性"];
                table.InitRecord(record);
                Autodesk.Gis.Map.Utilities.MapValue val;
                val = record[0];
                val.Assign(sx[0]);
                val = record[1];
                val.Assign(sx[1]);
                val = record[2];
                val.Assign(sx[2]);
                val = record[3];
                val.Assign(sx[3]);
                val = record[4];
                val.Assign(sx[4]);
                val = record[5];
                val.Assign(sx[5]);
                val = record[6];
                val.Assign(sx[6]);
                val = record[7];
                val.Assign(sx[7]);
                val = record[8];
                val.Assign(sx[8]);
                val = record[9];
                val.Assign(sx[9]);
                val = record[10];
                val.Assign(sx[10]);
                val = record[11];
                val.Assign(sx[11]);
                val = record[12];
                val.Assign(sx[12]);
                val = record[13];
                val.Assign(sx[13]);
                val = record[14];
                val.Assign(sx[14]);
                val = record[15];
                val.Assign(sx[15]);
                val = record[16];
                val.Assign(sx[16]);
                val = record[17];
                val.Assign(sx[17]);
                val = record[18];
                val.Assign(sx[18]);
                val = record[19];
                val.Assign(sx[19]);
                val = record[20];
                val.Assign(sx[20]);
                val = record[21];
                val.Assign(sx[21]);
                val = record[22];
                val.Assign(sx[22]);
                val = record[23];
                val.Assign(sx[23]);
                val = record[24];
                val.Assign(sx[24]);
                val = record[25];
                val.Assign(sx[25]);
                val = record[26];
                val.Assign(sx[26]);
                val = record[27];
                val.Assign(sx[27]);
                val = record[28];
                val.Assign(sx[28]);
                record.Dispose();
                //records.UpdateRecord(record);
                table.AddRecord(record, pl);
                trans.Commit();
                trans.Dispose();
                //trans.Clone();

                regAppTableRecord.Dispose();
            }
        }
        /// <summary>增加、修改房屋对象信息
        /// 增加、修改房屋对象信息
        /// </summary>
        /// <param name="plineid"></param>
        /// <param name="sx"></param>
        public void Add_FW_SX_XG(ObjectId plineid, string[] sx)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                ACDBPolyline pl = (ACDBPolyline)trans.GetObject(plineid, AcadOpenMode.ForWrite);
                SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, AcadOpenMode.ForWrite);
                #region 增加扩展属性
                TypedValue[] tv = new TypedValue[2];
                tv[0] = new TypedValue(1001, "SOUTH");
                tv[1] = new TypedValue(1000, sx[29]); 
                RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                regAppTableRecord.Name = "SOUTH";
                if (!symbolTable.Has("SOUTH"))
                {
                    symbolTable.Add(regAppTableRecord);
                    trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                }
                if (!pl.IsWriteEnabled)
                {
                    pl.UpgradeOpen();
                }
                pl.XData = new ResultBuffer(tv);
                #endregion
                #region 增加扩展属性2
                TypedValue[] tv2 = new TypedValue[2];
                tv2[0] = new TypedValue(1001, "JZWSX");
                tv2[1] = new TypedValue(1000, "JZWSX");
                RegAppTableRecord regAppTableRecord2 = new RegAppTableRecord();
                regAppTableRecord2.Name = "JZWSX";
                if (!symbolTable.Has("JZWSX"))
                {
                    symbolTable.Add(regAppTableRecord2);
                    trans.AddNewlyCreatedDBObject(regAppTableRecord2, true);
                }
                if (!pl.IsWriteEnabled)
                {
                    pl.UpgradeOpen();
                }
                pl.XData = new ResultBuffer(tv2);
                #endregion

                MapApplication mapApp = HostMapApplicationServices.Application;
                Tables tables = mapApp.ActiveProject.ODTables;
                Autodesk.Gis.Map.ObjectData.Table table;

                bool IsxxB = true;
                if (tables.TablesCount == 0) { IsxxB = true; }
                if (tables.TablesCount > 0)
                {
                    StringCollection tbablename = tables.GetTableNames();
                    for (int i = 0; i < tables.TablesCount; i++)
                    {
                        if ("房屋属性" == tbablename[i])
                        {
                            IsxxB = false;
                            break;
                        }
                    }
                }
                if (IsxxB == true) { Creat_FW_SX(); }
                table = tables["房屋属性"];

                Autodesk.Gis.Map.ObjectData.Record record = Autodesk.Gis.Map.ObjectData.Record.Create();
                table.InitRecord(record);

                Records records = table.GetObjectTableRecords(0, pl, Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, true);
                if (records.Count > 0)
                {
                    foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                    {
                        Autodesk.Gis.Map.Utilities.MapValue val;
                        val = record2[0];
                        val.Assign(sx[0]);
                        val = record2[1];
                        val.Assign(sx[1]);
                        val = record2[2];
                        val.Assign(sx[2]);
                        val = record2[3];
                        val.Assign(sx[3]);
                        val = record2[4];
                        val.Assign(sx[4]);
                        val = record2[5];
                        val.Assign(sx[5]);
                        val = record2[6];
                        val.Assign(sx[6]);
                        val = record2[7];
                        val.Assign(sx[7]);
                        val = record2[8];
                        val.Assign(sx[8]);
                        val = record2[9];
                        val.Assign(sx[9]);
                        val = record2[10];
                        val.Assign(sx[10]);
                        val = record2[11];
                        val.Assign(sx[11]);
                        val = record2[12];
                        val.Assign(sx[12]);
                        val = record2[13];
                        val.Assign(sx[13]);
                        val = record2[14];
                        val.Assign(sx[14]);
                        val = record2[15];
                        val.Assign(sx[15]);
                        val = record2[16];
                        val.Assign(sx[16]);
                        val = record2[17];
                        val.Assign(sx[17]);
                        val = record2[18];
                        val.Assign(sx[18]);
                        val = record2[19];
                        val.Assign(sx[19]);
                        val = record2[20];
                        val.Assign(sx[20]);
                        val = record2[21];
                        val.Assign(sx[21]);
                        val = record2[22];
                        val.Assign(sx[22]);
                        val = record2[23];
                        val.Assign(sx[23]);
                        val = record2[24];
                        val.Assign(sx[24]);
                        val = record2[25];
                        val.Assign(sx[25]);
                        val = record2[26];
                        val.Assign(sx[26]);
                        val = record2[27];
                        val.Assign(sx[27]);
                        val = record2[28];
                        val.Assign(sx[28]);

                         records.UpdateRecord(record2);
                        //table.AddRecord(record, pl);
                        trans.Commit();
                        trans.Dispose();
                    }
                }
                else
                {
                    Autodesk.Gis.Map.Utilities.MapValue val;
                    val = record[0];
                    val.Assign(sx[0]);
                    val = record[1];
                    val.Assign(sx[1]);
                    val = record[2];
                    val.Assign(sx[2]);
                    val = record[3];
                    val.Assign(sx[3]);
                    val = record[4];
                    val.Assign(sx[4]);
                    val = record[5];
                    val.Assign(sx[5]);
                    val = record[6];
                    val.Assign(sx[6]);
                    val = record[7];
                    val.Assign(sx[7]);
                    val = record[8];
                    val.Assign(sx[8]);
                    val = record[9];
                    val.Assign(sx[9]);
                    val = record[10];
                    val.Assign(sx[10]);
                    val = record[11];
                    val.Assign(sx[11]);
                    val = record[12];
                    val.Assign(sx[12]);
                    val = record[13];
                    val.Assign(sx[13]);
                    val = record[14];
                    val.Assign(sx[14]);
                    val = record[15];
                    val.Assign(sx[15]);
                    val = record[16];
                    val.Assign(sx[16]);
                    val = record[17];
                    val.Assign(sx[17]);
                    val = record[18];
                    val.Assign(sx[18]);
                    val = record[19];
                    val.Assign(sx[19]);
                    val = record[20];
                    val.Assign(sx[20]);
                    val = record[21];
                    val.Assign(sx[21]);
                    val = record[22];
                    val.Assign(sx[22]);
                    val = record[23];
                    val.Assign(sx[23]);
                    val = record[24];
                    val.Assign(sx[24]);
                    val = record[25];
                    val.Assign(sx[25]);
                    val = record[26];
                    val.Assign(sx[26]);
                    val = record[27];
                    val.Assign(sx[27]);
                    val = record[28];
                    val.Assign(sx[28]);

                    //records.UpdateRecord(record);
                    table.AddRecord(record, pl);
                    trans.Commit();
                    trans.Dispose();
                }
                records.Dispose();
                record.Dispose();
            }
        }
        /// <summary>增加、修改宗地界址线建筑面积属性
        /// 增加、修改宗地界址线建筑面积属性
        /// </summary>
        /// <param name="plineid"></param>
        /// <param name="jzmj">建筑面积</param>
        /// <param name="jzdzmj">建筑占地面积</param>
        public static void  Add_JZX_SX(ObjectId plineid, double jzmj,double jzdzmj)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                ACDBPolyline pl = (ACDBPolyline)trans.GetObject(plineid, AcadOpenMode.ForWrite);
                SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, AcadOpenMode.ForWrite);
 
                MapApplication mapApp = HostMapApplicationServices.Application;
                Tables tables = mapApp.ActiveProject.ODTables;
                Autodesk.Gis.Map.ObjectData.Table table;

                bool IsxxB = true;
                if (tables.TablesCount == 0) { IsxxB = true; }
                if (tables.TablesCount > 0)
                {
                    StringCollection tbablename = tables.GetTableNames();
                    for (int i = 0; i < tables.TablesCount; i++)
                    {
                        if ("建筑面积表" == tbablename[i])
                        {
                            IsxxB = false;
                            break;
                        }
                    }
                }
                if (IsxxB == true) 
                {
                    CADTools cadtools = new CADTools();
                    cadtools.Creat_JZX_SX();
                }
                table = tables["建筑面积表"];

                Autodesk.Gis.Map.ObjectData.Record record = Autodesk.Gis.Map.ObjectData.Record.Create();
                table.InitRecord(record);

                Records records = table.GetObjectTableRecords(0, pl, Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, true);
                if (records.Count > 0)
                {
                    foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                    {
                        Autodesk.Gis.Map.Utilities.MapValue val;
                        val = record2[0];
                        val.Assign(jzmj);
                        val = record2[1];
                        val.Assign(jzdzmj);
                        records.UpdateRecord(record2);
                        trans.Commit();
                        trans.Dispose();
                    }
                }
                else
                {
                    Autodesk.Gis.Map.Utilities.MapValue val;
                    val = record[0];
                    val.Assign(jzmj);
                    val = record[1];
                    val.Assign(jzdzmj);
                    table.AddRecord(record, pl);
                    trans.Commit();
                    trans.Dispose();
                }
                records.Dispose();
            }
        }
        /// <summary>
        /// 创建宗地建筑面积表
        /// </summary>
        public void Creat_JZX_SX()
        {
            MapApplication mapApp = HostMapApplicationServices.Application;
            Tables tables = mapApp.ActiveProject.ODTables;
            FieldDefinitions fieldDefs = mapApp.ActiveProject.MapUtility.NewODFieldDefinitions();
            FieldDefinition def1;

            bool IsxxB = true;
            if (tables.TablesCount == 0) { IsxxB = true; }
            if (tables.TablesCount > 0)
            {
                StringCollection tbablename = tables.GetTableNames();
                for (int i = 0; i < tables.TablesCount; i++)
                {
                    if ("建筑面积表" == tbablename[i])
                    {
                        IsxxB = false;
                        break;
                    }
                }
            }
            if (IsxxB == true)
            {
                def1 = fieldDefs.Add("建筑面积", "建筑面积", Autodesk.Gis.Map.Constants.DataType.Real, 0);
                def1 = fieldDefs.Add("建筑占地面积", "建筑占地面积", Autodesk.Gis.Map.Constants.DataType.Real, 1);
                def1 = fieldDefs.Add("备注", "备注", Autodesk.Gis.Map.Constants.DataType.Character, 2);
            }
            tables.Add("建筑面积表", fieldDefs, "建筑面积表", true);
        }
        /// <summary>创建房屋属性表
        /// 创建房屋属性表
        /// </summary>
        public void Creat_FW_SX()
        {
            MapApplication mapApp = HostMapApplicationServices.Application;
            Tables tables = mapApp.ActiveProject.ODTables;
            FieldDefinitions fieldDefs = mapApp.ActiveProject.MapUtility.NewODFieldDefinitions();
            FieldDefinition def1;

            bool IsxxB = true;
            if (tables.TablesCount == 0) { IsxxB = true; }
            if (tables.TablesCount > 0)
            {
                StringCollection tbablename = tables.GetTableNames();
                for (int i = 0; i < tables.TablesCount; i++)
                {
                    if ("房屋属性" == tbablename[i])
                    {
                        IsxxB = false;
                        break;
                    }
                }
            }
            if (IsxxB == true)
            {
                def1 = fieldDefs.Add("建筑物类型", "建筑物类型", Autodesk.Gis.Map.Constants.DataType.Character, 0);
                def1 = fieldDefs.Add("地籍号", "地籍号", Autodesk.Gis.Map.Constants.DataType.Character, 1);
                def1 = fieldDefs.Add("房屋性质", "房屋性质", Autodesk.Gis.Map.Constants.DataType.Character, 2);
                def1 = fieldDefs.Add("房屋产别", "房屋产别", Autodesk.Gis.Map.Constants.DataType.Character, 3);
                def1 = fieldDefs.Add("房屋用途", "房屋用途", Autodesk.Gis.Map.Constants.DataType.Character, 4);
                def1 = fieldDefs.Add("规划用途", "规划用途", Autodesk.Gis.Map.Constants.DataType.Character, 5);
                def1 = fieldDefs.Add("权利人类型", "权利人类型", Autodesk.Gis.Map.Constants.DataType.Character, 6);
                def1 = fieldDefs.Add("产权来源", "产权来源", Autodesk.Gis.Map.Constants.DataType.Character, 7);
                def1 = fieldDefs.Add("共有情况", "共有情况", Autodesk.Gis.Map.Constants.DataType.Character, 8);
                def1 = fieldDefs.Add("房屋幢号", "房屋幢号", Autodesk.Gis.Map.Constants.DataType.Character, 9);
                def1 = fieldDefs.Add("户号", "户号", Autodesk.Gis.Map.Constants.DataType.Character, 10);
                def1 = fieldDefs.Add("总层数", "总层数", Autodesk.Gis.Map.Constants.DataType.Character,11);
                def1 = fieldDefs.Add("所在层数", "所在层数", Autodesk.Gis.Map.Constants.DataType.Character, 12);
                def1 = fieldDefs.Add("房屋结构", "房屋结构", Autodesk.Gis.Map.Constants.DataType.Character, 13);
                def1 = fieldDefs.Add("竣工时间", "竣工时间", Autodesk.Gis.Map.Constants.DataType.Character, 14);
                def1 = fieldDefs.Add("专有建筑面积", "专有建筑面积", Autodesk.Gis.Map.Constants.DataType.Character, 15);
                def1 = fieldDefs.Add("分摊建筑面积", "分摊建筑面积", Autodesk.Gis.Map.Constants.DataType.Character, 16);
                def1 = fieldDefs.Add("房屋功能", "房屋功能", Autodesk.Gis.Map.Constants.DataType.Character, 17);
                def1 = fieldDefs.Add("附加说明", "附加说明", Autodesk.Gis.Map.Constants.DataType.Character, 18);
                def1 = fieldDefs.Add("调查日期", "调查日期", Autodesk.Gis.Map.Constants.DataType.Character, 19);
                def1 = fieldDefs.Add("调查意见", "调查意见", Autodesk.Gis.Map.Constants.DataType.Character, 20);
                def1 = fieldDefs.Add("调查员", "调查员", Autodesk.Gis.Map.Constants.DataType.Character, 21);
                def1 = fieldDefs.Add("面积折扣系数", "面积折扣系数", Autodesk.Gis.Map.Constants.DataType.Character, 22);
                def1 = fieldDefs.Add("北", "北", Autodesk.Gis.Map.Constants.DataType.Character, 23);
                def1 = fieldDefs.Add("东", "东", Autodesk.Gis.Map.Constants.DataType.Character, 24);
                def1 = fieldDefs.Add("南", "南", Autodesk.Gis.Map.Constants.DataType.Character, 25);
                def1 = fieldDefs.Add("西", "西", Autodesk.Gis.Map.Constants.DataType.Character, 26);
                def1 = fieldDefs.Add("自动分析地籍号", "自动分析地籍号", Autodesk.Gis.Map.Constants.DataType.Character, 27);
                def1 = fieldDefs.Add("备注", "备注", Autodesk.Gis.Map.Constants.DataType.Character, 28);
                def1 = fieldDefs.Add("本层建筑面积", "本层建筑面积", Autodesk.Gis.Map.Constants.DataType.Real, 29);
                tables.Add("房屋属性", fieldDefs, "房屋属性", true);
            }
        }
        /// <summary>创建或修改多段线
        /// 
        /// </summary>
        /// <param name="La">层名</param>
        /// <param name="Xx">线型</param>
        /// <param name="xk">全局线宽</param>
        /// <param name="color">颜色</param>
        /// <param name="hd">厚度</param>
        /// <param name="bg">标高</param>
        /// <param name="xxbl">线型比例</param>
        /// <returns></returns>
        public  static ObjectId CreatPolyline(string La, string Xx, double xk, int color, double hd, double bg, double xxbl)
        {
            ACDBPolyline PLOBJ = new ACDBPolyline();
            ObjectId PLOBJid = ObjectId.Null;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            PromptPointOptions oppt = new PromptPointOptions("");
            oppt.AllowNone = true;
            PromptPointResult respt;

            Point3d pt1 = new Point3d(0, 0, 0), pt2, Spt = new Point3d(0, 0, 0);
            int n = 0;
            do
            {
                switch (n)
                {
                    case 0:
                        oppt.Message = "\n指定第一点: ";
                        oppt.UseBasePoint = false;
                        break;
                    case 1:
                    case 2:
                        oppt.Message = "\n指定下一点或[放弃(U)]";
                        oppt.Keywords.Add("U", "U", "U", false, true);
                        oppt.UseBasePoint = true;
                        oppt.BasePoint = pt1;
                        break;
                    default:
                        oppt.Message = "\n指定下一点或[闭合(C)/放弃(U)]";
                        oppt.UseBasePoint = true;
                        oppt.BasePoint = pt1;
                        oppt.Keywords.Add("U", "U", "U", false, true);
                        oppt.Keywords.Add("C", "C", "C", false, true);
                        break;
                }
                respt = ed.GetPoint(oppt);
                if (respt.Status == PromptStatus.None || respt.Status == PromptStatus.Cancel)
                {
                    if (PLOBJ.NumberOfVertices <= 1)
                    {
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                            BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                            trans.GetObject(PLOBJid, AcadOpenMode.ForWrite);
                            PLOBJ.Erase();
                            trans.Commit();
                            trans.Dispose();
                            return ObjectId.Null;
                        }
                    }
                }
                else
                {
                    if (respt.Status == PromptStatus.Keyword)
                    {
                        switch (respt.StringResult)
                        {
                            case "U":
                                if (n > 1)
                                {
                                    using (Transaction trans = db.TransactionManager.StartTransaction())
                                    {
                                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                                        trans.GetObject(PLOBJid, AcadOpenMode.ForWrite);
                                        PLOBJ.RemoveVertexAt(PLOBJ.NumberOfVertices - 1);
                                        pt1 = PLOBJ.GetPoint3dAt(PLOBJ.NumberOfVertices - 1);
                                        PLOBJ.Plinegen = true;//线型生成
                                        trans.Commit();
                                        trans.Dispose();
                                    }
                                    n--;
                                }
                                else
                                {
                                    ed.WriteMessage("\n已经放弃所有线段。");
                                    return ObjectId.Null;
                                }
                                break;
                            case "C":
                                using (Transaction trans = db.TransactionManager.StartTransaction())
                                {
                                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                                    trans.GetObject(PLOBJid, AcadOpenMode.ForWrite);
                                    PLOBJ.Closed = true;
                                    PLOBJ.Plinegen = true;//线型生成
                                    pt1 = PLOBJ.GetPoint3dAt(PLOBJ.NumberOfVertices - 1);
                                    trans.Commit();
                                    trans.Dispose();
                                }
                                return PLOBJ.ObjectId;
                        }
                    }
                    else
                    {
                        pt2 = respt.Value;
                        Point2d p2d2 = new Point2d(pt2.X, pt2.Y);
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            if (n == 0)
                            {
                                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);

                                PLOBJ.AddVertexAt(0, p2d2, 0, 0, 0);
                                PLOBJid = btr.AppendEntity(PLOBJ);
                                PLOBJ.Plinegen = true;//线型生成
                                trans.AddNewlyCreatedDBObject(PLOBJ, true);
                                pt1 = PLOBJ.GetPoint3dAt(0);
                                trans.Commit();
                                trans.Dispose();
                            }
                            else
                            {
                                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead);
                                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite);
                                trans.GetObject(PLOBJid, AcadOpenMode.ForWrite);
                                if (PLOBJ.NumberOfVertices >= 3)
                                {
                                    MyPoint L1D1 = new MyPoint(p2d2.X, p2d2.Y);
                                    Point2d P1 = PLOBJ.GetPoint2dAt(PLOBJ.NumberOfVertices - 1);
                                    MyPoint L1D2 = new MyPoint(P1.X, P1.Y);
                                    bool Isxj = false;
                                    for (int i = 0; i < PLOBJ.NumberOfVertices - 2; i++)
                                    {
                                        Point2d LP1 = PLOBJ.GetPoint2dAt(i);
                                        Point2d LP2 = PLOBJ.GetPoint2dAt(i + 1);
                                        MyPoint L2D1 = new MyPoint(LP1.X, LP1.Y);
                                        MyPoint L2D2 = new MyPoint(LP2.X, LP2.Y);
                                        ////Line2d L1D2 = new Line2d(PLOBJ.GetPoint2dAt(i), PLOBJ.GetPoint2dAt(i + 1));
                                        PanDuanL1AndL2GuanXi JJ = new PanDuanL1AndL2GuanXi(L1D1, L1D2, L2D1, L2D2);

                                        if (JJ.JiaodianGeShu == 1)
                                        {
                                            Isxj = true;
                                        }
                                    }
                                    if (Isxj)
                                    {
                                        MessageBox.Show("线段自相交或重点", "安徽四院");
                                        continue;
                                    }
                                }
                                PLOBJ.AddVertexAt(n, p2d2, 0, 0, 0);
                                PLOBJ.Layer = La;
                                PLOBJ.Linetype = Xx;
                                PLOBJ.ConstantWidth = xk;
                                PLOBJ.ColorIndex = color;
                                PLOBJ.Thickness = hd;
                                PLOBJ.Elevation = bg;
                                PLOBJ.LinetypeScale = xxbl;
                                PLOBJ.Plinegen = true;//线型生成
                                pt1 = PLOBJ.GetPoint3dAt(PLOBJ.NumberOfVertices - 1);
                                trans.Commit();
                                trans.Dispose();
                            }
                        }
                        n++;
                    }
                }
                oppt.Keywords.Clear();
            } while (respt.Status == PromptStatus.OK || respt.Status == PromptStatus.Keyword);
            return PLOBJ.ObjectId;
        }
        /// <summary>增加普通字型
        /// 增加普通字型
        /// </summary>
        /// <param name="textstyle">字型名称</param>
        /// <param name="zt">使用的字体</param>
        /// <param name="gkb">高宽比</param>
        /// <param name="qxjd">倾斜角度，注意数值是否合法</param>
        /// <param name="bold">是否粗体</param>
        /// <returns>字型ID</returns>
        public static ObjectId AddNewTextStyle(string textstyle, string zt, double gkb, double qxjd, bool bold)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId TextStyleID = ObjectId.Null;
            try
            {
                Transaction trans = db.TransactionManager.StartTransaction();
                TextStyleTable TextStyleB = (TextStyleTable)trans.GetObject(db.TextStyleTableId, AcadOpenMode.ForWrite);
                if (TextStyleB.Has(textstyle))
                {
                    TextStyleID = TextStyleB[textstyle];
                }
                else
                {
                    TextStyleB.UpgradeOpen();
                    TextStyleTableRecord textStyleTableRecord = new TextStyleTableRecord();
                    textStyleTableRecord.Name = textstyle;
                    textStyleTableRecord.XScale = 1;
                    FontDescriptor font = new FontDescriptor(zt, bold, false, 1, 1);//
                    textStyleTableRecord.Font = font;
                    textStyleTableRecord.XScale = gkb;
                    textStyleTableRecord.ObliquingAngle = qxjd;
                    TextStyleID = TextStyleB.Add(textStyleTableRecord);
                    trans.AddNewlyCreatedDBObject(textStyleTableRecord, true);
                }
                trans.Commit();
                trans.Dispose();
                db.Textstyle = TextStyleID;
            }
            catch (System.Exception ex2)
            {
                MessageBox.Show(ex2.Message, "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return TextStyleID;
        }
        /// <summary>增加大字体字型字型
        /// 增加大字体字型字型
        /// </summary>
        /// <param name="textstyle">字型名称</param>
        /// <param name="textFileName">形文件大字体字体</param>
        /// <param name="gkb">高宽比</param>
        /// <param name="qxjd">倾斜角度,注意数值是否合法</param>
        /// <returns>字型ID</returns>
        public static ObjectId AddNewTextStyle(string textstyle, string textFileName, double gkb, double qxjd)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId TextStyleID = ObjectId.Null;
            try
            {
                Transaction trans = db.TransactionManager.StartTransaction();
                TextStyleTable TextStyleB = (TextStyleTable)trans.GetObject(db.TextStyleTableId, AcadOpenMode.ForWrite);

                if (TextStyleB.Has(textstyle))
                {
                    TextStyleID = TextStyleB[textstyle];
                }
                else
                {
                    TextStyleB.UpgradeOpen();
                    TextStyleTableRecord textStyleTableRecord = new TextStyleTableRecord();
                    textStyleTableRecord.Name = textstyle;
                    textStyleTableRecord.XScale = 1;
                    textStyleTableRecord.BigFontFileName = textFileName;
                    textStyleTableRecord.FileName = "romans.shx";
                    textStyleTableRecord.XScale = gkb;
                    textStyleTableRecord.ObliquingAngle = qxjd;
                    TextStyleID = TextStyleB.Add(textStyleTableRecord);
                    trans.AddNewlyCreatedDBObject(textStyleTableRecord, true);
                }
                trans.Commit();
                trans.Dispose();
                db.Textstyle = TextStyleID;
            }
            catch (System.Exception ex2)
            {

                MessageBox.Show(ex2.Message, "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return TextStyleID;
        }



        /// <summary>
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static NtsPoint ConvertToSharpMapPoint(double x, double y)
        {
            return new NtsPoint(x, y);
        }

        /// <summary>
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static NtsPoint ConvertToSharpMapPoint(Point2d point2d)
        {
            return new NtsPoint(point2d.X, point2d.Y);
        }

        /// <summary>
        /// 将点转换为TF.net下的Point 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static NtsPoint ConvertToSharpMapPoint(Point3d point3d)
        {
            return new NtsPoint(point3d.X, point3d.Y);
        }

        /// <summary>
        /// 将CAD的多段线转换为TF.net下的 LineString线  Autodesk.AutoCAD.DatabaseServices.Line
        /// </summary>
        /// <param name="polyline">多段线</param>
        /// <returns>ILineString</returns>
        public static GeoAPI.Geometries.ILineString ConvertToSharpMapILineString(ACDBPolyline polyline)
        {
            List<GeoCoordinate> points = new List<GeoCoordinate>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                points.Add(new GeoCoordinate(polyline.GetPoint2dAt(i).X, polyline.GetPoint2dAt(i).Y));
            }
            if (polyline.Closed == true) { points.Add(new GeoCoordinate(polyline.GetPoint2dAt(0).X, polyline.GetPoint2dAt(0).Y)); }
            if (points.Count > 1)
            {
                return new NtsLine(points.ToArray());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///将CAD下的直线写入LineString
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static GeoAPI.Geometries.ILineString ConvertToSharpMapILineString(Autodesk.AutoCAD.DatabaseServices.Line line)
        {
            GeoCoordinate[] points = new GeoCoordinate[2];
            points[0] = new GeoCoordinate(line.StartPoint.X, line.StartPoint.Y);
            points[1] = new GeoCoordinate(line.EndPoint.X, line.EndPoint.Y);
            return new NtsLine(points);
        }

        private static GeoCoordinate ReadSharpMapCoordinate(Point3d point3d)
        {
            GeoAPI.Geometries.Coordinate coord = new GeoAPI.Geometries.Coordinate(point3d.X, point3d.Y, point3d.Z);
            return coord;
        }

        /// <summary>
        /// 创建一个多边形,多段线必须是封闭的
        /// </summary>
        /// <param name="polyline">封闭的多段线</param>
        /// <returns>IPolygon</returns>
        public static GeoAPI.Geometries.IPolygon ConvertToSharpMapIPolygon(ACDBPolyline polyline)
        {
            if (polyline.Closed == false) { return null; }
            NtsILinearRing shell = ConvertToSharpMapRing(polyline);
            return new NtsPolygon(shell);
        }

        /// <summary>
        /// 创建环
        /// </summary>
        /// <param name="polyline">多段线</param>
        /// <returns></returns>
        public static GeoAPI.Geometries.ILinearRing ConvertToSharpMapRing(ACDBPolyline polyline)
        {
            List<GeoCoordinate> points = new List<GeoCoordinate>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                points.Add(new GeoCoordinate(polyline.GetPoint2dAt(i).X, polyline.GetPoint2dAt(i).Y));
            }
            points.Add(new GeoCoordinate(polyline.GetPoint2dAt(0).X, polyline.GetPoint2dAt(0).Y));
            return new NtsRing(points.ToArray());
        }

        /// <summary>
        /// 将topology下的多个多边形写入DWG
        /// </summary>
        /// <param name="multiPolygons"></param>
        /// <returns></returns>
        public static List<ACDBPolyline> ConvertToACDBPolyline(NetTopologySuite.Geometries.MultiPolygon multiPolygons)
        {
            List<ACDBPolyline> Polylines_list = new List<ACDBPolyline>();
            for (int i = 0; i < multiPolygons.Count; i++)
            {
                NtsGeometry SharpGeo_n = (NtsGeometry)multiPolygons.GetGeometryN(i);
                if (SharpGeo_n.GeometryType.ToLower() == "polygon".ToLower())
                {
                    NtsPolygon polygon = (NtsPolygon)multiPolygons.GetGeometryN(i);
                    List<ACDBPolyline> linesList = ConvertToACDBPolyline(polygon);
                    for (int j = 0; j < linesList.Count; j++)
                    {
                        Polylines_list.Add(linesList[j]);
                    }
                }
            }
            return Polylines_list;
        }

        /// <summary>
        /// 将topology下的多边形写入DWG
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<ACDBPolyline> ConvertToACDBPolyline(NtsPolygon polygon)
        {
            List<ACDBPolyline> ACDBPolyline_list = new List<ACDBPolyline>();
            ACDBPolyline_list.Add(ConvertToACDBPolyline((NtsLine)polygon.ExteriorRing));
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                NtsLine ring = (NtsLine)polygon.GetInteriorRingN(i);
                ACDBPolyline_list.Add(ConvertToACDBPolyline(ring));
            }
            return ACDBPolyline_list;
        }

        /// <summary>
        /// 将topology下的多边形写入DWG
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static ACDBPolyline ConvertToACDBPolyline(NtsLine lineString)
        {
            List<ACDBPolyline> ACDBPolyline_list = new List<ACDBPolyline>();

            GeoCoordinate[] coordinates = lineString.Coordinates;
            ACDBPolyline polyline = new ACDBPolyline();
            int i = 0;
            foreach (GeoAPI.Geometries.Coordinate coordinate in coordinates)
            {
                Point2d point = new Point2d(coordinate.X, coordinate.Y);
                polyline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            return polyline;
        }
        public static ACDBPolyline ConvertToACDBPolyline(NtsILinearRing linearRing)
        {
            GeoCoordinate[] coordinates = linearRing.Coordinates;
            ACDBPolyline polyline = new ACDBPolyline();
            int i = 0;
            foreach (GeoAPI.Geometries.Coordinate coordinate in coordinates)
            {
                Point2d point = new Point2d(coordinate.X, coordinate.Y);
                polyline.AddVertexAt(i, point, 0, 0, 0);
                i++;
            }
            return polyline;
        }

    }
}
