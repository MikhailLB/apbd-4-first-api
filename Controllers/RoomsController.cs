using FirstCrudApi.Data;
using FirstCrudApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstCrudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Room>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Room>> PobierzWszystkie(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        IEnumerable<Room> wynik = MagazynDanych.Sale;

        if (minCapacity.HasValue)
            wynik = wynik.Where(s => s.Capacity >= minCapacity.Value);

        if (hasProjector.HasValue)
            wynik = wynik.Where(s => s.HasProjector == hasProjector.Value);

        if (activeOnly == true)
            wynik = wynik.Where(s => s.IsActive);

        return Ok(wynik.ToList());
    }

    [HttpGet("building/{buildingCode}")]
    [ProducesResponseType(typeof(IEnumerable<Room>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Room>> PobierzWedlugBudynku([FromRoute] string buildingCode)
    {
        var lista = MagazynDanych.Sale
            .Where(s => string.Equals(s.BuildingCode, buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Room> PobierzPoId([FromRoute] int id)
    {
        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == id);
        if (sala is null)
            return NotFound(new { blad = $"Nie znaleziono sali o identyfikatorze {id}." });

        return Ok(sala);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Room), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Room> Utworz([FromBody] Room sala)
    {
        sala.Id = MagazynDanych.NastepneIdSali();
        MagazynDanych.Sale.Add(sala);

        return CreatedAtAction(nameof(PobierzPoId), new { id = sala.Id }, sala);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Room> Aktualizuj([FromRoute] int id, [FromBody] Room sala)
    {
        var istniejaca = MagazynDanych.Sale.FirstOrDefault(s => s.Id == id);
        if (istniejaca is null)
            return NotFound(new { blad = $"Nie znaleziono sali o identyfikatorze {id}." });

        sala.Id = id;
        var indeks = MagazynDanych.Sale.IndexOf(istniejaca);
        MagazynDanych.Sale[indeks] = sala;

        return Ok(sala);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Usun([FromRoute] int id)
    {
        var sala = MagazynDanych.Sale.FirstOrDefault(s => s.Id == id);
        if (sala is null)
            return NotFound(new { blad = $"Nie znaleziono sali o identyfikatorze {id}." });

        bool maRezerwacje = MagazynDanych.Rezerwacje.Any(r => r.RoomId == id);
        if (maRezerwacje)
        {
            return Conflict(new
            {
                blad = "Nie można usunąć sali, dla której istnieją rezerwacje.",
                wskazowka = "Usuń najpierw powiązane rezerwacje, a następnie spróbuj ponownie."
            });
        }

        MagazynDanych.Sale.Remove(sala);
        return NoContent();
    }
}
