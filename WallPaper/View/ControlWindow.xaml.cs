using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WallPaper.Model;
using WallPaper.ViewModel;

namespace WallPaper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ControlWindow: Window
    {
        public ControlWindow()
        {
            //绑定上下文；
            //view设定前端
            //model设定数据类型
            //ViewModel设定数据的传递和绑定。
            this.DataContext = new MainViewModel(); //注：对window的数据绑定，必须datacontext先于initializecomponent
            //否则就会无法获取到datacontext
            notifyIcon notifyicon = new notifyIcon((MainViewModel)this.DataContext);
            notifyicon.InitializeNotifyIcon();
            //这是因为InitializeComponent()方法会加载XAML文件并创建控件，如果您在调用InitializeComponent()方法之后设置DataContext属性，那么在XAML文件中定义的绑定可能无法正确解析源。
            InitializeComponent();
        }
        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }
    }
}
