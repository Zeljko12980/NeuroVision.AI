using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TumorDetectionService.Application.Common;

namespace TumorDetectionService.API;

internal static class TumorActorExtensions
{
    public static bool TryGetTumorActor(this ClaimsPrincipal user, out TumorActor actor)
    {
        actor = null!;

        var subject =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId) || userId == Guid.Empty)
            return false;

        var role =
            user.FindFirstValue(ClaimTypes.Role)
            ?? user.FindFirstValue("role")
            ?? string.Empty;

        actor = new TumorActor(userId, role);
        return true;
    }

    public static IActionResult? RequireActor(this ControllerBase controller, out TumorActor actor)
    {
        if (controller.User.TryGetTumorActor(out actor))
            return null;

        return controller.Unauthorized();
    }
}
