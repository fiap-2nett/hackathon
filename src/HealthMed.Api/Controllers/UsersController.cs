using System;
using System.Threading.Tasks;
using HealthMed.Api.Contracts;
using HealthMed.Api.Infrastructure;
using HealthMed.Application.Contracts.Authentication;
using HealthMed.Application.Contracts.Users;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HealthMed.Domain.Errors.DomainErrors;

namespace HealthMed.Api.Controllers;

public class UsersController : ApiController
{
    #region Read-Only Fields

    private readonly IUserService _userService;
    private readonly IUserSessionProvider _userSessionProvider;

    #endregion

    public UsersController(IUserService userService, IUserSessionProvider userSessionProvider)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userSessionProvider = userSessionProvider ?? throw new ArgumentNullException(nameof(userSessionProvider));
    }

    #region Endpoints

    /// <summary>
    /// Represents the query for inserting a new doctor/patient.
    /// </summary>
    [HttpPost(ApiRoutes.Users.Insert)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] InsertUserRequest insertUserRequest)
    {

        if (insertUserRequest.Name is null)
            throw new DomainException(DomainErrors.User.NameIsRequired);

        if (insertUserRequest.Role == UserRoles.Doctor)
        {
            if (insertUserRequest.Crm is null)
                throw new DomainException(DomainErrors.User.CRMCannotBeNull);

            if (insertUserRequest.Crm.Length > 10)
                throw new DomainException(DomainErrors.User.InvalidCRM);
        }


        if (insertUserRequest.Cpf is null || insertUserRequest.Cpf.Length > 11 )
            throw new DomainException(DomainErrors.User.InvalidCPF);

        var idUser = await _userService.CreateAsync(insertUserRequest.Name, insertUserRequest.Cpf, insertUserRequest.Crm, insertUserRequest.Email, insertUserRequest.Role, insertUserRequest.Password);

        return Created(new Uri(Url.ActionLink(nameof(GetById), "Users", new { idUser })), idUser);
    }


    /// <summary>
    /// Represents the query for getting a user by its id.
    /// </summary>
    /// <returns>The user with the informed id.</returns>
    [HttpGet(ApiRoutes.Users.GetById)]
    [ProducesResponseType(typeof(DetailedUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] int idUser)
        => Ok(await _userService.GetUserByIdAsync(idUser));
    
    /// <summary>
    /// Represents the query for getting a user by its email.
    /// </summary>
    /// <returns>The user with the informed email.</returns>
    [HttpGet(ApiRoutes.Users.GetByEmail)]
    [ProducesResponseType(typeof(DetailedUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmail([FromRoute] string email)
        => Ok(await _userService.GetUserByEmailAsync(email));

    #endregion

}
