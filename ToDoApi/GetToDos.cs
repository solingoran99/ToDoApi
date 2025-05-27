using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;
using ToDoApi.Models;
using ToDoApi.Services;
using System.Collections.Generic;

namespace ToDoApi
{
    public class GetToDos
    {
        [Function("GetToDos")]
        [OpenApiOperation("GetToDos", tags: new[] { "ToDo" }, Summary = "Get all ToDo items")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ToDoItem>), Description = "List of all ToDo items")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todo")] HttpRequestData req)
        {
            // Retrieve all ToDo items from the database
            var db = new MongoDbService();
            var items = await db.GetAsync();

            // Return 200 OK with the list of items
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }
    }
}
