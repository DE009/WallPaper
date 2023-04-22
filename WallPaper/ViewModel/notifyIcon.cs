using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallPaper.Utility;
using WallPaper.ViewModel;

namespace WallPaper.Model
{
    internal class notifyIcon
    {
        private NotifyIcon notifyicon = null;
        private MainViewModel _data_context;
        public notifyIcon(MainViewModel DataContext)
        {
            _data_context = DataContext;    
        }
        public void InitializeNotifyIcon()
        {
            System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();

            MenuItem Start = new MenuItem("▶设置壁纸");
            Start.Click += new EventHandler(StartClick);

            MenuItem Stop = new MenuItem("🛑关闭壁纸");
            Stop.Click += new EventHandler(StopClick);

            MenuItem Exit = new MenuItem("退出");
            Exit.Click += new EventHandler(ExitClick);
            contextMenu1.MenuItems.Add(Start);
            contextMenu1.MenuItems.Add(Stop);
            contextMenu1.MenuItems.Add(Exit);


            //系统托盘图标初始化
            //设置托盘的各个属性
            var info = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/window.ico"));
            var icon = new System.Drawing.Icon(info.Stream);
            notifyicon = new NotifyIcon
            {
                BalloonTipText = 
                "窗口："+(_data_context.ControlVisibility ==true?"已隐藏":"已显示")
                +"\n壁纸状态:"+(_data_context.ButtonStatus==(int)Utility.EnumVar.ButtonStatus.Stopped?"未设置":"正在运行"),
                Text = "动态壁纸",
                Icon = icon,
                Visible = true,
                ContextMenu = contextMenu1,
            };

            notifyicon.MouseClick += new MouseEventHandler(NotifyiconRightClick);
        }

        private void NotifyiconRightClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                _data_context.ControlVisibility = true;
                _data_context.ApplyButtonStatus();
            }
            return;
        }

        private void ExitClick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            _data_context.exit_reason = (int)EnumVar.ExitReason.ExitByTray;
            notifyicon.Dispose();
        }

        private void StopClick(object sender, EventArgs e)
        {
            _data_context.WallpaperClose();
        }

        private void StartClick(object sender, EventArgs e)
        {
            _data_context.WallPaperSet();
        }
    }
}
