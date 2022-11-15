﻿using CommerceApi.Authentication.Extensions;
using CommerceApi.BusinessLayer.Services.Interfaces;
using CommerceApi.Shared.Models;
using CommerceApi.Shared.Requests;
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

    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
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
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await userService.RefreshTokenAsync(request);
        if (response is null)
        {
            return BadRequest("couldn't validate access token");
        }

        return Ok(response);
    }


    [HttpGet("Me")]
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