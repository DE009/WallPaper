using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WallPaper.Model
{
    [Serializable]
    public class CurrentStatus
    {
        public String CurrentDir { get; set; }
        public String CurrentFile { get; set; }
    }
    public class VideoFilePreview
    {
        public String FileName { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
