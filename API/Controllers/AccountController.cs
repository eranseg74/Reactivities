using System;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(SignInManager<User> signInManager) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            DisplayName = registerDto.DisplayName
        };
        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return Ok();
        }
        foreach (var error in result.Errors)
        {
            // The ModelState.AddModelError method is used to add an error message to the model state, which is a collection of errors that can be returned to the client when the model validation fails. The first parameter is the key for the error, which can be used to identify the specific error, and the second parameter is the error message itself. In this case, we are using the error code as the key and the error description as the message.
            ModelState.AddModelError(error.Code, error.Description);
        }
        return ValidationProblem(); // The ValidationProblem method is used to return a response with a status code of 400 (Bad Request) and a body that contains the details of the validation errors. This is useful when the user input does not meet the required validation criteria, allowing the client to understand what went wrong and how to fix it.
    }

    // We need to call this endpoint when a user first reaches the application or refreshes the page. In order to log the user in we will need access to his info in order to authenticate him so we need to define this call as AllowAnonymous. Uf they are logged in all we have is access to the cookie but we cannot read the cookie from the browser using JS vanilla
    [AllowAnonymous]
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        // Because we are inside the context of an API controller we have access to the User object
        if (User.Identity?.IsAuthenticated == false)
        {
            return NoContent();
        }
        var user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(new
        {
            user.DisplayName,
            user.Email,
            user.Id,
            user.ImageUrl
        });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync(); // This also removes the cookie
        return NoContent();
    }
}
