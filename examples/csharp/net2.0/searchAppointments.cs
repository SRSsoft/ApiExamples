using System;
using System.Net;
using System.IO;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://[SERVERNAME]/SRSAPI/Generic/";

            string username = "userName";
            string password = "password";

            string token = Request(baseUrl + "Token", null, String.Format("{{\"username\":\"{0}\",\"password\":\"{1}\",\"dataSourceId\":0}}", username, password));

            // Deserialize into an object
            string appointments = Request(baseUrl + "Appointment?PersonId=9783", token);
        }


        static string Request(string url, string authToken)
        {
            return Request(url, authToken, null);
        }

        static string Request(string url, string authToken, string data)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));

            if (string.IsNullOrEmpty(data))
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";
            }

            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Add("Authorization", authToken);
            }

            request.ContentType = "application/json";
            request.Accept = "application/json";

            if (!string.IsNullOrEmpty(data))
            {
                using(StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(data);
                }
            }

            WebResponse response = request.GetResponse();

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string responseData = sr.ReadToEnd();
                return responseData;
            }
        }

    }
}
