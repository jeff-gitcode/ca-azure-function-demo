using Application.Interfaces.SPI;
using Domain;
using Infrastructure.Config;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistance;

public class UserCosmosDbRepository : IUserRepository
{
    private readonly ICosmosDbContainerFactory _cosmosDbContainerFactory;
    private readonly IOptions<CosmosDbSettings> _options;

    private readonly Container _container;

    public UserCosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory, IOptions<CosmosDbSettings> options)
    {
        this._cosmosDbContainerFactory = cosmosDbContainerFactory;
        this._options = options;
        this._container = this._cosmosDbContainerFactory.GetContainer(this._options.Value.Containers[0].Name)._container;
    }

    public async Task<List<UserDTO>> GetAllAsync()
    {
        try
        {
            string query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = this._container.GetItemQueryIterator<Users>(queryDefinition);

            List<UserDTO> results = new();

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var user in currentResultSet)
                {
                    results.Add(new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Password = user.Password,
                    });
                }
            }

            return results;

        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UserDTO> GetByIdAsync(string id)
    {

        string query = "SELECT * FROM c WHERE c.id = @id";
        var queryDefinition = new QueryDefinition(query).WithParameter("@id", id);
        var queryResultSetIterator = this._container.GetItemQueryIterator<Users>(queryDefinition);

        List<UserDTO> results = new();

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (var user in currentResultSet)
            {
                results.Add(new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                });
            }
        }

        return results.FirstOrDefault();
    }

    public Task<UserDTO> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDTO> AddAsync(UserDTO tempUser)
    {
        tempUser.Id = Guid.NewGuid().ToString();
        var user = new Users
        {
            Id = tempUser.Id,
            Email = tempUser.Email,
            FirstName = tempUser.FirstName,
            LastName = tempUser.LastName,
            Password = tempUser.Password,
        };

        await this._container.CreateItemAsync<Users>(user, new PartitionKey(user.Email));
        return tempUser;
    }

    public async Task<UserDTO> UpdateAsync(UserDTO tempUser)
    {
        var user = new Users
        {
            Id = tempUser.Id,
            Email = tempUser.Email,
            FirstName = tempUser.FirstName,
            LastName = tempUser.LastName,
            Password = tempUser.Password,
        };

        await this._container.UpsertItemAsync<Users>(user, new PartitionKey(user.Email));
        return tempUser;
    }

    public async Task<Unit> DeleteAsync(string id)
    {
        var user = await this.GetByIdAsync(id);
        await this._container.DeleteItemAsync<Users>(id, new PartitionKey(user.Email));
        return Unit.Value;
    }
}
