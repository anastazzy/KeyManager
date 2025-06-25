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
        var result = await _userService.RegisterAsync(request, $"{Url.ActionLink()}/confirmation");
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginUserRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("confirmation")]
    public async Task<ActionResult> ConfirmEmailAsync([FromQuery] string token)
    {
        var result = await _userService.ConfirmEmailAsync(token);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("test")]
    public async Task<ActionResult> GetAsync()
    {
        return Ok();
    }
}