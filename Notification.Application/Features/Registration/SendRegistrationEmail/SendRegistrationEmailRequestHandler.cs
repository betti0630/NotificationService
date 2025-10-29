using MediatR;

using Notification.Application.Contracts;


namespace Notification.Application.Features.Registration.SendRegistrationEmail;

internal sealed class SendRegistrationEmailRequestHandler(IIamService iamService, IEmailService emailService) : IRequestHandler<SendRegistrationEmailRequest, SendRegistrationEmailResponse>
{
    public async Task<SendRegistrationEmailResponse> Handle(SendRegistrationEmailRequest request, CancellationToken cancellationToken)
    {
        var userData = await iamService.GetUserDataByUserId(request.UserId, cancellationToken);

        if (userData == null)
        {
            return new SendRegistrationEmailResponse
            {
                Success = false,
                Message = $"Not valid userId"
            };
        }

        var verificationLink = $"{request.VerificationLink}{request.Token}";

        var body = CreateEmailBody(userData.FirstName, verificationLink);

        await emailService.SendEmailAsync(
              to: userData.Email,
              subject: "Email verfication",
              body: body
          );

        return new SendRegistrationEmailResponse
        {
            Success = true,
            Message = $"Registration email sent to {userData.UserName}"
        };
    }

    private static string CreateEmailBody(string firstName, string verificationLink) { 
        return $@"
        <p>Hello {firstName},</p>
        <p>Thank you for registering with us! Please confirm your email address by clicking the link below.</p>
        <p>
            <a href='{verificationLink}' style='
                background-color:#4CAF50;
                color:white;
                padding:10px 20px;
                text-decoration:none;
                border-radius:5px;
                display:inline-block;
            '>Verify Email</a>
        </p>
        <p>This link will expire in <strong>24 hours</strong>.</p>
        <p>If you did not create an account, you can safely ignore this message.</p>
        <br/>
        <p>Best regards,<br/>The MyApp Team</p>
    ";
    }
}
