using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Application.Interfaces.API;
using Domain;

namespace Azure.Function
{
    public class HttpApi
    {
        private readonly IUserUseCase _userUseCase;

        private readonly ILogger<HttpApi> _logger;

        public HttpApi(IHttpContextAccessor httpContextAccessor, ILogger<HttpApi> logger)
        {
            _userUseCase = httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IUserUseCase)) as IUserUseCase;
            _logger = logger;
        }

        // [FunctionName("HttpApi")]
        // public async Task<IActionResult> Run(
        //     [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        //     ILogger log)
        // {
        //     log.LogInformation("C# HTTP trigger function processed a request.");

        //     string name = req.Query["name"];

        //     string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //     dynamic data = JsonConvert.DeserializeObject(requestBody);
        //     name = name ?? data?.name;

        //     string responseMessage = string.IsNullOrEmpty(name)
        //         ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //         : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //     return new OkObjectResult(responseMessage);
        // }

        [FunctionName("GetAll")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAll")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request - GetAll");

            var users = await _userUseCase.GetAllUsers();

            return new OkObjectResult(users);
        }

        [FunctionName("GetById")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetById/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request - GetById");

            var user = await _userUseCase.GetUserById(id);

            return new OkObjectResult(user);
        }

        [FunctionName("Create")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Create")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request - Create");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<UserDTO>(requestBody);

            var user = await _userUseCase.CreateUser(data);

            return new OkObjectResult(user);
        }

        [FunctionName("Update")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Update")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request - Update");

            // try
            // {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<UserDTO>(requestBody);

            var user = await _userUseCase.UpdateUser(data);

            return new OkObjectResult(user);
            // }
            // catch (System.Exception ex)
            // {
            //     // return new BadRequestResult(ex.Message);
            // }
        }

        [FunctionName("Delete")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Delete/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request - Delete");

            var result = await _userUseCase.DeleteUser(id);

            return new OkObjectResult(result);
        }

    }
}
