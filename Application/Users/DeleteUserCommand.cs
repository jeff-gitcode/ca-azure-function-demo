using Application.Interfaces.SPI;
using MediatR;

namespace Application.Users;

public record DeleteUserCommand(string id) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.DeleteAsync(request.id);
    }
}