using System.Collections.Generic;

namespace PetizenApi.Providers
{
    public class EmailConfiguration
    {
        public string FromAddress { get; set; }
        public string FromAdressTitle { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPortNumber { get; set; }
        public string Password { get; set; }
        public List<string> BccEmail { get; set; }
        public List<string> CcEmail { get; set; }
    }
}
