using System.ComponentModel.DataAnnotations;

namespace KeyManager.Application.Requests;

public record ConfirmTransactionRequest([Required]Guid ApiKeiId, [Required]long TransactionId);