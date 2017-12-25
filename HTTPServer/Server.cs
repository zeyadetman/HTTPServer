using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //done
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            //done
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            //done
            serverSocket.Listen(500);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            //done
            //serverSocket.Accept();
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("new client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;
            byte[] data;
            int receivedLength;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                    return new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string rediredtPagePath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (rediredtPagePath != "")
                    return new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(Configuration.RedirectionDefaultPageName), rediredtPagePath);
                //TODO: check file exists
                if (!File.Exists(physicalPath))
                    return new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), "");
                //TODO: read the physical file
                content = File.ReadAllText(physicalPath);
                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, "");
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            //done
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            StreamReader ss;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("default page" + defaultPageName + "doesn't exists"));
                return string.Empty;
            }
            else {
            ss = new StreamReader(filePath);
            string file = ss.ReadToEnd();
            ss.Close();
            return file;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary
                StreamReader ss = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!ss.EndOfStream)
                {
                    string temp = ss.ReadLine();
                    string[] redirect = temp.Split(',');
                    Configuration.RedirectionRules.Add(redirect[0], redirect[1]);
                }
                ss.Close();

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                //done
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
