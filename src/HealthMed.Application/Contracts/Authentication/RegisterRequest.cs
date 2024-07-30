namespace HealthMed.Application.Contracts.Authentication
{
    public sealed class RegisterRequest
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public string CRM { get; set; }
        public bool IsDoctor { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
