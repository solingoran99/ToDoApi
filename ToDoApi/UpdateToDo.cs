using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoApi.Models;
using ToDoApi.Services;

namespace ToDoApi
{
    public class UpdateToDo
    {
        [Function("UpdateToDo")]
        [OpenApiOperation("UpdateToDo", tags: new[] { "ToDo" }, Summary = "Update an existing task")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID of the ToDo item to update")]
        [OpenApiRequestBody("application/json", typeof(ToDoItem), Required = true, Description = "Updated ToDo item")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ToDoItem), Description = "The updated item")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todo/{id}")] HttpRequestData req,
            string id)
        {
            // Read and deserialize request body
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedItem = JsonSerializer.Deserialize<ToDoItem>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            updatedItem.Id = id;

            // Attempt to update item in the database
            var db = new MongoDbService();
            var result = await db.UpdateAsync(id, updatedItem);

            // Return 200 OK with updated item, or 404 if not found
            var response = req.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            if (result)
                await response.WriteAsJsonAsync(updatedItem);

            return response;
        }
    }
}
