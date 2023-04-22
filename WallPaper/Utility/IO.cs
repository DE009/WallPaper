using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Forms;
using NReco.VideoConverter;
using WallPaper.Model;
using WallPaper.Properties;

namespace WallPaper.Utility
{
    internal class IO
    {
        /// <summary>
        /// 通过交互窗口，获得用户选中的目录
        /// </summary>
        /// <returns>返回选中文件路径</returns>
        public string OpenDirectory()
        {
            var resource_folderDialog = new FolderBrowserDialog();
            String resourcePath = "";
            List<String> video_files = new List<string>();
            if (resource_folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                resourcePath = resource_folderDialog.SelectedPath;
            }
            else
            {
                System.Windows.MessageBox.Show("目录选择失败");
                return null;
            }
            //video_files = ScanVideos(resourcePath);
            return resourcePath;


        }

        public List<VideoFilePreview> CombineVideoFilePreview(String Resource)
        {
            List<VideoFilePreview> previews = new List<VideoFilePreview>();
            List<string> Files = new List<string>();
            Files = ScanVideos(Resource);
            List<BitmapImage> ThumbNails = GeneratePreview(Files);
            foreach (var items in Files.Zip(ThumbNails, (x, y) => (x, y)))
            {
                previews.Add(new VideoFilePreview { FileName = System.IO.Path.GetFileName(items.x), Thumbnail = items.y });
            }
            return previews;
        }
        /// <summary>
        /// 获取每个文件路径，对应文件的缩略图，
        /// </summary>
        /// <param name="videofile">一个文件路径的List</param>
        /// <returns>返回bitmapImage类型，可直接作为image类型的source</returns>
        public List<BitmapImage> GeneratePreview(List<string> videofile)
        {
            var converter = new NReco.VideoConverter.FFMpegConverter();
            var thumbnail = new List<BitmapImage>();
            foreach (String VideoPath in videofile)
            {
                //使用NReo中的FFMpeg，提取预览图

                String FileName = System.IO.Path.GetFileName(VideoPath);
                var stream = new MemoryStream(); 
                try
                {

                    converter.GetVideoThumbnail(VideoPath, stream) ;
                    //将memorystream转换为bitmapimage对象。
                    stream.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    thumbnail.Add(bitmapImage);

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Can't extract img from file:" + VideoPath + ",file may be corrupted or wrong path; Error:" + ex.ToString());
                    return null;
                }

            }
            return thumbnail;
        }
        public List<String> ScanVideos(string folderPath)
        {
            // Get all files in the folder and its subfolders
            string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            List<String> result = new List<String>();
            // Check each file if it is a video file and add it to the list
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file).ToLower();
                if (extension == ".mp4" || extension == ".mkv" || extension == ".avi")
                {
                    result.Add(file);
                }
            }
            return result;
        }
}
}
