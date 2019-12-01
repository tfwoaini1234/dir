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
        private int testCount = 0;
        private long dateTime = 0;
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("启动");
            ThreadPool.SetMaxThreads(1, 1);
            ThreadPool.SetMinThreads(1, 1);

        }
        private   void Button_Click1(object sender, RoutedEventArgs e)
        {
            int maxThreadNum, portThreadNum, minThreadNum;
            ThreadPool.SetMaxThreads(1, 1);
            ThreadPool.SetMinThreads(1, 1);
            //获取线程池的最大线程数和维护的最小空闲线程数
            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
            Console.WriteLine("最大线程数：{0}", maxThreadNum);
            Console.WriteLine("最小空闲线程数：{0}", minThreadNum);
            int x = 100;
            while(x-- > 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(test1), x);
            }
            Console.WriteLine($"启动第二个任务：计算{x}的6次方根");
            ThreadPool.QueueUserWorkItem(new WaitCallback(test2), x);
        }

        private  void test1(object i) {
            int number = Convert.ToInt32(i);
            Thread.Sleep(1000);
            Console.WriteLine("测试数字1：{0}", number);
        }

        private  void test2(object i)
        {
            int number = Convert.ToInt32(i);
            number--;
                 Thread.Sleep(110);
            Console.WriteLine("测试数字2：{0}", number);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.SetMaxThreads(10,10);
            ThreadPool.SetMinThreads(1, 1);
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性ee
            {
                dirPath = openFileDialog.SelectedPath;
                deleteTest(dirPath);
                //计算文件总数
                count = 0;
                testCount = 0;
                getFileCount(dirPath);

                pbar.Minimum = 0;
                pbar.Maximum = count;
                pbar.Value = 0;
                Console.WriteLine("图片总数:{0}", count.ToString());
                dateTime = ConvertDateTimeToInt(DateTime.Now);
                Console.WriteLine("开始时间:{0}", dateTime.ToString());

                each(dirPath);
                //Thread.Sleep(30000);
                //Console.WriteLine("进度条更新数:{0}", testCount.ToString());
                //Console.WriteLine("进度条值:{0}", pbar.Value.ToString());
                //Thread.Sleep(30000);
                //Console.WriteLine("进度条更新数1:{0}", testCount.ToString());
                //Console.WriteLine("进度条值1:{0}", pbar.Value.ToString());
                //MessageBox.Show("压缩完毕","提示");



            }
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        private  long ConvertDateTimeToInt(System.DateTime time)
        {
            //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //long t = (time.Ticks - startTime.Ticks) / 10000000;   //除10000调整为13位      
            long t = (time.Ticks - 621356256000000000) / 10000000;
            return t;
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
                    ImageParams image = new ImageParams();
                    image.sFile = fileInfo.FullName;
                    image.dFile = savePath;
                    image.flag = 90;
                    image.size = 300;
                    image.sfsc = true;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CompressImage), image);
                    //CompressImage(fileInfo.FullName, savePath);
                 
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

        private void add(int  type) {
            testCount++;
           
            Console.WriteLine("进度条更新数:{0},状态入口:{1}", testCount.ToString(), type.ToString()) ;
            this.Dispatcher.Invoke(new Action(delegate
            {
                //这里写代码
                pbar.Value += 1;
            }));
            if (testCount == count){
                long time1 = ConvertDateTimeToInt(DateTime.Now);
                long res = time1 - dateTime;
                Console.WriteLine("结束时间:{0}", res.ToString());
                MessageBox.Show("压缩完成", "提示");
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
        private  void CompressImage(object iamgeObj)
        {
            ImageParams imageParams = (ImageParams)iamgeObj;
            string sFile = imageParams.sFile;
            string dFile = imageParams.dFile;
            int flag = imageParams.flag;
            int size = imageParams.size;
            bool sfsc = imageParams.sfsc;
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                add(1);
                //return true;
            }
            else
            {
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
                        }
                        if (jpegICIinfo != null)
                        {
                            ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                            FileInfo fi = new FileInfo(dFile);
                            if (fi.Length > 1024 * size)
                            {
                                flag = flag - 10;
                                ImageParams image = new ImageParams();
                                image.sFile = sFile;
                                image.dFile = dFile;
                                image.flag = flag;
                                image.size = size;
                                image.sfsc = false;
                                CompressImage(image);
                            }
                            else
                            {
                                add(2);
                            }
                        }
                        else
                        {
                            ob.Save(dFile, tFormat);
                            add(3);
                        }

                        iSource.Dispose();
                        ob.Dispose();
                        //return true;
                    }
                    catch
                    {
                        //return false;
                    }
                }
                catch (Exception e)
                {
                    //return false;
                }
            }
         
            
           
        }
    }

    public class ImageParams
    {
        public string sFile { get; set; }
        public string dFile { get; set; }
        public int flag { get; set; }
        public int size { get; set; }
        public bool sfsc { get; set; }

    }
}
