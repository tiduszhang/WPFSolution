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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WinUpdate : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// WinUpdate
        /// </summary>
        public WinUpdate()
        {
            InitializeComponent();

            this.Loaded += WinUpdate_Loaded;
        }

        /// <summary>
        /// 正在更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinUpdate_Loaded(object sender, RoutedEventArgs e)
        {
            //VersionFileInfo.ApplicationName = "AutoUpdate.Test";
            Message = "正在为您更新...";
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                //开始更新
                var localFileInfo = VersionFileInfo.GetLocalFileInfo();
                var localServiceFileInfo = VersionFileInfo.LoadServiceFileInfo();

                if (localServiceFileInfo.IsZip)
                {
                    localServiceFileInfo.Message = message =>
                    {
                        this.Message = message;
                    };
                    try
                    {
                        //正在更新
                        localServiceFileInfo.Replace();
                        //更新完成
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //this.Close();
                            MessageBox.Show("更新已完成，请重新启动程序！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        }));
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                        //更新完成
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //this.Close();
                            MessageBox.Show("更新遇到问题，请稍后重试！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        }));
                    } 
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Application.Current.Shutdown();
                }));
            });
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

    }
}
