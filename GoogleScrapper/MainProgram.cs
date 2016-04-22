using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;


namespace GoogleScrapper
{
    class MainProgram
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const bool isDebug = true;
        private MProxy _proxy;
        
        /// <summary>
        /// number of pages, including the first page of search results
        /// </summary>
        private int _search_depth  =3;
        /// <summary>
        /// number of results show in a single page, default is 30, controlling via &num=30 which is appending to
        /// google search queries
        /// </summary>
        private int _search_results_per_page = 30;
        internal System.Collections.ArrayList _keyword_list = new ArrayList();

        private System.Collections.ArrayList _click_domains;
        private System.Collections.ArrayList _click_topical_terms;
        private string _savingDirectory;
        private int _index_of_keyword;
        const int sleepQuery = 660000;//11 minute, 660000mms
        const bool login = false;//not login
        /// <summary>
        /// webbrowser must be initialized before passing to this variable
        /// </summary>
        private System.Windows.Forms.WebBrowser _mwebbrowser;

        public System.Windows.Forms.WebBrowser Mwebbrowser
        {
            get { return _mwebbrowser; }
            set { _mwebbrowser = value; }
        }
        private String _other;

        /// <summary>
        /// name of status file. Status file stores index of search keyword.
        /// This is full path
        /// </summary>
        private string _status_file;

        public MProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        
        
        

        public int Search_depth
        {
            get { return _search_depth; }
            set { _search_depth = value; }
        }
        

        internal int Search_results_per_page
        {
            get { return _search_results_per_page; }
            set { _search_results_per_page = value; }
        }
       

        public string Status_file
        {
            get { return _status_file; }
            set { _status_file = value; }
        }

       
        /// <summary>
        /// write the index of current searching keywords
        /// write after finishing search
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        public void write_index_to_file(string fileName, int index)
        {
            //if (!File.Exists(fileName))
            {
                try
                {

                    File.Create(fileName);
                    TextWriter tw = new StreamWriter(fileName);
                    tw.WriteLine(index);
                    tw.Close();
                }
                catch (Exception e)
                {
                    log.Error("Error:", e);
                }
            }
            //else if (File.Exists(fileName))
            //{
            //    TextWriter tw = new StreamWriter(fileName, true);
            //    tw.WriteLine(index);
            //    tw.Close();
            //}
        }

        /// <summary>
        /// read keywords from a text file,
        /// text format is asscii
        /// </summary>
        /// <param name="keyword_file">full path to file,could be relative path</param>
        /// <returns></returns>
        public System.Collections.ArrayList Read_keyword(string keyword_file)
        {

            this._keyword_list = new ArrayList();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(keyword_file))
                {
                    String line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        //add to array list
                        _keyword_list.Add(line);
                        log.Debug("Info - keyword: "+line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                //Console.WriteLine("The file could not be read:");
                //Console.WriteLine(e.Message);
                log.Error("Cannot read keyword file. ", e);
            }
            
            return this._keyword_list;
        }

        public void Create_Proxy(string address, int port)
        {
            throw new System.NotImplementedException();
        }


        const string home_url = "https://www.google.com/ncr";//no language setting
        
        public bool loadFinished = false;

        public void initWebbrowser()
        {
            this._mwebbrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this._mwebbrowser.Size = new System.Drawing.Size(1366,768);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath)
            {
                loadFinished = false;
                return;
            }

            //Page Loading Finished
            loadFinished = true;
            log.Debug("Finish loading webpage");
        }
        
        /// <summary>
        /// login google account
        /// </summary>
        /// <param name="account">must contain username and password</param>
        public void Log_in(Account account,WebBrowser wb, string url)
        {
            //https://accounts.google.com/ServiceLogin?hl=en&continue=https://www.google.com
            wb.Navigate(url);

            while (!loadFinished)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            wb.Document.GetElementById("Email").InnerText = account.Username;
            wb.Document.GetElementById("Passwd").InnerText = account.Password;

            wb.Document.GetElementById("signIn").InvokeMember("Click");
            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }




        }
        /// <summary>
        /// save the current webpage of webbrowser
        /// </summary>
        /// <param name="wb"></param>
        public void saveHTML(WebBrowser wb)
        {

        }
        /// <summary>
        /// save the screenshot of current webpage in webbrowser
        /// </summary>
        public void saveBitmap(WebBrowser wb,string keyword)
        {
            if (wb.Document == null)
            {
                return;
            }
            try
            {
                int scrollWidth = 0;
                int scrollHeight = 0;
                scrollHeight = 768;//wb.Document.Body.ScrollRectangle.Height;
                scrollWidth = 1366;//wb.Document.Body.ScrollRectangle.Width;
                wb.Size = new Size(scrollWidth, scrollHeight);
                Bitmap bm = new Bitmap(scrollWidth, scrollHeight);
                wb.DrawToBitmap(bm, new Rectangle(0, 0, bm.Width, bm.Height));
                string SaveAsName = null;
                SaveAsName = Regex.Replace(keyword, "(\\\\|\\/|\\:|\\*|\\?|\\\"|\\<|\\>|\\|)?", "");
                bm.Save(SaveAsName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                bm.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //
            }
        }



        public void Search_login(Account account, int page_depth, System.Collections.ArrayList keywords)
        {
            throw new System.NotImplementedException();
        }

        public void Search_login2(Account account, int results_per_pages, System.Collections.ArrayList keywords)
        {
            throw new System.NotImplementedException();
        }

        public void Search_no_login(System.Collections.ArrayList keywords)
        {
            throw new System.NotImplementedException();
        }

        public void Search_Click_Scroll(Account account, System.Collections.ArrayList keywords, System.Collections.ArrayList rules)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.ArrayList Extract_Results_to_Text(string html_text, string text_filename)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// main application
        /// </summary>
        /// <param name="args"></param>
        static void Main1(string[] args)
        {
            XmlConfigurator.Configure(); //only once
            Log4NetDemo.Demo.Main2();
        }
    }
}
