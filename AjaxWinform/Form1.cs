using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace AjaxWinform
{
    public partial class Form1 : Form
    {
        string token = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetToken();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Query();
        }

        private async void Query()
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri("https://localhost");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);

                //访问 https 内容的时候，看到证书错误（不在操作系统的证书信任链中）的提示，在浏览器中我们可以忽略错误的证书，继续访问网页内容。但是在.NET程序中，需要由代码来判断是否忽略错误的证书。
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //任意访问任意 https 内容的程序代码之前，设置一个证书处理程序
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => { return true; };

                //make request
                HttpResponseMessage response = await client.GetAsync("Labels");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                //do something
                Console.WriteLine(responseString);
            }
        }

        private async void GetToken()
        {
            JObject jobj = JObject.FromObject(new
            {
                username = "admin",
                password = "admin"
            });
            var json = new StringContent(JsonConvert.SerializeObject(jobj), Encoding.UTF8, "application/json");
            //var json = new StringContent("{\"username\":\"admin\",\"password\":\"admin\"}", Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri("https://localhost");

                //访问 https 内容的时候，看到证书错误（不在操作系统的证书信任链中）的提示，在浏览器中我们可以忽略错误的证书，继续访问网页内容。但是在.NET程序中，需要由代码来判断是否忽略错误的证书。
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //任意访问任意 https 内容的程序代码之前，设置一个证书处理程序
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => { return true; };

                //make request
                HttpResponseMessage response = await client.PostAsync("/Token", json);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                //do something
                jobj = JsonConvert.DeserializeObject<JObject>(responseBody);
                token = jobj["Result"]["Token"].ToString();
                label1.Text = token;
            }
        }

        private async Task<string> GetTokenAsync()
        {
            JObject jobj = JObject.FromObject(new
            {
                username = "admin",
                password = "admin"
            });
            var json = new StringContent(JsonConvert.SerializeObject(jobj), Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync("http://localhost:61649/Token", json);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
