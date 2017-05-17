using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace PluginAPI
{
    /// <summary>
    /// 插件常量类
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class PluginPath
    {
        /// <summary>
        /// 插件存储目录
        /// </summary>
        public static readonly string PluginsPath = WorkPath.ApplicationWorkPath + @"\Plugins\";
    }
}
