using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MvcApplicationPBI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplicationPBI.Controllers
{
    public class HomeController : Controller
    {

        public AuthenticationResult authResult { get; set; }
        string baseUri = "https://api.powerbi.com/beta/myorg/";

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult SignIn()
        {
            var @params = new NameValueCollection
            {
                //Azure AD will return an authorization code. 
                //See the Redirect class to see how "code" is used to AcquireTokenByAuthorizationCode
                {"response_type", "code"},

                //Client ID is used by the application to identify themselves to the users that they are requesting permissions from. 
                //You get the client id when you register your Azure app.
                {"client_id", "721219de-20fe-4761-87b1-09d852bbfba1"},

                //Resource uri to the Power BI resource to be authorized
                {"resource", "https://analysis.windows.net/powerbi/api"},

                //After user authenticates, Azure AD will redirect back to the web app
                {"redirect_uri", "http://localhost:58712/Home/Redirect"}
            };

            //Create sign-in query string
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(@params);

            //Redirect authority
            //Authority Uri is an Azure resource that takes a client id to get an Access token
            string authorityUri = "https://login.windows.net/common/oauth2/authorize/";

            return Redirect(String.Format("{0}?{1}", authorityUri, queryString)); 
 
        }
        

        public ActionResult Redirect()
        {
            string redirectUri = "http://localhost:58712/Home/Redirect";
            string authorityUri = "https://login.windows.net/common/oauth2/authorize/";

            // Get the auth code
            string code = Request.Params.GetValues(0)[0];

            // Get auth token from auth code       
            TokenCache TC = new TokenCache();

            AuthenticationContext AC = new AuthenticationContext(authorityUri, TC);

            ClientCredential cc = new ClientCredential
                ("721219de-20fe-4761-87b1-09d852bbfba1",
                "VBkR+bgonrKS2OQh3omxk0M2LVKi097FvSSAaGEPLBs=");

            AuthenticationResult AR = AC.AcquireTokenByAuthorizationCode(code, new Uri(redirectUri), cc);

            //Set Session "authResult" index string to the AuthenticationResult
            Session["authResult"] = AR;

            //Redirect back to Default.aspx
          return RedirectToAction("PBI");

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PBI()
        {

            if (Session["authResult"] != null)
            {
                //Get the authentication result from the session
                authResult = (AuthenticationResult)Session["authResult"];

                

                //Set user and toek from authentication result
                ViewBag.user= authResult.UserInfo.DisplayableId;
               

            }
    
            string responseContent = string.Empty;
            PBIReports PBIReports = null;
            AuthenticationResult AAR = Session["authResult"] as AuthenticationResult;

            //Configure reports request
            System.Net.WebRequest request = System.Net.WebRequest.Create(String.Format("{0}reports", baseUri)) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AAR.AccessToken));

            //Get reports response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();

                    //Deserialize JSON string
                    PBIReports = JsonConvert.DeserializeObject<PBIReports>(responseContent);

                    
                }
            }

            return View(PBIReports);
        }

        public PartialViewResult GetIframe( string embedurl)
        {
            //var str = "VisionAndMission";
            ViewBag.url = embedurl;
            AuthenticationResult AAR = Session["authResult"] as AuthenticationResult;
            ViewBag.token = AAR.AccessToken;

            return PartialView();
        }

        public PartialViewResult OurEthics()
        {
            return PartialView("OurEthics");
        }
    }
}
