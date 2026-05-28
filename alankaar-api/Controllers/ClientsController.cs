using alankaar_api.Data;
using alankaar_api.DTOs;
using alankaar_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alankaar_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(AlankaarDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetClients(
        [FromQuery] string? search,
        [FromQuery] string? status)
    {
        var isAdmin = IsAdminRequest();
        var clientsQuery = context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            clientsQuery = clientsQuery.Where(client =>
                client.Name.Contains(term)
                || client.FlatNumber.Contains(term)
                || client.Location.Contains(term)
                || client.PhoneNumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status) && ClientStatuses.IsValid(status))
        {
            var normalizedStatus = ClientStatuses.Normalize(status);
            clientsQuery = clientsQuery.Where(client => client.Status == normalizedStatus);
        }

        var clients = await clientsQuery
            .OrderBy(client => client.HandoverDate)
            .ThenBy(client => client.Name)
            .ToListAsync();

        return Ok(clients.Select(client => ToResponse(client, isAdmin)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientResponse>> GetClient(int id)
    {
        var client = await context.Clients
            .AsNoTracking()
            .SingleOrDefaultAsync(client => client.Id == id);

        if (client is null)
        {
            return NotFound(new { message = "Client not found." });
        }

        return Ok(ToResponse(client, IsAdminRequest()));
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> CreateClient(CreateClientRequest request)
    {
        if (!IsAdminRequest())
        {
            return AdminRequired();
        }

        var validationError = ValidateClientRequest(request);
        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var client = new Client
        {
            CreatedAtUtc = DateTime.UtcNow
        };
        ApplyClientRequest(client, request);

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        return Ok(ToResponse(client, isAdmin: true));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClientResponse>> UpdateClient(int id, UpdateClientRequest request)
    {
        if (!IsAdminRequest())
        {
            return AdminRequired();
        }

        var validationError = ValidateClientRequest(request);
        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var client = await context.Clients.SingleOrDefaultAsync(client => client.Id == id);
        if (client is null)
        {
            return NotFound(new { message = "Client not found." });
        }

        ApplyClientRequest(client, request);
        await context.SaveChangesAsync();

        return Ok(ToResponse(client, isAdmin: true));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        if (!IsAdminRequest())
        {
            return AdminRequired();
        }

        var client = await context.Clients.SingleOrDefaultAsync(client => client.Id == id);
        if (client is null)
        {
            return NotFound(new { message = "Client not found." });
        }

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool IsAdminRequest()
    {
        return UserRoles.IsAdmin(Request.Headers["X-User-Role"].FirstOrDefault());
    }

    private ObjectResult AdminRequired()
    {
        return StatusCode(StatusCodes.Status403Forbidden, new { message = "Admin access is required." });
    }

    private static string? ValidateClientRequest(CreateClientRequest request)
    {
        if (!ClientStatuses.IsValid(request.Status))
        {
            return "Please select a valid client status.";
        }

        if (request.HandoverDate is null)
        {
            return "Handover date is required.";
        }

        return null;
    }

    private static void ApplyClientRequest(Client client, CreateClientRequest request)
    {
        client.Name = request.Name.Trim();
        client.FlatNumber = request.FlatNumber.Trim();
        client.Location = request.Location.Trim();
        client.PhoneNumber = request.PhoneNumber.Trim();
        client.Status = ClientStatuses.Normalize(request.Status);
        client.HandoverDate = request.HandoverDate!.Value;
    }

    private static ClientResponse ToResponse(Client client, bool isAdmin)
    {
        return new ClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            FlatNumber = client.FlatNumber,
            Location = client.Location,
            PhoneNumber = isAdmin ? client.PhoneNumber : null,
            Status = client.Status,
            HandoverDate = client.HandoverDate
        };
    }
}
