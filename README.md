# CA.Azure.Function.Demo

vscode extension
memfiredb

## Tech Stack

- [x] Azure function
- [x] .NET 7
- [x] DDD
- [x] Mediator (CQRS)
- [x] Serilog
- [x] Fluent Validation
- [x] Swagger
- [x] Health Check
- [x] RestSharp

```powershell
# create solution
$ dotnet new sln -o ca-azure-function-demo

# switch to sln path
$ cd ca-azure-function-demo


# create domain project
$ dotnet new classlib -o Domain

# create application project
$ dotnet new classlib -o Application

# create infrastructure project
$ dotnet new classlib -o Infrastructure

# create azure function project
$ func init Azure.Function --dotnet

# add projects to sln
$ dotnet sln add (ls -r \*\*\*.csproj)
or
$ dotnet sln add .\Domain\Domain.csproj

$ dotnet sln add .\Application\Application.csproj

$ dotnet sln add .\Infrastructure\Infrastructure.csproj

$ dotnet sln add .\Azure.Function\Azure_Function.csproj

# setup reference
$ dotnet add .\Application\ reference .\Domain\

$ dotnet add .\Azure.Function\ reference .\Application\

$ dotnet add .\Infrastructure\ reference .\Application\

# build sln
$ dotnet build

# switch to azure function project
$ cd Azure.Function

# add support for dependency injection
$ dotnet add .\Azure.Function\ package Microsoft.Azure.Functions.Extensions
$ dotnet add .\Azure.Function\ package Microsoft.NET.Sdk.Functions
$ dotnet add .\Azure.Function\ package Microsoft.Extensions.DependencyInjection

# create a http function
func new --name HttpApi --template "HTTP trigger" --authlevel "anonymous"

# run function
$ func start

# cosmos db emulator, download and install
https://aka.ms/cosmosdb-emulator

# enable allow insecure localhost
chrome://flags/#allow-insecure-localhost

# swagger
http://localhost:7071/api/swagger/ui

# deploy function
$ func azure functionapp publish <APP_NAME>
```
