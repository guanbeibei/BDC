using NCZJDDJFZ.DiJitools;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Topology.Geometries;

namespace NCZJDDJFZ.Tools
{
    public class Sort_Comparer : IComparer
    {
        int IComparer.Compare(Object x1, Object x2)//X1和X2其实代表的是前一个数和后一个数(上下两行的数)，不是同一维(行)里的两个数
        {
            Dijicltools.zdhANDpl strobj1 = new Dijicltools.zdhANDpl();
            strobj1 = (Dijicltools.zdhANDpl)x1;
            Dijicltools.zdhANDpl strobj2 = new Dijicltools.zdhANDpl();
            strobj2 = (Dijicltools.zdhANDpl)x2;
            return ((new CaseInsensitiveComparer()).Compare(strobj1.zdh, strobj2.zdh));
        }

    }
    class Uitl
    {
        /// <summary>
        /// 分析两条界址线界址间距是否太小或悬挂
        /// </summary>
        /// <param name="Polygon1">第一个多边形</param>
        /// <param name="Polygon2">第二个多边形</param>
        /// <param name="buffdis">缓冲范围</param>
        /// <returns></returns>
        public static ArrayList JZXFX(Topology.Geometries.Polygon Polygon1, Topology.Geometries.Polygon Polygon2, double buffdis)
        {
            ArrayList pointARR = new ArrayList();
            //GeometryFactory geometryFactory = new GeometryFactory();
            //LinearRing linearRing = new LinearRing(null, GeometryFactory.Default);
            Topology.Geometries.Geometry Geometry_buff1 = (Geometry)Polygon1.Buffer(buffdis, 30);
            Topology.Geometries.Geometry Geometry_buff2 = (Geometry)Polygon2.Buffer(buffdis, 30);
            Topology.Geometries.LinearRing Polygon2_Ring = (LinearRing)Polygon2.ExteriorRing;
            Topology.Geometries.LinearRing Polygon1_Ring = (LinearRing)Polygon1.ExteriorRing;
            for (int i = 0; i < Polygon2_Ring.NumPoints - 1; i++)
            {
                Topology.Geometries.Point point1 = (Point)Polygon2_Ring.GetPointN(i);
                if (point1.Within(Geometry_buff1))
                {
                    bool IsOnPoint = false;
                    for (int j = 0; j < Polygon1_Ring.NumPoints; j++)
                    {
                        Topology.Geometries.Point point2 = (Point)Polygon1_Ring.GetPointN(j);
                        if (point2.Distance(point1) < 0.00001) { IsOnPoint = true; break; }
                    }
                    if (!IsOnPoint)
                    {
                        point1.UserData = "2";//表示是第二个多边形的点有问题
                        pointARR.Add(point1);
                    }
                }
            }

            for (int i = 0; i < Polygon1_Ring.NumPoints - 1; i++)
            {
                Topology.Geometries.Point point1 = (Point)Polygon1_Ring.GetPointN(i);
                if (point1.Within(Geometry_buff2))
                {
                    bool IsOnPoint = false;
                    for (int j = 0; j < Polygon2_Ring.NumPoints; j++)
                    {
                        Topology.Geometries.Point point2 = (Point)Polygon2_Ring.GetPointN(j);
                        if (point2.Distance(point1) < 0.00001) { IsOnPoint = true; break; }
                    }
                    if (!IsOnPoint)
                    {
                        point1.UserData = "1";//表示是第一个多边形的点有问题
                        pointARR.Add(point1);
                    }
                }
            }

            return pointARR;
        }
        /// <summary>使用电子表格数据绘制到行政村级别的节点树
        /// 使用电子表格数据绘制到行政村级别的节点树
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public bool DrawTree_XZC(TreeView treeView, ArrayList array)
        {
            bool Iscg = true;//画树是否成功
            #region 检查是否有重号
            for (int i = 0; i < array.Count - 1; i++)
            {
                string sshou = (string)array[i];
                sshou = sshou.Substring(0, 12);
                for (int J = i + 1; J < array.Count; J++)
                {
                    string sshou2 = (string)array[J];
                    sshou2 = sshou2.Substring(0, 12);
                    if (sshou2 == sshou)
                    {
                        int Jncon = J + 1;
                        int Incon = i + 1;
                        MessageBox.Show("第 " + Incon + " 行和" + "第" + Jncon.ToString() + " 行重号！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                }
            }
            #endregion
            array.Sort();
            #region 结构分析
            if (array.Count <= 0) { return false; }
            ArrayList arrayXX = new ArrayList();//各代码的结构信息
            int nn0 = 0, nn1 = 0, nn2 = -1, nn3 = -1, nn4 = -1;
            string s0 = "", s1 = "", s2 = "", s3 = "", s4 = "";
            string Bs0 = "", Bs1 = "", Bs2 = "", Bs3 = "", Bs4 = "";

            if (array.Count >= 1)
            {
                string zf = (string)array[0];
                s0 = zf.Substring(0, 2); s1 = zf.Substring(2, 2); s2 = zf.Substring(4, 2);
                s3 = zf.Substring(6, 3); s4 = zf.Substring(9, 3); //s5 = zf.Substring(12, 2); "341823008002"
                arrayXX.Add("000");
            }
            for (int i = 1; i < array.Count; i++)
            {
                string zf = (string)array[i];
                Bs0 = zf.Substring(0, 2); Bs1 = zf.Substring(2, 2); Bs2 = zf.Substring(4, 2);
                Bs3 = zf.Substring(6, 3); Bs4 = zf.Substring(9, 3); //Bs5 = zf.Substring(12, 2);
                string xxx = "";
                if (Bs0 != s0)
                {
                    nn0++; nn1 = -1; nn2 = -1;
                    nn3 = -1; nn4 = -1; //nn5 = -1;
                    if (Bs1 + Bs2 + Bs3 + Bs4 == "0000000000")
                    {
                        xxx = nn0.ToString("000");
                        arrayXX.Add(xxx);
                        s0 = Bs0; s1 = Bs1; s2 = Bs2;
                        s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                        nn1 = 0; nn2 = 0;
                        nn3 = 0; nn4 = 0; //nn5 = 0;
                        continue;
                    }
                    else
                    {
                        MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    }
                }
                else
                {
                    if (s1 != Bs1)
                    {
                        if (s1 != "00") { nn1++; }
                        if (Bs2 + Bs3 + Bs4 == "00000000")
                        {
                            xxx = nn0.ToString("000") + nn1.ToString("000");
                            arrayXX.Add(xxx);
                            nn2 = 0;
                            nn3 = 0; nn4 = 0; //nn5 = 0;
                            s0 = Bs0; s1 = Bs1; s2 = Bs2;
                            s3 = Bs3; s4 = Bs4; //s5 = Bs5;
                            continue;
                        }
                        else
                        {
                            MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;
                        }
                    }
                    else
                    {
                        if (s2 != Bs2)
                        {
                            if (s2 != "00") { nn2++; }
                            if (Bs3 + Bs4 == "000000")
                            {
                                xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000");
                                arrayXX.Add(xxx);
                                s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                s3 = Bs3; s4 = Bs4; //s5 = Bs5;
                                nn3 = 0; nn4 = 0; //nn5 = 0;
                                continue;
                            }
                            else
                            {
                                MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                break;
                            }
                        }
                        else
                        {
                            if (s3 != Bs3)
                            {
                                if (s3 != "000") { nn3++; }
                                if (Bs4 == "000")
                                {
                                    xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000") + nn3.ToString("000");
                                    arrayXX.Add(xxx);
                                    nn4 = 0; //nn5 = 0;
                                    s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                    s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                                    nn4 = 0;// nn5 = 0;
                                    continue;
                                }
                                else
                                {
                                    MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    break;
                                }
                            }
                            else
                            {


                                if (s4 != Bs4)
                                {
                                    if (s4 != "000") { nn4++; }
                                    xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000") + nn3.ToString("000") + nn4.ToString("000");
                                    arrayXX.Add(xxx);
                                    s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                    s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                                }
                                else
                                {
                                    MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 插入节点
            treeView.Nodes.Clear();
            for (int i = 0; i < arrayXX.Count; i++)//遍历结构信息表
            {
                string xx = (string)arrayXX[i];//结构信息表中的内容
                if (xx != "")
                {
                    string zf = (string)array[i];//来自数据库表中的内容
                    string dm = zf.Substring(0, 12);//代码
                    string mc = zf.Substring(12, zf.Length - 12);//名称
                    int nn = xx.Length;
                    switch (nn)
                    {
                        case 3://只有省代码
                            TreeNode node = treeView.Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 6://只有地市以上代码
                            int node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node = treeView.Nodes[node0].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 9://只有县区以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            int node1 = Convert.ToInt32(xx.Substring(3, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 12://只有乡镇以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node1 = Convert.ToInt32(xx.Substring(3, 3));
                            int node2 = Convert.ToInt32(xx.Substring(6, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 15://只有村以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node1 = Convert.ToInt32(xx.Substring(3, 3));
                            node2 = Convert.ToInt32(xx.Substring(6, 3));
                            int node3 = Convert.ToInt32(xx.Substring(9, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes[node3].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        //case 18://只有组代码
                        //    node0 = Convert.ToInt32(xx.Substring(0, 3));
                        //    node1 = Convert.ToInt32(xx.Substring(3, 3));
                        //    node2 = Convert.ToInt32(xx.Substring(6, 3));
                        //    node3 = Convert.ToInt32(xx.Substring(9, 3));
                        //    int node4 = Convert.ToInt32(xx.Substring(12, 3));
                        //    node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes[node3].Nodes[node4].Nodes.Add(mc);
                        //    node.Tag = dm;
                        //    node.Name = xx;
                        //    break;
                    }
                }
            }
            #endregion 插入节点
            return Iscg;
        }
        /// <summary>
        /// 根据 DataTable 数据创建节点树
        /// </summary>
        /// <param name="DS">DataTable表</param>
        /// <returns>是否成功</returns>
        public bool DrawTree(TreeView treeView, System.Data.DataTable DS)
        {
            bool Iscg = true;//画树是否成功
            ArrayList array = new ArrayList();
            int count = DS.Rows.Count;
            #region 将DataTable的代码和名称天然数组
            for (int i = 0; i < count; i++)
            {
                DataRow row = DS.Rows[i];
                string xzdm = row[0].ToString().Trim();
                if (xzdm.Length != 12)
                {
                    int hh = i + 1;
                    MessageBox.Show("第 " + hh.ToString() + " 行代码位数不对。", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                array.Add(xzdm + row[1].ToString().Trim());
            }
            array.Sort();
            #endregion
            #region 结构分析
            if (array.Count <= 0) { return false; }
            ArrayList arrayXX = new ArrayList();//各代码的结构信息
            int nn0 = 0, nn1 = 0, nn2 = -1, nn3 = -1, nn4 = -1;
            string s0 = "", s1 = "", s2 = "", s3 = "", s4 = "";
            string Bs0 = "", Bs1 = "", Bs2 = "", Bs3 = "", Bs4 = "";

            if (array.Count >= 1)
            {
                string zf = (string)array[0];
                s0 = zf.Substring(0, 2); s1 = zf.Substring(2, 2); s2 = zf.Substring(4, 2);
                s3 = zf.Substring(6, 3); s4 = zf.Substring(9, 3); //s5 = zf.Substring(12, 2); "341823008002"
                arrayXX.Add("000");
            }
            for (int i = 1; i < array.Count; i++)
            {
                string zf = (string)array[i];
                Bs0 = zf.Substring(0, 2); Bs1 = zf.Substring(2, 2); Bs2 = zf.Substring(4, 2);
                Bs3 = zf.Substring(6, 3); Bs4 = zf.Substring(9, 3); //Bs5 = zf.Substring(12, 2);
                string xxx = "";
                if (Bs0 != s0)
                {
                    nn0++; nn1 = -1; nn2 = -1;
                    nn3 = -1; nn4 = -1; //nn5 = -1;
                    if (Bs1 + Bs2 + Bs3 + Bs4 == "0000000000")
                    {
                        xxx = nn0.ToString("000");
                        arrayXX.Add(xxx);
                        s0 = Bs0; s1 = Bs1; s2 = Bs2;
                        s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                        nn1 = 0; nn2 = 0;
                        nn3 = 0; nn4 = 0; //nn5 = 0;
                        continue;
                    }
                    else
                    {
                        MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    }
                }
                else
                {
                    if (s1 != Bs1)
                    {
                        if (s1 != "00") { nn1++; }
                        if (Bs2 + Bs3 + Bs4 == "00000000")
                        {
                            xxx = nn0.ToString("000") + nn1.ToString("000");
                            arrayXX.Add(xxx);
                            nn2 = 0;
                            nn3 = 0; nn4 = 0; //nn5 = 0;
                            s0 = Bs0; s1 = Bs1; s2 = Bs2;
                            s3 = Bs3; s4 = Bs4; //s5 = Bs5;
                            continue;
                        }
                        else
                        {
                            MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;
                        }
                    }
                    else
                    {
                        if (s2 != Bs2)
                        {
                            if (s2 != "00") { nn2++; }
                            if (Bs3 + Bs4 == "000000")
                            {
                                xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000");
                                arrayXX.Add(xxx);
                                s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                s3 = Bs3; s4 = Bs4; //s5 = Bs5;
                                nn3 = 0; nn4 = 0; //nn5 = 0;
                                continue;
                            }
                            else
                            {
                                MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                break;
                            }
                        }
                        else
                        {
                            if (s3 != Bs3)
                            {
                                if (s3 != "000") { nn3++; }
                                if (Bs4 == "000")
                                {
                                    xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000") + nn3.ToString("000");
                                    arrayXX.Add(xxx);
                                    nn4 = 0; //nn5 = 0;
                                    s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                    s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                                    nn4 = 0;// nn5 = 0;
                                    continue;
                                }
                                else
                                {
                                    MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    break;
                                }
                            }
                            else
                            {


                                if (s4 != Bs4)
                                {
                                    if (s4 != "000") { nn4++; }
                                    xxx = nn0.ToString("000") + nn1.ToString("000") + nn2.ToString("000") + nn3.ToString("000") + nn4.ToString("000");
                                    arrayXX.Add(xxx);
                                    s0 = Bs0; s1 = Bs1; s2 = Bs2;
                                    s3 = Bs3; s4 = Bs4;// s5 = Bs5;
                                }
                                else
                                {
                                    MessageBox.Show(zf + "  没有父级代码！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 插入节点
            treeView.Nodes.Clear();
            for (int i = 0; i < arrayXX.Count; i++)//遍历结构信息表
            {
                string xx = (string)arrayXX[i];//结构信息表中的内容
                if (xx != "")
                {
                    string zf = (string)array[i];//来自数据库表中的内容
                    string dm = zf.Substring(0, 12);//代码
                    string mc = zf.Substring(12, zf.Length - 12);//名称
                    int nn = xx.Length;
                    switch (nn)
                    {
                        case 3://只有省代码
                            TreeNode node = treeView.Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 6://只有地市以上代码
                            int node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node = treeView.Nodes[node0].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 9://只有县区以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            int node1 = Convert.ToInt32(xx.Substring(3, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 12://只有乡镇以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node1 = Convert.ToInt32(xx.Substring(3, 3));
                            int node2 = Convert.ToInt32(xx.Substring(6, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        case 15://只有村以上代码
                            node0 = Convert.ToInt32(xx.Substring(0, 3));
                            node1 = Convert.ToInt32(xx.Substring(3, 3));
                            node2 = Convert.ToInt32(xx.Substring(6, 3));
                            int node3 = Convert.ToInt32(xx.Substring(9, 3));
                            node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes[node3].Nodes.Add(mc);
                            node.Tag = dm;
                            node.Name = xx;
                            break;
                        //case 18://只有组代码
                        //    node0 = Convert.ToInt32(xx.Substring(0, 3));
                        //    node1 = Convert.ToInt32(xx.Substring(3, 3));
                        //    node2 = Convert.ToInt32(xx.Substring(6, 3));
                        //    node3 = Convert.ToInt32(xx.Substring(9, 3));
                        //    int node4 = Convert.ToInt32(xx.Substring(12, 3));
                        //    node = treeView.Nodes[node0].Nodes[node1].Nodes[node2].Nodes[node3].Nodes[node4].Nodes.Add(mc);
                        //    node.Tag = dm;
                        //    node.Name = xx;
                        //    break;
                    }
                }
            }
            #endregion 插入节点
            return Iscg;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <returns>SQL Sever数据库连接字符串</returns>
        public static string GetConnectionString()
        {
            //Tools.Read_Write_Reg rwr = new Tools.Read_Write_Reg();
            string YHMC = ReadWriteReg.read_reg("用户名称");
            string YHMM = ReadWriteReg.read_reg("用户密码");
            string SJKMC = ReadWriteReg.read_reg("数据库名称");
            string FWQMC = ReadWriteReg.read_reg("服务器名称");
            string connectionString = "Persist Security Info=False;User id=" + YHMC + ";pwd=" +
                           YHMM + ";database=" + SJKMC + ";server=" + FWQMC;//写连接字符串
            return connectionString;
        }

        
    }
}
