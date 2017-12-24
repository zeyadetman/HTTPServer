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
            Response response;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    response = new Response(StatusCode.BadRequest, "text/html", content, "");
                    return response;

                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect

                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    string RedirectionPath = Configuration.RedirectionRules[request.relativeURI];
                    content = LoadDefaultPage(GetRedirectionPagePathIFExist(RedirectionPath));


                    response = new Response(StatusCode.Redirect, "text/html", content, RedirectionPath);
                    return response;
                }
                else
                {
                    //TODO: read the physical file

                    content = LoadDefaultPage(request.relativeURI);
                    //TODO: check file exists

                    if (content != "")
                    {

                        // Create OK response
                        return response = new Response(StatusCode.OK, "text/xml", content, null);
                    }
                    else
                    {
                        content = LoadDefaultPage("NotFound.html");
                        return response = new Response(StatusCode.NotFound, "text/html", content, null);
                    }
                }




                //done
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                //done
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error.
                //done
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, null);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            //done
            string redirectpath = Configuration.RootPath + "\\" + relativePath;
            if (File.Exists(redirectpath))
            {
                return relativePath;
            }
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
            ss = new StreamReader(filePath);
            string file = ss.ReadToEnd();
            ss.Close();
            return file;
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
