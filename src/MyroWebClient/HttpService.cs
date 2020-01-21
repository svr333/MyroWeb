using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MyroWebClient.Extensions;

namespace MyroWebClient
{
    public class HttpService
    {
        public string GetRawData(string username, string password, string schoolAbreviation, CookieContainer container = null)
        => HttpRequest(username, password, schoolAbreviation);

        public string HttpRequest(string username, string password, string schoolAbreviation)
        {
            CookieContainer cookies = new CookieContainer();

            #region to collect PHPSESSID

            var h = WebRequest.Create(Constants.PhpSessionUri) as HttpWebRequest;
            h.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)h.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                var headerValue = response.Headers.GetValues("Set-Cookie");

                cookies = cookies.AddCookiesFromHeader(headerValue, Constants.BaseUri.Host);
            }

            #endregion

            #region loginDo.php
            HttpWebRequest http = WebRequest.Create(Constants.LoginDoUri) as HttpWebRequest;

            http.Referer = "https://online.myro.be/login.php";

            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            string postData = "LoginStamp=29797c0e+a1192b54+bf2872e8+18a14562+cfdc8d0d+aabf4e4d+58a7d6e1+2b571e95+e225ee5f&Root=" + schoolAbreviation + "&Username=" + username + "&Password=" + password + "&Login=Log+in";
            byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
            http.ContentLength = dataBytes.Length;
            using (Stream postStream = http.GetRequestStream())
            {
               postStream.Write(dataBytes, 0, dataBytes.Length);
            }

            string result = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)http.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader responseReader = new StreamReader(responseStream))
            {              
                result = responseReader.ReadToEnd();
                var headerValue = response.Headers.GetValues("Set-Cookie");

                cookies = cookies.AddCookiesFromHeader(headerValue, Constants.LoginDoUri.Host);
            }

            #endregion

            #region index.php

            var http1 = WebRequest.Create(Constants.IndexUri) as HttpWebRequest;

            http1.KeepAlive = true;
            http1.Method = "GET";

            http1.CookieContainer = cookies;

            string html = string.Empty;


            using (HttpWebResponse response = (HttpWebResponse)http1.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {      
                var cookiesToAdd = response.Headers.GetValues("Set-Cookie");

                cookies = cookies.AddCookiesFromHeader(cookiesToAdd, Constants.IndexUri.Host);
                html = reader.ReadToEnd();
            }

            #endregion

            #region logbook.php

            var http2 = WebRequest.Create("https://online.myro.be/logbook.php") as HttpWebRequest;
            http2.KeepAlive = true;
            http2.Method = "GET";

            http2.CookieContainer = cookies;

            using (HttpWebResponse response1 = (HttpWebResponse)http2.GetResponse())
            using (Stream stream1 = response1.GetResponseStream())
            using (StreamReader reader1 = new StreamReader(stream1))
            {
                html = reader1.ReadToEnd();
            }

            var http3 = WebRequest.Create("https://online.myro.be/logbook.php") as HttpWebRequest;
            http3.Referer = "https://online.myro.be/logbook.php";
            http3.KeepAlive = true;
            http3.ContentType = "application/x-www-form-urlencoded";
            http3.Method = "POST";

            var test = GetUserIdFromLogBookPage(html);

            cookies.Add(new Cookie("MyroWebRapport[ShowedStudent]", test, "", Constants.PhpSessionUri.Host));

            http3.CookieContainer = cookies;

            string postData1 = $"currentStudent={test}&period{test}=-4096&count=10";
            byte[] dataBytes1 = UTF8Encoding.UTF8.GetBytes(postData1);
            http3.ContentLength = dataBytes1.Length;
            using (Stream postStream = http3.GetRequestStream())
            {
               postStream.Write(dataBytes1, 0, dataBytes1.Length);
            }

            using (HttpWebResponse response2 = (HttpWebResponse)http3.GetResponse())
            using (Stream stream2 = response2.GetResponseStream())
            using (StreamReader reader2 = new StreamReader(stream2))
            {
                html = reader2.ReadToEnd();
                return html;
            }

            #endregion
            
        }

        private string GetUserIdFromLogBookPage(string rawHtml)
        {
            var splitText = rawHtml.Split('\'');

            // user id's are two numbers, underscore, 4 numbers, eg: 17_3443
            var regex = new Regex("^\\d{2}?_\\d{4}");

            foreach (var s in splitText)
            {
                if (regex.IsMatch(s)) return s;
            }

            // TODO: THROW FAILED LOGIN HERE
            return string.Empty;
        }
    }
}
