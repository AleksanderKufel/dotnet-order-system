using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Application
{
    public record CreateOrderRequest(
        [Required, EmailAddress, StringLength(254)] string CustomerEmail,
        [Range(0.01, 1_000_000)] decimal Amount);
}
