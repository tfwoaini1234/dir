using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

namespace 音乐下载器
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dictionary<string, object> s = new Dictionary<string, object>();
            s.Add("text", "fjdksl");
       
            musicList.Items.Add(new { id="1", name= "name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1" });
        }

        /**
         * 
         *单选框 
         *
         */
        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            MessageBox.Show(checkBox.Tag.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = 1;
            string page = "1";
            WebClient web = new WebClient();
            string webSite = "http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword=" + search.Text + "&page=" + page + "&pagesize=100";
            byte[] buffer = web.DownloadData(webSite);
            string html = Encoding.UTF8.GetString(buffer);
            JObject kugou = JObject.Parse(html);
            List<JToken> all = kugou["data"]["info"].Children().ToList();
            all.ForEach(x =>
            {
            KugouResult kg = JsonConvert.DeserializeObject<KugouResult>(x.ToString());
            //kg.hash = x["320hash"].ToString();    //320音质的hash值
            if (kg.sqhash != "")
            {
                kg.key = GetMD5(kg.sqhash + "kgcloud");
                webSite = "http://trackercdn.kugou.com/i/?cmd=4&hash=" + kg.sqhash + "&key=" + kg.key + "&pid=1&forceDown=0&vip=1";
                buffer = web.DownloadData(webSite);
                html = Encoding.UTF8.GetString(buffer);
                JObject flac = JObject.Parse(html);
                if (flac["status"].ToString() == "1")   //成功获取才添加到显示列表和Result中
                {
                        string name = flac["fileName"].ToString();
                        string ext = flac["extName"].ToString();
                        string bt = flac["bitRate"].ToString();
                        string size = (double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB";
                        TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                        string time = (ts.Minutes + ":" + ts.Seconds.ToString("00"));
                        string url = flac["url"].ToString().Replace("\\", "");
                        musicList.Items.Add(new { id = id, name = flac["fileName"],ext=ext,bt=bt,size= size, time=time,url=url });
                        id++;
                        //SkinListBoxItem sl = new SkinListBoxItem(kg.filename);
                        //resultListView.Items.Add(sl);

                        //ListViewItem lvi = new ListViewItem();
                        //lvi.Text = kg.filename;
                        //lvi.SubItems.Add(flac["bitRate"].ToString());
                        //lvi.SubItems.Add(flac["extName"].ToString());
                        //lvi.SubItems.Add((double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB");  //将文件大小装换成MB的单位
                        //TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                        //lvi.SubItems.Add(ts.Minutes + ":" + ts.Seconds.ToString("00"));
                        //lvi.Tag = flac["url"].ToString().Replace("\\", "");
                        //listViewItems.Add(lvi);
                    }
                }
            });
        }

        /// <summary>
        /// 计算MD5获得下载的key值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetMD5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] bf = Encoding.Default.GetBytes(str);
            byte[] mbf = md5.ComputeHash(bf);
            string s = "";
            for (int i = 0; i < mbf.Length; i++)
            {
                s += mbf[i].ToString("x2");
            }
            return s;
        }

        public void DownloadHttpFile(String http_url, String save_url)
        {
            WebResponse response = null;
            //获取远程文件
            WebRequest request = WebRequest.Create(http_url);
            response = request.GetResponse();
            if (response == null) return;
            //读远程文件的大小
            pbDown.Maximum = response.ContentLength;
            //下载远程文件
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Stream netStream = response.GetResponseStream();
                Stream fileStream = new FileStream(save_url, FileMode.Create);
                byte[] read = new byte[1024];
                long progressBarValue = 0;
                int realReadLen = netStream.Read(read, 0, read.Length);
                while (realReadLen > 0)
                {
                    fileStream.Write(read, 0, realReadLen);
                    progressBarValue += realReadLen;
                    pbDown.Dispatcher.BeginInvoke(new ProgressBarSetter(SetProgressBar), progressBarValue);
                    realReadLen = netStream.Read(read, 0, read.Length);
                }
                netStream.Close();
                fileStream.Close();

            }, null);
        }
        /// <summary>
        ///  判断远程文件是否存在
        /// </summary>
        /// <param name="fileUrl">文件URL</param>
        /// <returns>存在-true，不存在-false</returns>
        private bool HttpFileExist(string http_file_url)
        {
            WebResponse response = null;
            bool result = false;//下载结果
            try
            {
                response = WebRequest.Create(http_file_url).GetResponse();
                result = response == null ? false : true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }
        public delegate void ProgressBarSetter(double value);
        public void SetProgressBar(double value)
        {
            //显示进度条
            pbDown.Value = value;
            //显示百分比
            //label1.Content = (value / pbDown.Maximum) * 100 + "%";
        }
    }

    class KugouResult
    {
        public string filename { get; set; }
        public string sqhash { get; set; }
        public string key { get; set; }
    }
}
