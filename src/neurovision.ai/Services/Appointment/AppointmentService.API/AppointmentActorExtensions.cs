using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppointmentService.Application.Common;

namespace AppointmentService.API;

internal static class AppointmentActorExtensions
{
    public static bool TryGetAppointmentActor(this ClaimsPrincipal user, out AppointmentActor actor)
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

        actor = new AppointmentActor(userId, role);
        return true;
    }

    public static IActionResult? RequireActor(this ControllerBase controller, out AppointmentActor actor)
    {
        if (controller.User.TryGetAppointmentActor(out actor))
            return null;

        return controller.Unauthorized();
    }
}
