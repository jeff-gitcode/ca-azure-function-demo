using Domain;
using MediatR;

namespace Application.Interfaces.SPI;

public interface IUserRepository
{
    Task<List<UserDTO>> GetAllAsync();
    Task<UserDTO> GetByIdAsync(string id);
    Task<UserDTO> GetByEmailAsync(string email);
    Task<UserDTO> AddAsync(UserDTO tempUser);
    Task<UserDTO> UpdateAsync(UserDTO tempUser);
    Task<Unit> DeleteAsync(string id);
}