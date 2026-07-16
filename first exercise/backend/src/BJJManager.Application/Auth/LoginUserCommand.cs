using BJJManager.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Auth;

public record LoginUserCommand(string Name, string Password) : IRequest<LoginUserResult>;

public record LoginUserResult(string Token, DateTime ExpiresAtUtc, string UserName);

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var candidates = await _userRepository.GetAllByNameAsync(request.Name, cancellationToken);

        var user = candidates.FirstOrDefault(u => _passwordHasher.Verify(request.Password, u.PasswordHash));

        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (token, expiresAtUtc) = _jwtTokenGenerator.GenerateToken(user);

        return new LoginUserResult(token, expiresAtUtc, user.Name);
    }
}
