using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoApi.Models;
using ToDoApi.Services;
using System.Net;
using MongoDB.Bson;

namespace ToDoApi
{
    public class CreateToDo
    {
        [Function("CreateToDo")]
        [OpenApiOperation("CreateToDo", tags: new[] { "ToDo" }, Summary = "Create a new task")]
        [OpenApiRequestBody("application/json", typeof(ToDoItem), Required = true, Description = "ToDo item to create")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ToDoItem), Description = "The created ToDo item")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todo")] HttpRequestData req)
        {
            // Read and deserialize the request body
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var todo = JsonSerializer.Deserialize<ToDoItem>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Clear invalid ID so MongoDB can auto-generate a valid ObjectId
            if (!string.IsNullOrWhiteSpace(todo?.Id) && !ObjectId.TryParse(todo.Id, out _))
            {
                todo.Id = null;
            }
            // Save to database
            var db = new MongoDbService();
            await db.CreateAsync(todo);

            // Return created item
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(todo);
            return response;
        }
    }
}
