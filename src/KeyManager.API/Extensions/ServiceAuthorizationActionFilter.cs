using KeyManager.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KeyManager.API.Extensions;

public class ServiceAuthorizationActionFilter : IAsyncActionFilter
{
    private readonly IServiceApiKeyManager _manager;

    public ServiceAuthorizationActionFilter(IServiceApiKeyManager manager)
    {
        _manager = manager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (await IsAuthorizationSuccessAsync(context, next)) await next();
    }

    public async Task<bool> IsAuthorizationSuccessAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request.Headers;
        var notAuthorizedResult = new ObjectResult(null) { StatusCode = 401 };
        if (!(request.TryGetValue("Authorization", out var header)
              || !request.TryGetValue("authorization", out header))
            || !Guid.TryParse(header, out var apikey) 
            || !await _manager.IsRegisteredAsync(apikey))
        {
            context.Result = notAuthorizedResult;
            return false;
        }

        return true;
    }
}