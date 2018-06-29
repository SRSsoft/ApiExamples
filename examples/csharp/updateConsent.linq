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

Console.WriteLine("\r\nSTATUS UPDATE\r\n"); 

string state = "1"; // SPINE CTRj. From the Location API.
string consentType = "IMMUNIZATION"; // From the Rooms API filtered by location
string consentDate = "2009-06-15T13:45:30.0000000-04:00";
string consentExpiredDate = "2009-06-15T13:45:30.0000000-04:00"; 
string isConsent = "true"; // from the PatientTrackingStatus API
string personId = "1195034206"; //Carter, Jimmy

HttpResponseMessage result = client.Post($"PatientConsent", new StringContent($@"{{
   ""state"":{state},
   ""consentType"":{consentType},
   ""consentDate"":""{consentDate}"",
   ""consentExpiredDate"":""{consentExpiredDate}"",
   ""isConsent"":""{isConsent}"",
   ""personId"":""{personId}""
}}", Encoding.UTF8, "application/json")).Result; // Blocking call!


if(result.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	string dataObjects = result.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)result.StatusCode, result.ReasonPhrase); 
	// Get the response body as a string. Blocking!
	string dataObjects = result.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(dataObjects); 
}