using AForge.Video.DirectShow;
using Aliyun.Acs.Core.Profile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 自动生成目录小工具
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dirPath = "E:/媒体库/视频/新番/";
        private string savePath = "D:/www/center/public/static/video/";
        private bool kg = true;
        private DispatcherTimer dispatcherTimer;
        private string saveIp = "";
        private int copyNum = 0;
        Dictionary<string, Dictionary<string, string>> zList = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> imageList = new Dictionary<string, Dictionary<string, string>>();
        private string apiUrl = "https://www.aitaotu.com";
        public MainWindow()
        {
            InitializeComponent();
            //定时任务
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(run);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();
            //bug();
            //addOneList();
            //ThreadPool.QueueUserWorkItem(o=>getImage());
        }
        
        private void run(object sender, EventArgs e) {    
            string logText = "";
            string ip = getIp();
            bool isIp = Regex.IsMatch(ip, @"^(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)$");
            Console.WriteLine(ip);
            if (isIp)
            {
                //对比ip是否变更
                if (!saveIp.Equals(ip))
                {
                    //不相等就更新
                    try
                    {
                        Dictionary<string, string> ditParam = new Dictionary<string, string>();
                        ditParam.Add("Action", "UpdateDomainRecord");
                        ditParam.Add("RecordId", "17595591211380736");
                        ditParam.Add("RR", "home");//主机记录的关键字
                        ditParam.Add("Type", "A");//解析类型的关键字
                        ditParam.Add("Value", ip);//记录值的关键字

                        qianming q = new qianming();
                        q.ComputeSignature(ditParam);
                        string param = string.Join("&", ditParam.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value)));
                        //string url = InterFaceUrl + "?" + param;
                        string InterFaceUrl = "http://alidns.aliyuncs.com"; //请求路径
                        Console.WriteLine(InterFaceUrl + "?" + param);
                        MyWebBrowser webBrowser1 = new MyWebBrowser();
                        FileInfo file =  webBrowser1.DownloadFile(InterFaceUrl + "?" + param, "test.txt");
                        saveIp = ip;
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            //这里写代码
                            string s = errorLog.Text;
                            errorLog.Text = "接口请求成功\r\n";
                            errorLog.Text += s;
                        }));
                    }
                    catch (Exception e1)
                    {
                        
                        Console.WriteLine(e1);
                    }
                }
            }
            else
            {
                logText = "未获取到IP";
            }

            this.Dispatcher.Invoke(new Action(delegate
            {
                //这里写代码
                log.Text = logText;
            }));
        }

        private  string getIp() {
            try
            {
               // string res = HttpRequestHelper.HttpGet("http://ip.taobao.com/service/getIpInfo.php", "ip=myip");
                MyWebBrowser webBrowser1 = new MyWebBrowser();
                FileInfo file = webBrowser1.DownloadFile("http://ip.taobao.com/service/getIpInfo.php?ip=myip","ip.txt");
                if (File.Exists(file.FullName))
                {
                    string text = System.IO.File.ReadAllText(file.FullName);
                    Console.WriteLine(text);
                    JObject json = (JObject)JsonConvert.DeserializeObject(text);
                    if ((Int16)json["code"] == 0)
                    {
                        JObject data = (JObject)json["data"];
                        string ip = (String)data["ip"];
                        return ip;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
                
            }
            catch(Exception e)
            {

                this.Dispatcher.Invoke(new Action(delegate
                {
                    //这里写代码
                    string s = log.Text;
                    log.Text = e.Message + "\r\n";
                    log.Text += s;
                }));
                return "";
            }
            
        }


        private  void addOneList() {

            //获取初始的列表
            MyWebBrowser webBrowser1 = new MyWebBrowser();
            FileInfo file = webBrowser1.DownloadFile("https://www.aitaotu.com/", "index.txt");
            if (File.Exists(file.FullName))
            {
                string text = System.IO.File.ReadAllText(file.FullName);
                Regex reg = new Regex(@"(?is)<a[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?>");
                MatchCollection mm = reg.Matches(text);
                foreach (Match m in mm)
                {
                    string it = m.Groups["href"].Value;
                    if (it.IndexOf("http") == -1)
                    {
                        if (!zList.ContainsKey(it))
                        {
                            //不存在key
                            Dictionary<string, string> item = new Dictionary<string, string>();
                            item.Add("value", it);
                            item.Add("isUse", "0");
                            zList.Add(it,item);
                            errorLog.Text += (it + "\r\n");
                        }

                    }
                   
                }
                //获取图片地址
                Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

                // 搜索匹配的字符串
                MatchCollection matches = regImg.Matches(text);
                foreach (Match m2 in matches)
                {
                    string it = m2.Groups["imgUrl"].Value;
                    if (it.IndexOf("http") == -1)
                    {
                        it = apiUrl + it;
                    }
                    if (!imageList.ContainsKey(it))
                    {
                        //不存在key
                        Dictionary<string, string> item = new Dictionary<string, string>();
                        item.Add("value", it);
                        item.Add("isUse", "0");
                        imageList.Add(it, item);
                        errorLog.Text += (it + "\r\n");
                    }
                }
            }
        }

        private void getImage() {
            Dictionary<string, string> img = new Dictionary<string, string>();
            bool kg = true;
            while (kg)
            {
                kg = false;
                foreach (var item in imageList)
                {
                    img = (Dictionary<string, string>)item.Value;
                    string isUse = img["isUse"];
                    if ("0".Equals(isUse))
                    {
                        kg = true;
                        //未使用,获取图片
                        ImgSave(img["value"]);
                        img["isUse"] = "1";
                        break;
                    }
                }
            }
         
            //开始下载图片

        }

        /// <summary>
        /// 图片另存为
        /// </summary>
        /// <param name="url">路径</param>
        public void ImgSave(string url)
        {
            //http://203.156.245.58/sipgl/index.jsp
            url = "https://img.aitaotu.cc:8089/Thumb/2019/0228/79693d79a4d52295363c12bbfb9becc9.jpg";
            WebRequest imgRequest = WebRequest.Create(url);

            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)imgRequest.GetResponse();
            }
            catch (WebException ex)
            {

                res = (HttpWebResponse)ex.Response;
            }

            if (res.StatusCode.ToString() == "OK")
            {
                System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequest.GetResponse().GetResponseStream());

                string deerory = string.Format(@"D:\img\{0}\", DateTime.Now.ToString("yyyy-MM-dd"));

                string fileName = string.Format("{0}.png", DateTime.Now.ToString("HHmmssffff"));


                if (!System.IO.Directory.Exists(deerory))
                {
                    System.IO.Directory.CreateDirectory(deerory);
                }
                downImage.Save(deerory + fileName);
                downImage.Dispose();
            }

        }
        private void bug() {
            MyWebBrowser webBrowser1 = new MyWebBrowser();
            FileInfo file = webBrowser1.DownloadFile("https://www.aitaotu.com/shanggan/", "index.txt");
            
            string text = System.IO.File.ReadAllText(file.FullName);
            Regex reg = new Regex(@"(?is)<a[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?>");
            MatchCollection mm = reg.Matches(text);
            foreach (Match m in mm)
            {
                errorLog.Text +=(m.Groups["href"].Value + "\r\n");
            }
            int q = text.IndexOf("list_3");
            Regex regex = new Regex(@"<a.*href=""(/.+/|/.+\.html)"".*>", RegexOptions.ECMAScript);
            MatchCollection match = regex.Matches(text);
            for (int i = 0; i < match.Count; i++)
            {
                string str = match[i].Value;
                log.Text += str + "\r\n";
                string[] sArray = Regex.Split(str, "</a>", RegexOptions.IgnoreCase);
                if (sArray.Length > 0)
                {
                    for (int s = 0; s < sArray.Length; s++)
                    {
                        string newstr = sArray[s];
                        Regex regex2 = new Regex(@"/.+/|/.+\.html", RegexOptions.ECMAScript);

                        Match match2 = regex2.Match(newstr);
                        if (match2.Length > 0)
                        {
                           // errorLog.Text += match2.Value + "\r\n";
                        }
                    }

                }
            }

        }

        private void shuaxin() {
            while (true)
            {
                eachDir();
                Thread.Sleep(1000*3600);
            }
        }
        private void scanning(object sender, EventArgs e) {
            try
            {
                dispatcherTimer.Stop();
                // HttpRequestHelper.HttpGet("http://video.zxchobits.com:8282/nas/dir/index", "");
                HttpRequestHelper.HttpGet("http://center.zxchobits.com/zxchobits/dns/checkIp?code=115599", "");
                //eachDir();
                dispatcherTimer.Start();
                log.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") + "刷新了一次\r\n";
                log.ScrollToEnd();
            }
            catch(Exception error)
            {
                errorLog.Text += error.Message+"\r\n";
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            //if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            //{
            //    dirName.Text = openFileDialog.SelectedPath;
            //    dirPath = dirName.Text;
            //}
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FilterInfoCollection videoDevices;
            VideoCaptureDevice videoSource;
        videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            int selectedDeviceIndex = 0;
            videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceIndex].MonikerString);//连接摄像头。
            videoSource.VideoResolution = videoSource.VideoCapabilities[selectedDeviceIndex];
            
            //videoDev.VideoSource = videoSource;
            // set NewFrame event handler
            //videoDev.Start();
            //log.Text = "";
            //try
            //{
            //    DirectoryInfo mypath = new DirectoryInfo(dirPath);
            //    if (!mypath.Exists)
            //    {
            //        MessageBox.Show("你还没有选择文件夹", "警告");
            //    }
            //    else
            //    {
            //        if (kg)
            //        {
            //            kg = false;
            //            //如果存在，就开始递归
            //            Thread thread = new Thread(eachDir);
            //            thread.Start();
            //        }


            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("你还没有选择文件夹", "警告");
            //}


        }
        public void eachDir()
        {

            copyNum = 0;
            each(dirPath,"");
            kg = true;
            if (copyNum > 0) {
                //如果 有复制文件，请求网页刷新列表
                HttpRequestHelper.HttpGet("http://video.zxchobits.com:8282/nas/dir/index", "");
            }
            else
            {
                //如果 没有复制文件，请求网页刷新列表
                HttpRequestHelper.HttpGet("http://video.zxchobits.com:8282/nas/dir/updateTime", "");
            }
            copyNum = 0;
        }

        private void each(string path,string p)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //先遍历当前文件夹的文件
            foreach (FileInfo fileInfo in TheFolder.GetFiles())
            {
                //判断是不是mp4文件
                string ext = fileInfo.Extension.ToLower();
                string extension = ".mp4";
                if (extension.IndexOf(ext) >= 0) {

                    //判断该移动文件夹是否存在
                    string sPath = savePath + p;
                    if (Directory.Exists(sPath) == false)//如果不存在就创建file文件夹
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    //判断文件是否存在
                    string fPath = sPath + "/" + fileInfo.Name;
                    if (File.Exists(fPath))
                    {
                        //如果文件存在，比较文件是否一致
                        FileInfo fileInfo2 = new FileInfo(fPath);
                        if (fileInfo.Length != fileInfo2.Length)
                        {
                            //删除目标目录文件，重新复制
                            fileInfo2.Delete();
                            File.Copy(fileInfo.FullName, fPath);
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                //这里写代码
                                log.Text += fileInfo.Name + "文件大小不对，重新复制\r\n";
                                log.ScrollToEnd();
                            }));
                        }
                        else
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                //这里写代码
                                log.Text += fileInfo.Name + "已存在\r\n";
                                log.ScrollToEnd();
                            }));
                        }
                    }
                    else
                    {
                        //如果文件不存在，直接复制
                        File.Copy(fileInfo.FullName, fPath);
                        copyNum++;
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            //这里写代码
                            log.Text += fileInfo.Name + "复制成功\r\n";
                            log.ScrollToEnd();
                        }));
                    }
                }
                  

                //strking ext = fileInfo.Extension.ToLower();
                //this.Dispatcher.Invoke(new Action(delegate
                //{
                //    //这里写代码
                //    log.Text += p+"\r\n";
                //    log.ScrollToEnd();
                //}));
                //string extension = ".mp4";
                //if (extension.IndexOf(ext) >= 0)
                //{
                    //string result = GetMD5HashFromFile(fileInfo.FullName);
                    //if (File.Exists(savePath+fileInfo.Name))
                    //{
                    //    this.Dispatcher.Invoke(new Action(delegate
                    //    {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "文件已经存在\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    //else
                    //{
                    //   // System.IO.File.Copy(fileInfo.FullName, savePath + fileInfo.Name);
                    //    this.Dispatcher.Invoke(new Action(delegate
                    //    {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "复制成功\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    ////是图片，开始上传信息
                    //try
                    //{

                    //    HttpRequestHelper.HttpGet("http://center.lovezhaoxin.com/zxchobits/szly_rand_q_q/wpfSetImage", "md5=" + result + "&content=" + fileInfo.Name.Replace(fileInfo.Extension, ""));
                    //    this.Dispatcher.Invoke(new Action(delegate {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "设置成功\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    //catch (Exception ex)
                    //{
                    //    this.Dispatcher.Invoke(new Action(delegate {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "出错拉\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}

                //}
            }
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                each(path + '/' + NextFolder.Name, p+"/"+NextFolder.Name);
            }
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
