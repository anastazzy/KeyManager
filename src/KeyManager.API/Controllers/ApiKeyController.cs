using System.Security.Claims;
using KeyManager.API.Dtos;
using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpGet]
    public async Task<EnumerableResultContainer> GetApiKeysByUserAsync()
    {
        var id = GetUserId();
        var keys = await _apiKeyService.GetUserKeysAsync(id);

        return new EnumerableResultContainer(keys);
    }

    [HttpPost]
    public async Task AddApiKeyAsync([FromBody] ApiKeyAddRequest request)
    {
        var id = GetUserId();
        await _apiKeyService.AddKeyAsync(id, request.Name);
    }

    [HttpDelete("{id}")]
    public async Task RemoveApiKeyAsync([FromRoute] Guid id)
    {
        await _apiKeyService.RemoveKeyAsync(id);
    }

    [HttpPut("{id}")]
    public async Task UpdateNameAsync([FromRoute] Guid id, [FromBody] ApiKeyAddRequest request)
    {
        await _apiKeyService.UpdateNameAsync(id, request.Name);
    }

    private Guid GetUserId() => Guid.TryParse(
        HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value, out var id)
        ? id
        : Guid.Empty;
}