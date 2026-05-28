namespace alankaar_api.Models;

public class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FlatNumber { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Status { get; set; } = ClientStatuses.Stage1;

    public DateOnly HandoverDate { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
