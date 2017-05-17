using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace WelcomeScreen
{
    /// <summary>
    /// 测试类
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    //todo:初始化自动更新程序集 
    [PluginAPI.Plugin(ID = "Class1", Type = "WelcomeScreen", RunMethod = "Test")]
    public class Class1
    {
        /// <summary>
        /// 
        /// </summary>
        public void Test()
        {
            var a = new object();

            "test".WriteToLog();
        }
    }
}
