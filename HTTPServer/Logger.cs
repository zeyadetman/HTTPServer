using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            // for each exception write its details associated with datetime
            //done
            string fileName = "log.txt";
            try
            {
                
                
                using (StreamWriter sw = new StreamWriter(fileName, true))
                {
                    sw.Write(DateTime.Now);
                    sw.Write(ex.Message);
                    sw.Close();
                }

            }
            catch (Exception e) { }
        }
    }
}
