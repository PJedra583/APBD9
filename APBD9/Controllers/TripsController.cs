using APBD9.Data;
using APBD9.Models;
using APBD9.Models.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ApbdContext _context;

    public TripsController(ApbdContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _context.Trips.OrderByDescending(e => e.DateFrom)
            .ToListAsync();
        return Ok(trips);
    }
    
    [HttpDelete]
    [Route("/api/clients/{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var client = await _context.Clients.FindAsync(idClient);
        if (client == null)
        {
            return NotFound("No such ID");
        }
        var clientTrips = await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == idClient);
        if (clientTrips)
        { 
            return BadRequest("Cannot delete client because he has trips");
        }
        else
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Ok("Successfully deleted client");
        }
    }

    [HttpPost]
    [Route("/api/trips/{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, AssignClientDTO dto)
    {
        var isPesel = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        if (isPesel != null)
        {
            return BadRequest("Client with given PESEL already exists");
        }
        // Jeżeli istnieje klient z danym peselem to zwracamy badRequest. Jak w takim razie porównać 
        // czy klient z danym peselem jest zapisany na wycieczke?
        var assignment = await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == dto.id && ct.IdTrip == idTrip);

        if (assignment)
        {
            return BadRequest("Client is already assigned");
        }
        
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
        {
            return NotFound("Trip not found");
        }
        
        if (trip.DateTo <= DateTime.Now)
        {
            return BadRequest("Trip already ended, cannot assign client");
        }
        
        
        DateTime currentTime = DateTime.Now;

        var clientTrip = new ClientTrip
        {
            IdClient = dto.id,
            IdTrip = idTrip,
            RegisteredAt = currentTime,
            PaymentDate = dto.GetPaymentDateAsDateTime()
        };
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok("Client assigned to trip successfully");
        
    }
}