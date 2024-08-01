namespace HealthMed.Infrastructure.Messaging.Settings
{
    public sealed class MailSettings
    {
        #region Constants

        public const string SettingsKey = "Mail";

        #endregion

        #region Properties

        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }

        #endregion
    }
}
