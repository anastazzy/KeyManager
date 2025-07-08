using KeyManager.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KeyManager.API.Utils;

public class ServiceAuthorizationActionFilter : IAsyncActionFilter
{
    private readonly IServiceApiKeyManager _manager;

    public ServiceAuthorizationActionFilter(IServiceApiKeyManager manager)
    {
        _manager = manager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (await IsAuthorizationSuccessAsync(context)) await next();
    }

    private async Task<bool> IsAuthorizationSuccessAsync(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request.Headers;
        var notAuthorizedResult = new ObjectResult(null) { StatusCode = 401 };

        if (!(request.TryGetValue(AuthConst.AuthFieldName, out var header)
              || !request.TryGetValue(AuthConst.AuthFieldName.ToLower(), out header))
            || !Guid.TryParse(header, out var apikey)
            || !await _manager.IsRegisteredAsync(apikey))
        {
            context.Result = notAuthorizedResult;
            return false;
        }

        return true;
    }
}