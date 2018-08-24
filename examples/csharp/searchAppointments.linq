<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Net.Http.Formatting.dll</Reference>
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
string authToken = Convert.ToBase64String(encoding.GetBytes(credential));

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

// Examples: 
// Date Range:	Appointment?personDeskDate=>=1/1/2016&personDeskDate=<=12/31/2016
// Equality:	Appointment?patientId=9820
// Or list:		Appointment?patientID=9820,9783
// paging: 		Appointment?patientId=9820&_page=2
HttpResponseMessage response = client.GetAsync("Appointment?patientId=9820&_page=2").Result; // Blocking call!

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
