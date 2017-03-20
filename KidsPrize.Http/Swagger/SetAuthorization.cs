using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KidsPrize.Http.Swagger
{
    public class SetAuthorization : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Description = "Bearer {Token}",
                Required = true,
                Type = "string"
            });
        }
    }
}