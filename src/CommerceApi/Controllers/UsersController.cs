﻿using CommerceApi.Authentication.Common;
using CommerceApi.Authentication.Extensions;
using CommerceApi.Authorization.Filters;
using CommerceApi.BusinessLayer.Services.Interfaces;
using CommerceApi.Shared.Models;
using CommerceApi.Shared.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApi.Controllers;

public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }


    [HttpDelete("DeleteAccount")]
    [RoleAuthorize(RoleNames.Administrator, RoleNames.PowerUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccount(Guid userId)
    {
        var response = await userService.DeleteAccountAsync(userId);
        if (response.Succeeded)
        {
            return Ok("User successfully deleted");
        }

        return BadRequest(response);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        //var validationResult = await validator.ValidateAsync(request);
        //if (!validationResult.IsValid)
        //{
        //    return BadRequest(validationResult.Errors);
        //}

        var response = await userService.LoginAsync(request);
        if (response is null)
        {
            return BadRequest("wrong email or password");
        }

        return Ok(response);
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromServices] IValidator<RegisterRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await userService.RegisterAsync(request);
        if (!response.Succeeded)
        {
            return BadRequest(response);
        }

        return Ok("user successfully registrated");
    }

    [HttpPost("Refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, [FromServices] IValidator<RefreshTokenRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await userService.RefreshTokenAsync(request);
        if (response is null)
        {
            return BadRequest("couldn't validate access token");
        }

        return Ok(response);
    }


    [HttpGet("Me")]
    [RoleAuthorize(RoleNames.Administrator, RoleNames.PowerUser, RoleNames.User)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetMe()
    {
        var user = new User
        {
            Id = User.GetId(),
            FirstName = User.GetFirstName(),
            LastName = User.GetLastName(),
            DateOfBirth = User.GetDateOfBirth(),
            PhoneNumber = User.GetPhoneNumber(),
            Email = User.GetEmail(),
            UserName = User.GetUserName()
        };

        return Ok(user);
    }
}