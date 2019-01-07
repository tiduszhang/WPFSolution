using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using Common;
using MVVM;
using MVVM.Model;

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

            ////分页DEMO
            //var pageViewModel = new PageViewModel<T2>();
            //pageViewModel.PageSize = 20;
            //this.pageView.PageViewModel = pageViewModel;
            //dataGrid.ItemsSource = pageViewModel.ObservableCollectionObject;
            //pageViewModel.ChangePageFun = () =>
            //{
            //    PageData<T2> pageData = NoSQLHelper.LoadByPage<T2>(pageViewModel.PageIndex, pageViewModel.PageSize);
            //    pageData.FillPage(pageViewModel);
            //    return pageData.QueryData;
            //};
            //pageViewModel.ChangePage();
            ////分页DEMO
            ObservableCollection<T2> observableCollection = new ObservableCollection<T2>();
            observableCollection.Add(new T2() { TTTTT = T1.a });
            observableCollection.Add(new T2() { TTTTT = T1.b });
            dataGrid.ItemsSource = observableCollection;
            //dataGrid.DataContext = observableCollection;
            grid1.DataContext = observableCollection[0]; 
            "加载完成".WriteToLog("", log4net.Core.Level.Info);
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


    public enum T1
    {
        [Display(Name = "Test1")]
        a,
        [Display(Name = "Test2")]
        b,
        [Display(Name = "Test3")]
        c,
    }

    public class T2 : NotifyBaseModel
    {
        [Display(Name = "属性TTTTT")]
        public T1 TTTTT
        {
            get
            {
                return this.GetValue(o => o.TTTTT);
            }
            set
            {
                this.SetValue(o => o.TTTTT, value);
            }
        }
    }
}
