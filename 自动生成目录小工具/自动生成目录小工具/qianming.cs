using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace 自动生成目录小工具
{
   
    public class qianming
    {
        string AccessKeyId = "LTAItASBmW3naEx2"; //密钥ID
        string AccessKeySecret = "9nnzgrrSm3udmfjbJgq6liyZlBQ2XA";
        string Format = "JSON"; //返回值的类型
        string Version = "2015-01-09"; //API版本号
        string Signature = ""; //签名结果串
        string SignatureMethod = "HMAC-SHA1"; //签名方式，目前支持HMAC-SHA1
        string Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"); //请求的时间戳 日期格式按照ISO8601标准表示，并需要使用UTC时间 YYYY-MM-DDThh:mm:ssZ;
        string SignatureVersion = "1.0"; //签名算法版本
        string SignatureNonce = Guid.NewGuid().ToString(); //唯一随机数，用于防止网络重放攻击。用户在不同请求间要使用不同的随机数值

        string InterFaceUrl = "http://alidns.aliyuncs.com/"; //请求路径






        /// <summary>
        /// 计算签名
        /// </summary>
        public void ComputeSignature(Dictionary<string, string> ditParam, string method = "GET")
        {
            BuildParameters(ditParam);

            //按ascii码排序   
            Dictionary<string, string> asciiDit = new Dictionary<string, string>();
            string[] KeyArr = ditParam.Keys.ToArray();
            Array.Sort(KeyArr, string.CompareOrdinal);
            foreach (var key in KeyArr)
            {
                string value = ditParam[key];
                asciiDit.Add(key, value);
            }


            //计算签名    
            var canonicalizedQueryString = string.Join("&",
        asciiDit.Select(x => PercentEncode(x.Key) + "=" + PercentEncode(x.Value)));

            var stringToSign = method.ToString().ToUpper() + "&" + PercentEncode("/") + "&" + PercentEncode(canonicalizedQueryString);
            var keyBytes = Encoding.UTF8.GetBytes(AccessKeySecret + "&");
            var hmac = new HMACSHA1(keyBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            Signature = Convert.ToBase64String(hashBytes);


            ditParam.Add("Signature", Signature);
        }





        //获取公共参数和相关方法中的参数
        private void BuildParameters(Dictionary<string, string> ditParam)
        {
            ditParam.Add("Format", Format.ToUpper());
            ditParam.Add("Version", Version);
            ditParam.Add("AccessKeyId", AccessKeyId);
            ditParam.Add("Timestamp", Timestamp);
            ditParam.Add("SignatureMethod", SignatureMethod);
            ditParam.Add("SignatureVersion", SignatureVersion);
            ditParam.Add("SignatureNonce", SignatureNonce);

        }




        private static string PercentEncode(string value)
        {
            return UpperCaseUrlEncode(value)
              .Replace("/", "%2F")
              .Replace("+", "%20")
              .Replace("*", "%2A")
              .Replace("%7E", "~")
              .Replace("!", "%21")
              .Replace("'", "%27")
              .Replace("(", "%28")
              .Replace(")", "%29");
        }

        private static string UpperCaseUrlEncode(string s)
        {

            // C# 的 HttpUtility.UrlEncode() 编码得到的形如 %xx%xx的内容是小写的，Java 的是大写的。

            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();

            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }





        public  string HttpGetUrl(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();


            return retString;
        }
    }


}
