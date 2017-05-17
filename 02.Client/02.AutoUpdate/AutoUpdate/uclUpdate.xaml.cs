using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoUpdate
{
    /// <summary>
    /// uclUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class uclUpdate : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// uclUpdate
        /// </summary>
        public uclUpdate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// message
        /// </summary>
        private string message = " ";

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary> 
        public void Check()
        {
            Message = "正在检查服务器程序版本...";
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var versionFileInfo = VersionFileInfo.Check();
                if (versionFileInfo != null)
                {
                    Message = "发现新版本，是否更新？";
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        wpControl.Visibility = Visibility.Visible;
                    }));
                }
                else
                {
                    Message = "您的程序已经是最新版本！";
                    NoUpdate?.Invoke();
                }
            });
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Message = "开始下载更新文件...";
            wpControl.Visibility = Visibility.Hidden;
            progress.Visibility = Visibility.Visible;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var versionFileInfo = VersionFileInfo.LoadServiceFileInfo();
                versionFileInfo.Message = message =>
                {
                    this.Message = message;
                };
                //下载更新包
                versionFileInfo.DownloadFile();
                Message = "下载已完成，即将为您更新！";

                this.Dispatcher.Invoke(new Action(() =>
                {
                    OKAction?.Invoke();//点触发外界OKAction一般用于退出程序，退出服务等资源释放操作。
                }));

                //等待彻底是放干净
                System.Threading.Thread.Sleep(500);

                //启动更新
                //var processStartInfo = new System.Diagnostics.ProcessStartInfo();
                var p = System.Diagnostics.Process.Start(System.IO.Path.GetFullPath(WorkPath.ExecPath + @"\AutoUpdate.exe"));
                p.WaitForExit();
                //Message = "即将为您启动程序...";
                //Message?.Invoke("即将为您启动程序...");
                //System.Threading.Thread.Sleep(100);

                //退出程序
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Application.Current.Shutdown();
                }));

                if (versionFileInfo.IsZip)
                {
                    //启动程序
                    string appPath = System.IO.Path.GetFullPath(WorkPath.ExecPath + @"\" + versionFileInfo.AppID + ".exe");
                    System.Diagnostics.Process.Start(appPath);
                }
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelAction?.Invoke();
        }

        /// <summary>
        /// 取消事件，取消后将不进入更新。
        /// </summary>
        public Action CancelAction { get; set; }

        /// <summary>
        /// 确认更新，一般用于更新之前释放资源，退出依赖的程序、服务、线程等资源释放操作。
        /// 注意：已经实现重新启动应用程序功能
        /// </summary>
        public Action OKAction { get; set; }

        /// <summary>
        /// 没有更新
        /// </summary>
        public Action NoUpdate { get; set; }
    }
}
