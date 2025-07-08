using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KeyManager.API.Extensions;

public class SwaggerHeaderOptions : IOperationFilter
{
    private const string Description = "Need to set the service api key";
    private const string AuthFieldName = "Authorization";
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var isServiceKeyAuthorization = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .FirstOrDefault(x => x.GetType() == typeof(ServiceFilterAttribute<ServiceAuthorizationActionFilter>)) is not null;
        if (!isServiceKeyAuthorization) return;

        (operation.Parameters ??= new List<OpenApiParameter>()).Add(new OpenApiParameter
        {
            Name = AuthFieldName,
            In = ParameterLocation.Header,
            Description = Description,
            Required = false,
            AllowEmptyValue = true,
            Style = ParameterStyle.Simple,
        });
    }
}