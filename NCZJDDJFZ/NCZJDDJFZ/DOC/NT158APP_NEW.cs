using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dog.NT158.NEW
{
    public class NT158App
    {
        #region 定义函数

        /// <summary>
        /// 查找加密锁
        /// </summary>
        /// <param name="appID">加密锁识别码</param>
        /// <param name="keyHandles">HANDLE值</param>
        /// <param name="keyNumber">找到几把加密锁</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158Find(string appID, int[] keyHandles, int[] keyNumber);


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <param name="uPin1">密码1</param>
        /// <param name="uPin2">密码2</param>
        /// <param name="uPin3">密码3</param>
        /// <param name="uPin4">密码4</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158Login(int keyHandle, int uPin1, int uPin2, int uPin3, int uPin4);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158Logout(int keyHandle);

        /// <summary>
        /// 检测加密锁
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158CheckExist(int keyHandle);

        /// <summary>
        /// 获取加密锁ID
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <param name="UID">加密锁硬件ID</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158GetUid(int keyHandle, StringBuilder UID);

        /// <summary>
        /// 读取存储区
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <param name="fileId">几号存储区</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="length">读取数据的长度</param>
        /// <param name="pBuffer">读取出来的数据</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158ReadFile(int keyHandle, int fileId, int startAddr, int length, byte[] pBuffer);

        /// <summary>
        /// 写入存储区
        /// </summary>
        /// <param name="keyHandle">要操作锁的HANDLE值</param>
        /// <param name="fileId">几号存储区</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="length">写入数据的长度</param>
        /// <param name="pBuffer">写入的数据</param>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158WriteFile(int keyHandle, int fileId, int startAddr, int length, byte[] pBuffer);

        /// <summary>
        /// 3DES加密
        /// </summary>
        /// <param name="keyHandle">操作锁的HANDLE值</param>
        /// <param name="buffSize">加密的长度</param>
        /// <param name="pBuffer">加密的数据</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158TriDesEncrypt(int keyHandle, int buffSize, byte[] pBuffer);

        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="keyHandle">操作锁的HANDLE值</param>
        /// <param name="buffSize">解密的长度</param>
        /// <param name="pBuffer">解密的数据</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158TriDesDecrypt(int keyHandle, int buffSize, byte[] pBuffer);

        /// <summary>
        /// LED灯状态
        /// </summary>
        /// <param name="keyHandle"></param>
        /// <param name="state">0-灯灭，1-亮，2-闪</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158Led(int keyHandle, int state);

        /// <summary>
        /// 获取激活状态
        /// </summary>
        /// <param name="keyHandle"></param>
        /// <param name="state">0是未激活，1是已经激活</param>
        /// <returns></returns>
#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158GetActiveState(int keyHandle, int[] state);

#if Win64
        [DllImport("NT158.X64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#else
        [DllImport("NT158DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
#endif
        public static extern int NT158Active(int keyHandle, int codeLen, string code);
        #endregion

        Key key = new Key(); //调用加密锁参数
        int Rtn = 0; //方法返回值
        int[] keyHandles = new int[8];
        int[] keyNum = new int[8];

        /// <summary>
        /// 查找加密狗
        /// </summary>
        /// <returns></returns>
        public int checkDog()
        {
            key.AppId = "BDC";//识别码
            Rtn = NT158App.NT158Find(key.AppId, keyHandles, keyNum);
            return Rtn;
        }


        /// <summary>
        /// 登录加密锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public int Login_Click()
        {
            //key.UserPin1 = -306874307;
            //key.UserPin2 = 1164321836;
            //key.UserPin3 = 1119679192;
            //key.UserPin4 = 1795531629;

            key.UserPin1 = 1489046975;
            key.UserPin2 = 676158292;
            key.UserPin3 = 2013626743;
            key.UserPin4 = 1834427932;

            Rtn = NT158App.NT158Login(keyHandles[0], key.UserPin1, key.UserPin2, key.UserPin3, key.UserPin4);
            return Rtn;
        }
        /// <summary>
        /// 检查加密狗
        /// </summary>
        /// <returns></returns>
        public bool Authenticated()
        {
            bool Iscg = true;
            if (checkDog() != 0)
            {
                MessageBox.Show("没有找到加密狗!", "集体土地所有权确权登记基础应用软件", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Iscg = false;
                return false;
            }

            if (Login_Click() != 0)
            {
                MessageBox.Show("加密登录失败!", "集体土地所有权确权登记基础应用软件", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Iscg = false;
                return false;
            }
            if (!ReadData())
            {
                // MessageBox.Show("加密登录失败!", "集体土地所有权确权登记基础应用软件", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Iscg = false;
                return false;
            }

            return Iscg;
        }
        private bool ReadData()
        {
            int add = 0;
            int fileNum = 2;//2号文件
            int len = 30;
            byte[] readBuffer = new byte[len];

            Rtn = NT158ReadFile(keyHandles[0], fileNum, add, len, readBuffer);
            if (Rtn != 0) return false;
            string LR2 = Encoding.GetEncoding("gb2312").GetString(readBuffer).Trim();
            LR2 = LR2.Replace("\0", "");

            fileNum = 1;
            Rtn = NT158ReadFile(keyHandles[0], fileNum, add, len, readBuffer);
            if (Rtn != 0) return false;
            string LR1 = Encoding.GetEncoding("gb2312").GetString(readBuffer).Trim();
            LR1 = LR1.Replace("\0", "");

            fileNum = 0;
            Rtn = NT158ReadFile(keyHandles[0], fileNum, add, len, readBuffer);
            if (Rtn != 0) return false;
            string LR0 = Encoding.GetEncoding("gb2312").GetString(readBuffer).Trim();
            LR0 = LR0.Replace("\0", "");

            if (LR1 != "RAN") return false;

            string LR10 = LR0.Substring(0, LR0.Length - 4);
            string LR21 = LR2.Substring(0, LR2.Length - 4);

            DateTime DogDate = Convert.ToDateTime(LR10).Date;//狗里存储的日期
            DateTime LimitDate = Convert.ToDateTime(LR21).Date;//狗里存储的限制日期
            LimitDate = LimitDate.AddYears(-7);//将狗里的存储限制日期减去7年
            DateTime now = DateTime.Now.Date;//当前日期

            //当前日期和里存储的日期比较
            int gqbj = DogDate.CompareTo(now);
            if (gqbj > 0)
            {
                MessageBox.Show("对不起，您不能修改系统日期!", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            //当前日期和狗里存储的限制日期比较
            int cr = now.CompareTo(LimitDate);
            if (cr > 0)
            {
                MessageBox.Show("对不起，您的狗已经过期!", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            else
            {
                byte[] pBuffer = new byte[100];
                Rtn = NT158WriteFile(keyHandles[0], 0, 0, len, pBuffer);
                if (Rtn != 0)
                    return false;
                else
                {
                    byte[] wBuffer = Encoding.Default.GetBytes(now.ToShortDateString() + "SCRQ");
                    Rtn = NT158WriteFile(keyHandles[0], 0, 0, len, wBuffer);

                    if (Rtn != 0) return false;
                    else return true;
                }
            }

        }


    }
    public class Key
    {
        private string appId;       //加密锁识别码
        public string AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        private int userPin1;
        public int UserPin1         //密码1
        {
            get { return userPin1; }
            set { userPin1 = value; }
        }

        private int userPin2;
        public int UserPin2         //密码2
        {
            get { return userPin2; }
            set { userPin2 = value; }
        }

        private int userPin3;       //密码3
        public int UserPin3
        {
            get { return userPin3; }
            set { userPin3 = value; }
        }

        private int userPin4;       //密码4
        public int UserPin4
        {
            get { return userPin4; }
            set { userPin4 = value; }
        }

        private StringBuilder id = new StringBuilder();   //加密锁硬件序列号

        public StringBuilder Id
        {
            get { return id; }
            set { id = value; }
        }

        private byte[] EnTriBuffer;        //3DES加密数据
        public byte[] EnTriBuffer1
        {
            get { return EnTriBuffer; }
            set { EnTriBuffer = value; }
        }
        private byte[] DeTriBuffer;        //3DES解密数据
        public byte[] DeTriBuffer1
        {
            get { return DeTriBuffer; }
            set { DeTriBuffer = value; }
        }

        private byte[] writeBuffer;       //读取存储区数据
        public byte[] WriteBuffer
        {
            get { return writeBuffer; }
            set { writeBuffer = value; }
        }

        private int state;
        public int State
        {
            get { return state; }
            set { state = value; }
        }
    }
}

