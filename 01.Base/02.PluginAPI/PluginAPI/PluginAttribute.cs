using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace PluginAPI
{
    /// <summary>
    /// 插件类特性-需要默认构造函数
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [ComVisible(true)]
    public sealed class PluginAttribute : Attribute
    {
        /// <summary>
        /// 插件ID
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
        /// 运行方法名称-只允许空参方法名称，区分大小写。
        /// </summary>
        public string RunMethod { get; set; }

        /// <summary>
        /// 停止方法名称-只允许空参方法名称区分大小写。
        /// </summary>
        public string StopMethod { get; set; }

        /// <summary>
        /// 插件归类
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否为内置插件-内置插件将为必选插件，一般不开放给用户选择
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// 是否为单实例
        /// </summary>
        public bool IsSingleInstance { get; set; }

        /// <summary>
        /// 父插件ID
        /// </summary>
        public string ParentPluginID { get; set; } 
         
    }
     
}
