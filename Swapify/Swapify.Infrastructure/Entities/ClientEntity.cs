using System.ComponentModel.DataAnnotations;

namespace Swapify.Infrastructure.Entities;

public class ClientEntity : IAuditable
{
    public int Id { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecretHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }
}