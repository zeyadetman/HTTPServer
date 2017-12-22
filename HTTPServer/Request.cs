using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter
            //done
            this.requestLines = this.requestString.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries); 

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            //done
            if (requestLines.Length < 3) return false;

            // Parse Request line
            //done
            if (!ParseRequestLine()) return false;

            // Validate blank line exists
            //done
            if(!ValidateBlankLine()) return false;
            // Load header lines into HeaderLines dictionary
            //done
            if(!LoadHeaderLines()) return false;
        }

        private bool ParseRequestLine()
        {
            string[] lines = this.requestLines[0].Split(' ');
            if (lines.Length < 3) return false;
            this.method = (RequestMethod)Enum.Parse(typeof(RequestMethod), lines[0]);
            this.relativeURI = lines[1].Substring(1);
            switch (lines[2])
            {
                case "HTTP/0.9":
                    this.httpVersion = HTTPVersion.HTTP09;
                    break;
                case "HTTP/1.0":
                    this.httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    this.httpVersion = HTTPVersion.HTTP11;
                    break;
            }
            return !ValidateIsURI(relativeURI) ? false : true;

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        //done
        private bool LoadHeaderLines()
        {
            for (int i = 0; i < this.requestLines.Length-2; i++)
            {
                string[] HeaderLine = requestLines[i].Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);
                if (HeaderLine.Length < 2) return false;
                this.HeaderLines.Add(HeaderLine[0], HeaderLine[1]);
            }
            return true;
        }
        //done
        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 2] == "") return false;
            return true;
        }

    }
}
