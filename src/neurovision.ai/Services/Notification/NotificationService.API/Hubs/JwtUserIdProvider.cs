using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Hubs;

public sealed class JwtUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
