using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Threading.Tasks;

namespace ToDoApi
{
    public class SwaggerUI
    {
        [Function("SwaggerUI")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger")] HttpRequestData req)
        {
            // Dynamically construct local OpenAPI URL
            var host = req.Url.Host;
            var port = req.Url.Port;
            var scheme = req.Url.Scheme;
            var swaggerJsonUrl = $"{scheme}://{host}:{port}/api/openapi/v1.json";

            // Redirect to Swagger UI with the local OpenAPI JSON
            var response = req.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Add("Location", $"https://petstore.swagger.io/?url={swaggerJsonUrl}");
            return response;
        }
    }
}
