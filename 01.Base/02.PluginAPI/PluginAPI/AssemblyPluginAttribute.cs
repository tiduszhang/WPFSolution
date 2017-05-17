using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PluginAPI
{
    /// <summary>
    /// 插件程序集特性
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyPluginAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public AssemblyPluginAttribute()
        { 
            ID = "";
            Name = "";
            Description = "";
            Type = "";
            DefaultPluginID = "";
            Key = "";
        }

        /// <summary>
        /// 插件程序集ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 程序集类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 默认入口插件ID
        /// </summary>
        public string DefaultPluginID { get; set; }

        /// <summary>
        /// 程序识别关键字
        /// </summary>
        public string Key { get; set; }
    }
}
