using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;
using Common.DB;
using MVVM;
using MVVM.Controls;

namespace WelcomeScreen
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    [PluginAPI.Plugin(ID = "WinScreen", Type = "WelcomeScreen", RunMethod = "Show")]
    public partial class WinTest : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public WinTest()
        {
            InitializeComponent();

            this.Loaded += WinScreen_Loaded;
        }

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsInDesignMode())
            {
                return;
            }

            //分页DEMO
            var pageViewModel = new PageViewModel<EntityBase>();
            pageViewModel.PageSize = 20;
            this.pageView.PageViewModel = pageViewModel;
            dataGrid.ItemsSource = pageViewModel.ObservableCollectionObject;
            pageViewModel.ChangePageFun = () =>
            {
                PageData<EntityBase> pageData = NoSQLHelper.LoadByPage<EntityBase>(pageViewModel.PageIndex, pageViewModel.PageSize);
                pageData.FillPage(pageViewModel);
                return pageData.QueryData;
            };
            pageViewModel.ChangePage();
            //分页DEMO


            "加载完成".WriteToLog(log4net.Core.Level.Info);
        }


        /// <summary>
        /// 鼠标拖拽界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// 测试按钮1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            btn2.PerformClick();
        }

        /// <summary>
        /// 测试按钮2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("btn2");
        }
    }
}
