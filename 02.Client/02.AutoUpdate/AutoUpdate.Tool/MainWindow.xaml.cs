using AutoUpdate;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
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

namespace AutoUpdate.Tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        /// <summary>
        /// MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            txtTip.Content = "" //+ System.Environment.NewLine 
                + "1、原程序包内必须包含版本文件文件名必须为version.json；" + System.Environment.NewLine
                + "2、程序标识必须与应用程序可执行主程序名相同且不得包含后缀名；" + System.Environment.NewLine
                + "3、本程序只打包程序文件夹并且生成gz包；" + System.Environment.NewLine
                + "4、使用其他压缩软件制作不被本更新程序识别；" + System.Environment.NewLine
                + "5、本更新程序支持HTTP和FTP协议；" + System.Environment.NewLine
                + "6、请将更新文件和版本信息文件放入服务器目录中；" + System.Environment.NewLine
                + "7、本更新程序为增量替换更新，不提供清理旧版本冗余文件功能。" + System.Environment.NewLine;

            MetroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary = DialogDictionary,
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                SuppressDefaultResources = true
            };

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
        /// DialogDictionary
        /// </summary>
        private ResourceDictionary DialogDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };
        /// <summary>
        /// MetroDialogSettings
        /// </summary>
        MetroDialogSettings MetroDialogSettings { get; set; }
        
        /// <summary>
        /// 开始压缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnZip_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(txtAppID.Text.Trim()))
            {
                await this.ShowMessageAsync("提示信息", "请先输入程序标识！", MessageDialogStyle.Affirmative, MetroDialogSettings);
                txtAppID.Focus();
                return;
            }
            if (String.IsNullOrWhiteSpace(txtVersion.Text.Trim()))
            {
                await this.ShowMessageAsync("提示信息", "请先输入程序版本！", MessageDialogStyle.Affirmative, MetroDialogSettings);
                txtVersion.Focus();
                return;
            }
            Version v = null;
            if (!Version.TryParse(txtVersion.Text.Trim(), out v))
            {
                await this.ShowMessageAsync("提示信息", "请先输入正确的程序版本！", MessageDialogStyle.Affirmative, MetroDialogSettings);
                txtVersion.Focus();
                return;
            }

            if (String.IsNullOrWhiteSpace(txtInput.Text.Trim()))
            {
                await this.ShowMessageAsync("提示信息", "请先输入程序所在文件夹！", MessageDialogStyle.Affirmative, MetroDialogSettings);
                txtInput.Focus();
                return;
            }
            else if (!System.IO.Directory.Exists(txtInput.Text.Trim()))
            {
                await this.ShowMessageAsync("提示信息", "您输入的程序所在文件夹不存在！", MessageDialogStyle.Affirmative, MetroDialogSettings);
                txtInput.Focus();
                return;
            }

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.AutoUpgradeEnabled = true;
            sfd.ValidateNames = true;
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;
            sfd.Filter = "*.gz|*.gz";
            sfd.FileOk += Sfd_FileOk;
            sfd.ShowDialog();

        }

        /// <summary>
        /// 是否选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Sfd_FileOk(object sender, CancelEventArgs e)
        {
            var sfd = sender as System.Windows.Forms.SaveFileDialog;

            var inputPath = txtInput.Text.Trim();
            var zipFileName = sfd.FileName.Trim();

            VersionFileInfo.ApplicationName = txtAppID.Text.Trim();

            var versionFileInfo = new VersionFileInfo();
            versionFileInfo.AppID = txtAppID.Text.Trim();
            versionFileInfo.Version = txtVersion.Text.Trim();
            versionFileInfo.FileName = System.IO.Path.GetFileName(zipFileName);
            versionFileInfo.IsZip = true;
            versionFileInfo.SaveAs();

            var path = VersionFileInfo.LocalFileInfoPath + @"\" + VersionFileInfo.FileInfoName;

            System.IO.File.Copy(path, inputPath + @"\" + VersionFileInfo.FileInfoName, true);
            System.IO.File.Copy(path, zipFileName.Substring(0, zipFileName.Length - versionFileInfo.FileName.Length) + @"\" + VersionFileInfo.FileInfoName, true);
            System.IO.File.Delete(path);



            var controller = await this.ShowProgressAsync("提示信息", "启动生成", false, MetroDialogSettings);
            controller.SetIndeterminate();

            //测试压缩和解压缩
            GZipCompress gz = new GZipCompress();
            gz.Message = message =>
            {
                controller.SetMessage(message + System.Environment.NewLine);
                //Message = message + System.Environment.NewLine;
            };

            await System.Threading.Tasks.Task.Factory.StartNew(() =>
             {
                 //压缩
                 gz.DirPath = inputPath; //@"D:\Users\Tidus\Desktop\EAP-System-备份20170315\EAP-System";
                 gz.ZipFileName = zipFileName;//@"D:\Users\Tidus\Desktop\EAP-System-备份20170315\zz.gz";
                 gz.Compress();

                 //Message = "-------------------------------------------------------------------------" + System.Environment.NewLine;
                 //Message = "-------------------------------------------------------------------------" + System.Environment.NewLine;
                 //Message = "-------------------------------------------------------------------------" + System.Environment.NewLine;

                 //解压缩
                 //gz.DirPath = inputPath + @"\..\" + "test"; //@"D:\Users\Tidus\Desktop\EAP-System-备份20170315\test";
                 //gz.ZipFileName = zipFileName;// @"D:\Users\Tidus\Desktop\EAP-System-备份20170315\zz.gz";
                 //gz.DeCompress();
             });

            await this.ShowMessageAsync("提示信息", "自动更新包已创建完成！", MessageDialogStyle.Affirmative, MetroDialogSettings);
            await controller.CloseAsync();

            txtAppID.Text = "";
            txtInput.Text = "";
            txtVersion.Text = "";
            //MessageBox.Show("已完成！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        /// <summary>
        /// 浏览输入文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInput_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "请选择程序所在文件夹：";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtInput.Text = fbd.SelectedPath;
            }
        }
    }
}
