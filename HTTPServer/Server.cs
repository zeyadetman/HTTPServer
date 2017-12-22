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
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            //done
            serverSocket.Listen(100000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            //done
            serverSocket.Accept();
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            //done
            Socket sok = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            //done
            sok.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request

                    // TODO: break the while loop if receivedLen==0

                    // TODO: Create a Request object using received request string

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    //done
                    Logger.LogException(ex);

                }
            }

            // TODO: close client socket
            //done
            sok.Shutdown(SocketShutdown.Both);
            sok.Close();
        }

        Response HandleRequest(Request request)
        {
            throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 

                //TODO: map the relativeURI in request to get the physical path of the resource.

                //TODO: check for redirect

                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
                //done
                Response res = new Response(StatusCode.OK, "text/html", content, Configuration.InternalErrorDefaultPageName);
                return res;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                //done
                Logger.LogException(ex);
                
                // TODO: in case of exception, return Internal Server Error.
                //done
                Response res = new Response(StatusCode.InternalServerError,"text/html",content,Configuration.InternalErrorDefaultPageName);
                return res;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            //done
            try
            {
                return Configuration.RedirectionRules[relativePath];
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            try
            {
                string ss = "";
                using (StreamReader sr = new StreamReader(filePath))
                {
                    ss += sr.Read();
                }
                return ss;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary


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
