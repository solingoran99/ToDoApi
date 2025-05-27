using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoApi
{
    public class DeleteToDo
    {
        [Function("DeleteToDo")]
        [OpenApiOperation("DeleteToDo", tags: new[] { "ToDo" }, Summary = "Delete a task by ID")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID of the ToDo item to delete")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent, Description = "Item successfully deleted")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Description = "Item not found")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todo/{id}")] HttpRequestData req,
            string id)

        {   // Attempt to delete the item from the database
            var db = new MongoDbService();
            var deleted = await db.DeleteAsync(id);

            // Return 204 if deleted, otherwise 404
            var response = req.CreateResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
            return response;
        }
    }
}
