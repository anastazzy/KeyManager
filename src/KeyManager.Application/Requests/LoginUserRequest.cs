using System.ComponentModel.DataAnnotations;

namespace KeyManager.Application.Requests;

public record LoginUserRequest([Required]string Email, [Required]string Password);