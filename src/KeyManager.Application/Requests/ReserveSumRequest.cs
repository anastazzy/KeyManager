using System.ComponentModel.DataAnnotations;

namespace KeyManager.Application.Requests;

public record ReserveSumRequest([Required]Guid ApiKeiId, [Required]decimal Sum);