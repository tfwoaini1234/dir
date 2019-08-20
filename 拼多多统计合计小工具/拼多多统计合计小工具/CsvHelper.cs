using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace 拼多多统计合计小工具
{
    /// <summary>
    /// //读CSV文件类,读取指定的CSV文件，可以导出DataTable..........add by chujianqin
    /// </summary>
    public class CsvHelper
    {
        /// <summary>
        /// 字符串是否包含奇数个引号
        /// </summary>
        /// <param name="str">相关字符串</param>
        /// <returns></returns>
        private static bool _isOddDoubleQuota(string str)
        {
            return _getDoubleQuotaCount(str) % 2 == 1;
        }

        private static int _getDoubleQuotaCount(string str)
        {
            string[] strArray = str.Split('"');
            int doubleQuotaCount = strArray.Length - 1;
            doubleQuotaCount = doubleQuotaCount < 0 ? 0 : doubleQuotaCount;
            return doubleQuotaCount;
        }

        /**
         * csv的每一行的每一项的引号个数必定为偶数
    　　　* 生成的Dictionary<string,List<string>>以每一列的第一行元素作为key,其它元素的集合作为value
         */
        public static Dictionary<string, List<string>> AnalysisCsvByStr(string csvInfo)
        {
            //首行的每列数据项作为字典的Key
            Dictionary<string, List<string>> csvInfoDic = new Dictionary<string, List<string>>();
            Regex regex = new Regex(@"\r\n");
            string[] infoLines = regex.Split(csvInfo);
            List<string>[] itemListArray = new List<string>[0];
            for (int i = 0, length = infoLines.Length; i < length; i++)
            {
                if (string.IsNullOrEmpty(infoLines[i]))
                {
                    continue;
                }
                string[] lineInfoArray = infoLines[i].Split(',');
                List<string> rowItemList = new List<string>();
                string strTemp = string.Empty;
                for (int j = 0; j < lineInfoArray.Length; j++)
                {
                    strTemp += lineInfoArray[j];
                    if (_isOddDoubleQuota(strTemp))
                    {
                        if (j != lineInfoArray.Length - 1)
                        {
                            strTemp += ",";
                        }
                    }
                    else
                    {
                        if (strTemp.StartsWith("\"") && strTemp.EndsWith("\""))
                        {
                            strTemp = strTemp.Substring(1, strTemp.Length - 2);
                        }
                        rowItemList.Add(strTemp);
                        strTemp = string.Empty;
                    }
                }
                if (i == 0)
                {
                    itemListArray = new List<string>[rowItemList.Count];
                    for (int temp = 0; temp < itemListArray.Length; temp++)
                    {
                        itemListArray[temp] = new List<string>();
                    }
                }
                int indexTemp = 0;
                for (; indexTemp < rowItemList.Count; indexTemp++)
                {
                    if (indexTemp == itemListArray.Length)
                    {
                        throw new ArgumentException("csv文件有误");
                    }
                    itemListArray[indexTemp].Add(rowItemList[indexTemp]);
                }
                if (indexTemp < itemListArray.Length - 1)
                {
                    throw new ArgumentException("csv文件有误");
                }
            }
            for (int i = 0; i < itemListArray.Length; i++)
            {
                string key = itemListArray[i][0];
                //去除第一个元素，其它元素集合作为value
                itemListArray[i].RemoveAt(0);
                csvInfoDic.Add(key, itemListArray[i]);
            }
            return csvInfoDic;
        }

        public static Dictionary<string, List<string>> AnalysisCsvByFile(string csvPath)
        {
            if (File.Exists(csvPath))
            {
                string csvInfo = File.ReadAllText(csvPath, Encoding.UTF8);
                return AnalysisCsvByStr(csvInfo);
            }
            else
            {
                throw new FileNotFoundException("未找到文件：" + csvPath);
            }
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            Encoding encoding = Encoding.ASCII; //Encoding.ASCII;//
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);

                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }


        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(DataTable dt, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
    }

}
