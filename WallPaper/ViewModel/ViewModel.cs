using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Drawing;
using WallPaper.Model;
using System.Runtime.InteropServices;
using WallPaper.Utility;

namespace WallPaper.ViewModel
{
    /*
     * 待做：
     * 窗口：特别是button美化
     * 系统托盘。
     * 添加作者信息
     */
    public class MainViewModel : INotifyPropertyChanged
    {
        /*
         * 初始化Model类
         * 初始化中间变量
         * 初始化工具类
         *初始化ICommand命令对象
         */
        //Model类
        private CurrentStatus _cur_status= new CurrentStatus();
        private List<VideoFilePreview> _video_previews = new List<VideoFilePreview>();  //中间变量用于返回图像和文件名

        //中间变量
        private VideoFilePreview _selected_previews= new VideoFilePreview();
        private Window WallPaper;
        private const String DataFile=".\\Data.bin";    //数据存储位置
        private const String ButtonRunning = "⏸停止壁纸";
        private const String ButtonStoped = "▶设为壁纸";
        private String _button_text=ButtonStoped;
        private Boolean _control_visibility = true;
        public int exit_reason=0;
        private int _button_status = 0;

        
        //初始化工具类
        private Utility.IO utility = new Utility.IO();
        private Utility.WallPaper windowcontrol = new Utility.WallPaper();
        private Utility.Serializer<CurrentStatus> serializer = new Utility.Serializer<CurrentStatus>();
        //初始化ICommand
        public ICommand PathSelect { get; set; }
        public ICommand WindowClose { get; set; }
        public ICommand WindowSet { get; set; }
        public ICommand ControlExit { get; set; }
        public ICommand ControlExiting { get; set; }
        private ICommand _window_control;
        public ICommand WindowControl { get
            {
                return this._window_control;
            }set { 
                this._window_control = value;
                OnPropertyChanged("WindowControl");
            }
            }
        /* 
         * 构造方法，初始化
         */
        public MainViewModel()
        {
            //页面各类触发ICommand绑定
            PathSelect = new RelayCommand(Preview);
            WindowClose = new RelayCommand(WallpaperClose);
            WindowSet = new RelayCommand(WallPaperSet);
            ControlExit = new RelayCommand(ControlExited);
            ControlExiting = new RelayCommand<CancelEventArgs>(ControlWindowExiting);
            WindowControl = WindowSet;
            //页面初始化
            if (File.Exists(DataFile))
            {
                CurStatus= serializer.Deserialize(DataFile);
                VideoFilePreviews = utility.CombineVideoFilePreview(CurStatus.CurrentDir);
                //待选：直接将读入的cur_file设置为壁纸,同时初始化按钮信息。
                //若不存在文件，则不运行。
                SelectedPreview.FileName = CurStatus.CurrentFile;
                WallPaperSet();
            }
        }
        /*
         * 构建公共实体类型
         * 设置返回值，并触发修改事件，同时前端绑定数据的控件更新数据。
         */
        /// <summary>
        /// 当前状态Model类
        /// </summary>
        public CurrentStatus CurStatus
        {
            get { return this._cur_status; }
            set {
                this._cur_status = value;
                OnPropertyChanged("CurStatus");
            }
        }
        public List<VideoFilePreview> VideoFilePreviews
        {
            get { return this._video_previews;  }
            set
            {
                this._video_previews = value;
                OnPropertyChanged("VideoFilePreviews");
            }
        }
        /// <summary>
        /// 被选中的VideoFile
        /// </summary>
        public VideoFilePreview SelectedPreview
        {
            get { return this._selected_previews; }
            set
            {
                this._selected_previews = value;
                OnPropertyChanged("SelectedPreview");
            }
        }
        public String ButtonText
        {
            get { return this._button_text; }
            set
            {
                this._button_text = value;
                OnPropertyChanged("ButtonText");
            }
        }		

        public Boolean ControlVisibility
        {
            get { return this._control_visibility; }
            set
            {
                this._control_visibility = value;
                OnPropertyChanged("ControlVisibility");
            }
        }
        public int ButtonStatus
        {
            get { return this._button_status;  }
        }

        /*
         * Command 对应的自定义触发函数。
         */

        /// <summary>
        /// 选择资源目录button对应触发函数
        /// 选择并扫描目录下视频文件，生成preview。
        /// </summary>
        private void Preview()
        {
            String Resource = utility.OpenDirectory();
            if (Resource != null)
            {
                VideoFilePreviews= utility.CombineVideoFilePreview(Resource);
                CurStatus.CurrentDir = Resource;
            }

        }
        public void WallPaperSet()
        {
            if (this.CurStatus.CurrentDir == null || this.SelectedPreview.FileName==null)
            {
                return;
            }
            //对背景窗口设置
            if (WallPaper != null)
            {
                windowcontrol.close_window(ref this.WallPaper);
            }
            windowcontrol.StartWallPaper(ref this.WallPaper, this.CurStatus.CurrentDir+"\\"+this.SelectedPreview.FileName);
            CurStatus.CurrentFile = this.SelectedPreview.FileName;
            //修改buttong对应ICommand和text
            this._button_status = (int)Utility.EnumVar.ButtonStatus.Running;
            ApplyButtonStatus();
            

        }
        public void WallpaperClose()
        {
            windowcontrol.close_window(ref this.WallPaper);
            CurStatus.CurrentFile = null;
            //修改button绑定事件和内容
            this._button_status = (int)Utility.EnumVar.ButtonStatus.Stopped;
            ApplyButtonStatus();

        }
        /// <summary>
        /// 窗口退出时，保存数据，并关闭壁纸窗口
        /// </summary>
        public void ControlExited()
        {
            windowcontrol.close_window(ref this.WallPaper);
            //序列化保存当前状态。
            serializer.Serialize(CurStatus, DataFile);
        }
        /// <summary>
        /// 用户希望退出时，判断是否最小化
        /// </summary>
        /// <param name="e"></param>
        private void ControlWindowExiting(CancelEventArgs e)
        {
            if (this.exit_reason == (int)EnumVar.ExitReason.ExitByWindow)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("是否需要最小化到系统托盘？", "确认", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    ControlVisibility = false;
                }
                return;
            }
            else if (this.exit_reason == (int)EnumVar.ExitReason.ExitByTray)
            {
                return;
            }
        }


        /*
         * 数据绑定，自动更新。
         * PropertyName就是xaml页面上，绑定的数据名称，只有名称一至，才会通知到。
         */
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string PropertyName)
        {
            //MessageBox.Show("onChan");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
                
            }
        }
        /*
         * 控制Button内容和触发函数修改，避免窗口隐藏时，因修改内容而重新显示
         */
        public void ApplyButtonStatus()
        {
            //若窗口显示，则将当前按钮状态映射
            //若窗口不显示，则什么都不做。
            if(this.ControlVisibility == true)
            {
                if (this._button_status == (int)Utility.EnumVar.ButtonStatus.Running)
                {
                    WindowControl = WindowClose;
                    ButtonText = ButtonRunning;
                }
                else
                {
                    WindowControl = WindowSet;
                    ButtonText = ButtonStoped;
                }
            }
            else
            {
                return;
            }

            
        }


        
    }
}
