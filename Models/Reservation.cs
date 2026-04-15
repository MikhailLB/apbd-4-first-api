using System.ComponentModel.DataAnnotations;

namespace FirstCrudApi.Models;

/// <summary> Rezerwacja sali na warsztat lub konsultacje. </summary>
public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Identyfikator sali musi być poprawny.")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko organizatora jest wymagane.")]
    [MinLength(1, ErrorMessage = "Imię i nazwisko organizatora nie może być puste.")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Temat jest wymagany.")]
    [MinLength(1, ErrorMessage = "Temat nie może być pusty.")]
    public string Topic { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data jest wymagana.")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Godzina rozpoczęcia jest wymagana.")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "Godzina zakończenia jest wymagana.")]
    public TimeOnly EndTime { get; set; }

    /// <summary> Np. planned, confirmed, cancelled. </summary>
    [Required(ErrorMessage = "Status jest wymagany.")]
    [MinLength(1, ErrorMessage = "Status nie może być pusty.")]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.",
                new[] { nameof(EndTime) });
        }
    }
}
