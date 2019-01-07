using MaterialDesign;
using PluginAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows; 
using System.Threading.Tasks;
using Common;

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
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

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
        /// 异步处理县城异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                ("异步处理县城异常：《" + e.Exception.ToString() + "》").WriteToLog("", log4net.Core.Level.Error);
                e.SetObserved();
            }
            catch (Exception ex)
            {
                ("不可恢复的异步处理县城异常：《" + ex.ToString() + "》").WriteToLog("", log4net.Core.Level.Error);
                //MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    ("非UI线程全局异常" + exception.ToString()).WriteToLog("", log4net.Core.Level.Error);
                }
            }
            catch (Exception ex)
            {
                ("不可恢复的非UI线程全局异常" + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
            }
        }

        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ("UI线程全局异常：《" + e.Exception.ToString() + "》").WriteToLog("", log4net.Core.Level.Error);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ("不可恢复的UI线程全局异常：《" + ex.ToString() + "》").WriteToLog("", log4net.Core.Level.Error);
                //MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
            //MessageBox.Show(e.Exception.ToString());
        }

        /// <summary>
        /// 程序退出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            ("程序退出").WriteToLog("", log4net.Core.Level.Info);
            base.OnExit(e);
        }

        /// <summary>
        /// 操作系统操作退出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            ("当用户结束时发生 Windows 通过注销或关闭操作系统的会话。").WriteToLog("", log4net.Core.Level.Info);
            base.OnSessionEnding(e);
        }

    }
}
