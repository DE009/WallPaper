using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using WallPaper.Model;
using System.IO;

namespace WallPaper.Utility
{
    internal class WallPaper
    {
        [DllImport(".\\Libs\\Dll1.dll")] 
        static extern void SetWindowHandle(IntPtr media);
        
        //Window Wallpaper_window
        public void StartWallPaper(ref Window WallPaper,String FilePath)
        {
            
            WallPaper = new Window();
            WallPaper.Loaded += wallpaper_window_load;   //设置新窗口load的触发函数
            //新建MediaEelement
            MediaElement wallpaper = new MediaElement();

            Grid grid = new Grid();
            grid.SetValue(Grid.BackgroundProperty, Brushes.Black);

            //生成storyboard
            var timeline = new MediaTimeline  //timeline，定义WPF中，动画的工作流程。内部定义了一系列对当前元素的操作。按时间顺序执行。
            //这里MediaTImeline是timeline的派生类，是对mediaelement元素进行timeline设定的事件流。
            {
                Source = new Uri(@FilePath),    //在timeline中，先对media，设定source
                
                RepeatBehavior = RepeatBehavior.Forever,//设定该mediaelement的重复行为为无限，可指定当前timeline重复几次
            };
            //Uri对象，是一个文件路径对象。
            //Uri 类表示统一资源标识符（URI），它是一种用于标识某个资源的字符串。URI 可以用来标识各种类型的资源，如网页、文件、图像等。
            var storyboard = new Storyboard();  //创建storyboard，可用于控制一组timeline操作。
            storyboard.Children.Add(timeline);
            Storyboard.SetTarget(storyboard, wallpaper);
            storyboard.Begin();

            //将内容填充进窗口。
            grid.Children.Add(wallpaper);
            WallPaper.Content = grid;

            WallPaper.Show(); // show the new window
            return;
        }
        private void wallpaper_window_load(object sender, RoutedEventArgs e)
        {
            //获取当前的视频窗口句柄，
            var windowInteropHelper = new WindowInteropHelper((Window)sender);
            IntPtr windowHandle = windowInteropHelper.Handle;   //得到当前窗口的handle
            Debug.WriteLine(windowHandle);
            Window Wallpaper_window = (Window)sender;


            //设置窗口全屏
            Wallpaper_window.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Wallpaper_window.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            //myMediaElement.Margin = new Thickness(0, 0, 0, 0);
            //变成无边窗体
            Wallpaper_window.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
            Wallpaper_window.WindowStyle = WindowStyle.None;
            Wallpaper_window.ResizeMode = ResizeMode.NoResize;
            Wallpaper_window.Topmost = true;//最大化后总是在最上面
            Wallpaper_window.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            Wallpaper_window.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            Wallpaper_window.WindowState = WindowState.Maximized;

            // DLL加载成功
            Console.WriteLine("DLL loaded successfully.");
            Debug.WriteLine("loaded");
            SetWindowHandle(windowHandle);
            Debug.WriteLine("over");
        }
        public bool close_window(ref Window window)    //加上ref关键字，才能修改传入的参数的值，类似&，且调用时也需要ref关键字
        {
            if (window == null)
            {
                return false;
            }
            try
            {
                //关闭并释放窗口所占用资源，但不会销毁window对象。
                window.Close();
                window = null;  //对象置空，方便判断当前window是否存在。
            }
            catch
            {
                MessageBox.Show("壁纸窗口关闭错误");
                return false;
            }
            return true;
        }
    }
}
