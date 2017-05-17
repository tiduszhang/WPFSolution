using MaterialDesign;
using PluginAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows; 
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 应用程序集类
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            //设置当前程序集
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //初始化当前程序集样式
            ThemeSwitcher.SetTheme(ThemeEnum.METRO);
            //初始化程序
            PluginHelper.LoadApplication();
            //创建入口程序对象

            var launcher = PluginHelper.CreateInstanceByKey("Launcher");
            if (launcher != null)
            {
                await Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        launcher.Run();
                    }));
                });
                base.OnStartup(e);
                return;
            }

            System.Environment.Exit(System.Environment.ExitCode);
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        { 
            base.OnExit(e);
        }
    }
}
