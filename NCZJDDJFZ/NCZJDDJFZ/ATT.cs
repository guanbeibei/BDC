using Autodesk.AutoCAD.ApplicationServices;

namespace NCZJDDJFZ
{
    class ATT
    {
        //public static double[] D1;//图廓1号点
        //public static double[] D2;
        //public static double[] D3;
        //public static double[] D4;
        public static int JZDGS;//宗地界址点个数
        public static double CEN_X;//宗地中心点
        public static double CEN_Y;//宗地中心点
        public static Document Document_ZDT_SC = null;// 生成的宗地图的Document
        public static Document Document_ZDT_XG = null;//修改的宗地图的Document
        public static Document Document_ZDT_LL = null;//浏览的宗地图的Document
        public static Document Document_ZDT_DY = null;//打印的宗地图的Document
        
        public static string ZDT_File;//宗地图文件名
        public static string FCFHT_File;//分层分户图
        public static int BLCH = 100;//宗地图当前比例尺
        public static string A4A3_HS = "A4H";//使用纸张及方向

        public static int DongBai;//东北方向
        public static int DongNan;//东南方向
        public static int XiNan;//西南方向
        public static string MRSZ = "";//自定义默认指界（如：巷道等）
        public static string ZDSZ = "";//宗地四至
        public static string BZZJR = "";//本宗指界人
        public static string JZZN = "";//  界址点个数
        public static string DJH = "";//  地籍号
        #region 默认值
        public static string SQRLX_m = "";//申请类型
        public static string QLR_ZJZL_m = "";//证件种类
        public static string DWXZ_m = "";//单位性质
        public static string TXDZ_m = "";//通讯地址
        public static string SQRQ_m = "";//申请日期
        public static string YZBM_m = "";//邮    编
        public static string TEXTBZ1_m = "";//所有权人
        public static string FRDBZJZL_m = "";//法人代表证件种类
        public static string DWXZ2_m = "";//构筑物类型
        public static string DLJGDH_m = "";//不动产类型
        public static string DLJGDH2_m = "";//等级
        public static string LXR_m = "";//联系人
        public static string TDZL_m = "";//坐    落
        public static string SJYT_m = "";//实际用途
       // public static string DLBM_m = "";//地类编码
        public static string QSXZ_m = "";//权属性质
        public static string QLSLQJ_m = "";//权利设立情况
        public static string SQDJLR_m = "";//申请登记的内容
        public static string DYSQRJZRQ_m = "";//第一申请人签章日期
        public static string DESQRJZRQ_m = "";//第二申请人签章日期
        public static string SQRLX2_m = "";
        public static bool DYB_m = false;
        public static bool FBCZ_YSE_m = false;
        public static string LXR2_m = "";
        public static string SYQLX_m = "";
        public static string XZGHBMFZR_m = "";//村民组名称

        public static string DCBRQ_m = "";//调查表日期
        public static string GMJJHYDM_m = "";//国民经济行业代码
        public static string PZYT_m = "";//批准用途
        public static string PZYT_DLDM_m = "";//批准用途地类编码
        public static string SYQX_Q_m = "";//使用期限  自
        public static string SYQX_Z_m = "";//使用期限  至

        public static string QSDCJS = "";//权属调查记事
        public static string DCYXM = "";//调查员姓名
        public static string DCRQ = "";//调查日期
        public static string DJKZJS = "";//地籍测量记事
        public static string CLYXM = "";//测量员姓名
        public static string CLRQ = "";//测量日期
        public static bool SHHG = true;//审核合格
        public static string DJDCJGSHYJ = "";//地籍调查结果审核意见
        public static string SHRXM = "";//审核人姓名
        public static string SHRQ = "";//审核日期

        public static string CSYJ = "";//国土资源行政主管部门初审意见
        public static string CSSCR = "";//审查人
        public static string CSRJ = "";//审查日  期
        public static string CSRTDDJSGZH = "";//审查人土地登记上岗资格证号
        public static bool CSHG = false;//初审合格
        public static string SHYJ = "";//国土资源行政主管部门审核意见
        public static string SHFZR = "";//负责人
        public static string SHRQ_SPB = "";//初审日  期
        public static string SHRTDDJSGZH = "";//负责人土地登记上岗资格证号
        public static bool SHTG = false;//审核通过
        public static string PZYJ = "";//人民政府批准意见
        public static string PZR = "";//人民政府负责人
        public static string PZRQ = "";//人民政府批准日  期
        public static bool PZTG = false;//通过批准
        public static string SPBBZ = "";//备注
        public static string FZJS = "";//土地证书记事

        public static string SBB_BH = "";// 
        public static string SBB_TBRQ = "";// 
        public static string ZZXM = "";// 
        public static string CMZQZRQ = "";
        public static string CMZYJ = "";
        public static string ZRXM = "";// 
        public static string CWHQZRQ = "";// 
        public static string CWHYJ = "";// 
        public static string KCR = "";// 
        public static string KCRQ = "";// 
        public static string XCKCYJ = "";// 

        public static string XZGHBMFZR = "";// 
        public static string XZGHBMQZRQ = "";// 
        public static string XZGHBMYJ = "";// 
        public static string XZJWFZR = "";// 
        public static string XZJWQZRQ = "";// 
        public static string XZJWGHYJ = "";// 
        public static string XZZFSPRQ = "";// 
        public static string XZZFSPYJ = "";// 
        public static string XGTBMSCR = "";// 
        public static string XGTBMSCRQ = "";// 
        public static string XGTBMSCYJ = "";// 
        public static string XZFSCRQ = "";// 
        public static string XZFSCYJ = "";// 
        public static string SQLY = "";// 申请理由
        
        #endregion
    }
}
