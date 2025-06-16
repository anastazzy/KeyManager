using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult> RegisterAsync([FromBody] LoginUserRequest request)
    {
        var newUserId = await _userService.RegisterAsync(request);
        return newUserId == Guid.Empty ? BadRequest() : Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginUserRequest request)
    {
        var token = await _userService.LoginAsync(request);
        return string.IsNullOrEmpty(token) ? NotFound() : Ok(token);
    }
    
    [Authorize]
    [HttpGet("test")]
    public async Task<ActionResult> GetAsync([FromHeader]string token)
    {
        return  Ok();
    }
}