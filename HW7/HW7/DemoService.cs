using System;

namespace CS422
{
    internal class DemoService : WebService
    {
        private const string c_template =
            "<html>This is the response to the request:<br>" +
            "Method: {0}<br>Request-Target/URI: {1}<br>" +
            "Request body size, in bytes: {2}<br><br>" +
            "Student ID: {3}<br>" + 
            "Time: {4}</html>";

        public override string ServiceURI {
            get
            {
                return "/";
            }
        }

        public override void Handler(WebRequest req)
        {
            req.WriteHTMLResponse(string.Format(c_template, req.HTTPMethod, req.RequestURI, req.BodySize, "11216720", DateTime.Now.ToString("h:mm:ss")));
        }
    }
}

