using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class NotificationHub : Hub;
