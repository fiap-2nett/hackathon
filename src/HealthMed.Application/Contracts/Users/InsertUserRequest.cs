using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMed.Domain.Enumerations;

namespace HealthMed.Application.Contracts.Users
{
    public sealed class InsertUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Cpf { get; set; }
        public string Crm { get; set; }
        public UserRoles Role { get; set; }
    }
}
