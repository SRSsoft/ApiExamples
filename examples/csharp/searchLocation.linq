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

// The base URL
const string URL = "http://[yourSeverName]/SRSAPI/Generic/"; 

HttpClient client = new HttpClient(); 
client.BaseAddress = new Uri(URL); 

// Add an Accept header for JSON format
client.DefaultRequestHeaders.Accept.Add(
	new MediaTypeWithQualityHeaderValue("application/json")
); 

Console.WriteLine("Token Retrieval");

HttpResponseMessage auth = client.PostAsync("Token", new StringContent("{\"userName\":\"yourUserName\",\"password\":\"yourPassword\",\"dataSourceId\":0}", Encoding.UTF8, "application/json")).Result; // Blocking call!
string jwt = null;
if(auth.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	jwt = auth.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(jwt); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)auth.StatusCode, auth.ReasonPhrase); 
}

Console.WriteLine("Authenticated Request");

// Add authorization token
client.DefaultRequestHeaders.Add(
	"Authorization",
	jwt
); 

// Examples: 
// Equality:	Location?locationId=1
// Or list:		Location?locationId=1,2
// paging: 		Location?isPrimaryRxAddress=false&_page=2
HttpResponseMessage response = client.GetAsync("Location?name=Phoenix Main Practice").Result; // Blocking call!

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

// Example2: 
// Filter based on two parameters (LOGICAL AND - using &)
HttpResponseMessage response = client.GetAsync("Location?isactive=true&isPrimaryRxAddress=false&_page=1").Result; // Blocking call!

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

/ Example3: 
// Filter based on comparision operator. Using "exact" to perform strict equality 
HttpResponseMessage response = client.GetAsync("Location?friendlyname:exact=Dan's Ortho at Warren").Result; // Blocking call!

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
