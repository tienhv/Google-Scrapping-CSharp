using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Reflection;
using log4net.Appender;
using log4net.Layout;

namespace Log4NetDemo
{

    class Demo
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main2()
        {
            //XmlConfigurator.Configure(); //only once
            log4net.NDC.Push("context");//for context
            Log.Debug("Application is starting");
            Console.WriteLine("Test Line");
            var testClass = new LogTestClass();
            testClass.LogSomething();

            Log.Debug("Application is ending");
            Log.Info("Info");
            Log.Error("Error");
            

            Log.Info("Application started");

            try
            {
                int value = int.Parse("Tiến");
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error: {0}", e.Message);
            }
        Console.Read();

        }
    }

    public class LogTestClass
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int LogSomething()
        {
            for (int i = 0; i < 4; i++)
            {
                Log.InfoFormat("CurrentTime is [{0}]", DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss~fff"));
            }
            return 1;
        }
    }
}
