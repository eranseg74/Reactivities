using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace Infrastructure.Email;

// The IServiceScopeFactory creates instances of IServiceScope, which is used to create services within a scope.
// We need this because we defined the email service as Transient and this throws a System when trying to run the application.InvalidOperationException: Cannot resolve 'Microsoft.AspNetCore.Identity.IEmailSender`1[Domain.User]' from root provider because it requires scoped service. To overcome this issue we inject the scopeFactory so we will be able to create the scope on runtime 
public class EmailSender(IServiceScopeFactory scopeFactory) : IEmailSender<User>
{
    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var subject = "Confirm your email address";
        var body = $@"
            <p>Hi {user.DisplayName}</p>
            <p>Please confirm your email by clicking the link below</p>
            <p><a href='{confirmationLink}'>Click here to verify email</a></p>
            <p>Thanks</p>
        ";
        await SendEmailAsync(email, subject, body);
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    private async Task SendEmailAsync(string email, string subject, string body)
    {
        // Creating a scope while defining it as using so it will be destroyed once the method is complete.
        using var scope = scopeFactory.CreateScope();
        // Fetching the service that was declared for the IResend interface in the Program.cs class.
        var resend = scope.ServiceProvider.GetRequiredService<IResend>();
        var message = new EmailMessage
        {
            From = "whatever@resend.dev", // The domain must be @resend.dev. A different domain requires configuring a cusom domain in Resend
            Subject = subject,
            HtmlBody = body
        };
        message.To.Add(email);
        Console.WriteLine(message.HtmlBody);
        await resend.EmailSendAsync(message);
        // await Task.CompletedTask;
    }
}
