using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GoogleScrapper;
using System.IO;
namespace WebBrowserWithoutAForm
{
    class Program
    {
        private static bool completed = false;
        private static WebBrowser wb;

        [STAThread]
        static void Main(string[] args)
        {
            MainProgram m = new MainProgram();
            wb = new WebBrowser();
            m.Mwebbrowser = wb;
            m.initWebbrowser();//add loading complete
            //m.Mwebbrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
            string url = "https://accounts.google.com/ServiceLogin?hl=en&continue=https://www.google.com/";
           
            Account a = new Account();
            a.Username = "leliasiciliani@lab.imtlucca.it";
            a.Password = "imtlucca1234";
            m.Log_in(a, m.Mwebbrowser, url);

            while (!m.loadFinished)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            //m.saveBitmap(m.Mwebbrowser, "login");
            Console.WriteLine(wb.Document.Body.InnerHtml);
            Console.Write("\n\nDone with it!\n\n");
            Console.ReadLine();
        }


        static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Console.WriteLine(wb.Document.Body.InnerHtml);
            completed = true;
        }

 
    }
}