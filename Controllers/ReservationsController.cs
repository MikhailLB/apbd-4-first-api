using FirstCrudApi.Data;
using FirstCrudApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstCrudApi.Controllers;

/// <summary>
/// Zarządza rezerwacjami sal dydaktycznych.
/// Atrybut [ApiController] automatycznie zwraca 400 Bad Request,
/// gdy dane wejściowe nie przejdą walidacji Data Annotations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    /// <summary>
    /// Zwraca wszystkie rezerwacje lub przefiltrowane wg parametrów query stringu.
    /// GET /api/reservations
    /// GET /api/reservations?date=2026-05-10&amp;status=confirmed&amp;roomId=2
    /// </summary>
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
            wynik = wynik.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));

        if (roomId.HasValue)
            wynik = wynik.Where(r => r.RoomId == roomId.Value);

        return Ok(wynik.ToList());
    }

    /// <summary>
    /// Zwraca pojedynczą rezerwację po identyfikatorze.
    /// GET /api/reservations/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Reservation> PobierzPoId([FromRoute] int id)
    {
        var rezerwacja = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (rezerwacja is null)
            return NotFound(new { blad = $"Nie znaleziono rezerwacji o identyfikatorze {id}." });

        return Ok(rezerwacja);
    }

    /// <summary>
    /// Tworzy nową rezerwację. Dane przesyłane w body żądania jako JSON.
    /// POST /api/reservations
    /// Reguły biznesowe:
    ///   - sala musi istnieć (404),
    ///   - sala musi być aktywna (400),
    ///   - rezerwacja nie może kolidować z inną tego samego dnia (409).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Reservation> Utworz([FromBody] Reservation rezerwacja)
    {
        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == rezerwacja.RoomId);
        if (sala is null)
            return NotFound(new { blad = $"Nie znaleziono sali o identyfikatorze {rezerwacja.RoomId}." });

        if (!sala.IsActive)
            return BadRequest(new { blad = "Nie można utworzyć rezerwacji dla sali nieaktywnej." });

        if (MagazynDanych.CzyNakladanieCzasowe(rezerwacja.RoomId, rezerwacja.Date, rezerwacja.StartTime, rezerwacja.EndTime))
            return Conflict(new { blad = "Rezerwacja koliduje czasowo z inną rezerwacją tej samej sali w wybranym dniu." });

        rezerwacja.Id = MagazynDanych.NastepneIdRezerwacji();
        MagazynDanych.Rezerwacje.Add(rezerwacja);

        return CreatedAtAction(nameof(PobierzPoId), new { id = rezerwacja.Id }, rezerwacja);
    }

    /// <summary>
    /// Aktualizuje istniejącą rezerwację (pełna aktualizacja, nie częściowa).
    /// PUT /api/reservations/1
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Reservation> Aktualizuj([FromRoute] int id, [FromBody] Reservation rezerwacja)
    {
        var istniejaca = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (istniejaca is null)
            return NotFound(new { blad = $"Nie znaleziono rezerwacji o identyfikatorze {id}." });

        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == rezerwacja.RoomId);
        if (sala is null)
            return NotFound(new { blad = $"Nie znaleziono sali o identyfikatorze {rezerwacja.RoomId}." });

        if (!sala.IsActive)
            return BadRequest(new { blad = "Nie można przypisać rezerwacji do sali nieaktywnej." });

        if (MagazynDanych.CzyNakladanieCzasowe(rezerwacja.RoomId, rezerwacja.Date, rezerwacja.StartTime, rezerwacja.EndTime, id))
            return Conflict(new { blad = "Rezerwacja koliduje czasowo z inną rezerwacją tej samej sali w wybranym dniu." });

        rezerwacja.Id = id;
        var indeks = MagazynDanych.Rezerwacje.IndexOf(istniejaca);
        MagazynDanych.Rezerwacje[indeks] = rezerwacja;

        return Ok(rezerwacja);
    }

    /// <summary>
    /// Usuwa rezerwację.
    /// DELETE /api/reservations/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Usun([FromRoute] int id)
    {
        var rezerwacja = MagazynDanych.Rezerwacje.FirstOrDefault(r => r.Id == id);
        if (rezerwacja is null)
            return NotFound(new { blad = $"Nie znaleziono rezerwacji o identyfikatorze {id}." });

        MagazynDanych.Rezerwacje.Remove(rezerwacja);
        return NoContent();
    }
}
