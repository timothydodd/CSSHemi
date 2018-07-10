using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HTMLScrape
{
    public class HTMLClient
    {
        public static string PostDATA<T>(CookieContainer myCookie, string URL, T postData,
            Action<HttpWebRequest> config = null, JsonSerializerSettings jsonSettings = null)
        {

            var str = JsonConvert.SerializeObject(postData,
                Formatting.None,jsonSettings
               );
            return PostDATA(myCookie, URL, str, config);
        }

        public static string PostDATA(CookieContainer myCookie, string URL, string postData,
            Action<HttpWebRequest> config = null)
        {
            string source = "";
            HttpWebRequest myReq = (HttpWebRequest) WebRequest.Create(URL);
            // myReq.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            myReq.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            myReq.Method = "POST";
            //  BugFix_CookieDomain(myCookie);
            myReq.CookieContainer = myCookie;

            //   myReq.Headers.Add("Accept-Encoding: gzip");
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = postData.Length;
            myReq.KeepAlive = true;
            //myReq.Proxy = null;
            if (config != null)
            {
                config(myReq);
            }
            if (postData.Length > 0)
            {
                Stream OutStream = myReq.GetRequestStream();

                try
                {
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] baPostData = encoder.GetBytes(postData);
                    OutStream.Write(baPostData, 0, baPostData.Length);
                }
                finally
                {
                    OutStream.Close();
                }

            }
            using (WebResponse response = myReq.GetResponse())
            {
                try
                {
                    // Get the stream associated with the response.
                    using (Stream receiveStream =
                        response.GetResponseStream())
                    {

                        if (receiveStream != null)
                            using (StreamReader readStream = new
                                StreamReader(receiveStream, Encoding.UTF8))
                            {
                                source = readStream.ReadToEnd();
                            }
                    }
                }
                finally
                {
                    response.Close();
                }
            }

            return source;


        }

        public static string GetPage(CookieContainer myCookie, string url, Action<HttpWebRequest> config = null)
        {
           

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Method = "GET";
                //request.Proxy = null;
                request.AllowAutoRedirect = true;
              request.CookieContainer = myCookie;
               request.KeepAlive = true;
            request.Headers.Add("Accept-Language: en-US,en;q=0.8");
                request.Headers.Add("Accept-Encoding: gzip");
                if (config != null)
                {
                    config(request);
                }

                HttpWebResponse res;


                res = (HttpWebResponse) request.GetResponse();

                string encode = "";
                foreach (string bah in res.Headers)
                {

                    if (bah == "Content-Encoding")
                        encode = res.GetResponseHeader(bah.ToString());
                }
                string responseStr = string.Empty;
                if (res == null)
                    return responseStr;
                var responseType = res.ContentType;
                //if(!(responseType.Contains("html") || responseType.Contains("text")))
                //     return "";
                if (encode == "gzip")
                {
                    try
                    {
                        GZipStream x = new GZipStream(res.GetResponseStream(), CompressionMode.Decompress);
                        StreamReader resStream = new StreamReader(x);
                        responseStr = resStream.ReadToEnd();
                        resStream.Close();
                    }
                    catch
                    {

                    }
                }
                else
                {

                    try
                    {
                       
                        StreamReader resStream = new StreamReader(res.GetResponseStream());

                        responseStr = resStream.ReadToEnd();
                        resStream.Close();
                    }
                    catch
                    {

                    }
                }
                res.Close();
            res.Dispose();
         
                return responseStr;
        

        }
    }
}
