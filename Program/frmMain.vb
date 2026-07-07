Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Linq

Public Class frmMain
    Inherits Form

    Private ReadOnly clrBackground As Color = ColorTranslator.FromHtml("#FBEFEF")
    Private ReadOnly clrCard As Color = ColorTranslator.FromHtml("#FFE2E2")
    Private ReadOnly clrAccent As Color = ColorTranslator.FromHtml("#F5CBCB")
    Private ReadOnly clrPrimary As Color = ColorTranslator.FromHtml("#C5B3D3")
    Private ReadOnly clrTextDark As Color = ColorTranslator.FromHtml("#5C4568")

    Private pnlNav As Panel
    Private pnlContent As Panel

    Private pnlHome As Panel
    Private pnlReserve As Panel
    Private pnlReservations As Panel
    Private pnlAvailability As Panel
    Private pnlAbout As Panel
    Private dgvReservations As DataGridView
    Private dgvAvailability As DataGridView

    Private txtRoomNumber As TextBox
    Private cmbCapacity As ComboBox
    Private cmbStatus As ComboBox

    Private btnAddRoom As Button
    Private btnDeleteRoom As Button

    Private cmbRoom As ComboBox
    Private txtStudentId As TextBox
    Private txtStudentName As TextBox
    Private txtCourse As TextBox
    Private txtPurpose As TextBox
    Private cmbTimeSlot As ComboBox
    Private cmbOccupants As ComboBox
    Private dtpDate As DateTimePicker

    Private navButtons As New List(Of Button)

    Private WithEvents tmrExpiry As New Timer With {.Interval = 60000}

    Public Sub New()
        InitializeComponent()
        ShowSection(pnlHome, Nothing)

        AddHandler tmrExpiry.Tick, AddressOf CheckExpiredReservations
        tmrExpiry.Start()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Library Study Room Reservation System"
        Me.ClientSize = New Size(1200, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = clrBackground
        Me.MinimumSize = New Size(1000, 600)

        BuildContentPanel()
        BuildNavPanel()

        Me.Controls.Add(pnlContent)
        Me.Controls.Add(pnlNav)
    End Sub

    Private Sub BuildNavPanel()
        pnlNav = New Panel With {
            .Dock = DockStyle.Left,
            .Width = 230,
            .BackColor = clrPrimary
        }

        Dim lblLogo As New Label With {
            .Text = "MAIN MENU",
            .Font = New Font("Segoe UI", 13, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Top,
            .Height = 70
        }

        Dim pnlButtons As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 300
        }

        Dim btnHome = CreateNavButton("Home")
        Dim btnReserve = CreateNavButton("Reserve Study Room")
        Dim btnReservations = CreateNavButton("View Reservations")
        Dim btnAvailability = CreateNavButton("Manage Room")

        pnlButtons.Controls.AddRange({btnHome, btnReserve, btnReservations, btnAvailability})

        AddHandler btnHome.Click, AddressOf GoToHome
        AddHandler btnReserve.Click, AddressOf GoToReserve
        AddHandler btnReservations.Click, AddressOf GoToReservations
        AddHandler btnAvailability.Click, AddressOf GoToAvailability

        navButtons.AddRange({btnHome, btnReserve, btnReservations, btnAvailability})

        Dim btnLogout As New Button With {
            .Text = "Exit Application",
            .Dock = DockStyle.Bottom,
            .Height = 50,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = ColorTranslator.FromHtml("#B39FC4"),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        btnLogout.FlatAppearance.BorderSize = 0
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        pnlNav.Controls.Add(pnlButtons)
        pnlNav.Controls.Add(btnLogout)
        pnlNav.Controls.Add(lblLogo)
    End Sub

    Private Function CreateNavButton(text As String) As Button
        Dim btn As New Button With {
            .Text = text,
            .Dock = DockStyle.Top,
            .Height = 55,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = clrPrimary,
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(20, 0, 0, 0),
            .Cursor = Cursors.Hand
        }
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#B39FC4")
        Return btn
    End Function

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Exit the application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Application.Exit()
        End If
    End Sub

    Private Sub GoToHome()
        ShowSection(pnlHome, navButtons(0))
    End Sub

    Private Sub GoToReserve()
        ShowSection(pnlReserve, navButtons(1))
    End Sub

    Private Sub GoToReservations()
        CheckExpiredReservations()
        ShowSection(pnlReservations, navButtons(2))
    End Sub

    Private Sub GoToAvailability()
        CheckExpiredReservations()
        ShowSection(pnlAvailability, navButtons(3))
    End Sub

    Private Sub ShowSection(panelToShow As Panel, activeButton As Button)
        For Each p As Panel In New Panel() {pnlHome, pnlReserve, pnlReservations, pnlAvailability}
            p.Visible = (p Is panelToShow)
        Next

        For Each b As Button In navButtons
            b.BackColor = If(b Is activeButton, ColorTranslator.FromHtml("#B39FC4"), clrPrimary)
        Next
    End Sub

    Private Sub BuildContentPanel()
        pnlContent = New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = clrBackground,
            .Padding = New Padding(30)
        }

        BuildHomePanel()
        BuildReservePanel()
        BuildReservationsPanel()
        BuildAvailabilityPanel()

        pnlContent.Controls.AddRange({pnlHome, pnlReserve, pnlReservations, pnlAvailability, pnlAbout})
    End Sub

    Private Sub BuildHomePanel()
        pnlHome = New Panel With {.Dock = DockStyle.Fill, .BackColor = clrBackground}

        Dim lblWelcome As New Label With {
            .Text = "Welcome to Library Study Room Reservation System!",
            .Font = New Font("Segoe UI", 20, FontStyle.Bold),
            .ForeColor = clrTextDark,
            .AutoSize = True,
            .Location = New Point(30, 30)
        }

        Dim lblSub As New Label With {
            .Text = "Choose an option below to get started.",
            .Font = New Font("Segoe UI", 10),
            .ForeColor = Color.Gray,
            .AutoSize = True,
            .Location = New Point(30, 70)
        }

        Dim cardReserve = CreateDashboardCard("Reserve Study Room", "Reserve an available study room.", New Point(30, 120), AddressOf GoToReserve)
        Dim cardMyRes = CreateDashboardCard("View Reservations", "View and manage your reservations.", New Point(280, 120), AddressOf GoToReservations)
        Dim cardAvail = CreateDashboardCard("Manage Rooms", "Add new rooms or remove existing rooms from the system.", New Point(530, 120), AddressOf GoToAvailability)

        Dim lblDate As New Label With {
            .Text = "Today is " & DateTime.Now.ToString("MMMM d, yyyy"),
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.Gray,
            .AutoSize = True,
            .Location = New Point(30, 320)
        }

        pnlHome.Controls.AddRange({lblWelcome, lblSub, cardReserve, cardMyRes, cardAvail, lblDate})
    End Sub

    Private Function CreateDashboardCard(title As String, description As String, location As Point, onClick As Action) As Panel
        Dim card As New Panel With {
            .BackColor = clrCard,
            .Location = location,
            .Size = New Size(220, 160),
            .Cursor = Cursors.Hand
        }

        Dim lblTitle As New Label With {
            .Text = title,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = clrTextDark,
            .AutoSize = False,
            .Size = New Size(190, 40),
            .Location = New Point(15, 70),
            .TextAlign = ContentAlignment.TopLeft,
            .Cursor = Cursors.Hand
        }

        Dim lblDesc As New Label With {
            .Text = description,
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.DimGray,
            .AutoSize = False,
            .Size = New Size(190, 45),
            .Location = New Point(15, 105),
            .Cursor = Cursors.Hand
        }

        card.Controls.AddRange({lblTitle, lblDesc})

        AddHandler card.Click, Sub() onClick()
        AddHandler lblTitle.Click, Sub() onClick()
        AddHandler lblDesc.Click, Sub() onClick()

        Return card
    End Function

    Private Sub BuildReservePanel()
        pnlReserve = New Panel With {.Dock = DockStyle.Fill, .BackColor = clrBackground, .Visible = False}

        Dim lblHeader As New Label With {
            .Text = "RESERVE STUDY ROOM",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = clrTextDark,
            .AutoSize = True,
            .Location = New Point(30, 25)
        }

        Dim y As Integer = 80
        Dim colX1 As Integer = 30
        Dim colX2 As Integer = 460

        Dim lblStudentId As New Label With {.Text = "Student ID:", .Location = New Point(colX1, y), .AutoSize = True}
        txtStudentId = New TextBox With {.Location = New Point(colX1, y + 22), .Size = New Size(380, 26)}

        Dim lblTimeSlot As New Label With {.Text = "Time Slot:", .Location = New Point(colX2, y), .AutoSize = True}
        cmbTimeSlot = New ComboBox With {.Location = New Point(colX2, y + 22), .Size = New Size(280, 26), .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbTimeSlot.Items.AddRange({"08:00 AM - 10:00 AM", "10:00 AM - 12:00 PM", "01:00 PM - 03:00 PM", "02:00 PM - 04:00 PM", "04:00 PM - 06:00 PM"})

        y += 60
        Dim lblStudentName As New Label With {.Text = "Student Name:", .Location = New Point(colX1, y), .AutoSize = True}
        txtStudentName = New TextBox With {.Location = New Point(colX1, y + 22), .Size = New Size(380, 26)}

        Dim lblOccupants As New Label With {.Text = "No. of Occupants:", .Location = New Point(colX2, y), .AutoSize = True}
        cmbOccupants = New ComboBox With {.Location = New Point(colX2, y + 22), .Size = New Size(280, 26), .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbOccupants.Items.AddRange({"1", "2", "3", "4", "5", "6", "7", "8"})

        y += 60
        Dim lblCourse As New Label With {.Text = "Course:", .Location = New Point(colX1, y), .AutoSize = True}
        txtCourse = New TextBox With {.Location = New Point(colX1, y + 22), .Size = New Size(380, 26)}

        Dim lblRoom As New Label With {.Text = "Room Number:", .Location = New Point(colX2, y), .AutoSize = True}
        cmbRoom = New ComboBox With {
            .Location = New Point(colX2, y + 22),
            .Size = New Size(280, 26),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        y += 60
        Dim lblDate As New Label With {.Text = "Date:", .Location = New Point(colX1, y), .AutoSize = True}
        dtpDate = New DateTimePicker With {.Location = New Point(colX1, y + 22), .Size = New Size(380, 26), .Format = DateTimePickerFormat.Short}

        Dim lblPurpose As New Label With {.Text = "Purpose of Study:", .Location = New Point(colX2, y), .AutoSize = True}
        txtPurpose = New TextBox With {.Location = New Point(colX2, y + 22), .Size = New Size(280, 70), .Multiline = True}

        Dim btnClear As New Button With {
            .Text = "CLEAR",
            .Location = New Point(colX1, 480),
            .Size = New Size(150, 42),
            .BackColor = clrCard,
            .ForeColor = clrTextDark,
            .FlatStyle = FlatStyle.Flat
        }
        btnClear.FlatAppearance.BorderColor = clrAccent

        Dim btnReserveRoom As New Button With {
            .Text = "RESERVE ROOM",
            .Location = New Point(colX1 + 160, 480),
            .Size = New Size(180, 42),
            .BackColor = clrPrimary,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9, FontStyle.Bold)
        }
        btnReserveRoom.FlatAppearance.BorderSize = 0

        Dim lblNote As New Label With {
            .Text = "Note: Please arrive on time. Late arrivals may be subject to cancellation.",
            .Font = New Font("Segoe UI", 8, FontStyle.Italic),
            .ForeColor = Color.Firebrick,
            .AutoSize = True,
            .Location = New Point(colX1, 535)
        }

        AddHandler btnClear.Click, AddressOf btnClear_Click
        AddHandler btnReserveRoom.Click, AddressOf btnReserveRoom_Click

        pnlReserve.Controls.AddRange({lblHeader, lblStudentId, txtStudentId, lblTimeSlot, cmbTimeSlot,
                                       lblStudentName, txtStudentName, lblOccupants, cmbOccupants,
                                       lblCourse, txtCourse, lblRoom, cmbRoom,
                                       lblDate, dtpDate, lblPurpose, txtPurpose,
                                       btnClear, btnReserveRoom, lblNote})
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs)
        txtStudentId.Clear()
        txtStudentName.Clear()
        txtCourse.Clear()
        cmbTimeSlot.SelectedIndex = -1
        cmbOccupants.SelectedIndex = -1
        cmbRoom.SelectedIndex = -1
        txtPurpose.Clear()
        dtpDate.Value = DateTime.Now
    End Sub

    Private Sub btnReserveRoom_Click(sender As Object, e As EventArgs)
        CheckExpiredReservations()

        If txtStudentId.Text.Trim() = "" OrElse cmbRoom.SelectedIndex = -1 OrElse cmbTimeSlot.SelectedIndex = -1 Then
            MessageBox.Show("Please complete the required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim roomNumber As String = cmbRoom.SelectedItem.ToString().Split("("c)(0).Trim()
        Dim room = DataStore.Rooms.FirstOrDefault(Function(r) r.RoomNumber = roomNumber)

        If room Is Nothing OrElse room.Status <> "Available" Then
            MessageBox.Show("The selected room is no longer available. Please choose another room.", "Room Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            LoadRoomDropdown()
            Return
        End If

        Dim reservation As New Reservation With {
            .ReservationId = "R" & DateTime.Now.ToString("yyMMddHHmmssfff"),
            .StudentId = txtStudentId.Text.Trim(),
            .StudentName = txtStudentName.Text.Trim(),
            .Course = txtCourse.Text.Trim(),
            .RoomNumber = roomNumber,
            .Date = dtpDate.Value.Date,
            .Time = cmbTimeSlot.Text,
            .Occupants = If(cmbOccupants.SelectedIndex >= 0, Integer.Parse(cmbOccupants.Text), 0),
            .Purpose = txtPurpose.Text.Trim(),
            .Status = "Reserved"
        }

        DataStore.Reservations.Add(reservation)
        OccupyRoom(room, reservation)
        DataStore.Save()

        LoadReservations()
        LoadRooms()
        LoadRoomDropdown()

        MessageBox.Show("Your study room has been reserved.", "Reservation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

        btnClear_Click(sender, e)
    End Sub

    Private Sub BuildReservationsPanel()
        pnlReservations = New Panel With {.Dock = DockStyle.Fill, .BackColor = clrBackground, .Visible = False}

        Dim lblHeader As New Label With {
            .Text = "RESERVATIONS",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = clrTextDark,
            .AutoSize = True,
            .Location = New Point(30, 25)
        }

        dgvReservations = New DataGridView With {
            .Location = New Point(30, 75),
            .Size = New Size(880, 400),
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .RowHeadersVisible = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        dgvReservations.ColumnHeadersDefaultCellStyle.BackColor = clrPrimary
        dgvReservations.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvReservations.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgvReservations.EnableHeadersVisualStyles = False
        dgvReservations.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        dgvReservations.DefaultCellStyle.SelectionBackColor = clrAccent
        dgvReservations.DefaultCellStyle.SelectionForeColor = clrTextDark

        dgvReservations.Columns.Add("ReservationId", "Reservation ID")
        dgvReservations.Columns.Add("RoomNumber", "Room Number")
        dgvReservations.Columns.Add("Date", "Date")
        dgvReservations.Columns.Add("Time", "Time")
        dgvReservations.Columns.Add("Occupants", "No. of Occupants")
        dgvReservations.Columns.Add("Status", "Status")

        Dim lblTotal As New Label With {
            .Text = "Total Reservations: " & dgvReservations.Rows.Count,
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.Gray,
            .AutoSize = True,
            .Location = New Point(30, 490)
        }

        Dim btnCancel As New Button With {
            .Text = "CANCEL RESERVATION",
            .Location = New Point(650, 485),
            .Size = New Size(160, 34),
            .BackColor = clrCard,
            .ForeColor = Color.Firebrick,
            .FlatStyle = FlatStyle.Flat
        }
        btnCancel.FlatAppearance.BorderColor = clrAccent

        Dim btnRefresh As New Button With {
            .Text = "REFRESH",
            .Location = New Point(820, 485),
            .Size = New Size(90, 34),
            .BackColor = clrPrimary,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        btnRefresh.FlatAppearance.BorderSize = 0

        AddHandler btnCancel.Click, AddressOf btnCancel_Click
        AddHandler btnRefresh.Click, AddressOf btnReservationsRefresh_Click

        pnlReservations.Controls.AddRange({lblHeader, dgvReservations, lblTotal, btnCancel, btnRefresh})
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        If dgvReservations.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a reservation to cancel.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim reservationId As String = dgvReservations.SelectedRows(0).Cells("ReservationId").Value.ToString()
        Dim reservation = DataStore.Reservations.FirstOrDefault(Function(r) r.ReservationId = reservationId)

        If reservation Is Nothing Then Return

        reservation.Status = "Cancelled"
        FreeRoom(reservation.RoomNumber)
        DataStore.Save()

        LoadReservations()
        LoadRooms()
        LoadRoomDropdown()
    End Sub

    Private Sub btnReservationsRefresh_Click(sender As Object, e As EventArgs)
        CheckExpiredReservations()
        RemoveCancelledReservations()
        LoadReservations()
    End Sub

    Private Sub BuildAvailabilityPanel()
        pnlAvailability = New Panel With {.Dock = DockStyle.Fill, .BackColor = clrBackground, .Visible = False}

        Dim lblHeader As New Label With {
            .Text = "ROOM AVAILABILITY",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = clrTextDark,
            .AutoSize = True,
            .Location = New Point(30, 25)
        }

        dgvAvailability = New DataGridView With {
            .Location = New Point(30, 75),
            .Size = New Size(880, 400),
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .RowHeadersVisible = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        dgvAvailability.ColumnHeadersDefaultCellStyle.BackColor = clrPrimary
        dgvAvailability.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvAvailability.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgvAvailability.EnableHeadersVisualStyles = False
        dgvAvailability.DefaultCellStyle.Font = New Font("Segoe UI", 9)

        dgvAvailability.Columns.Add("RoomNumber", "Room Number")
        dgvAvailability.Columns.Add("Capacity", "Capacity")
        dgvAvailability.Columns.Add("Status", "Status")
        dgvAvailability.Columns.Add("CurrentReserved", "Current Reserved")
        dgvAvailability.Columns.Add("NextAvailable", "Next Available")

        Dim lblRoom As New Label With {
            .Text = "Room Number:",
            .Location = New Point(30, 500),
            .AutoSize = True
        }

        txtRoomNumber = New TextBox With {
            .Location = New Point(130, 497),
            .Size = New Size(120, 26)
        }

        Dim lblCapacity As New Label With {
            .Text = "Capacity:",
            .Location = New Point(270, 500),
            .AutoSize = True
        }

        cmbCapacity = New ComboBox With {
            .Location = New Point(340, 497),
            .Size = New Size(80, 26),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        cmbCapacity.Items.AddRange({"1", "2", "3", "4", "5", "6", "7", "8"})

        Dim lblStatus As New Label With {
            .Text = "Status:",
            .Location = New Point(440, 500),
            .AutoSize = True
        }

        cmbStatus = New ComboBox With {
            .Location = New Point(500, 497),
            .Size = New Size(120, 26),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        cmbStatus.Items.AddRange({"Available", "Occupied"})

        btnAddRoom = New Button With {
            .Text = "ADD ROOM",
            .Location = New Point(650, 495),
            .Size = New Size(110, 32),
            .BackColor = clrPrimary,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        btnAddRoom.FlatAppearance.BorderSize = 0

        btnDeleteRoom = New Button With {
            .Text = "DELETE",
            .Location = New Point(770, 495),
            .Size = New Size(110, 32),
            .BackColor = clrCard,
            .ForeColor = Color.Firebrick,
            .FlatStyle = FlatStyle.Flat
        }

        Dim btnRefresh As New Button With {
            .Text = "REFRESH",
            .Location = New Point(890, 495),
            .Size = New Size(90, 32),
            .BackColor = clrPrimary,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        btnRefresh.FlatAppearance.BorderSize = 0

        AddHandler btnAddRoom.Click, AddressOf btnAddRoom_Click
        AddHandler btnDeleteRoom.Click, AddressOf btnDeleteRoom_Click
        AddHandler btnRefresh.Click, AddressOf btnAvailabilityRefresh_Click

        pnlAvailability.Controls.AddRange({
            lblHeader,
            dgvAvailability,
            lblRoom,
            txtRoomNumber,
            lblCapacity,
            cmbCapacity,
            lblStatus,
            cmbStatus,
            btnAddRoom,
            btnDeleteRoom,
            btnRefresh
        })
    End Sub

    Private Sub btnAvailabilityRefresh_Click(sender As Object, e As EventArgs)
        CheckExpiredReservations()
        LoadRooms()
    End Sub

    Private Sub LoadRooms()
        dgvAvailability.Rows.Clear()

        For Each room In DataStore.Rooms
            dgvAvailability.Rows.Add(room.RoomNumber, room.Capacity, room.Status, room.CurrentReserved, room.NextAvailable)
        Next
    End Sub

    Private Sub LoadReservations()
        dgvReservations.Rows.Clear()

        For Each reservation In DataStore.Reservations
            dgvReservations.Rows.Add(reservation.ReservationId, reservation.RoomNumber, reservation.Date.ToShortDateString(),
                                      reservation.Time, reservation.Occupants, reservation.Status)
        Next
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        DataStore.Initialize()
        CheckExpiredReservations()

        LoadRooms()
        LoadReservations()
        LoadRoomDropdown()
    End Sub

    Private Sub btnAddRoom_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtRoomNumber.Text) Then
            MessageBox.Show("Enter a room number.")
            Return
        End If

        Dim room As New Room With {
            .RoomNumber = txtRoomNumber.Text,
            .Capacity = Integer.Parse(cmbCapacity.Text),
            .Status = cmbStatus.Text,
            .CurrentReserved = "",
            .NextAvailable = "Now"
        }

        DataStore.Rooms.Add(room)
        DataStore.Save()
        LoadRooms()
        LoadRoomDropdown()

        txtRoomNumber.Clear()
        cmbCapacity.SelectedIndex = -1
        cmbStatus.SelectedIndex = -1
    End Sub

    Private Sub btnDeleteRoom_Click(sender As Object, e As EventArgs)
        If dgvAvailability.SelectedRows.Count = 0 Then
            MessageBox.Show("Select a room first.")
            Return
        End If

        Dim roomNumber As String = dgvAvailability.SelectedRows(0).Cells("RoomNumber").Value.ToString()
        Dim room = DataStore.Rooms.FirstOrDefault(Function(r) r.RoomNumber = roomNumber)

        If room Is Nothing Then Return

        DataStore.Rooms.Remove(room)
        DataStore.Save()

        LoadRooms()
        LoadRoomDropdown()
    End Sub

    Private Sub LoadRoomDropdown()
        cmbRoom.Items.Clear()

        For Each room In DataStore.Rooms
            If room.Status = "Available" Then
                cmbRoom.Items.Add(room.RoomNumber & " (Capacity: " & room.Capacity & ")")
            End If
        Next
    End Sub

    Private Sub OccupyRoom(room As Room, reservation As Reservation)
        room.Status = "Occupied"
        room.CurrentReserved = reservation.ReservationId
        room.NextAvailable = GetSlotEndTimeText(reservation.Time)
    End Sub

    Private Sub FreeRoom(roomNumber As String)
        Dim room = DataStore.Rooms.FirstOrDefault(Function(r) r.RoomNumber = roomNumber)

        If room Is Nothing Then Return

        room.Status = "Available"
        room.CurrentReserved = ""
        room.NextAvailable = "Now"
    End Sub

    Private Sub CheckExpiredReservations()
        Dim now As DateTime = DateTime.Now
        Dim expired = DataStore.Reservations.Where(Function(r) r.Status = "Reserved" AndAlso GetSlotEndDateTime(r) <= now).ToList()

        If expired.Count = 0 Then Return

        For Each reservation In expired
            FreeRoom(reservation.RoomNumber)
        Next

        DataStore.Reservations.RemoveAll(Function(r) expired.Contains(r))
        DataStore.Save()

        If dgvReservations IsNot Nothing Then LoadReservations()
        If dgvAvailability IsNot Nothing Then LoadRooms()
        If cmbRoom IsNot Nothing Then LoadRoomDropdown()
    End Sub

    Private Sub RemoveCancelledReservations()
        If DataStore.Reservations.RemoveAll(Function(r) r.Status = "Cancelled") > 0 Then
            DataStore.Save()
        End If
    End Sub

    Private Function ParseSlotEndTime(timeSlot As String) As String
        If String.IsNullOrWhiteSpace(timeSlot) Then Return ""

        Dim parts = timeSlot.Split("-"c)
        Return If(parts.Length < 2, "", parts(1).Trim())
    End Function

    Private Function GetSlotEndDateTime(reservation As Reservation) As DateTime
        Dim endText = ParseSlotEndTime(reservation.Time)
        Dim parsedTime As DateTime

        If endText <> "" AndAlso DateTime.TryParse(endText, parsedTime) Then
            Return reservation.Date.Date.Add(parsedTime.TimeOfDay)
        End If

        Return reservation.Date
    End Function

    Private Function GetSlotEndTimeText(timeSlot As String) As String
        Dim endText = ParseSlotEndTime(timeSlot)
        Return If(endText = "", "Unknown", endText)
    End Function

End Class