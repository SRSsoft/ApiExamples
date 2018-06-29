<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <GACReference>System.Net.Http.Formatting, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35</GACReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Formatting</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

// NameSpaces:
// System.Net.Http;
// System.Net.Http.Formatting;
// System.Net.Http.Headers;

// The token URL for getting Access to SRS.
// Replace "[yourServerName]" with the name of your application server.
string tokenEndpoint = "http://[yourSeverName]/SRSIdentityServer/connect/token";

// Encode the authentication token
Encoding encoding = Encoding.UTF8;

// Replace [ClientIdentifier] and [ClientSecret] with the values issued to you by srs.
string credential = string.Format("{0}:{1}", "[ClientIdentifier]", "[ClientSecret]");
string authToken = authToken = Convert.ToBase64String(encoding.GetBytes(credential));

HttpClient tokenClient = new HttpClient(); 

// Add authorization token
tokenClient.DefaultRequestHeaders.Add(
	"Authorization",
	"Basic " + authToken
); 

// Post data for getting the token
// Replace [UserSecret] with the value issued to you by SRS.
string postData = "grant_type=SrsCustomClientCredentials&scope=srsclient&usersecret=[UserSecret]";

HttpResponseMessage auth = tokenClient.PostAsync(tokenEndpoint, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")).Result; // Blocking call!

string jwt = null;
if(auth.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	jwt = auth.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// extract the access_token. This will be attached to every subsequent request going forward.
	MatchCollection matches = Regex.Matches(jwt, "\"access_token\": \"(.+)\"");
	jwt = matches[0].Groups[1].Value;
	
	Console.WriteLine(jwt); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)auth.StatusCode, auth.ReasonPhrase); 
}

Console.WriteLine("Authenticated Request");


HttpClient client = new HttpClient();

// Add an Accept header for JSON format
client.DefaultRequestHeaders.Accept.Add(
	new MediaTypeWithQualityHeaderValue("application/json")
); 

// Replace "[yourServerName]" with the proper server name.
client.BaseAddress = new Uri("http://[yourSeverName]/SRSAPI/Generic/"); 
// Add authorization token
client.DefaultRequestHeaders.Add(
	"Authorization",
	"Bearer " + jwt
); 

Console.WriteLine("Token Retrieval");


Console.WriteLine("\r\nLOCATIONS\r\n"); 

response = client.GetAsync("Location?isActive=true").Result; // Blocking call!

if(response.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	string dataObjects = response.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase); 
}

Console.WriteLine("\r\nROOMS\r\n"); 

response = client.GetAsync("Room?locationId=6").Result; // Blocking call!

if(response.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	string dataObjects = response.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase); 
}

Console.WriteLine("\r\nSTATUS UPDATE\r\n"); 

string locationId = "6"; // SPINE CTRj. From the Location API.
string roomId = "6"; // From the Rooms API filtered by location
string checkInTime = "2018-04-10 9:01"; // value must be present, but does not matter (current time will be used instead)
string patientTrackingStatusId = "1"; // from the PatientTrackingStatus API
string encounterId = "AF157580-D7FF-4F55-AE61-B2B5FFF8D447"; //Carter, Jimmy

HttpResponseMessage diagnosisResult = client.PutAsync($"PatientTracking/{encounterId}", new StringContent($@"{{
   ""locationId"":{locationId},
   ""currentRoomId"":{roomId},
   ""checkInTime"":""{checkInTime}"",
   ""currentPatientStatusTypeId"":{patientTrackingStatusId}
}}", Encoding.UTF8, "application/json")).Result; // Blocking call!


if(diagnosisResult.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	string dataObjects = diagnosisResult.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)diagnosisResult.StatusCode, diagnosisResult.ReasonPhrase); 
	// Get the response body as a string. Blocking!
	string dataObjects = diagnosisResult.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}