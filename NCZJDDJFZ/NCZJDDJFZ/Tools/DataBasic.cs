using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace NCZJDDJFZ.Tools
{
    class DataBasic
    {
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="BMC">表名称</param>
        /// <param name="commandTEXT">创建命令字符串</param>
        /// <returns>创建是否成功</returns>
        public static bool Create_DCB_table(string connectionString, string BMC, string commandTEXT)
        {
            bool Iscg = false;
            bool IsTable = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataTable dt = connection.GetSchema("Tables");//获取数据库中所有的表
                    foreach (System.Data.DataRow row in dt.Rows)//遍历表中各行
                    {
                        if (row[2].ToString().ToUpper() == BMC.ToUpper())//找到"DataDic"
                        {
                            IsTable = false;
                            Iscg = true;
                        }
                    }
                    #region 如果数据库中没有《DataDic》表则创建 DataDic表
                    if (IsTable & (connection.State == ConnectionState.Open))
                    {
                        SqlCommand sqllCommand = new SqlCommand(commandTEXT, connection);
                        sqllCommand.ExecuteNonQuery();
                        Iscg = true;
                    }
                    #endregion
                }
                catch (Exception Mess)
                {
                    MessageBox.Show(Mess.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return Iscg;
            }
        }

        /// <summary>
        /// 界址表示表和DataSet匹配
        /// </summary>
        /// <param name="SelectString">条件选择</param>
        /// <param name="Connectionstring">数据连接字符串</param>
        /// <returns>返回SqlDataAdapter实例</returns>
        public static SqlDataAdapter D_ZDD_Initadapter(string SelectString, string Connectionstring)
        {
            SqlConnection Connection1 = new SqlConnection(Connectionstring);
            Connection1.Open();
            SqlDataAdapter dataAdpater = new SqlDataAdapter(SelectString, Connection1);
            dataAdpater.InsertCommand = new SqlCommand(Tools.DataBasicString.Insert_D_ZDD, Connection1);
            //新增
            dataAdpater.InsertCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");
            dataAdpater.InsertCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");
            dataAdpater.InsertCommand.Parameters.Add("@JZDH", SqlDbType.Int, 4, "JZDH");
            dataAdpater.InsertCommand.Parameters.Add("@JBZL", SqlDbType.Char, 10, "JBZL");
            dataAdpater.InsertCommand.Parameters.Add("@KZBC", SqlDbType.Float, 8, "KZBC");
            dataAdpater.InsertCommand.Parameters.Add("@JZXLB", SqlDbType.Char, 16, "JZXLB");
            dataAdpater.InsertCommand.Parameters.Add("@JZXWZ", SqlDbType.Char, 2, "JZXWZ");
            dataAdpater.InsertCommand.Parameters.Add("@SM", SqlDbType.Char, 30, "SM");
            dataAdpater.InsertCommand.Parameters.Add("@JZJJ", SqlDbType.Float, 8, "JZJJ");
            dataAdpater.InsertCommand.Parameters.Add("@X", SqlDbType.Float, 8, "X");
            dataAdpater.InsertCommand.Parameters.Add("@Y", SqlDbType.Float, 8, "Y");
            dataAdpater.InsertCommand.Parameters.Add("@BZ", SqlDbType.Char, 50, "BZ");
            //修改

            dataAdpater.UpdateCommand = new SqlCommand(Tools.DataBasicString.Update_D_ZDD, Connection1);
            //dataAdpater.UpdateCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");
            dataAdpater.UpdateCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");
            dataAdpater.UpdateCommand.Parameters.Add("@JZDH", SqlDbType.Int, 4, "JZDH");
            dataAdpater.UpdateCommand.Parameters.Add("@JBZL", SqlDbType.Char, 10, "JBZL");
            dataAdpater.UpdateCommand.Parameters.Add("@KZBC", SqlDbType.Float, 8, "KZBC");
            dataAdpater.UpdateCommand.Parameters.Add("@JZXLB", SqlDbType.Char, 16, "JZXLB");
            dataAdpater.UpdateCommand.Parameters.Add("@JZXWZ", SqlDbType.Char, 2, "JZXWZ");
            dataAdpater.UpdateCommand.Parameters.Add("@SM", SqlDbType.Char, 30, "SM");
            dataAdpater.UpdateCommand.Parameters.Add("@JZJJ", SqlDbType.Float, 8, "JZJJ");
            dataAdpater.UpdateCommand.Parameters.Add("@X", SqlDbType.Float, 8, "X");
            dataAdpater.UpdateCommand.Parameters.Add("@Y", SqlDbType.Float, 8, "Y");
            dataAdpater.UpdateCommand.Parameters.Add("@BZ", SqlDbType.Char, 50, "BZ");

            SqlParameter parameter2 = dataAdpater.UpdateCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//匹配参数（同 WHERE条件一样的参数）
            parameter2.SourceColumn = "DJH";
            parameter2.SourceVersion = DataRowVersion.Original;

            //删除
            dataAdpater.DeleteCommand = new SqlCommand("delete D_ZDD   WHERE DJH = @DJH AND MBBSM = @MBBSM", Connection1);
            dataAdpater.DeleteCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//状态
            dataAdpater.DeleteCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");//状态
            parameter2.SourceVersion = DataRowVersion.Original;

            //添加映射
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping = dataAdpater.TableMappings.Add("D_ZDD", "D_ZDD");
            tableMapping.DataSetTable = "D_ZDD";
            Connection1.Close();
            return dataAdpater;
        }
        /// <summary>
        /// 界址签章表和DataSet匹配
        /// </summary>
        /// <param name="SelectString">条件选择</param>
        /// <param name="Connectionstring">数据连接字符串</param>
        /// <returns>返回SqlDataAdapter实例</returns>
        public static SqlDataAdapter D_ZDX_Initadapter(string SelectString, string Connectionstring)
        {
            SqlConnection Connection1 = new SqlConnection(Connectionstring);
            Connection1.Open();
            SqlDataAdapter dataAdpater = new SqlDataAdapter(SelectString, Connection1);
            dataAdpater.InsertCommand = new SqlCommand(Tools.DataBasicString.Insert_D_ZDX, Connection1);
            //新增
            dataAdpater.InsertCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");
            dataAdpater.InsertCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");
            dataAdpater.InsertCommand.Parameters.Add("@QDH", SqlDbType.Int, 4, "QDH");
            dataAdpater.InsertCommand.Parameters.Add("@ZJDH", SqlDbType.Int, 4, "ZJDH");
            dataAdpater.InsertCommand.Parameters.Add("@ZDH", SqlDbType.Int, 4, "ZDH");
            dataAdpater.InsertCommand.Parameters.Add("@LZDDJH", SqlDbType.Char, 30, "LZDDJH");
            dataAdpater.InsertCommand.Parameters.Add("@LZDZJRXM", SqlDbType.Char, 30, "LZDZJRXM");
            dataAdpater.InsertCommand.Parameters.Add("@BZDZJRXM", SqlDbType.Char, 30, "BZDZJRXM");
            dataAdpater.InsertCommand.Parameters.Add("@ZJRQ", SqlDbType.Char, 14, "ZJRQ");
            dataAdpater.InsertCommand.Parameters.Add("@BZ", SqlDbType.Char, 50, "BZ");
            //修改
            dataAdpater.UpdateCommand = new SqlCommand(Tools.DataBasicString.Update_D_ZDX, Connection1);
            //dataAdpater.UpdateCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");
            dataAdpater.UpdateCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");
            dataAdpater.UpdateCommand.Parameters.Add("@QDH", SqlDbType.Int, 4, "QDH");
            dataAdpater.UpdateCommand.Parameters.Add("@ZJDH", SqlDbType.Int, 4, "ZJDH");
            dataAdpater.UpdateCommand.Parameters.Add("@ZDH", SqlDbType.Int, 4, "ZDH");
            dataAdpater.UpdateCommand.Parameters.Add("@LZDDJH", SqlDbType.Char, 30, "LZDDJH");
            dataAdpater.UpdateCommand.Parameters.Add("@LZDZJRXM", SqlDbType.Char, 30, "LZDZJRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@BZDZJRXM", SqlDbType.Char, 30, "BZDZJRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@ZJRQ", SqlDbType.Char, 14, "ZJRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@BZ", SqlDbType.Char, 50, "BZ");

            SqlParameter parameter2 = dataAdpater.UpdateCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//匹配参数（同 WHERE条件一样的参数）
            parameter2.SourceColumn = "DJH";
            parameter2.SourceVersion = DataRowVersion.Original;

            //删除
            dataAdpater.DeleteCommand = new SqlCommand("delete D_ZDX   WHERE DJH = @DJH AND MBBSM = @MBBSM", Connection1);
            dataAdpater.DeleteCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//状态
            dataAdpater.DeleteCommand.Parameters.Add("@MBBSM", SqlDbType.Int, 4, "MBBSM");//状态
            parameter2.SourceVersion = DataRowVersion.Original;

            //添加映射
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping = dataAdpater.TableMappings.Add("D_ZDX", "D_ZDX");
            tableMapping.DataSetTable = "D_ZDX";
            Connection1.Close();
            return dataAdpater;
        }
        /// <summary>
        /// 调查表存储过程和DataSet匹配
        /// </summary>
        /// <param name="SelectString">条件选择</param>
        /// <param name="Connectionstring">数据连接字符串</param>
        /// <returns>返回SqlDataAdapter实例</returns>
        public static SqlDataAdapter DCB_Initadapter(string SelectString, string Connectionstring)
        {
            SqlConnection Connection1 = new SqlConnection(Connectionstring);
            Connection1.Open();
            SqlDataAdapter dataAdpater = new SqlDataAdapter(SelectString, Connection1);
            dataAdpater.InsertCommand = new SqlCommand(Tools.DataBasicString.Insert_DCB, Connection1);
            //新增
            dataAdpater.InsertCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");
            dataAdpater.InsertCommand.Parameters.Add("@WZDJH", SqlDbType.Char, 19, "WZDJH");
            dataAdpater.InsertCommand.Parameters.Add("@SFTXSBB", SqlDbType.Bit, 1, "SFTXSBB");
            dataAdpater.InsertCommand.Parameters.Add("@Xmax", SqlDbType.Float, 8, "Xmax");
            dataAdpater.InsertCommand.Parameters.Add("@Xmin", SqlDbType.Float, 8, "Xmin");
            dataAdpater.InsertCommand.Parameters.Add("@Ymax", SqlDbType.Float, 8, "Ymax");
            dataAdpater.InsertCommand.Parameters.Add("@Ymin", SqlDbType.Float, 8, "Ymin");
            dataAdpater.InsertCommand.Parameters.Add("@JZXJB", SqlDbType.Char, 20, "JZXJB");
            dataAdpater.InsertCommand.Parameters.Add("@SQSBH", SqlDbType.Char, 11, "SQSBH");
            dataAdpater.InsertCommand.Parameters.Add("@QLR", SqlDbType.Char, 60, "QLR");
            dataAdpater.InsertCommand.Parameters.Add("@SQRLX", SqlDbType.Char, 20, "SQRLX");
            dataAdpater.InsertCommand.Parameters.Add("@QLR_ZJZL", SqlDbType.Char, 20, "QLR_ZJZL");
            dataAdpater.InsertCommand.Parameters.Add("@QLR_ZJBH", SqlDbType.Char, 20, "QLR_ZJBH");
            dataAdpater.InsertCommand.Parameters.Add("@DWXZ", SqlDbType.Char, 14, "DWXZ");
            dataAdpater.InsertCommand.Parameters.Add("@FRDBXM", SqlDbType.Char, 30, "FRDBXM");
            dataAdpater.InsertCommand.Parameters.Add("@TXDZ", SqlDbType.NVarChar, 100, "TXDZ");
            dataAdpater.InsertCommand.Parameters.Add("@YZBM", SqlDbType.Char, 6, "YZBM");
            dataAdpater.InsertCommand.Parameters.Add("@DZYJ", SqlDbType.Char, 40, "DZYJ");
            dataAdpater.InsertCommand.Parameters.Add("@LXR", SqlDbType.Char, 60, "LXR");
            dataAdpater.InsertCommand.Parameters.Add("@LXDH", SqlDbType.Char, 15, "LXDH");
            dataAdpater.InsertCommand.Parameters.Add("@SQS_DLRXM", SqlDbType.Char, 60, "SQS_DLRXM");
            dataAdpater.InsertCommand.Parameters.Add("@ZYZGZSH", SqlDbType.Char, 20, "ZYZGZSH");
            dataAdpater.InsertCommand.Parameters.Add("@DLJGMC", SqlDbType.Char, 60, "DLJGMC");
            dataAdpater.InsertCommand.Parameters.Add("@DLJGDH", SqlDbType.Char, 15, "DLJGDH");
            dataAdpater.InsertCommand.Parameters.Add("@SQRQ", SqlDbType.Char, 14, "SQRQ");
            dataAdpater.InsertCommand.Parameters.Add("@QLR2", SqlDbType.Char, 60, "QLR2");
            dataAdpater.InsertCommand.Parameters.Add("@SQRLX2", SqlDbType.Char, 20, "SQRLX2");
            dataAdpater.InsertCommand.Parameters.Add("@QLR_ZJZL2", SqlDbType.Char, 20, "QLR_ZJZL2");
            dataAdpater.InsertCommand.Parameters.Add("@QLR_ZJBH2", SqlDbType.Char, 20, "QLR_ZJBH2");
            dataAdpater.InsertCommand.Parameters.Add("@DWXZ2", SqlDbType.Char, 14, "DWXZ2");
            dataAdpater.InsertCommand.Parameters.Add("@FRDBXM2", SqlDbType.Char, 30, "FRDBXM2");
            dataAdpater.InsertCommand.Parameters.Add("@TXDZ2", SqlDbType.NVarChar, 100, "TXDZ2");
            dataAdpater.InsertCommand.Parameters.Add("@YZBM2", SqlDbType.Char, 6, "YZBM2");
            dataAdpater.InsertCommand.Parameters.Add("@DZYJ2", SqlDbType.Char, 40, "DZYJ2");
            dataAdpater.InsertCommand.Parameters.Add("@LXR2", SqlDbType.Char, 60, "LXR2");
            dataAdpater.InsertCommand.Parameters.Add("@LXDH2", SqlDbType.Char, 15, "LXDH2");
            dataAdpater.InsertCommand.Parameters.Add("@SQS_DLRXM2", SqlDbType.Char, 60, "SQS_DLRXM2");
            dataAdpater.InsertCommand.Parameters.Add("@ZYZGZSH2", SqlDbType.Char, 20, "ZYZGZSH2");
            dataAdpater.InsertCommand.Parameters.Add("@DLJGMC2", SqlDbType.Char, 60, "DLJGMC2");
            dataAdpater.InsertCommand.Parameters.Add("@DLJGDH2", SqlDbType.Char, 15, "DLJGDH2");
            dataAdpater.InsertCommand.Parameters.Add("@TDZL", SqlDbType.NVarChar, 100, "TDZL");
            dataAdpater.InsertCommand.Parameters.Add("@ZDMJ", SqlDbType.Float, 8, "ZDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@SJYT", SqlDbType.Char, 20, "SJYT");
            dataAdpater.InsertCommand.Parameters.Add("@QSXZ", SqlDbType.Char, 30, "QSXZ");
            dataAdpater.InsertCommand.Parameters.Add("@DLBM", SqlDbType.Char, 3, "DLBM");
            dataAdpater.InsertCommand.Parameters.Add("@SYQLX", SqlDbType.Char, 20, "SYQLX");
            dataAdpater.InsertCommand.Parameters.Add("@QLSLQJ", SqlDbType.Char, 4, "QLSLQJ");
            dataAdpater.InsertCommand.Parameters.Add("@XYDZL", SqlDbType.NVarChar, 100, "XYDZL");
            dataAdpater.InsertCommand.Parameters.Add("@TDJG", SqlDbType.Float, 8, "TDJG");
            dataAdpater.InsertCommand.Parameters.Add("@TDDYMJ", SqlDbType.Float, 8, "TDDYMJ");
            dataAdpater.InsertCommand.Parameters.Add("@DYSX", SqlDbType.Char, 20, "DYSX");
            dataAdpater.InsertCommand.Parameters.Add("@TDDYJE", SqlDbType.Float, 8, "TDDYJE");
            dataAdpater.InsertCommand.Parameters.Add("@DYQX_Q", SqlDbType.Char, 14, "DYQX_Q");
            dataAdpater.InsertCommand.Parameters.Add("@DYQX_Z", SqlDbType.Char, 14, "DYQX_Z");
            dataAdpater.InsertCommand.Parameters.Add("@SQDJLR", SqlDbType.NVarChar, 500, "SQDJLR");
            dataAdpater.InsertCommand.Parameters.Add("@SQSBZ", SqlDbType.NVarChar, 100, "SQSBZ");
            dataAdpater.InsertCommand.Parameters.Add("@DYSQRJZRQ", SqlDbType.Char, 14, "DYSQRJZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@DESQRJZRQ", SqlDbType.Char, 14, "DESQRJZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@DCBRQ", SqlDbType.Char, 14, "DCBRQ");
            dataAdpater.InsertCommand.Parameters.Add("@FRDBZJBH", SqlDbType.Char, 20, "FRDBZJBH");
            dataAdpater.InsertCommand.Parameters.Add("@FRDBZJZL", SqlDbType.Char, 20, "FRDBZJZL");
            dataAdpater.InsertCommand.Parameters.Add("@FRDBDH", SqlDbType.Char, 15, "FRDBDH");
            dataAdpater.InsertCommand.Parameters.Add("@JBB_DLRXM", SqlDbType.Char, 30, "JBB_DLRXM");
            dataAdpater.InsertCommand.Parameters.Add("@JBB_DLRZJBH", SqlDbType.Char, 20, "JBB_DLRZJBH");
            dataAdpater.InsertCommand.Parameters.Add("@JBB_DLRZJZL", SqlDbType.Char, 20, "JBB_DLRZJZL");
            dataAdpater.InsertCommand.Parameters.Add("@JBB_DLRDH", SqlDbType.Char, 15, "JBB_DLRDH");
            dataAdpater.InsertCommand.Parameters.Add("@GMJJHYDM", SqlDbType.Char, 40, "GMJJHYDM");
            dataAdpater.InsertCommand.Parameters.Add("@YBZDDM", SqlDbType.Char, 19, "YBZDDM");
            dataAdpater.InsertCommand.Parameters.Add("@TFH", SqlDbType.Char, 20, "TFH");
            dataAdpater.InsertCommand.Parameters.Add("@PZYT", SqlDbType.Char, 20, "PZYT");
            dataAdpater.InsertCommand.Parameters.Add("@PZYT_DLDM", SqlDbType.Char, 3, "PZYT_DLDM");
            dataAdpater.InsertCommand.Parameters.Add("@JZZDMJ", SqlDbType.Float, 8, "JZZDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@JZMJ", SqlDbType.Float, 8, "JZMJ");
            dataAdpater.InsertCommand.Parameters.Add("@SYQX_Q", SqlDbType.Char, 14, "SYQX_Q");
            dataAdpater.InsertCommand.Parameters.Add("@SYQX_Z", SqlDbType.Char, 14, "SYQX_Z");
            dataAdpater.InsertCommand.Parameters.Add("@GYQLRQK", SqlDbType.NVarChar, 500, "GYQLRQK");
            dataAdpater.InsertCommand.Parameters.Add("@SM", SqlDbType.NVarChar, 100, "SM");
            dataAdpater.InsertCommand.Parameters.Add("@BZ", SqlDbType.Char, 60, "BZ");
            dataAdpater.InsertCommand.Parameters.Add("@DZ", SqlDbType.Char, 60, "DZ");
            dataAdpater.InsertCommand.Parameters.Add("@NZ", SqlDbType.Char, 60, "NZ");
            dataAdpater.InsertCommand.Parameters.Add("@XZ", SqlDbType.Char, 60, "XZ");
            dataAdpater.InsertCommand.Parameters.Add("@QSDCJS", SqlDbType.NVarChar, 500, "QSDCJS");
            dataAdpater.InsertCommand.Parameters.Add("@DCYXM", SqlDbType.Char, 30, "DCYXM");
            dataAdpater.InsertCommand.Parameters.Add("@DCRQ", SqlDbType.Char, 14, "DCRQ");
            dataAdpater.InsertCommand.Parameters.Add("@DJKZJS", SqlDbType.NVarChar, 500, "DJKZJS");
            dataAdpater.InsertCommand.Parameters.Add("@CLYXM", SqlDbType.Char, 30, "CLYXM");
            dataAdpater.InsertCommand.Parameters.Add("@CLRQ", SqlDbType.Char, 14, "CLRQ");
            dataAdpater.InsertCommand.Parameters.Add("@DJDCJGSHYJ", SqlDbType.NVarChar, 500, "DJDCJGSHYJ");
            dataAdpater.InsertCommand.Parameters.Add("@SHRXM", SqlDbType.Char, 30, "SHRXM");
            dataAdpater.InsertCommand.Parameters.Add("@SHRQ", SqlDbType.Char, 14, "SHRQ");
            dataAdpater.InsertCommand.Parameters.Add("@SHHG", SqlDbType.Bit, 1, "SHHG");
            dataAdpater.InsertCommand.Parameters.Add("@FZMJ", SqlDbType.Float, 8, "FZMJ");
            dataAdpater.InsertCommand.Parameters.Add("@CSYJ", SqlDbType.NVarChar, 500, "CSYJ");
            dataAdpater.InsertCommand.Parameters.Add("@CSSCR", SqlDbType.Char, 30, "CSSCR");
            dataAdpater.InsertCommand.Parameters.Add("@CSRJ", SqlDbType.Char, 14, "CSRJ");
            dataAdpater.InsertCommand.Parameters.Add("@CSRTDDJSGZH", SqlDbType.Char, 20, "CSRTDDJSGZH");
            dataAdpater.InsertCommand.Parameters.Add("@CSHG", SqlDbType.Bit, 1, "CSHG");
            dataAdpater.InsertCommand.Parameters.Add("@SHYJ", SqlDbType.NVarChar, 500, "SHYJ");
            dataAdpater.InsertCommand.Parameters.Add("@SHFZR", SqlDbType.Char, 30, "SHFZR");
            dataAdpater.InsertCommand.Parameters.Add("@SHRQ_SPB", SqlDbType.Char, 14, "SHRQ_SPB");
            dataAdpater.InsertCommand.Parameters.Add("@SHRTDDJSGZH", SqlDbType.Char, 20, "SHRTDDJSGZH");
            dataAdpater.InsertCommand.Parameters.Add("@SHTG", SqlDbType.Bit, 1, "SHTG");
            dataAdpater.InsertCommand.Parameters.Add("@PZYJ", SqlDbType.NVarChar, 500, "PZYJ");
            dataAdpater.InsertCommand.Parameters.Add("@PZR", SqlDbType.Char, 30, "PZR");
            dataAdpater.InsertCommand.Parameters.Add("@PZRQ", SqlDbType.Char, 14, "PZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@PZTG", SqlDbType.Bit, 1, "PZTG");
            dataAdpater.InsertCommand.Parameters.Add("@SPBH", SqlDbType.Char, 11, "SPBH");
            dataAdpater.InsertCommand.Parameters.Add("@SPBBZ", SqlDbType.NVarChar, 100, "SPBBZ");
            dataAdpater.InsertCommand.Parameters.Add("@SBB_BH", SqlDbType.Char, 19, "SBB_BH");
            dataAdpater.InsertCommand.Parameters.Add("@SBB_TBRQ", SqlDbType.Char, 14, "SBB_TBRQ");
            dataAdpater.InsertCommand.Parameters.Add("@JTRK", SqlDbType.Int, 4, "JTRK");
            dataAdpater.InsertCommand.Parameters.Add("@RKJG", SqlDbType.Int, 4, "RKJG");
            dataAdpater.InsertCommand.Parameters.Add("@YYLFJS", SqlDbType.Int, 4, "YYLFJS");
            dataAdpater.InsertCommand.Parameters.Add("@YYLFMJ", SqlDbType.Float, 8, "YYLFMJ");
            dataAdpater.InsertCommand.Parameters.Add("@YYPFJS", SqlDbType.Int, 4, "YYPFJS");
            dataAdpater.InsertCommand.Parameters.Add("@YYPFMJ", SqlDbType.Float, 8, "YYPFMJ");
            dataAdpater.InsertCommand.Parameters.Add("@LJLFJS", SqlDbType.Int, 4, "LJLFJS");
            dataAdpater.InsertCommand.Parameters.Add("@LJLFMJ", SqlDbType.Float, 8, "LJLFMJ");
            dataAdpater.InsertCommand.Parameters.Add("@LJPFJS", SqlDbType.Int, 4, "LJPFJS");
            dataAdpater.InsertCommand.Parameters.Add("@LJPFMJ", SqlDbType.Float, 8, "LJPFMJ");
            dataAdpater.InsertCommand.Parameters.Add("@STMJ", SqlDbType.Float, 8, "STMJ");
            dataAdpater.InsertCommand.Parameters.Add("@HDMJ", SqlDbType.Float, 8, "HDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@CDMJ", SqlDbType.Float, 8, "CDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@YZJDMJ", SqlDbType.Float, 8, "YZJDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@KXDMJ", SqlDbType.Float, 8, "KXDMJ");
            dataAdpater.InsertCommand.Parameters.Add("@HSMJ", SqlDbType.Float, 8, "HSMJ");
            dataAdpater.InsertCommand.Parameters.Add("@CMZYJ", SqlDbType.NVarChar, 500, "CMZYJ");
            dataAdpater.InsertCommand.Parameters.Add("@ZZXM", SqlDbType.Char, 30, "ZZXM");
            dataAdpater.InsertCommand.Parameters.Add("@CMZQZRQ", SqlDbType.Char, 14, "CMZQZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@CWHYJ", SqlDbType.NVarChar, 500, "CWHYJ");
            dataAdpater.InsertCommand.Parameters.Add("@ZRXM", SqlDbType.Char, 30, "ZRXM");
            dataAdpater.InsertCommand.Parameters.Add("@CWHQZRQ", SqlDbType.Char, 14, "CWHQZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XCKCYJ", SqlDbType.NVarChar, 500, "XCKCYJ");
            dataAdpater.InsertCommand.Parameters.Add("@KCR", SqlDbType.Char, 30, "KCR");
            dataAdpater.InsertCommand.Parameters.Add("@KCRQ", SqlDbType.Char, 14, "KCRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XZGHBMYJ", SqlDbType.NVarChar, 500, "XZGHBMYJ");
            dataAdpater.InsertCommand.Parameters.Add("@XZGHBMFZR", SqlDbType.Char, 30, "XZGHBMFZR");
            dataAdpater.InsertCommand.Parameters.Add("@XZGHBMQZRQ", SqlDbType.Char, 14, "XZGHBMQZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XZJWGHYJ", SqlDbType.NVarChar, 500, "XZJWGHYJ");
            dataAdpater.InsertCommand.Parameters.Add("@XZJWFZR", SqlDbType.Char, 30, "XZJWFZR");
            dataAdpater.InsertCommand.Parameters.Add("@XZJWQZRQ", SqlDbType.Char, 14, "XZJWQZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XZZFSPYJ", SqlDbType.NVarChar, 500, "XZZFSPYJ");
            dataAdpater.InsertCommand.Parameters.Add("@XZZFSPRQ", SqlDbType.Char, 14, "XZZFSPRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XGTBMSCYJ", SqlDbType.NVarChar, 500, "XGTBMSCYJ");
            dataAdpater.InsertCommand.Parameters.Add("@XGTBMSCR", SqlDbType.Char, 30, "XGTBMSCR");
            dataAdpater.InsertCommand.Parameters.Add("@XGTBMSCRQ", SqlDbType.Char, 14, "XGTBMSCRQ");
            dataAdpater.InsertCommand.Parameters.Add("@XZFSCYJ", SqlDbType.NVarChar, 500, "XZFSCYJ");
            dataAdpater.InsertCommand.Parameters.Add("@XZFSCRQ", SqlDbType.Char, 14, "XZFSCRQ");
            dataAdpater.InsertCommand.Parameters.Add("@SBBSFDY", SqlDbType.Bit, 1, "SBBSFDY");
            dataAdpater.InsertCommand.Parameters.Add("@TDZ_JY", SqlDbType.Char, 10, "TDZ_JY");
            dataAdpater.InsertCommand.Parameters.Add("@TDZ_LF", SqlDbType.Char, 10, "TDZ_LF");
            dataAdpater.InsertCommand.Parameters.Add("@TDZ_BH", SqlDbType.Char, 11, "TDZ_BH");
            dataAdpater.InsertCommand.Parameters.Add("@FZRQ", SqlDbType.Char, 14, "FZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@FZJS", SqlDbType.NVarChar, 500, "FZJS");
            dataAdpater.InsertCommand.Parameters.Add("@DYMJ", SqlDbType.Float, 8, "DYMJ");
            dataAdpater.InsertCommand.Parameters.Add("@FTMJ", SqlDbType.Float, 8, "FTMJ");
            dataAdpater.InsertCommand.Parameters.Add("@ZZRQ", SqlDbType.Char, 14, "ZZRQ");
            dataAdpater.InsertCommand.Parameters.Add("@TDZSFDY", SqlDbType.Bit, 1, "TDZSFDY");
            dataAdpater.InsertCommand.Parameters.Add("@GHKBH", SqlDbType.Char, 11, "GHKBH");
            dataAdpater.InsertCommand.Parameters.Add("@GHKDYRQ", SqlDbType.Char, 14, "GHKDYRQ");
            dataAdpater.InsertCommand.Parameters.Add("@GHKSFDY", SqlDbType.Bit, 1, "GHKSFDY");
            dataAdpater.InsertCommand.Parameters.Add("@GHKJBR", SqlDbType.Char, 30, "GHKJBR");
            dataAdpater.InsertCommand.Parameters.Add("@DJKSFDY", SqlDbType.Bit, 1, "DJKSFDY");
            dataAdpater.InsertCommand.Parameters.Add("@TEXTBZ1", SqlDbType.Char, 60, "TEXTBZ1");
            dataAdpater.InsertCommand.Parameters.Add("@TEXTBZ2", SqlDbType.Char, 60, "TEXTBZ2");
            dataAdpater.InsertCommand.Parameters.Add("@TEXTBZ3", SqlDbType.NVarChar, 500, "TEXTBZ3");
            dataAdpater.InsertCommand.Parameters.Add("@FBZ1", SqlDbType.Float, 8, "FBZ1");
            dataAdpater.InsertCommand.Parameters.Add("@FBZ2", SqlDbType.Float, 8, "FBZ2");
            dataAdpater.InsertCommand.Parameters.Add("@FBZ3", SqlDbType.Float, 8, "FBZ3");
            dataAdpater.InsertCommand.Parameters.Add("@IBZ1", SqlDbType.Int, 4, "IBZ1");
            dataAdpater.InsertCommand.Parameters.Add("@IBZ2", SqlDbType.Int, 4, "IBZ2");
            dataAdpater.InsertCommand.Parameters.Add("@IBZ3", SqlDbType.Int, 4, "IBZ3");
            //修改
            //修改
            dataAdpater.UpdateCommand = new SqlCommand(Tools.DataBasicString.Update_DCB, Connection1);
            dataAdpater.UpdateCommand.Parameters.Add("@WZDJH", SqlDbType.Char, 19, "WZDJH");
            dataAdpater.UpdateCommand.Parameters.Add("@SFTXSBB", SqlDbType.Bit, 1, "SFTXSBB");
            dataAdpater.UpdateCommand.Parameters.Add("@Xmax", SqlDbType.Float, 8, "Xmax");
            dataAdpater.UpdateCommand.Parameters.Add("@Xmin", SqlDbType.Float, 8, "Xmin");
            dataAdpater.UpdateCommand.Parameters.Add("@Ymax", SqlDbType.Float, 8, "Ymax");
            dataAdpater.UpdateCommand.Parameters.Add("@Ymin", SqlDbType.Float, 8, "Ymin");
            dataAdpater.UpdateCommand.Parameters.Add("@JZXJB", SqlDbType.Char, 20, "JZXJB");
            dataAdpater.UpdateCommand.Parameters.Add("@SQSBH", SqlDbType.Char, 11, "SQSBH");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR", SqlDbType.Char, 60, "QLR");
            dataAdpater.UpdateCommand.Parameters.Add("@SQRLX", SqlDbType.Char, 20, "SQRLX");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR_ZJZL", SqlDbType.Char, 20, "QLR_ZJZL");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR_ZJBH", SqlDbType.Char, 20, "QLR_ZJBH");
            dataAdpater.UpdateCommand.Parameters.Add("@DWXZ", SqlDbType.Char, 14, "DWXZ");
            dataAdpater.UpdateCommand.Parameters.Add("@FRDBXM", SqlDbType.Char, 30, "FRDBXM");
            dataAdpater.UpdateCommand.Parameters.Add("@TXDZ", SqlDbType.NVarChar, 100, "TXDZ");
            dataAdpater.UpdateCommand.Parameters.Add("@YZBM", SqlDbType.Char, 6, "YZBM");
            dataAdpater.UpdateCommand.Parameters.Add("@DZYJ", SqlDbType.Char, 40, "DZYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@LXR", SqlDbType.Char, 60, "LXR");
            dataAdpater.UpdateCommand.Parameters.Add("@LXDH", SqlDbType.Char, 15, "LXDH");
            dataAdpater.UpdateCommand.Parameters.Add("@SQS_DLRXM", SqlDbType.Char, 60, "SQS_DLRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@ZYZGZSH", SqlDbType.Char, 20, "ZYZGZSH");
            dataAdpater.UpdateCommand.Parameters.Add("@DLJGMC", SqlDbType.Char, 60, "DLJGMC");
            dataAdpater.UpdateCommand.Parameters.Add("@DLJGDH", SqlDbType.Char, 15, "DLJGDH");
            dataAdpater.UpdateCommand.Parameters.Add("@SQRQ", SqlDbType.Char, 14, "SQRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR2", SqlDbType.Char, 60, "QLR2");
            dataAdpater.UpdateCommand.Parameters.Add("@SQRLX2", SqlDbType.Char, 20, "SQRLX2");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR_ZJZL2", SqlDbType.Char, 20, "QLR_ZJZL2");
            dataAdpater.UpdateCommand.Parameters.Add("@QLR_ZJBH2", SqlDbType.Char, 20, "QLR_ZJBH2");
            dataAdpater.UpdateCommand.Parameters.Add("@DWXZ2", SqlDbType.Char, 14, "DWXZ2");
            dataAdpater.UpdateCommand.Parameters.Add("@FRDBXM2", SqlDbType.Char, 30, "FRDBXM2");
            dataAdpater.UpdateCommand.Parameters.Add("@TXDZ2", SqlDbType.NVarChar, 100, "TXDZ2");
            dataAdpater.UpdateCommand.Parameters.Add("@YZBM2", SqlDbType.Char, 6, "YZBM2");
            dataAdpater.UpdateCommand.Parameters.Add("@DZYJ2", SqlDbType.Char, 40, "DZYJ2");
            dataAdpater.UpdateCommand.Parameters.Add("@LXR2", SqlDbType.Char, 60, "LXR2");
            dataAdpater.UpdateCommand.Parameters.Add("@LXDH2", SqlDbType.Char, 15, "LXDH2");
            dataAdpater.UpdateCommand.Parameters.Add("@SQS_DLRXM2", SqlDbType.Char, 60, "SQS_DLRXM2");
            dataAdpater.UpdateCommand.Parameters.Add("@ZYZGZSH2", SqlDbType.Char, 20, "ZYZGZSH2");
            dataAdpater.UpdateCommand.Parameters.Add("@DLJGMC2", SqlDbType.Char, 60, "DLJGMC2");
            dataAdpater.UpdateCommand.Parameters.Add("@DLJGDH2", SqlDbType.Char, 15, "DLJGDH2");
            dataAdpater.UpdateCommand.Parameters.Add("@TDZL", SqlDbType.NVarChar, 100, "TDZL");
            dataAdpater.UpdateCommand.Parameters.Add("@ZDMJ", SqlDbType.Float, 8, "ZDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@SJYT", SqlDbType.Char, 20, "SJYT");
            dataAdpater.UpdateCommand.Parameters.Add("@QSXZ", SqlDbType.Char, 30, "QSXZ");
            dataAdpater.UpdateCommand.Parameters.Add("@DLBM", SqlDbType.Char, 3, "DLBM");
            dataAdpater.UpdateCommand.Parameters.Add("@SYQLX", SqlDbType.Char, 20, "SYQLX");
            dataAdpater.UpdateCommand.Parameters.Add("@QLSLQJ", SqlDbType.Char, 4, "QLSLQJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XYDZL", SqlDbType.NVarChar, 100, "XYDZL");
            dataAdpater.UpdateCommand.Parameters.Add("@TDJG", SqlDbType.Float, 8, "TDJG");
            dataAdpater.UpdateCommand.Parameters.Add("@TDDYMJ", SqlDbType.Float, 8, "TDDYMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@DYSX", SqlDbType.Char, 20, "DYSX");
            dataAdpater.UpdateCommand.Parameters.Add("@TDDYJE", SqlDbType.Float, 8, "TDDYJE");
            dataAdpater.UpdateCommand.Parameters.Add("@DYQX_Q", SqlDbType.Char, 14, "DYQX_Q");
            dataAdpater.UpdateCommand.Parameters.Add("@DYQX_Z", SqlDbType.Char, 14, "DYQX_Z");
            dataAdpater.UpdateCommand.Parameters.Add("@SQDJLR", SqlDbType.NVarChar, 500, "SQDJLR");
            dataAdpater.UpdateCommand.Parameters.Add("@SQSBZ", SqlDbType.NVarChar, 100, "SQSBZ");
            dataAdpater.UpdateCommand.Parameters.Add("@DYSQRJZRQ", SqlDbType.Char, 14, "DYSQRJZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@DESQRJZRQ", SqlDbType.Char, 14, "DESQRJZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@DCBRQ", SqlDbType.Char, 14, "DCBRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@FRDBZJBH", SqlDbType.Char, 20, "FRDBZJBH");
            dataAdpater.UpdateCommand.Parameters.Add("@FRDBZJZL", SqlDbType.Char, 20, "FRDBZJZL");
            dataAdpater.UpdateCommand.Parameters.Add("@FRDBDH", SqlDbType.Char, 15, "FRDBDH");
            dataAdpater.UpdateCommand.Parameters.Add("@JBB_DLRXM", SqlDbType.Char, 30, "JBB_DLRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@JBB_DLRZJBH", SqlDbType.Char, 20, "JBB_DLRZJBH");
            dataAdpater.UpdateCommand.Parameters.Add("@JBB_DLRZJZL", SqlDbType.Char, 20, "JBB_DLRZJZL");
            dataAdpater.UpdateCommand.Parameters.Add("@JBB_DLRDH", SqlDbType.Char, 15, "JBB_DLRDH");
            dataAdpater.UpdateCommand.Parameters.Add("@GMJJHYDM", SqlDbType.Char, 40, "GMJJHYDM");
            dataAdpater.UpdateCommand.Parameters.Add("@YBZDDM", SqlDbType.Char, 19, "YBZDDM");
            dataAdpater.UpdateCommand.Parameters.Add("@TFH", SqlDbType.Char, 20, "TFH");
            dataAdpater.UpdateCommand.Parameters.Add("@PZYT", SqlDbType.Char, 20, "PZYT");
            dataAdpater.UpdateCommand.Parameters.Add("@PZYT_DLDM", SqlDbType.Char, 3, "PZYT_DLDM");
            dataAdpater.UpdateCommand.Parameters.Add("@JZZDMJ", SqlDbType.Float, 8, "JZZDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@JZMJ", SqlDbType.Float, 8, "JZMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@SYQX_Q", SqlDbType.Char, 14, "SYQX_Q");
            dataAdpater.UpdateCommand.Parameters.Add("@SYQX_Z", SqlDbType.Char, 14, "SYQX_Z");
            dataAdpater.UpdateCommand.Parameters.Add("@GYQLRQK", SqlDbType.NVarChar, 500, "GYQLRQK");
            dataAdpater.UpdateCommand.Parameters.Add("@SM", SqlDbType.NVarChar, 100, "SM");
            dataAdpater.UpdateCommand.Parameters.Add("@BZ", SqlDbType.Char, 60, "BZ");
            dataAdpater.UpdateCommand.Parameters.Add("@DZ", SqlDbType.Char, 60, "DZ");
            dataAdpater.UpdateCommand.Parameters.Add("@NZ", SqlDbType.Char, 60, "NZ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZ", SqlDbType.Char, 60, "XZ");
            dataAdpater.UpdateCommand.Parameters.Add("@QSDCJS", SqlDbType.NVarChar, 500, "QSDCJS");
            dataAdpater.UpdateCommand.Parameters.Add("@DCYXM", SqlDbType.Char, 30, "DCYXM");
            dataAdpater.UpdateCommand.Parameters.Add("@DCRQ", SqlDbType.Char, 14, "DCRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@DJKZJS", SqlDbType.NVarChar, 500, "DJKZJS");
            dataAdpater.UpdateCommand.Parameters.Add("@CLYXM", SqlDbType.Char, 30, "CLYXM");
            dataAdpater.UpdateCommand.Parameters.Add("@CLRQ", SqlDbType.Char, 14, "CLRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@DJDCJGSHYJ", SqlDbType.NVarChar, 500, "DJDCJGSHYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@SHRXM", SqlDbType.Char, 30, "SHRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@SHRQ", SqlDbType.Char, 14, "SHRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@SHHG", SqlDbType.Bit, 1, "SHHG");
            dataAdpater.UpdateCommand.Parameters.Add("@FZMJ", SqlDbType.Float, 8, "FZMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@CSYJ", SqlDbType.NVarChar, 500, "CSYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@CSSCR", SqlDbType.Char, 30, "CSSCR");
            dataAdpater.UpdateCommand.Parameters.Add("@CSRJ", SqlDbType.Char, 14, "CSRJ");
            dataAdpater.UpdateCommand.Parameters.Add("@CSRTDDJSGZH", SqlDbType.Char, 20, "CSRTDDJSGZH");
            dataAdpater.UpdateCommand.Parameters.Add("@CSHG", SqlDbType.Bit, 1, "CSHG");
            dataAdpater.UpdateCommand.Parameters.Add("@SHYJ", SqlDbType.NVarChar, 500, "SHYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@SHFZR", SqlDbType.Char, 30, "SHFZR");
            dataAdpater.UpdateCommand.Parameters.Add("@SHRQ_SPB", SqlDbType.Char, 14, "SHRQ_SPB");
            dataAdpater.UpdateCommand.Parameters.Add("@SHRTDDJSGZH", SqlDbType.Char, 20, "SHRTDDJSGZH");
            dataAdpater.UpdateCommand.Parameters.Add("@SHTG", SqlDbType.Bit, 1, "SHTG");
            dataAdpater.UpdateCommand.Parameters.Add("@PZYJ", SqlDbType.NVarChar, 500, "PZYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@PZR", SqlDbType.Char, 30, "PZR");
            dataAdpater.UpdateCommand.Parameters.Add("@PZRQ", SqlDbType.Char, 14, "PZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@PZTG", SqlDbType.Bit, 1, "PZTG");
            dataAdpater.UpdateCommand.Parameters.Add("@SPBH", SqlDbType.Char, 11, "SPBH");
            dataAdpater.UpdateCommand.Parameters.Add("@SPBBZ", SqlDbType.NVarChar, 100, "SPBBZ");
            dataAdpater.UpdateCommand.Parameters.Add("@SBB_BH", SqlDbType.Char, 19, "SBB_BH");
            dataAdpater.UpdateCommand.Parameters.Add("@SBB_TBRQ", SqlDbType.Char, 14, "SBB_TBRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@JTRK", SqlDbType.Int, 4, "JTRK");
            dataAdpater.UpdateCommand.Parameters.Add("@RKJG", SqlDbType.Int, 4, "RKJG");
            dataAdpater.UpdateCommand.Parameters.Add("@YYLFJS", SqlDbType.Int, 4, "YYLFJS");
            dataAdpater.UpdateCommand.Parameters.Add("@YYLFMJ", SqlDbType.Float, 8, "YYLFMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@YYPFJS", SqlDbType.Int, 4, "YYPFJS");
            dataAdpater.UpdateCommand.Parameters.Add("@YYPFMJ", SqlDbType.Float, 8, "YYPFMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@LJLFJS", SqlDbType.Int, 4, "LJLFJS");
            dataAdpater.UpdateCommand.Parameters.Add("@LJLFMJ", SqlDbType.Float, 8, "LJLFMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@LJPFJS", SqlDbType.Int, 4, "LJPFJS");
            dataAdpater.UpdateCommand.Parameters.Add("@LJPFMJ", SqlDbType.Float, 8, "LJPFMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@STMJ", SqlDbType.Float, 8, "STMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@HDMJ", SqlDbType.Float, 8, "HDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@CDMJ", SqlDbType.Float, 8, "CDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@YZJDMJ", SqlDbType.Float, 8, "YZJDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@KXDMJ", SqlDbType.Float, 8, "KXDMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@HSMJ", SqlDbType.Float, 8, "HSMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@CMZYJ", SqlDbType.NVarChar, 500, "CMZYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@ZZXM", SqlDbType.Char, 30, "ZZXM");
            dataAdpater.UpdateCommand.Parameters.Add("@CMZQZRQ", SqlDbType.Char, 14, "CMZQZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@CWHYJ", SqlDbType.NVarChar, 500, "CWHYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@ZRXM", SqlDbType.Char, 30, "ZRXM");
            dataAdpater.UpdateCommand.Parameters.Add("@CWHQZRQ", SqlDbType.Char, 14, "CWHQZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XCKCYJ", SqlDbType.NVarChar, 500, "XCKCYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@KCR", SqlDbType.Char, 30, "KCR");
            dataAdpater.UpdateCommand.Parameters.Add("@KCRQ", SqlDbType.Char, 14, "KCRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZGHBMYJ", SqlDbType.NVarChar, 500, "XZGHBMYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZGHBMFZR", SqlDbType.Char, 30, "XZGHBMFZR");
            dataAdpater.UpdateCommand.Parameters.Add("@XZGHBMQZRQ", SqlDbType.Char, 14, "XZGHBMQZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZJWGHYJ", SqlDbType.NVarChar, 500, "XZJWGHYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZJWFZR", SqlDbType.Char, 30, "XZJWFZR");
            dataAdpater.UpdateCommand.Parameters.Add("@XZJWQZRQ", SqlDbType.Char, 14, "XZJWQZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZZFSPYJ", SqlDbType.NVarChar, 500, "XZZFSPYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZZFSPRQ", SqlDbType.Char, 14, "XZZFSPRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XGTBMSCYJ", SqlDbType.NVarChar, 500, "XGTBMSCYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XGTBMSCR", SqlDbType.Char, 30, "XGTBMSCR");
            dataAdpater.UpdateCommand.Parameters.Add("@XGTBMSCRQ", SqlDbType.Char, 14, "XGTBMSCRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZFSCYJ", SqlDbType.NVarChar, 500, "XZFSCYJ");
            dataAdpater.UpdateCommand.Parameters.Add("@XZFSCRQ", SqlDbType.Char, 14, "XZFSCRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@SBBSFDY", SqlDbType.Bit, 1, "SBBSFDY");
            dataAdpater.UpdateCommand.Parameters.Add("@TDZ_JY", SqlDbType.Char, 10, "TDZ_JY");
            dataAdpater.UpdateCommand.Parameters.Add("@TDZ_LF", SqlDbType.Char, 10, "TDZ_LF");
            dataAdpater.UpdateCommand.Parameters.Add("@TDZ_BH", SqlDbType.Char, 11, "TDZ_BH");
            dataAdpater.UpdateCommand.Parameters.Add("@FZRQ", SqlDbType.Char, 14, "FZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@FZJS", SqlDbType.NVarChar, 500, "FZJS");
            dataAdpater.UpdateCommand.Parameters.Add("@DYMJ", SqlDbType.Float, 8, "DYMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@FTMJ", SqlDbType.Float, 8, "FTMJ");
            dataAdpater.UpdateCommand.Parameters.Add("@ZZRQ", SqlDbType.Char, 14, "ZZRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@TDZSFDY", SqlDbType.Bit, 1, "TDZSFDY");
            dataAdpater.UpdateCommand.Parameters.Add("@GHKBH", SqlDbType.Char, 11, "GHKBH");
            dataAdpater.UpdateCommand.Parameters.Add("@GHKDYRQ", SqlDbType.Char, 14, "GHKDYRQ");
            dataAdpater.UpdateCommand.Parameters.Add("@GHKSFDY", SqlDbType.Bit, 1, "GHKSFDY");
            dataAdpater.UpdateCommand.Parameters.Add("@GHKJBR", SqlDbType.Char, 30, "GHKJBR");
            dataAdpater.UpdateCommand.Parameters.Add("@DJKSFDY", SqlDbType.Bit, 1, "DJKSFDY");
            dataAdpater.UpdateCommand.Parameters.Add("@TEXTBZ1", SqlDbType.Char, 60, "TEXTBZ1");
            dataAdpater.UpdateCommand.Parameters.Add("@TEXTBZ2", SqlDbType.Char, 60, "TEXTBZ2");
            dataAdpater.UpdateCommand.Parameters.Add("@TEXTBZ3", SqlDbType.NVarChar, 500, "TEXTBZ3");
            dataAdpater.UpdateCommand.Parameters.Add("@FBZ1", SqlDbType.Float, 8, "FBZ1");
            dataAdpater.UpdateCommand.Parameters.Add("@FBZ2", SqlDbType.Float, 8, "FBZ2");
            dataAdpater.UpdateCommand.Parameters.Add("@FBZ3", SqlDbType.Float, 8, "FBZ3");
            dataAdpater.UpdateCommand.Parameters.Add("@IBZ1", SqlDbType.Int, 4, "IBZ1");
            dataAdpater.UpdateCommand.Parameters.Add("@IBZ2", SqlDbType.Int, 4, "IBZ2");
            dataAdpater.UpdateCommand.Parameters.Add("@IBZ3", SqlDbType.Int, 4, "IBZ3");
            SqlParameter parameter2 = dataAdpater.UpdateCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//匹配参数（同 WHERE条件一样的参数）
            parameter2.SourceColumn = "DJH";
            parameter2.SourceVersion = DataRowVersion.Original;

            //删除
            dataAdpater.DeleteCommand = new SqlCommand("delete DCB   WHERE DJH = @DJH", Connection1);
            dataAdpater.DeleteCommand.Parameters.Add("@DJH", SqlDbType.Char, 17, "DJH");//状态
            parameter2.SourceVersion = DataRowVersion.Original;

            //添加映射
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping = dataAdpater.TableMappings.Add("DCB", "DCB");
            tableMapping.DataSetTable = "DCB";
            Connection1.Close();
            return dataAdpater;
        }
        /// <summary>
        /// 创建数据字典表
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>创建是否成功</returns>
        public static bool Create_Dic_table(string connectionString)
        {
            bool Iscg = false;
            bool IsTable = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataTable dt = connection.GetSchema("Tables");//获取数据库中所有的表
                    foreach (System.Data.DataRow row in dt.Rows)//遍历表中各行
                    {
                        if (row[2].ToString() == "DataDic")//找到"DataDic"
                        {
                            IsTable = false;
                            Iscg = true;
                        }
                    }
                    #region 如果数据库中没有《DataDic》表则创建 DataDic表
                    if (IsTable & (connection.State == ConnectionState.Open))
                    {
                        string str = "CREATE TABLE " + " DataDic" +
                                 "(ZXDM  char(12) NOT NULL  PRIMARY KEY," +//行政代码
                                  "MC nvarchar(30)  NOT NULL," +//名称
                                  "SJDM  char(2)," +//省级代码
                                  "DSDM char(2)," +//地市代码
                                  "XQDM char(2)," +//县区代码
                                  "XZDM  char(3)," +//乡镇代码
                                  "XZCDM char(3)," +//行政村代码
                            //"CMZDM  char(2)," +//村民组代码
                                  "WZMC  nvarchar(60)," +//完整名称
                                  "BZ nvarchar(60))"; //备注
                        SqlCommand sqllCommand = new SqlCommand(str, connection);
                        sqllCommand.ExecuteNonQuery();
                        Iscg = true;
                    }
                    #endregion
                }
                catch (Exception Mess)
                {
                    MessageBox.Show(Mess.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return Iscg;
            }
        }

        /// <summary>
        /// 数据字典存储过程和DataSet匹配
        /// </summary>
        /// <param name="SelectString">条件选择</param>
        /// <param name="Connectionstring">数据连接字符串</param>
        /// <returns>返回SqlDataAdapter实例</returns>
        public static SqlDataAdapter Dic_Initadapter(string SelectString, string Connectionstring)
        {
            SqlConnection Connection1 = new SqlConnection(Connectionstring);
            Connection1.Open();
            SqlDataAdapter dataAdpater = new SqlDataAdapter(SelectString, Connection1);

            //新增
            dataAdpater.InsertCommand = new SqlCommand("insert into DataDic("
                + "ZXDM,MC,SJDM,DSDM,XQDM,XZDM,XZCDM,"
                + "WZMC,BZ)"
                + " values "
                + "(@ZXDM,@MC,@SJDM,@DSDM,@XQDM,@XZDM,@XZCDM,"
                + "@WZMC,@BZ)", Connection1);
            dataAdpater.InsertCommand.Parameters.Add("@ZXDM", SqlDbType.NVarChar, 12, "ZXDM");
            dataAdpater.InsertCommand.Parameters.Add("@MC", SqlDbType.NVarChar, 30, "MC");
            dataAdpater.InsertCommand.Parameters.Add("@SJDM", SqlDbType.Char, 2, "SJDM");
            dataAdpater.InsertCommand.Parameters.Add("@DSDM", SqlDbType.Char, 2, "DSDM");
            dataAdpater.InsertCommand.Parameters.Add("@XQDM", SqlDbType.Char, 2, "XQDM");
            dataAdpater.InsertCommand.Parameters.Add("@XZDM", SqlDbType.Char, 3, "XZDM");
            dataAdpater.InsertCommand.Parameters.Add("@XZCDM", SqlDbType.Char, 3, "XZCDM");
            dataAdpater.InsertCommand.Parameters.Add("@WZMC", SqlDbType.NVarChar, 60, "WZMC");
            dataAdpater.InsertCommand.Parameters.Add("@BZ", SqlDbType.NVarChar, 60, "BZ");
            //修改
            dataAdpater.UpdateCommand = new SqlCommand(
                "UPDATE DataDic SET MC = @MC "
                + ", SJDM = @SJDM "
                + ", DSDM = @DSDM "
                + ", XQDM = @XQDM "
                + ", XZDM = @XZDM "
                + ", XZCDM = @XZCDM "
                + ", WZMC = @WZMC "
                + ", BZ = @BZ "
                + " WHERE  ZXDM=@ZXDM", Connection1);
            dataAdpater.UpdateCommand.Parameters.Add("@MC", SqlDbType.NVarChar, 30, "MC");
            dataAdpater.UpdateCommand.Parameters.Add("@SJDM", SqlDbType.Char, 2, "SJDM");
            dataAdpater.UpdateCommand.Parameters.Add("@DSDM", SqlDbType.Char, 2, "DSDM");
            dataAdpater.UpdateCommand.Parameters.Add("@XQDM", SqlDbType.Char, 2, "XQDM");
            dataAdpater.UpdateCommand.Parameters.Add("@XZDM", SqlDbType.Char, 3, "XZDM");
            dataAdpater.UpdateCommand.Parameters.Add("@XZCDM", SqlDbType.Char, 3, "XZCDM");
            dataAdpater.UpdateCommand.Parameters.Add("@WZMC", SqlDbType.NVarChar, 60, "WZMC");
            dataAdpater.UpdateCommand.Parameters.Add("@BZ", SqlDbType.NVarChar, 60, "BZ");
            SqlParameter parameter2 = dataAdpater.UpdateCommand.Parameters.Add("@ZXDM", SqlDbType.NVarChar, 14, "ZXDM");//匹配参数（同 WHERE条件一样的参数）
            parameter2.SourceColumn = "ZXDM";
            parameter2.SourceVersion = DataRowVersion.Original;

            //删除
            dataAdpater.DeleteCommand = new SqlCommand("delete DataDic   WHERE ZXDM = @ZXDM", Connection1);
            dataAdpater.DeleteCommand.Parameters.Add("@ZXDM", SqlDbType.NVarChar, 14, "ZXDM");//状态
            parameter2.SourceVersion = DataRowVersion.Original;

            //添加映射
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping = dataAdpater.TableMappings.Add("DataDic", "DataDic");
            tableMapping.DataSetTable = "DataDic";
            Connection1.Close();
            return dataAdpater;
        }

    }
}
