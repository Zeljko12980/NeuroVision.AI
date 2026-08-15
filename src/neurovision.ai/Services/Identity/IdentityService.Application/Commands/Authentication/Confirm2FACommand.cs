namespace IdentityService.Application.Commands.Authentication;

public record Confirm2FACommand(Confirm2FARequest Confirm2FARequest) : ICommand<Result<Confirm2FAResponse>>;

public class Confirm2FACommandValidator : AbstractValidator<Confirm2FACommand>
{
    public Confirm2FACommandValidator()
    {
        RuleFor(x => x.Confirm2FARequest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Confirm2FARequest.Code)
            .NotEmpty().WithMessage("2FA code is required.")
            .Length(6).WithMessage("2FA code must be 6 characters long.");
    }
}

public class Confirm2FACommandHandler : ICommandHandler<Confirm2FACommand, Result<Confirm2FAResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwt;

    public Confirm2FACommandHandler(
        IIdentityService identityService,
        IUserService userService,
        IJwtTokenGenerator jwt)
    {
        _identityService = identityService;
        _userService = userService;
        _jwt = jwt;
    }

    public async Task<Result<Confirm2FAResponse>> Handle(Confirm2FACommand command, CancellationToken cancellationToken)
    {
        var email = command.Confirm2FARequest.Email;
        var verified = await _identityService.VerifyTwoFactorAsync(
            email,
            command.Confirm2FARequest.Code,
            cancellationToken);

        if (!verified)
            return Result<Confirm2FAResponse>.Fail("Invalid or expired 2FA code.");

        var roles = await _identityService.GetUserRolesAsync(email, cancellationToken);

        if (roles is null)
            return Result<Confirm2FAResponse>.Fail("Roles not found.");

        var userResult = await _userService.GetByEmailAsync(email, cancellationToken);

        if (!userResult.IsSuccess)
            return Result<Confirm2FAResponse>.Fail("User not found.");

        var token = _jwt.GenerateToken(
            userResult.Value.Id,
            email,
            userResult.Value.UserName,
            roles.ToList());

        return Result<Confirm2FAResponse>.Ok(new Confirm2FAResponse
        {
            Token = token,
            Message = "Login successful."
        });
    }
}
