using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yei3.PersonalEvaluation.Web.Host.Startup
{
    public class ToFileSwaggerFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apifileget")
            {
                operation.Produces = new[] { "application/octet-stream" };
                operation.Responses["200"].Schema = new Schema { Type = "file", Description = "Download file" };
            }
        }
    }
}