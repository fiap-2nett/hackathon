using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;

namespace HealthMed.Application.Contracts.Users;

public sealed class DetailedUserResponse
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public int IdUser { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the user cpf.
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    /// Gets or sets the user crm.
    /// </summary>
    public string Crm { get; set; }

    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public RoleResponse Role { get; set; }

    /// <summary>
    /// Gets the user's creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the user's last updated date.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }
}
