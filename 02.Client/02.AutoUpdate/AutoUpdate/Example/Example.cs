using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutoUpdate
{
    /// <summary>
    /// 注意：例子代码
    /// 1、在程序窗口中放入控件
    /// 2、加入下面的参考代码
    /// </summary>
    public class Example
    {
        ///// <summary>
        ///// DEMO程序窗口加载
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    VersionFileInfo.FileService = @"D:\";//测试文件地址存放，注意如果使用HTTPs或FTP协议，请将身份信息一并放入请求地址中。
        //    update.CancelAction = () =>// 取消动作
        //    {
        //        //检查到更新，但用户点击了取消，此时将不更新。
        //        Application.Current.Shutdown();
        //    };
        //    update.OKAction = () =>// 确认动作
        //    {
        //        //用户点击了确定，此时下载已经完成，需要将程序彻底退出，比如退出服务，退出程序，退出线程等等，在此之后将自动启动更新程序 
        //    };
        //    update.NoUpdate = () =>
        //    {
        //        //此时没有更新，表示已经是最新版本了。
        //    };
        //    //throw new NotImplementedException();
        //    update.Check();//检查更新 
        //}
    }
}
