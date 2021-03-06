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

// Parameters.
int personId = 9783; //SRS Patient Id
int status = 0; //0 = Active, 1 = Inactive, 2 = Resolved
Guid encounterId = new Guid("FBFF4DAC-E07C-4A53-B337-30774B60A38D"); // SRS appointment/encounter Id
DateTime date = DateTime.Now; // The entry date of the data.
int codeSystem = 16; //The type of code: 16 = ICD-10, 3 = ICD-9, 13 = SNOMED
string code = "S06.0x1A"; // The code
string codeDescription = "Concussion with loss of consciousness of 30 minutes or less, initial encounter"; // The code description
int sourceId = 0; // 0 = SRS. You will need to use an assigned ID associated to your user account.

HttpResponseMessage diagnosisResult = client.PostAsync("Diagnosis", new StringContent($@"{{
   ""patientId"":{personId},
   ""sourceId"":{sourceId},
   ""recDate"":""{date}"",
   ""code"":{{  
      ""isActive"":true,
      ""code"":""{code}"",
      ""description"":""{codeDescription}"",
      ""codeSystem"":{{  
         ""codeSystemId"":{codeSystem}
      }}
   }},
   ""activities"":[  
      {{    
         ""recDate"":""{date}"",
         ""extDate"":""{date}"",
         ""status"":{status},
         ""sourceId"":{sourceId},
         ""encounterId"":""{encounterId}""
      }}
   ]
}}", Encoding.UTF8, "application/json")).Result; // Blocking call!

string result;
if(diagnosisResult.IsSuccessStatusCode)
{
	// Get the response body as a string. Blocking!
	result = diagnosisResult.Content.ReadAsStringAsync().Result; // ReadAsAsync can also parse directly to an object
	// Deserialize or parse (Newtonsoft.JSON, etc.)
	Console.WriteLine(result); 
}
else
{
	Console.WriteLine("{0} ({1})", (int)auth.StatusCode, auth.ReasonPhrase); 
}