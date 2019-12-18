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
        private string savePath = "";//保存目录
        private int page = 1;//起始页数
        List<Object> lists = new List<Object>();//搜索结果集合
        List<int> selectLists = new List<int>();
        List<Object> downList = new List<object>();
        int downType = 0;// 0未下载 1正在下载中 2下载完成
        public MainWindow()
        {
            InitializeComponent();
          //  musicList.Items.Add(new { id="1", name= "name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1name1" });
        }

        /**
         * 
         *单选框 
         *
         */
        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            selectLists.Add(int.Parse(checkBox.Tag.ToString()));
        }

        /**
         * 
         * 搜索
         * 
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            searchF();
        }

        private void searchF()
        {
            //重置列表
            musicList.Items.Clear();
            int id = 1;
            
            WebClient web = new WebClient();
            string webSite = "http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword=" + search.Text + "&page=" + page.ToString() + "&pagesize=100";
            byte[] buffer = web.DownloadData(webSite);
            string html = Encoding.UTF8.GetString(buffer);
            JObject kugou = JObject.Parse(html);
            List<JToken> all = kugou["data"]["info"].Children().ToList();
            
            all.ForEach(x =>
            {
                KugouResult kg = JsonConvert.DeserializeObject<KugouResult>(x.ToString());
                kg.hash320 = x["320hash"].ToString();    //320音质的hash值
                Dictionary<string, string> s = new Dictionary<string, string>();
                if (kg.hash320 != "") s.Add("hash320", kg.hash320);//320音质
                if (kg.hash != "") s.Add("hash", kg.hash);//普通音质
                if (kg.sqhash != "") s.Add("sqhash", kg.sqhash);//无损音质
                if(s.Count > 0)
                {
                    foreach (KeyValuePair<string, string> hash in s)
                    {
                        
                        string key = GetMD5(hash.Value + "kgcloud");
                        webSite = "http://trackercdn.kugou.com/i/?cmd=4&hash=" + hash.Value + "&key=" + key + "&pid=1&forceDown=0&vip=1";
                        buffer = web.DownloadData(webSite);
                        html = Encoding.UTF8.GetString(buffer);
                        JObject musicObj = JObject.Parse(html);
                        if(musicObj != null)
                        {
                            if (musicObj["status"].ToString() == "1")   //成功获取才添加到显示列表和Result中
                            {
                                Console.WriteLine(x["songname"]);
                                string name = x["songname"].ToString();
                                string ext = musicObj["extName"].ToString();
                                string bt = musicObj["bitRate"].ToString();
                                string size = (double.Parse(musicObj["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB";
                                TimeSpan ts = new TimeSpan(0, 0, int.Parse(musicObj["timeLength"].ToString())); //把秒数换算成分钟数
                                string time = (ts.Minutes + ":" + ts.Seconds.ToString("00"));
                                string url = musicObj["url"].ToString().Replace("\\", "");
                                
                                lists.Add(new { id = id, name = name, ext = ext, bt = bt, size = size, time = time, url = url });
                                musicList.Items.Add(new { id = id, name = name, ext = ext, bt = bt, size = size, time = time, url = url });
                                id++;
                            }
                        }
                    }
                }
               
                //if (kg.sqhash != "")
                //{
                //    kg.key = GetMD5(kg.sqhash + "kgcloud");
                //    webSite = "http://trackercdn.kugou.com/i/?cmd=4&hash=" + kg.sqhash + "&key=" + kg.key + "&pid=1&forceDown=0&vip=1";
                //    buffer = web.DownloadData(webSite);
                //    html = Encoding.UTF8.GetString(buffer);
                //    JObject flac = JObject.Parse(html);
                //    if (flac["status"].ToString() == "1")   //成功获取才添加到显示列表和Result中
                //    {
                //        string name = flac["fileName"].ToString();
                //        string ext = flac["extName"].ToString();
                //        string bt = flac["bitRate"].ToString();
                //        string size = (double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB";
                //        TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                //        string time = (ts.Minutes + ":" + ts.Seconds.ToString("00"));
                //        string url = flac["url"].ToString().Replace("\\", "");
                //        musicList.Items.Add(new { id = id, name = flac["fileName"], ext = ext, bt = bt, size = size, time = time, url = url });
                //        id++;
                //        //SkinListBoxItem sl = new SkinListBoxItem(kg.filename);
                //        //resultListView.Items.Add(sl);

                //        //ListViewItem lvi = new ListViewItem();
                //        //lvi.Text = kg.filename;
                //        //lvi.SubItems.Add(flac["bitRate"].ToString());
                //        //lvi.SubItems.Add(flac["extName"].ToString());
                //        //lvi.SubItems.Add((double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB");  //将文件大小装换成MB的单位
                //        //TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                //        //lvi.SubItems.Add(ts.Minutes + ":" + ts.Seconds.ToString("00"));
                //        //lvi.Tag = flac["url"].ToString().Replace("\\", "");
                //        //listViewItems.Add(lvi);
                //    }
                //}
                //if (kg.hash != "")
                //{
                //    kg.key = GetMD5(kg.hash + "kgcloud");
                //    webSite = "http://trackercdn.kugou.com/i/?cmd=4&hash=" + kg.hash + "&key=" + kg.key + "&pid=1&forceDown=0&vip=1";
                //    buffer = web.DownloadData(webSite);
                //    html = Encoding.UTF8.GetString(buffer);
                //    JObject flac = JObject.Parse(html);
                //    if (flac["status"].ToString() == "1")   //成功获取才添加到显示列表和Result中
                //    {
                //        string name = flac["fileName"].ToString();
                //        string ext = flac["extName"].ToString();
                //        string bt = flac["bitRate"].ToString();
                //        string size = (double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB";
                //        TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                //        string time = (ts.Minutes + ":" + ts.Seconds.ToString("00"));
                //        string url = flac["url"].ToString().Replace("\\", "");
                //        musicList.Items.Add(new { id = id, name = flac["fileName"], ext = ext, bt = bt, size = size, time = time, url = url });
                //        id++;
                //        //SkinListBoxItem sl = new SkinListBoxItem(kg.filename);
                //        //resultListView.Items.Add(sl);

                //        //ListViewItem lvi = new ListViewItem();
                //        //lvi.Text = kg.filename;
                //        //lvi.SubItems.Add(flac["bitRate"].ToString());
                //        //lvi.SubItems.Add(flac["extName"].ToString());
                //        //lvi.SubItems.Add((double.Parse(flac["fileSize"].ToString()) / (1024 * 1024)).ToString("F2") + "MB");  //将文件大小装换成MB的单位
                //        //TimeSpan ts = new TimeSpan(0, 0, int.Parse(flac["timeLength"].ToString())); //把秒数换算成分钟数
                //        //lvi.SubItems.Add(ts.Minutes + ":" + ts.Seconds.ToString("00"));
                //        //lvi.Tag = flac["url"].ToString().Replace("\\", "");
                //        //listViewItems.Add(lvi);
                //    }
                //}
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
            //pbDown.Maximum = response.ContentLength;
            //下载远程文件
            //ThreadPool.QueueUserWorkItem((obj) =>
            //{
                Stream netStream = response.GetResponseStream();
                Stream fileStream = new FileStream(save_url, FileMode.Create);
                byte[] read = new byte[1024];
                long progressBarValue = 0;
                int realReadLen = netStream.Read(read, 0, read.Length);
                while (realReadLen > 0)
                {
                    fileStream.Write(read, 0, realReadLen);
                    progressBarValue += realReadLen;
                    //pbDown.Dispatcher.BeginInvoke(new ProgressBarSetter(SetProgressBar), progressBarValue);
                    realReadLen = netStream.Read(read, 0, read.Length);
                }
                netStream.Close();
                fileStream.Close();
                //下载进度+1
                this.Dispatcher.Invoke(new Action(delegate
                {
                    //这里写代码
                    pbDown.Value += 1;
                }));

            //}, null);
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

        /**
         * 
         * 选择保存目录
         * 
         */
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性ee
            {
                savePath = openFileDialog.SelectedPath;
                DirectoryInfo path = new DirectoryInfo(savePath);
                if (!path.Exists)
                {
                    //不存在就创建目录
                    path.Create();
                }
                pathText.Text = savePath;
            }
        }

        /**
         * 
         * 上一页
         * 
         */
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(page <= 1)
            {
                page = 1;
            }
            else
            {
                page--;
            }
            searchF();
        }


        /**
         * 
         * 下一页
         * 
         */
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            page++;
            searchF();
        }

        /**
         * 
         * 开始下载
         * 
         *
         */
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (savePath == "")
            {
                MessageBox.Show("请先选择保存地址", "提示");
            }
            //获取被选择的集合

            addDown();
            pbDown.Maximum = selectLists.Count;
            pbDown.Value = 0;
            downListF();
        }

        private void addDown() {
            foreach(int id in selectLists)
            {
                foreach(object item in lists)
                {
                    int id2 = int.Parse(item.GetType().GetProperty("id").GetValue(item).ToString());
                    string url = item.GetType().GetProperty("url").GetValue(item).ToString();
                    string name = item.GetType().GetProperty("name").GetValue(item).ToString();
                    if (id == id2)
                    {
                        object obj = new { url = url, name = name };
                        downList.Add(item);
                    }
                }
            }
        }
        private void downListF()
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                foreach(object item in downList)
                {
                    string url = item.GetType().GetProperty("url").GetValue(item).ToString();
                    string name = item.GetType().GetProperty("name").GetValue(item).ToString();
                    string bt = item.GetType().GetProperty("bt").GetValue(item).ToString();
                    string ext = item.GetType().GetProperty("ext").GetValue(item).ToString();
                    //判断文件是否存在
                    FileInfo file = new FileInfo(savePath +"\\"+ name+"_"+bt+"."+ext);
                    if (!file.Exists)
                    {
                        DownloadHttpFile(url, savePath + "\\" + name + "_" + bt + "." + ext);
                    }
                    else
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            //这里写代码
                            pbDown.Value +=1;
                        }));
                    }
                }
                this.Dispatcher.Invoke(new Action(delegate
                {
                    //这里写代码
                    pbDown.Value = pbDown.Maximum;
                }));
                
                
            });
        }
    }

    class KugouResult
    {
        public string filename { get; set; }
        public string sqhash { get; set; }
        public string key { get; set; }

        public string hash { get; set; }

        public string hash320 {get; set;}
    }

    class downObj
    {
        public int id { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string time { get; set; }
        public string url { get; set; }
        public string ext { get; set; }
        public string bt { get; set; }

    }
}
