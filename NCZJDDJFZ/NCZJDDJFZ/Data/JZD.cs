using NetTopologySuite.IO;
using System;

namespace NCZJDDJFZ.Data
{
    /// <summary>
    /// 界址点
    /// </summary>
    public class JZD
    {
        private DbaseFieldDescriptor[] dbFields;

        public JZD()
        {
            // --------------------- DbaseType ----------------------
            // 字段类型
            //      C - 字符型  
            //      Y - 货币型  
            //      N - 数值型  
            //      F - 浮点型  
            //      D - 日期型  
            //      T - 日期时间型  
            //      B - 双精度型  
            //      I - 整型  
            //      L - 逻辑型 
            //      M - 备注型  
            //      G - 通用型  
            //      C - 字符型（二进制） 
            //      M - 备注型（二进制） 
            //      P - 图片型  
            // -------------------------------------------------------
            this.dbFields = new DbaseFieldDescriptor[2];

            // 0 属性代码 - 字符串 6
            dbFields[0] = new DbaseFieldDescriptor();
            dbFields[0].Name = "属性代码";
            dbFields[0].DbaseType = 'C';
            dbFields[0].Length = 6;

            // 1 界址点号 - 字符串型 8
            dbFields[1] = new DbaseFieldDescriptor();
            dbFields[1].Name = "界址点号";
            dbFields[1].DbaseType = 'C';
            dbFields[1].Length = 8;

            
        }

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string JZDH;

        /// <summary>
        /// 
        /// </summary>
        public string SXDM;

        

        #endregion

        public DbaseFieldDescriptor[] GetDbaseFieldsDescriptor()
        {
            return this.dbFields;
        }
    }
}
