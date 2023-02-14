namespace Azure.FunctionTest;

using Xunit;
using Domain;
using Microsoft.AspNetCore.Http;
using Azure.Function;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Moq;
using Microsoft.Extensions.Logging;
using Application.Interfaces.API;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

// using Microsoft.Extensions.Logging.Abstractions;
// using System.Net;

public class HttpApiTests
{
    private readonly HttpApi httpApi;
    private readonly Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();

    private Mock<IUserUseCase> userUseCase = new Mock<IUserUseCase>();

    private Mock<ILogger<HttpApi>> logger = new Mock<ILogger<HttpApi>>();

    private readonly UserDTO user = new UserDTO
    {
        Id = "1",
        FirstName = "Test",
        LastName = "Test",
        Email = "email@email.com",
        Password = "password"
    };

    public HttpApiTests()
    {
        userUseCase = new Mock<IUserUseCase>();
        userUseCase.Setup(x => x.CreateUser(It.IsAny<UserDTO>()))
            .ReturnsAsync(user);
        userUseCase.Setup(x => x.GetAllUsers())
            .ReturnsAsync(new List<UserDTO> { user });

        httpContextAccessorMock.Setup(x => x.HttpContext.RequestServices.GetService(typeof(IUserUseCase)))
            .Returns(userUseCase.Object);

        httpApi = new HttpApi(httpContextAccessorMock.Object, logger.Object);
    }

    [Fact]
    public async Task Should_Return_When_Create_User()
    {
        // Arrange
        var request = CreateMockRequest(user);
        var logger = Mock.Of<ILogger>();

        // Act
        var response = await httpApi.Create(request.Object, logger);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Should_Return_When_Get_All_Users()
    {
        // Arrange
        var request = CreateMockRequest(user);
        var logger = Mock.Of<ILogger>();

        // Act
        var response = await httpApi.GetAll(request.Object, logger);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        (response as OkObjectResult).Value.Should().BeEquivalentTo(new List<UserDTO> { user });
    }

    [Fact]
    public async Task Should_Return_When_Get_User_By_Id()
    {
        // Arrange
        var request = CreateMockRequest(user);
        var logger = Mock.Of<ILogger>();

        userUseCase.Setup(x => x.GetUserById(It.IsAny<string>()))
                    .ReturnsAsync(user);

        // Act
        var response = await httpApi.GetById(request.Object, logger, "1");

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        (response as OkObjectResult).Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task Should_Return_When_Update_User()
    {
        // Arrange
        var request = CreateMockRequest(user);
        var logger = Mock.Of<ILogger>();

        userUseCase.Setup(x => x.UpdateUser(It.IsAny<UserDTO>()))
                    .ReturnsAsync(user);

        // Act
        var response = await httpApi.Update(request.Object, logger);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        (response as OkObjectResult).Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task Should_Return_When_Delete_User()
    {
        // Arrange
        var request = CreateMockRequest(user);
        var logger = Mock.Of<ILogger>();

        userUseCase.Setup(x => x.DeleteUser(It.IsAny<string>()));

        // Act
        var response = await httpApi.Delete(request.Object, logger, "1");

        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    private Mock<HttpRequest> CreateMockRequest(object body)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        var json = JsonConvert.SerializeObject(body, Formatting.Indented);

        sw.Write(json);
        sw.Flush();

        ms.Position = 0;

        var mockRequest = new Mock<HttpRequest>();

        var query = new QueryCollection(new Dictionary<string, StringValues> { { "q", new StringValues("test") } });

        mockRequest.Setup(x => x.Query).Returns(() => query);

        mockRequest.Setup(x => x.Body).Returns(ms);

        return mockRequest;
    }
}