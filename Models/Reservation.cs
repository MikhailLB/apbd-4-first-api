using System.ComponentModel.DataAnnotations;

namespace FirstCrudApi.Models;

/// <summary> Rezerwacja sali na warsztat lub konsultacje. </summary>
public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Identyfikator sali musi być liczbą większą od zera.")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko organizatora jest wymagane.")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Temat rezerwacji jest wymagany.")]
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Data rezerwacji w formacie YYYY-MM-DD.
    /// [Required] nie działa na struct, więc walidacja daty odbywa się przez IValidatableObject.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary> Godzina rozpoczęcia w formacie HH:mm:ss. </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary> Godzina zakończenia w formacie HH:mm:ss. Musi być późniejsza niż StartTime. </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary> Status rezerwacji: planned, confirmed lub cancelled. </summary>
    [Required(ErrorMessage = "Status rezerwacji jest wymagany.")]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date == default)
        {
            yield return new ValidationResult(
                "Data rezerwacji jest wymagana.",
                new[] { nameof(Date) });
            yield break;
        }

        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.",
                new[] { nameof(EndTime) });
        }
    }
}
