using System.Net.Http.Headers;
using System.Text;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using static API.DTOs.GitHubInfo;

namespace API.Controllers;

public class AccountController(SignInManager<User> signInManager, IEmailSender<User> emailSender, IConfiguration configuration) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("github-login")]
    public async Task<ActionResult> LoginWithGithub(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Missing authorization code");
        }
        // Using the 'using' keyword because we need an instance of the HttpClient object which will be desposed once the call is done.
        using var httpClient = new HttpClient();
        // The DefaultRequestHeaders gets the headers which should be sent with each request.
        // The Accept gets the value of the Accept header for an HTTP request.
        // Add - Adds an entry to the HttpHeaderValueCollection<T>.
        // new MediaTypeWithQualityHeaderValue("application/json") - Initializes a new instance of the MediaTypeWithQualityHeaderValue class and validates that the response will be in a json format. Otherwise it will return an xml format.
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Step 1 - Exchange code for access token
        // The PastAsJsonAsync sends a POST request to the specified Uri containing the value serialized as JSON in the request body.
        // It returns the task object representing the asynchronous operation.
        var tokenResponse = await httpClient.PostAsJsonAsync(
            "https://github.com/login/oauth/access_token",
            new GitHubAuthRequest
            {
                Code = code,
                ClientId = configuration["Authentication:GitHub:ClientId"]!,
                ClientSecret = configuration["Authentication:GitHub:ClientSecret"]!,
                RedirectUri = $"{configuration["ClientAppUrl"]}/auth-callback"
            }
        );
        if (!tokenResponse.IsSuccessStatusCode)
        {
            return BadRequest("Failed to get access token");
        }
        var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<GitHubTokenResponse>();
        if (string.IsNullOrEmpty(tokenContent?.AccessToken))
        {
            return BadRequest("Failed to retrieve access token");
        }
        // Step 2 - Fetch user info from GitHub
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenContent.AccessToken);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Reactivities");
        var userResponse = await httpClient.GetAsync("https://api.github.com/user");
        if (!userResponse.IsSuccessStatusCode)
        {
            return BadRequest("Failed to fetch user from GitHub");
        }
        var user = await userResponse.Content.ReadFromJsonAsync<GitHubUser>();
        if (user == null)
        {
            return BadRequest("Failed to read user from GitHub");
        }

        // Step 3 - Getting the email if needed (in case the user defined the email as private)
        if (string.IsNullOrEmpty(user?.Email))
        {
            var emailResponse = await httpClient.GetAsync("https://api.github.com/user/emails");
            if (emailResponse.IsSuccessStatusCode)
            {
                var emails = await emailResponse.Content.ReadFromJsonAsync<List<GitHubEmail>>();
                var primary = emails?.FirstOrDefault(e => e is { Primary: true, Verified: true })?.Email;
                if (string.IsNullOrEmpty(primary))
                {
                    return BadRequest("Failed to get email from GitHub");
                }
                user!.Email = primary; // Checked for null so it can't be null here
            }
        }

        // Step 4 - Find or create user and sign in
        var existingUser = await signInManager.UserManager.FindByEmailAsync(user!.Email);
        if (existingUser == null)
        {
            existingUser = new User
            {
                Email = user.Email,
                UserName = user.Email,
                DisplayName = user.Name,
                ImageUrl = user.ImageUrl
            };
            var createdResult = await signInManager.UserManager.CreateAsync(existingUser);
            if (!createdResult.Succeeded)
            {
                return BadRequest("Failed to create user");
            }
        }
        await signInManager.SignInAsync(existingUser, false);

        return Ok();
    }

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
            await SendConfirmationEmailAsync(user, registerDto.Email);
            return Ok();
        }
        foreach (var error in result.Errors)
        {
            // The ModelState.AddModelError method is used to add an error message to the model state, which is a collection of errors that can be returned to the client when the model validation fails. The first parameter is the key for the error, which can be used to identify the specific error, and the second parameter is the error message itself. In this case, we are using the error code as the key and the error description as the message.
            ModelState.AddModelError(error.Code, error.Description);
        }
        return ValidationProblem(); // The ValidationProblem method is used to return a response with a status code of 400 (Bad Request) and a body that contains the details of the validation errors. This is useful when the user input does not meet the required validation criteria, allowing the client to understand what went wrong and how to fix it.
    }

    [AllowAnonymous]
    [HttpGet("resendConfirmEmail")]
    public async Task<ActionResult> ResendConfirmEmail(string? email, string? userId)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId))
        {
            return BadRequest("Email or UserId must be provided");
        }
        var user = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Email == email || x.Id == userId);
        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            return BadRequest("User not found");
        }
        await SendConfirmationEmailAsync(user, user.Email);
        return Ok();
    }

    private async Task SendConfirmationEmailAsync(User user, string email)
    {
        // Generates an email confirmation token for the specified user.
        // Returns a Task that represents the asynchronous operation, an email confirmation token.
        var code = await signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
        // Since the code might inculde characters that are not valid in a URL we encode it:
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        // Creating a confirmEmailUrl. This is going to be a client side link
        var confirmEmailUrl = $"{configuration["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={code}";
        await emailSender.SendConfirmationLinkAsync(user, email, confirmEmailUrl);
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

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto passwordDto)
    {
        // The User here refers to the ClaimPrinciple we get from the Identity framework
        var user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var result = await signInManager.UserManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors.First().Description);
    }
}
