using FirstCrudApi.Data;
using FirstCrudApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstCrudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    /// <summary> Wszystkie rezerwacje lub przefiltrowane (data, status, sala). </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Reservation>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Reservation>> PobierzWszystkie(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        IEnumerable<Reservation> wynik = MagazynDanych.Rezerwacje;

        if (date.HasValue)
            wynik = wynik.Where(r => r.Date == date.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            wynik = wynik.Where(r =>
                string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));
        }

        if (roomId.HasValue)
            wynik = wynik.Where(r => r.RoomId == roomId.Value);

        return Ok(wynik.ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Reservation> PobierzPoId([FromRoute] int id)
    {
        var rezerwacja = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (rezerwacja is null)
            return NotFound($"Nie znaleziono rezerwacji o identyfikatorze {id}.");
        return Ok(rezerwacja);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Reservation> Utworz([FromBody] Reservation rezerwacja)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == rezerwacja.RoomId);
        if (sala is null)
            return NotFound($"Nie znaleziono sali o identyfikatorze {rezerwacja.RoomId}.");

        if (!sala.IsActive)
        {
            return BadRequest("Nie można utworzyć rezerwacji dla sali nieaktywnej.");
        }

        if (MagazynDanych.CzyNakladanieCzasowe(
                rezerwacja.RoomId,
                rezerwacja.Date,
                rezerwacja.StartTime,
                rezerwacja.EndTime))
        {
            return Conflict(
                "Rezerwacja koliduje czasowo z inną rezerwacją tej samej sali w wybranym dniu.");
        }

        rezerwacja.Id = MagazynDanych.NastepneIdRezerwacji();
        MagazynDanych.Rezerwacje.Add(rezerwacja);
        return CreatedAtAction(nameof(PobierzPoId), new { id = rezerwacja.Id }, rezerwacja);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Reservation> Aktualizuj([FromRoute] int id, [FromBody] Reservation rezerwacja)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var istniejaca = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (istniejaca is null)
            return NotFound($"Nie znaleziono rezerwacji o identyfikatorze {id}.");

        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == rezerwacja.RoomId);
        if (sala is null)
            return NotFound($"Nie znaleziono sali o identyfikatorze {rezerwacja.RoomId}.");

        if (!sala.IsActive)
            return BadRequest("Nie można przypisać rezerwacji do sali nieaktywnej.");

        if (MagazynDanych.CzyNakladanieCzasowe(
                rezerwacja.RoomId,
                rezerwacja.Date,
                rezerwacja.StartTime,
                rezerwacja.EndTime,
                id))
        {
            return Conflict(
                "Rezerwacja koliduje czasowo z inną rezerwacją tej samej sali w wybranym dniu.");
        }

        rezerwacja.Id = id;
        var indeks = MagazynDanych.Rezerwacje.IndexOf(istniejaca);
        MagazynDanych.Rezerwacje[indeks] = rezerwacja;
        return Ok(rezerwacja);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Usun([FromRoute] int id)
    {
        var rezerwacja = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (rezerwacja is null)
            return NotFound($"Nie znaleziono rezerwacji o identyfikatorze {id}.");

        MagazynDanych.Rezerwacje.Remove(rezerwacja);
        return NoContent();
    }
}
