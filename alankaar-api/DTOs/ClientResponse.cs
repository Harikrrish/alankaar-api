namespace alankaar_api.DTOs;

public class ClientResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FlatNumber { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateOnly HandoverDate { get; set; }
}
