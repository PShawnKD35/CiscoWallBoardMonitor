using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinitorWallBoard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //public Boolean needWarning = true;

        /// <summary>
        /// 测试Get方法
        /// </summary>
        public void test()
        {
            String calls = HttpGet("http://itseelm-nt3047/wallboard/tel/server.php", "noCache=1500544586550&countries=280,279,274,273,329,339");
            MessageBox.Show(calls);
        }

        /// <summary>
        /// 后台发送POST请求 http://www.ityouzi.com/archives/http-post-get.html
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns>服务器返回的字符串</returns>
        public string HttpPost(string url, string data)
        {
            try
            {
                //创建post请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                byte[] payload = Encoding.UTF8.GetBytes(data);
                request.ContentLength = payload.Length;

                //发送post的请求
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();

                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string value = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();

                return value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
          /// 后台发送GET请求
          /// </summary>
          /// <param name="url">服务器地址</param>
          /// <param name="data">发送的数据</param>
          /// <returns></returns>
        public string HttpGet(string url, string data)
        {
            try
            {
                //创建Get请求
                url = url + (data == "" ? "" : "?") + data;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = streamReader.ReadToEnd();

                streamReader.Close();
                stream.Close();
                response.Close();

                return retString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://itseelm-nt3047/wallboard/tel/index.php?z=0.8&p=Shanghai&q=266");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HtmlElement table = webBrowser1.Document.GetElementById("dragcont"); //数据表格的ID
            HtmlElementCollection trs = table.GetElementsByTagName("tr");
            foreach (HtmlElement tr in trs)
            {
                try
                {
                    HtmlElementCollection tds = tr.Children;
                    string ready = tds[2].OuterText;
                    string queue = tds[1].OuterText;
                    int queueInt = int.Parse(queue); //会遍历到table的head和foot，所以会无法解析
                    if (queueInt > 0 && ready == "0")
                    {
                        string queueName = tds[0].OuterText;
                        MessageBox.Show("No agent ready at: " + queueName, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }

                }
                catch (Exception error)
                {
                    continue;
                }
                //string ready = tr.FirstChild.NextSibling.NextSibling.OuterText;

            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            timer1.Start();
        }
    }
}
