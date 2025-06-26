using System.Security.Claims;
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
    public async Task<ActionResult> GetApiKeysByUserAsync()
    {
        var id = GetUserId();
        var result = await _apiKeyService.GetUserKeysAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> AddApiKeyAsync([FromBody] ApiKeyAddRequest request)
    {
        var id = GetUserId();
        var result = await _apiKeyService.AddKeyAsync(id, request.Name);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveApiKeyAsync([FromRoute] Guid id)
    {
        await _apiKeyService.RemoveKeyAsync(id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateNameAsync([FromRoute] Guid id, [FromBody] ApiKeyAddRequest request)
    {
        await _apiKeyService.UpdateNameAsync(id, request.Name);
        return Ok();
    }

    private Guid GetUserId() => Guid.TryParse(
        HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value, out var id)
        ? id
        : Guid.Empty;
}