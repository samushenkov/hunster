
namespace HunsterService.Swagger
{
    static class SwaggerExtensions
    {
        public static void UseSwaggerByRoute(this IApplicationBuilder builder, string swaggerPrefix = "api/swagger")
        {
            builder.UseSwagger((options) =>
            {
                options.RouteTemplate = $"/{swaggerPrefix}/{{documentName}}/swagger.json";
            });

            builder.UseSwaggerUI((options) =>
            {
                options.RoutePrefix = swaggerPrefix;
            });
        }
    }
}
