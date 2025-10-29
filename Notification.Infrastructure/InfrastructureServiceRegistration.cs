using Notification.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Notification.Application.Contracts;
using Notification.Infrastructure.Models;

namespace Notification.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        if (emailSettings == null ) {
            throw new InvalidOperationException("Missing email settings");
        }

        var emailService = new EmailService(
            credentialsPath: emailSettings.CredentialPath,
            fromEmail: emailSettings.FromEmail,
            tokenPath: emailSettings.TokenPath
        );
        services.AddSingleton<IEmailService>( emailService );

        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();

        if (apiSettings != null && Uri.TryCreate(apiSettings.IamBaseUrl, new UriCreationOptions(), out Uri? iamBaseUrl))
        {
            services.AddSingleton<IIamService>(new IamGrpcService(iamBaseUrl));
        }

        return services;
    }
}
