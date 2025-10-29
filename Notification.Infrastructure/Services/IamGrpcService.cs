using Grpc.Common.Iam;

using Grpc.Net.Client;

using System.Net;

using static Grpc.Common.Iam.IamService;
using Notification.Application.Contracts;

namespace Notification.Infrastructure.Services;

public class IamGrpcService : IIamService
{
    private readonly IamServiceClient _client;

    public IamGrpcService(Uri iamApiUrl)
    {
        var handler = new HttpClientHandler
        {
            // Ha self-signed cert van (pl. dev Docker környezet)
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var channel = GrpcChannel.ForAddress(iamApiUrl, new GrpcChannelOptions
        {
            HttpHandler = handler,
            HttpVersion = HttpVersion.Version20,
            HttpVersionPolicy = HttpVersionPolicy.RequestVersionExact
        });

        _client = new IamServiceClient(channel);
    }


    public async Task<UserData?> GetUserDataByUserId(int userId, CancellationToken ct = default)
    {
        try { 
            var result = await _client.GetUserDataAsync(new GetUserDataRequest { UserId = userId }, cancellationToken: ct);
            return new UserData
            {
                Id = result.UserId,
                UserName = result.UserName,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email
            };
        } catch (Grpc.Core.RpcException)
        {
            //TODO: exception handling and log
            return null;
        }
    }
}
