namespace CmmandService
{
    public class EmailSettingsOptions
    {
        public string Host { get; set; } 
        public string Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string EnableSsl { get; set; }
        public string From { get; set; }
        public string NoReply { get; set; }
        public string Destinatario { get; set; }

    }
}