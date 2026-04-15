using System.ComponentModel.DataAnnotations;

namespace FirstCrudApi.Models;

public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Identyfikator sali musi być liczbą większą od zera.")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko organizatora jest wymagane.")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Temat rezerwacji jest wymagany.")]
    public string Topic { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

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
