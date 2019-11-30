using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace 压缩图片小工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dirPath = "";
        private int count = 0 ;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                dirPath = openFileDialog.SelectedPath;
                deleteTest(dirPath);
                //计算文件总数
                count = 0;
                getFileCount(dirPath);

                pbar.Minimum = 0;
                pbar.Maximum = count;
                pbar.Value = 0;
                Thread thread = new Thread(()=> {
                    each(dirPath);
                    MessageBox.Show("压缩完毕","提示");
                });
                thread.Start();
                
            }
        }


        private void getFileCount(string path) {

            DirectoryInfo TheFolder = new DirectoryInfo(path);

            DirectoryInfo[] directoryInfos = TheFolder.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                getFileCount(directoryInfo.FullName);
            }
            
            FileInfo[] files = TheFolder.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".jpg" || file.Extension == "png" || file.Extension == "gif")
                {
                    count++;
                }
            }

        }
        private void deleteTest(string path) {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //判断是否有压缩文件夹
            DirectoryInfo mypath = new DirectoryInfo(path + "\\压缩\\");
            if (mypath.Exists)
            {
                deleteDir(mypath.FullName);
                //MessageBox.Show("压缩文件夹删除完毕");
            }
            DirectoryInfo[] directoryInfos = TheFolder.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                deleteTest(directoryInfo.FullName);
            }
        }

        private void each( string path) {
            DirectoryInfo TheFolder = new DirectoryInfo(path);

            DirectoryInfo[] directoryInfos = TheFolder.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                each(directoryInfo.FullName);
            }
            //判断是否有压缩文件夹
            DirectoryInfo mypath = new DirectoryInfo(path + "\\压缩\\");
        
            //先判断是否有需要压缩的图片
            FileInfo[] files = TheFolder.GetFiles();
            int fileCount = 0;
            foreach( FileInfo file in files) {
                if(file.Extension == ".jpg"||file.Extension == "png" || file.Extension == "gif")
                {
                    fileCount++;
                }
            }
            if(fileCount > 0)
            {
                

                //创建压缩文件夹
                mypath.Create();
                
                foreach (FileInfo fileInfo in TheFolder.GetFiles())
                {

                    string savePath = fileInfo.DirectoryName + "\\压缩\\" + fileInfo.Name;
                    CompressImage(fileInfo.FullName, savePath);
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        //这里写代码
                        pbar.Value += 1;
                    }));
                }
            }

          
        }

        private void deleteDir(string dirPath) {
            DirectoryInfo mypath = new DirectoryInfo(dirPath);
            if (mypath.Exists)
            {
                //如果存在文件夹，执行删除
                //判断是否有文件
                FileInfo[] fileList =  mypath.GetFiles();
                if (fileList.Length > 0) {
                    foreach (FileInfo f in fileList) {
                        f.Delete();
                    }
                }
                //判断是否有文件夹
                DirectoryInfo[] directoryList = mypath.GetDirectories();
                if (directoryList.Length > 0) {
                    foreach (DirectoryInfo directoryInfo in directoryList)
                    {
                        deleteDir(directoryInfo.FullName);
                    }
                }
                mypath.Delete();
            }
        }


        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        private  bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                return true;
            }
            try
            {
                System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
                ImageFormat tFormat = iSource.RawFormat;
                int dHeight = iSource.Height / 2;
                int dWidth = iSource.Width / 2;
                int sW = 0, sH = 0;
                //按比例缩放
                System.Drawing.Size tem_size = new System.Drawing.Size(iSource.Width, iSource.Height);
                if (tem_size.Width > dHeight || tem_size.Width > dWidth)
                {
                    if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = Convert.ToInt32(dWidth) * tem_size.Height / tem_size.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (tem_size.Width * dHeight) / tem_size.Height;
                    }
                }
                else
                {
                    sW = tem_size.Width;
                    sH = tem_size.Height;
                }

                Bitmap ob = new Bitmap(dWidth, dHeight);
                Graphics g = Graphics.FromImage(ob);

                g.Clear(System.Drawing.Color.WhiteSmoke);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(iSource, new System.Drawing.Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

                g.Dispose();

                //以下代码为保存图片时，设置压缩质量
                EncoderParameters ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = flag;//设置压缩的比例1-100
                EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
                ep.Param[0] = eParam;

                try
                {
                    ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo jpegICIinfo = null;
                    for (int x = 0; x < arrayICI.Length; x++)
                    {
                        if (arrayICI[x].FormatDescription.Equals("JPEG"))
                        {
                            jpegICIinfo = arrayICI[x];
                            break;
                        }
                        //if (arrayICI[x].FormatDescription.Equals("PNG"))
                        //{
                        //    jpegICIinfo = arrayICI[x];
                        //    break;
                        //}
                        //if (arrayICI[x].FormatDescription.Equals("GIF"))
                        //{
                        //    jpegICIinfo = arrayICI[x];
                        //    break;
                        //}
                    }
                    if (jpegICIinfo != null)
                    {
                        ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                        FileInfo fi = new FileInfo(dFile);
                        if (fi.Length > 1024 * size)
                        {
                            flag = flag - 10;
                            CompressImage(sFile, dFile, flag, size, false);
                        }
                    }
                    else
                    {
                        ob.Save(dFile, tFormat);
                    }

                    iSource.Dispose();
                    ob.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception e) {
                return false;
            }
            
           
        }
    }
}
