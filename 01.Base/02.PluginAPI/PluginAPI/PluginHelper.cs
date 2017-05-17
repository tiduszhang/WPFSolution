using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;

namespace PluginAPI
{
    /// <summary>
    /// 插件帮助类
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class PluginHelper
    {
        /// <summary>
        /// 对象列表
        /// </summary>
        private static Dictionary<string, object> ListObject { get; set; }


        /// <summary>
        /// 本程序程序集集合
        /// </summary>
        public static List<System.Reflection.Assembly> ApplicationAssembly { get; set; }

        /// <summary>
        /// 插件程序集集合
        /// </summary>
        public static List<System.Reflection.Assembly> PluginAssembly { get; set; }

        /// <summary>
        /// 初始化程序
        /// </summary>
        public static void LoadApplication()
        {
            if (!Directory.Exists(WorkPath.ExecPath))
            {
                Directory.CreateDirectory(WorkPath.ExecPath);
            }
            var files = Directory.GetFiles(WorkPath.ExecPath, "*", SearchOption.AllDirectories).Where(file => !file.Contains("LibVlc") && file.ToLower().EndsWith(".dll".ToLower())).ToList();
            //var models = AppDomain.CurrentDomain.GetAssemblies();
            if (ApplicationAssembly == null)
            {
                ApplicationAssembly = new List<System.Reflection.Assembly>();
            }
            LoadAssembly(files, ApplicationAssembly);
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="files"></param>
        /// <param name="Assemblys"></param>
        private static void LoadAssembly(List<string> files, List<System.Reflection.Assembly> Assemblys)
        {
            var models = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var file in files)
            {
                var existModel = models.FirstOrDefault(model => model.Location == file);
                if (existModel == null)
                {
                    //AppDomain.CurrentDomain.Load(File.ReadAllBytes(file));  
                    //var assembly = System.Reflection.Assembly.Load(File.ReadAllBytes(file));
                    //existModel = _AppDomain.Load(assembly.FullName);
                    //existModel = _AppDomain.Load(File.ReadAllBytes(file));
                    try
                    {
                        existModel = System.Reflection.Assembly.Load(File.ReadAllBytes(file));
                    }
                    catch (Exception ex)
                    {
                        ex.ToString().WriteToLog(log4net.Core.Level.Error);
                    }
                }
                if (existModel != null)
                {
                    if (Assemblys.FirstOrDefault(applicationAssembly => applicationAssembly.ManifestModule.ScopeName == existModel.ManifestModule.ScopeName) == null)
                    {
                        Assemblys.Add(existModel);
                    }
                }
            }
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        public static void LoadPlugin()
        {
            if (!Directory.Exists(PluginPath.PluginsPath))
            {
                Directory.CreateDirectory(PluginPath.PluginsPath);
            }
            var files = Directory.GetFiles(PluginPath.PluginsPath, "*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith(".dll".ToLower()) || file.ToLower().EndsWith(".exe".ToLower())).ToList();
            //var models = AppDomain.CurrentDomain.GetAssemblies(); 
            if (PluginAssembly == null)
            {
                PluginAssembly = new List<System.Reflection.Assembly>();
            }
            LoadAssembly(files, PluginAssembly);
        }

        /// <summary>
        /// 获取程序集版本
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Version GetVersion(this System.Reflection.Assembly model)
        {
            var assemblyVersionAttributes = (System.Reflection.AssemblyVersionAttribute[])model.GetCustomAttributes(typeof(System.Reflection.AssemblyVersionAttribute), false);
            if (assemblyVersionAttributes != null && assemblyVersionAttributes.Length > 0)
            {
                var assemblyVersionAttribute = assemblyVersionAttributes[0];
                return Version.Parse(assemblyVersionAttribute.Version);
            }
            return Version.Parse("0");
        }

        /// <summary>
        /// 根据关键字创建程序对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static object CreateInstanceByKey(string key, string PluginId = "")
        {
            var models = ApplicationAssembly.ToArray();//_AppDomain.GetAssemblies();
            var model = models.GetModelByKey(key);

            object obj = null;
            if (model != null)
            {
                var assemblyPluginAttribute = model.GetAssemblyPluginAttribute();
                if (!String.IsNullOrWhiteSpace(PluginId))
                {
                    obj = CreateInstanceByAssemblyIdAndPluginId(assemblyPluginAttribute.ID, PluginId);
                }
                else if (!String.IsNullOrWhiteSpace(assemblyPluginAttribute.DefaultPluginID))
                {
                    obj = CreateInstanceByAssemblyIdAndPluginId(assemblyPluginAttribute.ID, assemblyPluginAttribute.DefaultPluginID);
                }
            }
            return obj;
        }

        /// <summary>
        /// 根据关键字创建程序对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static List<object> CreateInstancesByKey(string key, string PluginId = "")
        {
            var models = ApplicationAssembly.ToArray();
            var modelsByKey = models.GetModelsByKey(key);
            List<object> lstObj = new List<object>();
            if (modelsByKey != null && modelsByKey.Count > 0)
            {
                modelsByKey.ForEach(modelByKey =>
                {
                    var assemblyPluginAttribute = modelByKey.GetAssemblyPluginAttribute();
                    if (!String.IsNullOrWhiteSpace(PluginId))
                    {
                        var obj = CreateInstanceByAssemblyIdAndPluginId(assemblyPluginAttribute.ID, PluginId);
                        lstObj.Add(obj);
                    }
                    else if (!String.IsNullOrWhiteSpace(assemblyPluginAttribute.DefaultPluginID))
                    {
                        var obj = CreateInstanceByAssemblyIdAndPluginId(assemblyPluginAttribute.ID, assemblyPluginAttribute.DefaultPluginID);
                        lstObj.Add(obj);
                    }
                });
            }
            return lstObj;
        }

        /// <summary>
        /// 根据ID创建对象
        /// </summary>
        /// <param name="AssemblyId"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static object CreateInstanceByAssemblyIdAndPluginId(string AssemblyId, string PluginId = "")
        {
            var models = ApplicationAssembly.ToArray();
            var obj = models.CreateInstanceByAssemblyIdAndPluginId(AssemblyId, PluginId);
            return obj;
            //return AppDomain.CurrentDomain.GetAssemblies().CreateInstanceByAssemblyIdAndPluginId(AssemblyId, PluginId);
        }

        /// <summary>
        /// 根据ID创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AssemblyId"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static object CreateInstanceByAssemblyIdAndPluginId<T>(string AssemblyId, string PluginId = "") where T : class
        {
            return CreateInstanceByAssemblyIdAndPluginId(AssemblyId, PluginId) as T;
        }

        /// <summary>
        /// 运行默认执行方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public static object Run(this object value, params object[] Parameter)
        {
            var pluginAttribute = value.GetPluginAttribute();
            if (pluginAttribute != null)
            {
                //var type = value.GetType();
                if (!String.IsNullOrWhiteSpace(pluginAttribute.RunMethod))
                {
                    return value.InvokeMethod(pluginAttribute.RunMethod, Parameter);
                }
            }
            return null;
        }

        /// <summary>
        /// 运行默认停止方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Parameter"></param>
        public static object Stop(this object value, params object[] Parameter)
        {
            var pluginAttribute = value.GetPluginAttribute();
            if (pluginAttribute != null)
            {
                //var type = value.GetType();
                if (!String.IsNullOrWhiteSpace(pluginAttribute.StopMethod))
                {
                    return value.InvokeMethod(pluginAttribute.StopMethod, Parameter);
                }
            }
            return null;
        }


        /// <summary>
        /// 获取当前对象的插件特性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PluginAttribute GetPluginAttribute(this object value)
        {
            var type = value.GetType();
            return type.GetPluginAttribute();
        }

        /// <summary>
        /// 获取当前对象的插件特性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PluginAttribute GetPluginAttribute(this Type type)
        {
            var pluginAttributes = type.GetCustomAttributes(typeof(PluginAttribute), false);
            if (pluginAttributes != null && pluginAttributes.Length > 0)
            {
                return pluginAttributes.FirstOrDefault(pluginAttribute => pluginAttribute is PluginAttribute) as PluginAttribute;
            }
            return null;
        }


        /// <summary>
        /// 获取程序集的插件特性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AssemblyPluginAttribute GetAssemblyPluginAttribute(this System.Reflection.Assembly model)
        {
            try
            {
                var assemblyPluginAttributes = model.GetCustomAttributes(typeof(AssemblyPluginAttribute), false);
                if (assemblyPluginAttributes != null && assemblyPluginAttributes.Length > 0)
                {
                    return assemblyPluginAttributes.FirstOrDefault(assemblyPluginAttribute => assemblyPluginAttribute is AssemblyPluginAttribute) as AssemblyPluginAttribute;
                }
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog();
            }

            return null;
        }

        /// <summary>
        /// 获取所有插件信息
        /// </summary>
        /// <returns></returns>
        public static List<AssemblyPluginAttribute> GetAssemblyPluginAttributes()
        {
            //var models = AppDomain.CurrentDomain.GetAssemblies();

            var models = ApplicationAssembly.ToArray();
            List<AssemblyPluginAttribute> lstObject = new List<AssemblyPluginAttribute>();
            System.Threading.Tasks.Parallel.ForEach(models, model =>
            {
                var assemblyPluginAttribute = model.GetAssemblyPluginAttribute();
                if (assemblyPluginAttribute != null)
                {
                    lstObject.Add(assemblyPluginAttribute);
                }
            });
            return lstObject;
        }

        /// <summary>
        /// 获取所有插件内插件类插件信息
        /// </summary>
        /// <returns></returns>
        public static List<PluginAttribute> GetPluginAttributes(this System.Reflection.Assembly model)
        {
            List<PluginAttribute> lstObject = new List<PluginAttribute>();
            var types = model.GetTypesByType();
            if (types != null && types.Count > 0)
            {
                System.Threading.Tasks.Parallel.ForEach(types, type =>
                {
                    PluginAttribute pluginAttribute = type.GetPluginAttribute();
                    if (pluginAttribute != null)
                    {
                        lstObject.Add(pluginAttribute);
                    }
                });
            }
            return lstObject;
        }

        /// <summary>
        /// 获取所有插件内插件类插件信息
        /// </summary>
        /// <returns></returns>
        public static List<PluginAttribute> GetPluginAttributes()
        {
            //var models = AppDomain.CurrentDomain.GetAssemblies();

            var models = ApplicationAssembly.ToArray();
            List<PluginAttribute> lstObject = new List<PluginAttribute>();
            System.Threading.Tasks.Parallel.ForEach(models, model =>
            {
                model.GetPluginAttributes().ForEach(pluginAttribute => lstObject.Add(pluginAttribute));
            });
            return lstObject;
        }

        /// <summary>
        /// 根据插件类型获取相同类型的所有插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param> 
        /// <returns></returns>
        public static List<T> CreateInstancesByPluginType<T>(string Type = "") where T : class
        {

            var models = ApplicationAssembly.ToArray();
            var lstObjs = models.CreateInstancesByPluginType<T>(Type);

            return lstObjs;
            //return AppDomain.CurrentDomain.GetAssemblies().CreateInstancesByPluginType<T>(Type);
        }

        /// <summary>
        /// 根据插件类型获取相同类型的所有插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static List<T> CreateInstancesByPluginType<T>(this System.Reflection.Assembly[] models, string Type = "") where T : class
        {
            if (ListObject == null)
            {
                ListObject = new Dictionary<string, object>();
            }
            List<T> lstObject = new List<T>();

            foreach (var model in models)
            {
                //System.Threading.Tasks.Parallel.ForEach(models, model =>
                //{
                var types = model.GetTypesByType(Type);
                if (types != null && types.Count > 0)
                {
                    foreach (var type in types)
                    {
                        //System.Threading.Tasks.Parallel.ForEach(types, type =>
                        //{
                        string assemblyPluginId = model.GetAssemblyPluginAttribute().ID;
                        PluginAttribute pluginAttribute = type.GetPluginAttribute();
                        string pluginId = pluginAttribute.ID;
                        object obj = null;
                        if (ListObject.ContainsKey(assemblyPluginId.ToLower() + pluginId.ToLower()))
                        {
                            obj = ListObject[assemblyPluginId.ToLower() + pluginId.ToLower()];
                        }
                        if (obj != null)
                        {
                            lstObject.Add(obj as T);
                        }
                        else
                        {
                            obj = type.GetConstructor(new Type[] { }).Invoke(null);
                            //obj = Activator.CreateInstance(type);
                            lstObject.Add(obj as T);
                            if (pluginAttribute.IsSingleInstance)
                            {
                                ListObject.Add(assemblyPluginId.ToLower() + pluginId.ToLower(), obj);
                            }
                        }
                        //});
                    }
                }
                //});
            }
            return lstObject;
        }

        /// <summary>
        /// 根据ID创建对象
        /// </summary>
        /// <param name="models"></param>
        /// <param name="AssemblyId"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static object CreateInstanceByAssemblyIdAndPluginId(this System.Reflection.Assembly[] models, string AssemblyId, string PluginId = "")
        {
            object obj = null;
            if (ListObject == null)
            {
                ListObject = new Dictionary<string, object>();
            }
            var existModel = models.GetModelByAssemblyId(AssemblyId);
            if (existModel != null)
            {
                if (String.IsNullOrWhiteSpace(PluginId))
                {
                    PluginId = existModel.GetAssemblyPluginAttribute().DefaultPluginID;
                }
                if (String.IsNullOrWhiteSpace(PluginId))
                {
                    return null;
                }
                if (ListObject.ContainsKey(AssemblyId.ToLower() + PluginId.ToLower()))
                {
                    obj = ListObject[AssemblyId.ToLower() + PluginId.ToLower()];
                }
                if (obj == null)
                {
                    Type existType = existModel.GetTypeByPluginId(PluginId);
                    if (existType != null)
                    {
                        obj = existType.GetConstructor(new Type[] { }).Invoke(null);
                        //obj = Activator.CreateInstance(existType);
                        if (existType.GetPluginAttribute().IsSingleInstance)
                        {
                            ListObject.Add(AssemblyId.ToLower() + PluginId.ToLower(), obj);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 根据插件特性ID获取插件模块
        /// </summary>
        /// <param name="models"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static System.Reflection.Assembly GetModelByKey(this System.Reflection.Assembly[] models, string key)
        {
            var existModel = models.FirstOrDefault(model =>
            {
                var assemblyPluginAttribute = model.GetAssemblyPluginAttribute();
                if (assemblyPluginAttribute != null)
                {
                    return assemblyPluginAttribute.Key.ToLower() == key.ToLower();
                }
                else
                {
                    return false;
                }
            });
            return existModel;
        }

        /// <summary>
        /// 根据插件特性ID获取插件模块
        /// </summary>
        /// <param name="models"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<System.Reflection.Assembly> GetModelsByKey(this System.Reflection.Assembly[] models, string key)
        {
            List<System.Reflection.Assembly> lstModel = new List<System.Reflection.Assembly>();
            System.Threading.Tasks.Parallel.ForEach(models, model =>
            {
                var assemblyPluginAttribute = model.GetAssemblyPluginAttribute();
                if (assemblyPluginAttribute != null && assemblyPluginAttribute.Key.ToLower() == key.ToLower())
                {
                    lstModel.Add(model);
                }
            });
            return lstModel;
        }

        /// <summary>
        /// 根据插件特性ID获取插件模块
        /// </summary>
        /// <param name="models"></param>
        /// <param name="AssemblyId"></param>
        /// <returns></returns>
        public static System.Reflection.Assembly GetModelByAssemblyId(this System.Reflection.Assembly[] models, string AssemblyId)
        {
            var existModel = models.FirstOrDefault(model =>
            {
                var assemblyPluginAttribute = model.GetAssemblyPluginAttribute();
                if (assemblyPluginAttribute != null)
                {
                    return assemblyPluginAttribute.ID.ToLower() == AssemblyId.ToLower();
                }
                else
                {
                    return false;
                }
            });
            return existModel;
        }

        /// <summary>
        /// 在指定的模块中根据插件ID获取插件类
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PluginId"></param>
        /// <returns></returns>
        public static Type GetTypeByPluginId(this System.Reflection.Assembly model, string PluginId = "")
        {
            var types = model.GetExportedTypes();
            if (String.IsNullOrWhiteSpace(PluginId))
            {
                PluginId = model.GetAssemblyPluginAttribute().DefaultPluginID;
            }
            if (String.IsNullOrWhiteSpace(PluginId))
            {
                return null;
            }
            var existType = types.FirstOrDefault(type =>
            {
                var pluginAttribute = type.GetPluginAttribute();
                if (pluginAttribute != null)
                {
                    return pluginAttribute.ID.ToLower() == PluginId.ToLower();
                }
                else
                {
                    return false;
                }
            });
            return existType;
        }

        /// <summary>
        /// 在指定的模块中根据插件类型获取插件类
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static List<Type> GetTypesByType(this System.Reflection.Assembly model, string Type = "")
        {
            List<Type> lstType = new List<Type>();
            var types = model.GetExportedTypes();
            if (String.IsNullOrWhiteSpace(Type))
            {
                Type = model.GetAssemblyPluginAttribute().Type;
            }
            foreach (var type in types)
            {
                //System.Threading.Tasks.Parallel.ForEach(types, type =>
                //{
                var pluginAttribute = type.GetPluginAttribute();
                if (!String.IsNullOrWhiteSpace(Type) && pluginAttribute != null && pluginAttribute.Type.ToLower() == Type.ToLower())
                {
                    lstType.Add(type);
                }
                if (pluginAttribute != null && String.IsNullOrWhiteSpace(pluginAttribute.Type))
                {
                    lstType.Add(type);
                }
                //});
            }
            return lstType;
        }

    }
}
