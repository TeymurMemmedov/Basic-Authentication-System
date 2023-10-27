using System;

namespace EmailService
{

    //Appimizdən e-mail göndərə bilmək üçün proyektin ana appsettings-ində lazımı konfiqlər edilməlidir. Həmin konfiqləri sonradan appdə qarşılayacaq class budur
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
