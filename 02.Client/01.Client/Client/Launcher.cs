using Client;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    /// <summary>
    /// 入口程序
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class Launcher
    {
        /// <summary>
        /// 程序入口点
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceManager.Launcher<App>(args);
        } 
    }
}
 