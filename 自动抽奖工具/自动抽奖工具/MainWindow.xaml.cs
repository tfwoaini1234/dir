using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MySql.Data.MySqlClient;
namespace 自动抽奖工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lunshu = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lunshu = 1;
            int endCount = GetCount();
            Dictionary<string, Dictionary<string, string> > allList = GetList();
            bool kg = true;
            while (kg)
            {
                foreach (string mykey in allList.Keys)
                {
                    Dictionary<string, string> item = allList[mykey];
                    string number = item["number"];
                    List<string> numberList = zhongjiangList();
                    int count = Convert.ToInt32(item["count"]);
                    List<int> list = choujiang(count, endCount);
                    Random ran = new Random();
                    int key = ran.Next(1, endCount);
                    int res = list.IndexOf(key);
                    if (res >= 0)
                    {
                        //中奖了
                        double gailv = Convert.ToDouble(count) / endCount;
                        string gl = Convert.ToDouble(gailv).ToString("P");
                        log.Text += "恭喜【" + number + "】在经过【" + lunshu + "】轮后抽奖奖品恭喜。该中奖者的中奖概率为【" + gl + "】\r\n";
                        kg = false;
                        break;
                    }
                }
                this.Dispatcher.Invoke(new Action(delegate
                {
                    //这里写代码
                    log.ScrollToEnd();
                }));
                lunshu++;
            }
            
        }
        private List<string> zhongjiangList() {
            List<string> list = new List<string>();
            return list;
        }
        private List<int> choujiang(int uc,int ec) {
            int start = 1;
            double end = Convert.ToDouble(ec);
            double userCount = Convert.ToDouble(uc);
            List<int> list = new List<int>();
            while (start <= end)
            {
                int count = 0;
                double n = userCount / end;
                if(Convert.ToInt32((start - 1)*n) != Convert.ToInt32(start *n))
                {
                    list.Add(start);
                    //log.Text += start.ToString() + ",";
                    count++;
                }
                start++;
            }
            return list;
        }

        private Dictionary<string, Dictionary<string, string>> GetList() {
            string connstr = "data source=localhost;database=center;user id=root;password=root;pooling=false;charset=utf8";//pooling代表是否使用连接池
            MySqlConnection conn = new MySqlConnection(connstr);
           
            string sql = "select number,count(number) numbers from qq_msg where number >0 AND number != 1000000 GROUP BY number ORDER BY numbers DESC";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            Dictionary<string, Dictionary<string, string>> allList = new Dictionary<string, Dictionary<string, string>>();
            while (reader.Read())
            {
                Dictionary<string, string> list = new Dictionary<string, string>();
                list.Add("number", reader.GetString("number"));
                list.Add("count", reader.GetString("numbers"));
                allList.Add(reader.GetString("number"), list);
            }
            return allList;
        }

        private int GetCount() {
            string connstr = "data source=localhost;database=center;user id=root;password=root;pooling=false;charset=utf8";//pooling代表是否使用连接池
            MySqlConnection conn = new MySqlConnection(connstr);
            string sql = "select count(id) counts from qq_msg where number >0 AND number != 1000000 ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            int counts = Convert.ToInt32( cmd.ExecuteScalar().ToString());
            return counts;       
        }
    }
}
