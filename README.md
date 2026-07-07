# Library Study Room Reservation System

A Windows Forms (VB.NET) desktop application for reserving library study rooms, tracking reservations, and managing room availability. Data is persisted to a local Excel workbook using ClosedXML — no database server required.

## Project Structure

```
Project/
├── frmMain.vb        Main form: all UI, navigation, and event handlers
├── DataStore.vb       Loads/saves data to LibraryData.xlsx via ClosedXML
├── Room.vb            Room model
├── Reservation.vb     Reservation model
└── Program.vb         Application entry point
```

## Features

### Home
Landing dashboard with three clickable cards that jump to the other sections: Reserve Study Room, View Reservations, and Manage Rooms.

### Reserve Study Room
A form to book a room: Student ID, Student Name, Course, Purpose of Study, number of occupants, date, time slot, and an available room chosen from a dropdown. Only rooms currently marked `Available` appear in the room dropdown.

Submitting the form:
- Validates that Student ID, Room, and Time Slot are filled in.
- Creates a new reservation with status `Reserved`.
- Marks the chosen room as `Occupied` and records when it will free up (the end of the selected time slot).
- Saves everything to the Excel file and refreshes the Reservations and Availability views.
- Clears the form for the next entry.

### View Reservations
A grid of every reservation (ID, room, date, time, occupants, status).

- **Cancel Reservation** — sets the selected reservation's status to `Cancelled` and immediately frees its room back to `Available`.
- **Refresh** — re-checks for any reservations whose time slot has ended (auto-expiring them, see below) and permanently removes any `Cancelled` reservations from the list.

### Manage Rooms (Room Availability)
A grid of every room (number, capacity, status, current reservation, next available time), plus controls to:
- **Add Room** — add a new room with a number, capacity, and status.
- **Delete** — remove the selected room.
- **Refresh** — re-check for expired reservations and reload the grid.

## How It Works

### Data Model
- **Room**: `RoomNumber`, `Capacity`, `Status` (`Available`/`Occupied`), `CurrentReserved` (reservation ID or blank), `NextAvailable` (display text).
- **Reservation**: `ReservationId`, `StudentId`, `StudentName`, `Course`, `RoomNumber`, `Date`, `Time` (slot string, e.g. `"08:00 AM - 10:00 AM"`), `Occupants`, `Purpose`, `Status` (`Reserved`/`Cancelled`/removed once expired).

### Persistence
`DataStore.vb` keeps `Rooms` and `Reservations` in memory as `List(Of Room)` / `List(Of Reservation)`. On startup it loads them from `LibraryData.xlsx` (created automatically if missing, with a `Rooms` and a `Reservations` sheet). Every add, cancel, or delete action calls `DataStore.Save()`, which rewrites the whole workbook from the in-memory lists.

### Automatic Room Freeing (Expiry)
A background `Timer` ticks every 60 seconds and calls `CheckExpiredReservations`, which:
1. Parses the end time out of each active (`Reserved`) reservation's time-slot string.
2. Compares it against the current date/time.
3. For any reservation whose slot has ended: frees its room (`Available`, clears `CurrentReserved`, sets `NextAvailable` to `"Now"`) and removes the reservation entirely.
4. Saves the change and refreshes whichever grids/dropdowns are currently built.

This same check also runs:
- On application startup, so anything that expired while the app was closed is cleaned up immediately.
- Whenever you navigate to View Reservations or Manage Rooms.
- Whenever you press Reserve Room, Refresh (on either grid).

> Note: since this uses a WinForms `Timer`, expiry only progresses while the app is running. Nothing expires in the background if the app is closed — but the startup check catches up as soon as it's reopened.

### Cancelled Reservations
Cancelling a reservation doesn't delete it immediately — it's marked `Cancelled` so it's still visible in the grid. Pressing **Refresh** on the Reservations screen is what actually removes `Cancelled` entries from the list (and from the saved file).

### Navigation
All navigation — the sidebar buttons and the Home dashboard cards — routes through the same four methods (`GoToHome`, `GoToReserve`, `GoToReservations`, `GoToAvailability`), so behavior (like triggering the expiry check) stays consistent no matter which button was clicked.

## Setup

### Requirements
- Windows with .NET (Windows Forms support — .NET Framework or .NET 6+/8 with Windows Desktop workload)
- Visual Studio (Community edition is fine)
- NuGet package: **ClosedXML** (used by `DataStore.vb` to read/write the `.xlsx` file)

### Steps
1. Create a new **Windows Forms App (VB.NET)** project in Visual Studio.
2. Replace/add the five files (`frmMain.vb`, `DataStore.vb`, `Room.vb`, `Reservation.vb`, `Program.vb`) into the project, matching the structure above.
3. Set `frmMain` as the application's startup form (already handled by `Program.vb`, which calls `Application.Run(New frmMain())`).
4. Install ClosedXML via NuGet:
   - Visual Studio: **Tools → NuGet Package Manager → Manage NuGet Packages for Solution** → search `ClosedXML` → Install.
   - Or via Package Manager Console: `Install-Package ClosedXML`
5. Build and run (F5).

### Data File
On first run, the app creates `LibraryData.xlsx` next to the executable (`Application.StartupPath`) with empty `Rooms` and `Reservations` sheets. You can add rooms directly from the **Manage Rooms** screen — there's no need to pre-populate the Excel file by hand, though you can open and edit it directly if you prefer (just make sure the app isn't running at the same time, since it overwrites the whole file on every save).
