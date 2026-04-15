using FirstCrudApi.Models;

namespace FirstCrudApi.Data;

/// <summary> Statyczne listy danych w pamięci aplikacji (bez bazy SQL). </summary>
public static class MagazynDanych
{
    public static List<Room> Sale { get; } = new();

    public static List<Reservation> Rezerwacje { get; } = new();

    /// <summary> Wywołaj raz przy starcie aplikacji — wstawia przykładowe rekordy. </summary>
    public static void Inicjalizuj()
    {
        Sale.Clear();
        Rezerwacje.Clear();

        Sale.AddRange(new[]
        {
            new Room
            {
                Id = 1,
                Name = "Laboratorium 101",
                BuildingCode = "A",
                Floor = 1,
                Capacity = 30,
                HasProjector = true,
                IsActive = true
            },
            new Room
            {
                Id = 2,
                Name = "Lab 204",
                BuildingCode = "B",
                Floor = 2,
                Capacity = 24,
                HasProjector = true,
                IsActive = true
            },
            new Room
            {
                Id = 3,
                Name = "Sala konferencyjna Główna",
                BuildingCode = "A",
                Floor = 0,
                Capacity = 50,
                HasProjector = false,
                IsActive = true
            },
            new Room
            {
                Id = 4,
                Name = "Pokój szkoleniowy Mały",
                BuildingCode = "C",
                Floor = 1,
                Capacity = 10,
                HasProjector = false,
                IsActive = false
            },
            new Room
            {
                Id = 5,
                Name = "Audytorium",
                BuildingCode = "B",
                Floor = 0,
                Capacity = 100,
                HasProjector = true,
                IsActive = true
            }
        });

        Rezerwacje.AddRange(new[]
        {
            new Reservation
            {
                Id = 1,
                RoomId = 2,
                OrganizerName = "Anna Kowalska",
                Topic = "Warsztaty z HTTP i REST",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(10, 0, 0),
                EndTime = new TimeOnly(12, 30, 0),
                Status = "confirmed"
            },
            new Reservation
            {
                Id = 2,
                RoomId = 1,
                OrganizerName = "Jan Nowak",
                Topic = "Wprowadzenie do C#",
                Date = new DateOnly(2026, 5, 12),
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(11, 0, 0),
                Status = "planned"
            },
            new Reservation
            {
                Id = 3,
                RoomId = 3,
                OrganizerName = "Maria Wiśniewska",
                Topic = "Spotkanie zespołu",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(14, 0, 0),
                EndTime = new TimeOnly(15, 0, 0),
                Status = "confirmed"
            },
            new Reservation
            {
                Id = 4,
                RoomId = 5,
                OrganizerName = "Piotr Zieliński",
                Topic = "Prezentacja projektu",
                Date = new DateOnly(2026, 6, 1),
                StartTime = new TimeOnly(16, 0, 0),
                EndTime = new TimeOnly(17, 30, 0),
                Status = "planned"
            },
            new Reservation
            {
                Id = 5,
                RoomId = 2,
                OrganizerName = "Ewa Mazur",
                Topic = "Konsultacje indywidualne",
                Date = new DateOnly(2026, 5, 15),
                StartTime = new TimeOnly(13, 0, 0),
                EndTime = new TimeOnly(14, 0, 0),
                Status = "cancelled"
            },
            new Reservation
            {
                Id = 6,
                RoomId = 1,
                OrganizerName = "Tomasz Lewandowski",
                Topic = "Szkolenie BHP",
                Date = new DateOnly(2026, 5, 20),
                StartTime = new TimeOnly(8, 30, 0),
                EndTime = new TimeOnly(9, 30, 0),
                Status = "confirmed"
            }
        });
    }

    public static int NastepneIdSali() => Sale.Count == 0 ? 1 : Sale.Max(s => s.Id) + 1;

    public static int NastepneIdRezerwacji() =>
        Rezerwacje.Count == 0 ? 1 : Rezerwacje.Max(r => r.Id) + 1;

    /// <summary> Czy dwa przedziały czasu nakładają się (ten sam dzień, ta sama sala). Kolejne rezerwacji „back-to-back” są dozwolone. </summary>
    public static bool CzyNakladanieCzasowe(int roomId, DateOnly date, TimeOnly start, TimeOnly end, int? pominIdRezerwacji = null)
    {
        return Rezerwacje.Any(r =>
            r.RoomId == roomId
            && r.Date == date
            && (!pominIdRezerwacji.HasValue || r.Id != pominIdRezerwacji.Value)
            && start < r.EndTime
            && r.StartTime < end);
    }
}
