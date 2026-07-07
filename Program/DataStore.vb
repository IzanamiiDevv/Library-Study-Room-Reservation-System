Imports ClosedXML.Excel
Imports System.IO

Public Module DataStore
    Public Rooms As New List(Of Room)
    Public Reservations As New List(Of Reservation)
    Public ReadOnly FileName As String =
        Path.Combine(Application.StartupPath, "LibraryData.xlsx")
    Public Sub Initialize()
        If File.Exists(FileName) Then
            LoadExcel()
        Else
            CreateExcel()
        End If
    End Sub

    Private Sub CreateExcel()
        Dim wb As New XLWorkbook()

        wb.AddWorksheet("Rooms")
        wb.AddWorksheet("Reservations")

        wb.SaveAs(FileName)
    End Sub

    Private Sub LoadExcel()
        Rooms.Clear()
        Reservations.Clear()

        Using wb As New XLWorkbook(FileName)
            Dim wsRooms = wb.Worksheet("Rooms")

            For Each row In wsRooms.RowsUsed().Skip(1)
                Rooms.Add(New Room With {
                    .RoomNumber = row.Cell(1).GetString(),
                    .Capacity = row.Cell(2).GetValue(Of Integer),
                    .Status = row.Cell(3).GetString(),
                    .CurrentReserved = row.Cell(4).GetString(),
                    .NextAvailable = row.Cell(5).GetString()
                })

            Next

            Dim wsReservations = wb.Worksheet("Reservations")

            For Each row In wsReservations.RowsUsed().Skip(1)
                Reservations.Add(New Reservation With {
                    .ReservationId = row.Cell(1).GetString(),
                    .RoomNumber = row.Cell(2).GetString(),
                    .Date = row.Cell(3).GetDateTime(),
                    .Time = row.Cell(4).GetString(),
                    .Occupants = row.Cell(5).GetValue(Of Integer),
                    .Status = row.Cell(6).GetString()
                })

            Next

        End Using
    End Sub

    Public Sub Save()

        Dim wb As New XLWorkbook()

        Dim roomSheet = wb.AddWorksheet("Rooms")

        roomSheet.Cell(1, 1).Value = "Room Number"
        roomSheet.Cell(1, 2).Value = "Capacity"
        roomSheet.Cell(1, 3).Value = "Status"
        roomSheet.Cell(1, 4).Value = "Current Reserved"
        roomSheet.Cell(1, 5).Value = "Next Available"

        Dim r = 2

        For Each room In Rooms

            roomSheet.Cell(r, 1).Value = room.RoomNumber
            roomSheet.Cell(r, 2).Value = room.Capacity
            roomSheet.Cell(r, 3).Value = room.Status
            roomSheet.Cell(r, 4).Value = room.CurrentReserved
            roomSheet.Cell(r, 5).Value = room.NextAvailable

            r += 1

        Next

        Dim reservationSheet = wb.AddWorksheet("Reservations")

        reservationSheet.Cell(1, 1).Value = "Reservation ID"
        reservationSheet.Cell(1, 2).Value = "Room Number"
        reservationSheet.Cell(1, 3).Value = "Date"
        reservationSheet.Cell(1, 4).Value = "Time"
        reservationSheet.Cell(1, 5).Value = "Occupants"
        reservationSheet.Cell(1, 6).Value = "Status"

        r = 2

        For Each reservation In Reservations

            reservationSheet.Cell(r, 1).Value = reservation.ReservationId
            reservationSheet.Cell(r, 2).Value = reservation.RoomNumber
            reservationSheet.Cell(r, 3).Value = reservation.Date
            reservationSheet.Cell(r, 4).Value = reservation.Time
            reservationSheet.Cell(r, 5).Value = reservation.Occupants
            reservationSheet.Cell(r, 6).Value = reservation.Status

            r += 1

        Next
        wb.SaveAs(FileName)
    End Sub
End Module