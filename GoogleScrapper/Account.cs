using log4net;
using System.Reflection;
namespace GoogleScrapper
{
    public class Account
    {
        private string _username;
        private string _password;
        private string _otherinfo;
        private string _location;
        private int _age;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Username
        {
            get { log.Info(this._username); return _username; }
            set { _username = value; log.Info(this._username); }
        }
        
        public string Password
        {
            get { log.Info(this._password); return _password; }
            set { _password = value; log.Info(this._password); }
        }
        

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
       

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }
        

        public string Otherinfo
        {
            get { return _otherinfo; }
            set { _otherinfo = value; }
        }
    
        
    }
}
