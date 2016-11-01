using System;
using System.Net;
using System.IO;
using System.Xml;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "https://[SERVERNAME]/SRSAPI/Generic/";

            string username = "username";
            string password = "password";

            int personId = 1128750026;//SRS Patient Id
            Guid encounterId = new Guid("656dd969-12f2-4419-9944-2ffb60da8c03");// SRS appointment/encounter Id
            int sourceId = 0; // 0 = SRS. You will need to use an assigned ID associated to your user account.
            int statusId = 0; //0 = Active, 1 = Inactive, 2 = Resolved

            string token = Authenticate(baseUrl + "Token", username, password);

            // Save a new diagnosis
            // An acitivity is automatically created
            string newDiagnosis = Create(baseUrl + "Diagnosis", token, 
                getDiagnosisJSON(
                    personId,
                    statusId,
                    encounterId,
                    DateTime.Now,
                    16,
                    "S06.0x1A",
                    "Concussion with loss of consciousness of 30 minutes or less, initial encounter",
                    sourceId
                )
            );

            XmlDocument xmlDiagnosis = new XmlDocument();
            xmlDiagnosis.LoadXml(newDiagnosis);

            //Get the Patient Problem Id
            int patientProblemId = Convert.ToInt32(
                xmlDiagnosis.SelectSingleNode(
                    // http://stackoverflow.com/a/4313696/402706
                    "/*[local-name() = 'PatientProblem']/*[local-name() = 'PatientProblemId']"
                ).InnerText
            );

            // Save a new activity.
            string newDiagnosisActivity = Create(baseUrl + "DiagnosisActivity", token,
                getDiagnosisActivityJSON(
                    patientProblemId,
                    personId,
                    encounterId,
                    DateTime.Now,
                    statusId,
                    sourceId
                )
            );

            XmlDocument xmlDiagnosisActivity = new XmlDocument();
            xmlDiagnosisActivity.LoadXml(newDiagnosisActivity);

            //Get the Patient Problem Activity Id
            int patientProblemActivityId = Convert.ToInt32(
                xmlDiagnosisActivity.SelectSingleNode(
                    // http://stackoverflow.com/a/4313696/402706
                    "/*[local-name() = 'PatientProblemActivity']/*[local-name() = 'ActivityId']"
                ).InnerText
            );

            // Delete the Activity
            Delete(baseUrl + "DiagnosisActivity", token, patientProblemActivityId.ToString());

            // Delete the Problem
            Delete(baseUrl + "Diagnosis", token, patientProblemId.ToString());
            
        }

        static string getDiagnosisActivityJSON(int patientProblemId, int patientId, Guid encounterId, DateTime date, int status, int sourceId)
        {
            return string.Format(@"{{
                ""patientProblemId"":{0},
                ""patientId"":{5},
                ""recDate"":""{2}"",
                ""extDate"":""{2}"",
                ""status"":{3},
                ""sourceId"":{4},
                ""encounterId"":""{1}""
            }}", patientProblemId, encounterId, date, status, sourceId, patientId);
        }

        static string getDiagnosisJSON(int personId, int statusId, Guid encounterId, DateTime date, int codeSystem, string code, string codeDesc, int sourceId)
        {
            return string.Format(@"{{
               ""patientId"":{0},
               ""sourceId"":{7},
               ""recDate"":""{3}"",
               ""code"":{{  
                  ""isActive"":true,
                  ""code"":""{5}"",
                  ""description"":""{6}"",
                  ""codeSystem"":{{  
                     ""codeSystemId"":{4}
                  }}
               }},
               ""activities"":[  
                  {{    
                     ""recDate"":""{3}"",
                     ""extDate"":""{3}"",
                     ""status"":{1},
                     ""sourceId"":{7},
                     ""encounterId"":""{2}""
                  }}
               ]
            }}", personId, statusId, encounterId, date, codeSystem, code, codeDesc, sourceId);
        }

        static string Create(string url, string authToken, string data)
        {
            return Request(url, authToken, data, null, false);
        }

        static string Read(string url, string authToken, string id)
        {
            return Request(url, authToken, null, id, false);
        }

        static string Update(string url, string authToken, string id, string data)
        {
            return Request(url, authToken, data, id, false);
        }

        static void Delete(string url, string authToken, string id)
        {
            Request(url, authToken, null, id, true);
        }

        static string Search(string url, string authToken)
        {
            return Request(url, authToken, null, null, false);
        }

        static string Authenticate(string url, string username, string password)
        {
            return Request(url, null, String.Format("{{\"username\":\"{0}\",\"password\":\"{1}\",\"dataSourceId\":0}}", username, password), null, false);
        }

        static string Request(string url, string authToken, string data, string id, bool shouldDelete)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.IsNullOrEmpty(id) ? new Uri(url): new Uri(new Uri(url + "/"), id));

            if (string.IsNullOrEmpty(data))
            {
                if (!string.IsNullOrEmpty(id) && shouldDelete)
                {
                    request.Method = "DELETE";
                }
                else
                {
                    request.Method = "GET";
                }
            }
            else
            {
                request.Method = string.IsNullOrEmpty(id) ? "POST" : "PUT";
            }

            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Add("Authorization", authToken);
            }

            request.ContentType = "application/json";
            request.Accept = "application/xml";

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
