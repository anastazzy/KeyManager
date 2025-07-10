using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
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
    public async Task RegisterAsync([FromBody] LoginUserRequest request)
    {
        await _userService.RegisterAsync(request, $"{Url.ActionLink()}/confirmation");
    }

    [HttpPost("login")]
    public async Task<string> LoginAsync([FromBody] LoginUserRequest request)
    {
        return await _userService.LoginAsync(request);
    }

    [HttpGet("confirmation")]
    public async Task<string> ConfirmEmailAsync([FromQuery] string token)
    {
        return await _userService.ConfirmEmailAsync(token);
    }
}