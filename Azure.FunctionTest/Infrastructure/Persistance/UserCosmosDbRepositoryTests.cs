using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Infrastructure.Config;
using Infrastructure.Persistance;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Moq;

namespace Azure.FunctionTest.Infrastructure.Persistance;

public class UserCosmosDbRepositoryTests
{
    private Mock<ICosmosDbContainerFactory> _cosmosDbContainerFactory;
    private Mock<IOptions<CosmosDbSettings>> _options;
    private Mock<Container> _container;
    private UserCosmosDbRepository _userCosmosDbRepository;

    public UserCosmosDbRepositoryTests()
    {
        _cosmosDbContainerFactory = new Mock<ICosmosDbContainerFactory>();
        _options = new Mock<IOptions<CosmosDbSettings>>();
        _container = new Mock<Container>();

        _options.Setup(x => x.Value).Returns(new CosmosDbSettings { Containers = new List<ContainerInfo> { new ContainerInfo { Name = "Users", PartitionKey = "Email" } } });

    }

    [Theory, AutoData]
    public async Task Should_Return_When_Get_All_Users(List<Users> data)
    {
        // Arrange
        var feedResponseMock = new Mock<FeedResponse<Users>>();
        feedResponseMock.SetupGet(x => x.Resource).Returns(data);
        feedResponseMock.Setup(x => x.GetEnumerator()).Returns(() => data.GetEnumerator());

        var users = new Mock<FeedIterator<Users>>();
        users.SetupSequence(x => x.HasMoreResults).Returns(true);
        users.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedResponseMock.Object);

        _container.Setup(x => x.GetItemQueryIterator<Users>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
            .Returns(users.Object);

        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        // Act
        _userCosmosDbRepository = new UserCosmosDbRepository(_cosmosDbContainerFactory.Object, _options.Object);
        var result = await _userCosmosDbRepository.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(data);

    }

    [Theory, AutoData]
    public async Task Should_Return_When_Get_User_By_Id(Users data)
    {
        // Arrange
        var feedResponseMock = new Mock<FeedResponse<Users>>();
        feedResponseMock.SetupGet(x => x.Resource).Returns(new List<Users> { data });
        feedResponseMock.Setup(x => x.GetEnumerator()).Returns(() => new List<Users> { data }.GetEnumerator());

        var users = new Mock<FeedIterator<Users>>();
        users.SetupSequence(x => x.HasMoreResults).Returns(true);
        users.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedResponseMock.Object);

        _container.Setup(x => x.GetItemQueryIterator<Users>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
            .Returns(users.Object);

        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        // Act
        _userCosmosDbRepository = new UserCosmosDbRepository(_cosmosDbContainerFactory.Object, _options.Object);
        var result = await _userCosmosDbRepository.GetByIdAsync(data.Id);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Should_Return_When_Get_User_By_Email()
    {

    }

    [Theory, AutoData]
    public async Task Should_Return_When_Add_User(UserDTO data)
    {
        // Arrange
        _container.Setup(x => x.CreateItemAsync<Users>(It.IsAny<Users>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()));

        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        // Act
        _userCosmosDbRepository = new UserCosmosDbRepository(_cosmosDbContainerFactory.Object, _options.Object);
        await _userCosmosDbRepository.AddAsync(data);

        // Assert
        _container.Verify(x => x.CreateItemAsync<Users>(It.IsAny<Users>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Should_Return_When_Update_User(UserDTO data)
    {
        // Arrange
        _container.Setup(x => x.UpsertItemAsync<Users>(It.IsAny<Users>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()));
        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        // Act
        _userCosmosDbRepository = new UserCosmosDbRepository(_cosmosDbContainerFactory.Object, _options.Object);
        await _userCosmosDbRepository.UpdateAsync(data);

        // Assert
        _container.Verify(x => x.UpsertItemAsync<Users>(It.IsAny<Users>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Should_Return_When_Delete_User(Users user, UserDTO data)
    {
        // Arrange
        var feedResponseMock = new Mock<FeedResponse<Users>>();
        feedResponseMock.SetupGet(x => x.Resource).Returns(new List<Users> { user });
        feedResponseMock.Setup(x => x.GetEnumerator()).Returns(() => new List<Users> { user }.GetEnumerator());

        var users = new Mock<FeedIterator<Users>>();
        users.SetupSequence(x => x.HasMoreResults).Returns(true);
        users.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedResponseMock.Object);

        _container.Setup(x => x.GetItemQueryIterator<Users>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
            .Returns(users.Object);

        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        _container.Setup(x => x.DeleteItemAsync<Users>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()));
        _cosmosDbContainerFactory.Setup(x => x.GetContainer(It.IsAny<string>()))
            .Returns(_container.Object);

        // Act
        _userCosmosDbRepository = new UserCosmosDbRepository(_cosmosDbContainerFactory.Object, _options.Object);
        await _userCosmosDbRepository.DeleteAsync(user.Id);

        // Assert
        _container.Verify(x => x.DeleteItemAsync<Users>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}