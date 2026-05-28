using System.ComponentModel.DataAnnotations;

namespace alankaar_api.DTOs;

public class CreateClientRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string FlatNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public DateOnly? HandoverDate { get; set; }
}

public class UpdateClientRequest : CreateClientRequest
{
}
