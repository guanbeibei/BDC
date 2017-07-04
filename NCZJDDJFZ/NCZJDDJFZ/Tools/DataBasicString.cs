
namespace NCZJDDJFZ.Tools
{
    class DataBasicString
    {
        #region 修改调查表字符串
        public static string Update_DCB = "UPDATE DCB SET "
                                   // + "DJH = @DJH "
                                    + " WZDJH = @WZDJH "
                                    + ", SFTXSBB = @SFTXSBB "
                                    + ", Xmax = @Xmax "
                                    + ", Xmin = @Xmin "
                                    + ", Ymax = @Ymax "
                                    + ", Ymin = @Ymin "
                                    + ", JZXJB = @JZXJB "
                                    + ", SQSBH = @SQSBH "
                                    + ", QLR = @QLR "
                                    + ", SQRLX = @SQRLX "
                                    + ", QLR_ZJZL = @QLR_ZJZL "
                                    + ", QLR_ZJBH = @QLR_ZJBH "
                                    + ", DWXZ = @DWXZ "
                                    + ", FRDBXM = @FRDBXM "
                                    + ", TXDZ = @TXDZ "
                                    + ", YZBM = @YZBM "
                                    + ", DZYJ = @DZYJ "
                                    + ", LXR = @LXR "
                                    + ", LXDH = @LXDH "
                                    + ", SQS_DLRXM = @SQS_DLRXM "
                                    + ", ZYZGZSH = @ZYZGZSH "
                                    + ", DLJGMC = @DLJGMC "
                                    + ", DLJGDH = @DLJGDH "
                                    + ", SQRQ = @SQRQ "
                                    + ", QLR2 = @QLR2 "
                                    + ", SQRLX2 = @SQRLX2 "
                                    + ", QLR_ZJZL2 = @QLR_ZJZL2 "
                                    + ", QLR_ZJBH2 = @QLR_ZJBH2 "
                                    + ", DWXZ2 = @DWXZ2 "
                                    + ", FRDBXM2 = @FRDBXM2 "
                                    + ", TXDZ2 = @TXDZ2 "
                                    + ", YZBM2 = @YZBM2 "
                                    + ", DZYJ2 = @DZYJ2 "
                                    + ", LXR2 = @LXR2 "
                                    + ", LXDH2 = @LXDH2 "
                                    + ", SQS_DLRXM2 = @SQS_DLRXM2 "
                                    + ", ZYZGZSH2 = @ZYZGZSH2 "
                                    + ", DLJGMC2 = @DLJGMC2 "
                                    + ", DLJGDH2 = @DLJGDH2 "
                                    + ", TDZL = @TDZL "
                                    + ", ZDMJ = @ZDMJ "
                                    + ", SJYT = @SJYT "
                                    + ", QSXZ = @QSXZ "
                                    + ", DLBM = @DLBM "
                                    + ", SYQLX = @SYQLX "
                                    + ", QLSLQJ = @QLSLQJ "
                                    + ", XYDZL = @XYDZL "
                                    + ", TDJG = @TDJG "
                                    + ", TDDYMJ = @TDDYMJ "
                                    + ", DYSX = @DYSX "
                                    + ", TDDYJE = @TDDYJE "
                                    + ", DYQX_Q = @DYQX_Q "
                                    + ", DYQX_Z = @DYQX_Z "
                                    + ", SQDJLR = @SQDJLR "
                                    + ", SQSBZ = @SQSBZ "
                                    + ", DYSQRJZRQ = @DYSQRJZRQ "
                                    + ", DESQRJZRQ = @DESQRJZRQ "
                                    + ", DCBRQ = @DCBRQ "
                                    + ", FRDBZJBH = @FRDBZJBH "
                                    + ", FRDBZJZL = @FRDBZJZL "
                                    + ", FRDBDH = @FRDBDH "
                                    + ", JBB_DLRXM = @JBB_DLRXM "
                                    + ", JBB_DLRZJBH = @JBB_DLRZJBH "
                                    + ", JBB_DLRZJZL = @JBB_DLRZJZL "
                                    + ", JBB_DLRDH = @JBB_DLRDH "
                                    + ", GMJJHYDM = @GMJJHYDM "
                                    + ", YBZDDM = @YBZDDM "
                                    + ", TFH = @TFH "
                                    + ", PZYT = @PZYT "
                                    + ", PZYT_DLDM = @PZYT_DLDM "
                                    + ", JZZDMJ = @JZZDMJ "
                                    + ", JZMJ = @JZMJ "
                                    + ", SYQX_Q = @SYQX_Q "
                                    + ", SYQX_Z = @SYQX_Z "
                                    + ", GYQLRQK = @GYQLRQK "
                                    + ", SM = @SM "
                                    + ", BZ = @BZ "
                                    + ", DZ = @DZ "
                                    + ", NZ = @NZ "
                                    + ", XZ = @XZ "
                                    + ", QSDCJS = @QSDCJS "
                                    + ", DCYXM = @DCYXM "
                                    + ", DCRQ = @DCRQ "
                                    + ", DJKZJS = @DJKZJS "
                                    + ", CLYXM = @CLYXM "
                                    + ", CLRQ = @CLRQ "
                                    + ", DJDCJGSHYJ = @DJDCJGSHYJ "
                                    + ", SHRXM = @SHRXM "
                                    + ", SHRQ = @SHRQ "
                                    + ", SHHG = @SHHG "
                                    + ", FZMJ = @FZMJ "
                                    + ", CSYJ = @CSYJ "
                                    + ", CSSCR = @CSSCR "
                                    + ", CSRJ = @CSRJ "
                                    + ", CSRTDDJSGZH = @CSRTDDJSGZH "
                                    + ", CSHG = @CSHG "
                                    + ", SHYJ = @SHYJ "
                                    + ", SHFZR = @SHFZR "
                                    + ", SHRQ_SPB = @SHRQ_SPB "
                                    + ", SHRTDDJSGZH = @SHRTDDJSGZH "
                                    + ", SHTG = @SHTG "
                                    + ", PZYJ = @PZYJ "
                                    + ", PZR = @PZR "
                                    + ", PZRQ = @PZRQ "
                                    + ", PZTG = @PZTG "
                                    + ", SPBH = @SPBH "
                                    + ", SPBBZ = @SPBBZ "
                                    + ", SBB_BH = @SBB_BH "
                                    + ", SBB_TBRQ = @SBB_TBRQ "
                                    + ", JTRK = @JTRK "
                                    + ", RKJG = @RKJG "
                                    + ", YYLFJS = @YYLFJS "
                                    + ", YYLFMJ = @YYLFMJ "
                                    + ", YYPFJS = @YYPFJS "
                                    + ", YYPFMJ = @YYPFMJ "
                                    + ", LJLFJS = @LJLFJS "
                                    + ", LJLFMJ = @LJLFMJ "
                                    + ", LJPFJS = @LJPFJS "
                                    + ", LJPFMJ = @LJPFMJ "
                                    + ", STMJ = @STMJ "
                                    + ", HDMJ = @HDMJ "
                                    + ", CDMJ = @CDMJ "
                                    + ", YZJDMJ = @YZJDMJ "
                                    + ", KXDMJ = @KXDMJ "
                                    + ", HSMJ = @HSMJ "
                                    + ", CMZYJ = @CMZYJ "
                                    + ", ZZXM = @ZZXM "
                                    + ", CMZQZRQ = @CMZQZRQ "
                                    + ", CWHYJ = @CWHYJ "
                                    + ", ZRXM = @ZRXM "
                                    + ", CWHQZRQ = @CWHQZRQ "
                                    + ", XCKCYJ = @XCKCYJ "
                                    + ", KCR = @KCR "
                                    + ", KCRQ = @KCRQ "
                                    + ", XZGHBMYJ = @XZGHBMYJ "
                                    + ", XZGHBMFZR = @XZGHBMFZR "
                                    + ", XZGHBMQZRQ = @XZGHBMQZRQ "
                                    + ", XZJWGHYJ = @XZJWGHYJ "
                                    + ", XZJWFZR = @XZJWFZR "
                                    + ", XZJWQZRQ = @XZJWQZRQ "
                                    + ", XZZFSPYJ = @XZZFSPYJ "
                                    + ", XZZFSPRQ = @XZZFSPRQ "
                                    + ", XGTBMSCYJ = @XGTBMSCYJ "
                                    + ", XGTBMSCR = @XGTBMSCR "
                                    + ", XGTBMSCRQ = @XGTBMSCRQ "
                                    + ", XZFSCYJ = @XZFSCYJ "
                                    + ", XZFSCRQ = @XZFSCRQ "
                                    + ", SBBSFDY = @SBBSFDY "
                                    + ", TDZ_JY = @TDZ_JY "
                                    + ", TDZ_LF = @TDZ_LF "
                                    + ", TDZ_BH = @TDZ_BH "
                                    + ", FZRQ = @FZRQ "
                                    + ", FZJS = @FZJS "
                                    + ", DYMJ = @DYMJ "
                                    + ", FTMJ = @FTMJ "
                                    + ", ZZRQ = @ZZRQ "
                                    + ", TDZSFDY = @TDZSFDY "
                                    + ", GHKBH = @GHKBH "
                                    + ", GHKDYRQ = @GHKDYRQ "
                                    + ", GHKSFDY = @GHKSFDY "
                                    + ", GHKJBR = @GHKJBR "
                                    + ", DJKSFDY = @DJKSFDY "
                                    + ", TEXTBZ1 = @TEXTBZ1 "
                                    + ", TEXTBZ2 = @TEXTBZ2 "
                                    + ", TEXTBZ3 = @TEXTBZ3 "
                                    + ", FBZ1 = @FBZ1 "
                                    + ", FBZ2 = @FBZ2 "
                                    + ", FBZ3 = @FBZ3 "
                                    + ", IBZ1 = @IBZ1 "
                                    + ", IBZ2 = @IBZ2 "
                                    + ", IBZ3 = @IBZ3 "
                                    + " WHERE  DJH = @DJH";
        #endregion
        #region 新增调查表字符串
        // <summary>
        /// 新增调查表字符串
        /// </summary>
        public static string Insert_DCB = "insert into DCB("
                                    + "DJH,WZDJH,SFTXSBB,Xmax,Xmin,Ymax,Ymin,JZXJB,SQSBH,QLR,"
                                    + "SQRLX,QLR_ZJZL,QLR_ZJBH,DWXZ,FRDBXM,TXDZ,YZBM,DZYJ,LXR,LXDH,"
                                    + "SQS_DLRXM,ZYZGZSH,DLJGMC,DLJGDH,SQRQ,QLR2,SQRLX2,QLR_ZJZL2,QLR_ZJBH2,DWXZ2,"
                                    + "FRDBXM2,TXDZ2,YZBM2,DZYJ2,LXR2,LXDH2,SQS_DLRXM2,ZYZGZSH2,DLJGMC2,DLJGDH2,"
                                    + "TDZL,ZDMJ,SJYT,QSXZ,DLBM,SYQLX,QLSLQJ,XYDZL,TDJG,TDDYMJ,"
                                    + "DYSX,TDDYJE,DYQX_Q,DYQX_Z,SQDJLR,SQSBZ,DYSQRJZRQ,DESQRJZRQ,DCBRQ,FRDBZJBH,"
                                    + "FRDBZJZL,FRDBDH,JBB_DLRXM,JBB_DLRZJBH,JBB_DLRZJZL,JBB_DLRDH,GMJJHYDM,YBZDDM,TFH,PZYT,"
                                    + "PZYT_DLDM,JZZDMJ,JZMJ,SYQX_Q,SYQX_Z,GYQLRQK,SM,BZ,DZ,NZ,"
                                    + "XZ,QSDCJS,DCYXM,DCRQ,DJKZJS,CLYXM,CLRQ,DJDCJGSHYJ,SHRXM,SHRQ,"
                                    + "SHHG,FZMJ,CSYJ,CSSCR,CSRJ,CSRTDDJSGZH,CSHG,SHYJ,SHFZR,SHRQ_SPB,"
                                    + "SHRTDDJSGZH,SHTG,PZYJ,PZR,PZRQ,PZTG,SPBH,SPBBZ,SBB_BH,SBB_TBRQ,"
                                    + "JTRK,RKJG,YYLFJS,YYLFMJ,YYPFJS,YYPFMJ,LJLFJS,LJLFMJ,LJPFJS,LJPFMJ,"
                                    + "STMJ,HDMJ,CDMJ,YZJDMJ,KXDMJ,HSMJ,CMZYJ,ZZXM,CMZQZRQ,CWHYJ,"
                                    + "ZRXM,CWHQZRQ,XCKCYJ,KCR,KCRQ,XZGHBMYJ,XZGHBMFZR,XZGHBMQZRQ,XZJWGHYJ,XZJWFZR,"
                                    + "XZJWQZRQ,XZZFSPYJ,XZZFSPRQ,XGTBMSCYJ,XGTBMSCR,XGTBMSCRQ,XZFSCYJ,XZFSCRQ,SBBSFDY,TDZ_JY,"
                                    + "TDZ_LF,TDZ_BH,FZRQ,FZJS,DYMJ,FTMJ,ZZRQ,TDZSFDY,GHKBH,GHKDYRQ,"
                                    + "GHKSFDY,GHKJBR,DJKSFDY,TEXTBZ1,TEXTBZ2,TEXTBZ3,FBZ1,FBZ2,FBZ3,IBZ1,"
                                    + "IBZ2,IBZ3)"
                                    + " values "
                                    + "(@DJH,@WZDJH,@SFTXSBB,@Xmax,@Xmin,@Ymax,@Ymin,@JZXJB,@SQSBH,@QLR,"
                                    + "@SQRLX,@QLR_ZJZL,@QLR_ZJBH,@DWXZ,@FRDBXM,@TXDZ,@YZBM,@DZYJ,@LXR,@LXDH,"
                                     + "@SQS_DLRXM,@ZYZGZSH,@DLJGMC,@DLJGDH,@SQRQ,@QLR2,@SQRLX2,@QLR_ZJZL2,@QLR_ZJBH2,@DWXZ2,"
                                     + "@FRDBXM2,@TXDZ2,@YZBM2,@DZYJ2,@LXR2,@LXDH2,@SQS_DLRXM2,@ZYZGZSH2,@DLJGMC2,@DLJGDH2,"
                                     + "@TDZL,@ZDMJ,@SJYT,@QSXZ,@DLBM,@SYQLX,@QLSLQJ,@XYDZL,@TDJG,@TDDYMJ,"
                                     + "@DYSX,@TDDYJE,@DYQX_Q,@DYQX_Z,@SQDJLR,@SQSBZ,@DYSQRJZRQ,@DESQRJZRQ,@DCBRQ,@FRDBZJBH,"
                                    + "@FRDBZJZL,@FRDBDH,@JBB_DLRXM,@JBB_DLRZJBH,@JBB_DLRZJZL,@JBB_DLRDH,@GMJJHYDM,@YBZDDM,@TFH,@PZYT,"
                                    + "@PZYT_DLDM,@JZZDMJ,@JZMJ,@SYQX_Q,@SYQX_Z,@GYQLRQK,@SM,@BZ,@DZ,@NZ,"
                                    + "@XZ,@QSDCJS,@DCYXM,@DCRQ,@DJKZJS,@CLYXM,@CLRQ,@DJDCJGSHYJ,@SHRXM,@SHRQ,"
                                    + "@SHHG,@FZMJ,@CSYJ,@CSSCR,@CSRJ,@CSRTDDJSGZH,@CSHG,@SHYJ,@SHFZR,@SHRQ_SPB,"
                                    + "@SHRTDDJSGZH,@SHTG,@PZYJ,@PZR,@PZRQ,@PZTG,@SPBH,@SPBBZ,@SBB_BH,@SBB_TBRQ,"
                                    + "@JTRK,@RKJG,@YYLFJS,@YYLFMJ,@YYPFJS,@YYPFMJ,@LJLFJS,@LJLFMJ,@LJPFJS,@LJPFMJ,"
                                    + "@STMJ,@HDMJ,@CDMJ,@YZJDMJ,@KXDMJ,@HSMJ,@CMZYJ,@ZZXM,@CMZQZRQ,@CWHYJ,"
                                    + "@ZRXM,@CWHQZRQ,@XCKCYJ,@KCR,@KCRQ,@XZGHBMYJ,@XZGHBMFZR,@XZGHBMQZRQ,@XZJWGHYJ,@XZJWFZR,"
                                    + "@XZJWQZRQ,@XZZFSPYJ,@XZZFSPRQ,@XGTBMSCYJ,@XGTBMSCR,@XGTBMSCRQ,@XZFSCYJ,@XZFSCRQ,@SBBSFDY,@TDZ_JY,"
                                     + "@TDZ_LF,@TDZ_BH,@FZRQ,@FZJS,@DYMJ,@FTMJ,@ZZRQ,@TDZSFDY,@GHKBH,@GHKDYRQ,"
                                     + "@GHKSFDY,@GHKJBR,@DJKSFDY,@TEXTBZ1,@TEXTBZ2,@TEXTBZ3,@FBZ1,@FBZ2,@FBZ3,@IBZ1,"
                                     + "@IBZ2,@IBZ3)";
        #endregion
        #region 创建调查表字符串
        /// <summary>
        /// 创建调查表字符串
        /// </summary>
        public static string Create_DCB = "CREATE TABLE DCB " +
                                    "(DJH char(17) NOT NULL  PRIMARY KEY," + //地籍号
                                    "WZDJH char(19) ," + //完整地籍号
                                    "SFTXSBB bit ," + //是否填写申报表
                                    "Xmax float ," + //X最大坐标
                                    "Xmin float ," + //X最小坐标
                                    "Ymax float ," + //Y最大坐标
                                    "Ymin float ," + //Y最小坐标
                                    "JZXJB char(20) ," + //界址线句柄
                                    "SQSBH char(11) ," + //申请书编号
                                    "QLR char(60) ," + //权利人
                                    "SQRLX char(20) ," + //第一申请人类型（权利人）
                                    "QLR_ZJZL char(20) ," + //权利人证件种类
                                    "QLR_ZJBH char(20) ," + //权利人证件编号
                                    "DWXZ char(14) ," + //单位性质
                                    "FRDBXM char(30) ," + //法人代表姓名
                                    "TXDZ nvarchar(100) ," + //通信地址
                                    "YZBM char(6) ," + //邮政编码
                                    "DZYJ char(40) ," + //电子邮件
                                    "LXR char(60) ," + //联系人
                                    "LXDH char(15) ," + //联系电话
                                    "SQS_DLRXM char(60) ," + //申请书代理人姓名
                                    "ZYZGZSH char(20) ," + //职业资格证书号
                                    "DLJGMC char(60) ," + //代理机构名称
                                    "DLJGDH char(15) ," + //代理机构电话
                                    "SQRQ char(14) ," + //申请日期
                                    "QLR2 char(60) ," + //第二申请人
                                    "SQRLX2 char(20) ," + //第二申请人类型（权利人）
                                    "QLR_ZJZL2 char(20) ," + //第二申请人证件种类
                                    "QLR_ZJBH2 char(20) ," + //第二申请人证件编号
                                    "DWXZ2 char(14) ," + //第二申请人单位性质
                                    "FRDBXM2 char(30) ," + //第二申请人法人代表姓名
                                    "TXDZ2 nvarchar(100) ," + //第二申请人通信地址
                                    "YZBM2 char(6) ," + //第二申请人邮政编码
                                    "DZYJ2 char(40) ," + //第二申请人电子邮件
                                    "LXR2 char(60) ," + //第二申请人联系人
                                    "LXDH2 char(15) ," + //第二申请人联系电话
                                    "SQS_DLRXM2 char(60) ," + //第二申请人申请书代理人姓名
                                    "ZYZGZSH2 char(20) ," + //第二申请人职业资格证书号
                                    "DLJGMC2 char(60) ," + //第二申请人代理机构名称
                                    "DLJGDH2 char(15) ," + //第二申请人代理机构电话
                                    "TDZL nvarchar(100) ," + //土地坐落
                                    "ZDMJ float ," + //宗地面积
                                    "SJYT char(20) ," + //实际用途
                                    "QSXZ char(30) ," + //权属性质
                                    "DLBM char(3) ," + //地类编码
                                    "SYQLX char(20) ," + //使用权类型
                                    "QLSLQJ char(4) ," + //权利设立情况
                                    "XYDZL nvarchar(100) ," + //需役地坐落
                                    "TDJG float ," + //土地价格
                                    "TDDYMJ float ," + //土地抵押面积
                                    "DYSX char(20) ," + //抵押顺序
                                    "TDDYJE float ," + //土地抵押金额
                                    "DYQX_Q char(14) ," + //抵押期限(起）
                                    "DYQX_Z char(14) ," + //抵押期限(止）
                                    "SQDJLR nvarchar(500) ," + //申请登记的内容
                                    "SQSBZ nvarchar(100) ," + //申请书备注
                                    "DYSQRJZRQ char(14) ," + //第一申请人签章日期
                                    "DESQRJZRQ char(14) ," + //第二申请人签章日期
                                    "DCBRQ char(14) ," + //调查表日期
                                    "FRDBZJBH char(20) ," + //法人代表证件编号
                                    "FRDBZJZL char(20) ," + //法人代表证件种类
                                    "FRDBDH char(15) ," + //法人代表电话
                                    "JBB_DLRXM char(30) ," + //基本表代理人姓名
                                    "JBB_DLRZJBH char(20) ," + //基本表代理人证件编号
                                    "JBB_DLRZJZL char(20) ," + //基本表代理人证件种类
                                    "JBB_DLRDH char(15) ," + //基本表代理人电话
                                    "GMJJHYDM char(40) ," + //国民经济行业代码
                                    "YBZDDM char(19) ," + //预编宗地代码
                                    "TFH char(20) ," + //图幅号
                                    "PZYT char(20) ," + //批准用途
                                    "PZYT_DLDM char(3) ," + //地类代码(批准用途)
                                    "JZZDMJ float ," + //建筑占地面积
                                    "JZMJ float ," + //建筑面积
                                    "SYQX_Q char(14) ," + //使用期限(起）
                                    "SYQX_Z char(14) ," + //使用期限(止)
                                    "GYQLRQK nvarchar(500) ," + //共有/共用权利人情况
                                    "SM nvarchar(100) ," + //说明
                                    "BZ char(60) ," + //北至
                                    "DZ char(60) ," + //东至
                                    "NZ char(60) ," + //南至
                                    "XZ char(60) ," + //西至
                                    "QSDCJS nvarchar(500) ," + //权属调查记事
                                    "DCYXM char(30) ," + //调查员姓名
                                    "DCRQ char(14) ," + //调查日期
                                    "DJKZJS nvarchar(500) ," + //地籍测量记事
                                    "CLYXM char(30) ," + //测量员姓名
                                    "CLRQ char(14) ," + //测量日期
                                    "DJDCJGSHYJ nvarchar(500) ," + //地籍调查结果审核意见
                                    "SHRXM char(30) ," + //审核人姓名
                                    "SHRQ char(14) ," + //审核日期
                                    "SHHG bit ," + //审核合格
                                    "FZMJ float ," + //发证面积
                                    "CSYJ nvarchar(500) ," + //国土资源行政主管部门初审意见
                                    "CSSCR char(30) ," + //初审审查人
                                    "CSRJ char(14) ," + //初审日期
                                    "CSRTDDJSGZH char(20) ," + //初审人土地登记上岗资格证号
                                    "CSHG bit ," + //初审合格
                                    "SHYJ nvarchar(500) ," + //国土资源行政主管部门审核意见
                                    "SHFZR char(30) ," + //审核负责人
                                    "SHRQ_SPB char(14) ," + //审核日期
                                    "SHRTDDJSGZH char(20) ," + //审核人土地登记上岗资格证号
                                    "SHTG bit ," + //审核通过
                                    "PZYJ nvarchar(500) ," + //人民政府批准意见
                                    "PZR char(30) ," + //批准人
                                    "PZRQ char(14) ," + //批准日期
                                    "PZTG bit ," + //批准通过
                                    "SPBH char(11) ," + //审批表号
                                    "SPBBZ nvarchar(100) ," + //审批表备注
                                    "SBB_BH char(19) ," + //申报表编号
                                    "SBB_TBRQ char(14) ," + //申报表填表日期
                                    "JTRK int ," + //家庭人口
                                    "RKJG int ," + //人口结构
                                    "YYLFJS int ," + //原有楼房间数
                                    "YYLFMJ float ," + //原有楼房面积
                                    "YYPFJS int ," + //原有平房间数
                                    "YYPFMJ float ," + //原有平房面积
                                    "LJLFJS int ," + //拟建楼房间数
                                    "LJLFMJ float ," + //拟建楼房面积
                                    "LJPFJS int ," + //拟建平房间数
                                    "LJPFMJ float ," + //拟建平房面积
                                    "STMJ float ," + //水田面积
                                    "HDMJ float ," + //旱地面积
                                    "CDMJ float ," + //菜地面积
                                    "YZJDMJ float ," + //原宅基面积
                                    "KXDMJ float ," + //空闲地面积
                                    "HSMJ float ," + //荒山地面积
                                    "CMZYJ nvarchar(500) ," + //村民组意见
                                    "ZZXM char(30) ," + //组长姓名
                                    "CMZQZRQ char(14) ," + //村民组签字日期
                                    "CWHYJ nvarchar(500) ," + //村委会意见
                                    "ZRXM char(30) ," + //主任姓名
                                    "CWHQZRQ char(14) ," + //村委会签字日期
                                    "XCKCYJ nvarchar(500) ," + //现场勘察意见
                                    "KCR char(30) ," + //勘察人
                                    "KCRQ char(14) ," + //勘察日期
                                    "XZGHBMYJ nvarchar(500) ," + //镇规划部门意见
                                    "XZGHBMFZR char(30) ," + //乡镇规划部门负责人
                                    "XZGHBMQZRQ char(14) ," + //乡镇规划签字日期
                                    "XZJWGHYJ nvarchar(500) ," + //县住建委规划意见
                                    "XZJWFZR char(30) ," + //县住建委负责人
                                    "XZJWQZRQ char(14) ," + //县住建委签字日期
                                    "XZZFSPYJ nvarchar(500) ," + //乡(镇)政府审批意见
                                    "XZZFSPRQ char(14) ," + //乡(镇)政府审批日期
                                    "XGTBMSCYJ nvarchar(500) ," + //县国土部门审查意见
                                    "XGTBMSCR char(30) ," + //县国土部门审查人
                                    "XGTBMSCRQ char(14) ," + //县国土部门审查日期
                                    "XZFSCYJ nvarchar(500) ," + //县政府审查意见
                                    "XZFSCRQ char(14) ," + //县政府审查日期
                                    "SBBSFDY bit ," + //申报表是否打印
                                    "TDZ_JY char(10) ," + //土地证“集用”
                                    "TDZ_LF char(10) ," + //土地证“年份”
                                    "TDZ_BH char(11) ," + //土地证编号
                                    "FZRQ char(14) ," + //发证日期
                                    "FZJS nvarchar(500) ," + //发证记事
                                    "DYMJ float ," + //独用面积
                                    "FTMJ float ," + //分摊面积
                                    "ZZRQ char(14) ," + //终止日期
                                    "TDZSFDY bit ," + //土地证是否打印
                                    "GHKBH char(11) ," + //归户卡编号
                                    "GHKDYRQ char(14) ," + //归户卡打印日期
                                    "GHKSFDY bit ," + //归户卡是否打印
                                    "GHKJBR char(30) ," + //归户卡经办人
                                    "DJKSFDY bit ," + //登记卡是否打印
                                    "TEXTBZ1 char(60) ," + //文本备注1
                                    "TEXTBZ2 char(60) ," + //文本备注2
                                    "TEXTBZ3 nvarchar(500) ," + //文本备注3
                                    "FBZ1 float ," + //浮点备注1
                                    "FBZ2 float ," + //浮点备注2
                                    "FBZ3 float ," + //浮点备注3
                                    "IBZ1 int ," + //整型备注1
                                    "IBZ2 int ," + //整型备注2
                                    "IBZ3 int )"; //整型备注3
        #endregion
        #region 创建界址表示表字符串
        public static string Create_D_ZDD = "CREATE TABLE D_ZDD " +
                                        "(DJH char(17)  NOT NULL  PRIMARY KEY(DJH,MBBSM) ," + //地籍号
                                        "MBBSM Int ," + //序号
                                        "JZDH Int ," + //界址点号
                                        "JBZL char(10) ," + //界标种类
                                        "KZBC Float ," + //勘丈边长
                                        "JZXLB char(16) ," + //界址线类别
                                        "JZXWZ char(2) ," + //界址线位置
                                        "SM char(30) ," + //说明
                                        "JZJJ Float ," + //界址间距
                                        "X Float ," + //X坐标
                                        "Y Float ," + //Y坐标
                                        "BZ char(50) )" ; //备注      
        #endregion
        #region 新增界址表示表字符串
        public static string Insert_D_ZDD = "insert into D_ZDD("
                                            + "DJH,MBBSM,JZDH,JBZL,KZBC,JZXLB,JZXWZ,SM,JZJJ,X,"
                                            + "Y,BZ)"
                                            + " values "
                                           + "(@DJH,@MBBSM,@JZDH,@JBZL,@KZBC,@JZXLB,@JZXWZ,@SM,@JZJJ,@X,"
                                            + "@Y,@BZ)";
        #endregion
        #region 修改界址表示表字符串
        public static string Update_D_ZDD = "UPDATE D_ZDD SET "
                                           // + "DJH = @DJH "
                                           // + ", MBBSM = @MBBSM "
                                            + " JZDH = @JZDH "
                                            + ", JBZL = @JBZL "
                                            + ", KZBC = @KZBC "
                                            + ", JZXLB = @JZXLB "
                                            + ", JZXWZ = @JZXWZ "
                                            + ", SM = @SM "
                                            + ", JZJJ = @JZJJ "
                                            + ", X = @X "
                                            + ", Y = @Y "
                                            + ", BZ = @BZ "
                                            + " WHERE  DJH = @DJH AND MBBSM=@MBBSM";
        #endregion
        #region 创建界址签章表字符串
        public static string Create_D_ZDX = "CREATE TABLE D_ZDX " +
                                            "(DJH char(17) NOT NULL  PRIMARY KEY(DJH,MBBSM) ," + //地籍号
                                            "MBBSM Int NOT NULL ," + //排序号
                                            "QDH Int ," + //起点号
                                            "ZJDH Int ," + //中间点号
                                            "ZDH Int ," + //终点号
                                            "LZDDJH char(30) ," + //邻宗地地籍号
                                            "LZDZJRXM char(30) ," + //邻宗地指界人姓名
                                            "BZDZJRXM char(30) ," + //本宗地指界人姓名
                                            "ZJRQ char(14) ," + //指界日期
                                            "BZ char(50))"; //备注
        #endregion
        #region 新增界址签章表字符串
        public static string Insert_D_ZDX = "insert into D_ZDX("
                                            +"DJH,MBBSM,QDH,ZJDH,ZDH,LZDDJH,LZDZJRXM,BZDZJRXM,ZJRQ,BZ)"
                                            + " values "  
                                            +"(@DJH,@MBBSM,@QDH,@ZJDH,@ZDH,@LZDDJH,@LZDZJRXM,@BZDZJRXM,@ZJRQ,@BZ)";
        #endregion
        #region 修改界址签章字符串
        public static string Update_D_ZDX = "UPDATE D_ZDX SET "
                                            //+ "DJH = @DJH "
                                           // + ", MBBSM = @MBBSM "
                                            + " QDH = @QDH "
                                            + ", ZJDH = @ZJDH "
                                            + ", ZDH = @ZDH "
                                            + ", LZDDJH = @LZDDJH "
                                            + ", LZDZJRXM = @LZDZJRXM "
                                            + ", BZDZJRXM = @BZDZJRXM "
                                            + ", ZJRQ = @ZJRQ "
                                            + ", BZ = @BZ "
                                            + " WHERE DJH = @DJH AND MBBSM = @MBBSM";
        #endregion
    }
}
