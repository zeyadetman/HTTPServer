using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            //done
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("Date: " + DateTime.Now);
            if (redirectoinPath != "") headerLines.Add("location: " + redirectoinPath);



            
            // TODO: Create the request string
            //done
            responseString = GetStatusLine(code);
            foreach (var item in headerLines)
            {
                responseString += item + "\r\n";
            }
            responseString += "\r\n" + content;
           
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            //done
            string statusLine = Enum.GetName(typeof(StatusCode), code);
            return statusLine;
        }
    }
}
