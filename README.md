# API Centrum Szkoleniowego

Aplikacja ASP.NET Core Web API do zarządzania salami dydaktycznymi i rezerwacjami.
Dane przechowywane są wyłącznie w pamięci aplikacji (bez bazy danych).

## Uruchomienie

```bash
dotnet run
```

Aplikacja startuje pod adresem: **http://localhost:5000**

Po uruchomieniu Swagger UI jest dostępny od razu pod adresem głównym:
**http://localhost:5000**

---

## Struktura projektu

```
firstcrudapi/
├── Controllers/
│   ├── RoomsController.cs        # Zarządzanie salami
│   └── ReservationsController.cs # Zarządzanie rezerwacjami
├── Data/
│   └── MagazynDanych.cs          # Statyczne listy + dane startowe
├── Models/
│   ├── Room.cs                   # Model sali
│   └── Reservation.cs            # Model rezerwacji
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.json
└── firstcrudapi.csproj
```

---

## Endpointy — Sale (`/api/rooms`)

| Metoda | Endpoint | Opis | Kody HTTP |
|--------|----------|------|-----------|
| GET | `/api/rooms` | Wszystkie sale | 200 |
| GET | `/api/rooms?minCapacity=20&hasProjector=true&activeOnly=true` | Filtrowanie przez query string | 200 |
| GET | `/api/rooms/{id}` | Pojedyncza sala po ID | 200, 404 |
| GET | `/api/rooms/building/{buildingCode}` | Sale z wybranego budynku | 200 |
| POST | `/api/rooms` | Dodanie nowej sali | 201, 400 |
| PUT | `/api/rooms/{id}` | Pełna aktualizacja sali | 200, 400, 404 |
| DELETE | `/api/rooms/{id}` | Usunięcie sali | 204, 404, 409 |

### Przykładowe body POST/PUT dla sali

```json
{
  "name": "Lab 204",
  "buildingCode": "B",
  "floor": 2,
  "capacity": 24,
  "hasProjector": true,
  "isActive": true
}
```

---

## Endpointy — Rezerwacje (`/api/reservations`)

| Metoda | Endpoint | Opis | Kody HTTP |
|--------|----------|------|-----------|
| GET | `/api/reservations` | Wszystkie rezerwacje | 200 |
| GET | `/api/reservations?date=2026-05-10&status=confirmed&roomId=2` | Filtrowanie przez query string | 200 |
| GET | `/api/reservations/{id}` | Pojedyncza rezerwacja po ID | 200, 404 |
| POST | `/api/reservations` | Utworzenie rezerwacji | 201, 400, 404, 409 |
| PUT | `/api/reservations/{id}` | Pełna aktualizacja rezerwacji | 200, 400, 404, 409 |
| DELETE | `/api/reservations/{id}` | Usunięcie rezerwacji | 204, 404 |

### Przykładowe body POST/PUT dla rezerwacji

```json
{
  "roomId": 2,
  "organizerName": "Anna Kowalska",
  "topic": "Warsztaty z HTTP i REST",
  "date": "2026-05-10",
  "startTime": "10:00:00",
  "endTime": "12:30:00",
  "status": "confirmed"
}
```

---

## Reguły biznesowe

- Nie można zarezerwować sali, która **nie istnieje** → 404 Not Found
- Nie można zarezerwować sali **nieaktywnej** (`isActive: false`) → 400 Bad Request
- Dwie rezerwacje tej samej sali **nie mogą nakładać się** czasowo tego samego dnia → 409 Conflict
- Nie można usunąć sali, która ma **powiązane rezerwacje** → 409 Conflict

---

## Walidacja danych wejściowych

Walidacja odbywa się automatycznie dzięki atrybutowi `[ApiController]`.
Przy błędnych danych zwracany jest kod **400 Bad Request** z opisem błędu.

| Pole | Walidacja |
|------|-----------|
| `Name`, `BuildingCode` | Wymagane, nie może być puste |
| `OrganizerName`, `Topic` | Wymagane, nie może być puste |
| `Capacity` | Musi być większa od zera |
| `Status` | Wymagany |
| `EndTime` | Musi być późniejsza niż `StartTime` |

---

## Dane startowe

Aplikacja uruchamia się z **5 salami** i **6 rezerwacjami** gotowymi do testowania.

| ID | Sala | Budynek | Pojemność | Projektor | Aktywna |
|----|------|---------|-----------|-----------|---------|
| 1 | Laboratorium 101 | A | 30 | tak | tak |
| 2 | Lab 204 | B | 24 | tak | tak |
| 3 | Sala konferencyjna Główna | A | 50 | nie | tak |
| 4 | Pokój szkoleniowy Mały | C | 10 | nie | **nie** |
| 5 | Audytorium | B | 100 | tak | tak |

---

## Testowanie w Postmanie

Minimalny zestaw scenariuszy:

1. `GET /api/rooms` — pobranie wszystkich sal
2. `GET /api/rooms/1` — pobranie sali o ID 1
3. `GET /api/rooms/building/B` — sale z budynku B (code z trasy)
4. `GET /api/rooms?minCapacity=30&hasProjector=true&activeOnly=true` — filtrowanie
5. `POST /api/rooms` — dodanie nowej sali (body JSON)
6. `PUT /api/rooms/1` — aktualizacja sali
7. `POST /api/reservations` — poprawna rezerwacja
8. `POST /api/reservations` z nakładającym się przedziałem → **409 Conflict**
9. `DELETE /api/reservations/5` — usunięcie rezerwacji
10. `GET /api/rooms/999` → **404 Not Found**
