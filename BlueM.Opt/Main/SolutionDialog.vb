'BlueM.Opt
'Copyright (C) BlueM Dev Group
'Website: <https://www.bluemodel.org>
'
'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
'GNU General Public License for more details.
'
'You should have received a copy of the GNU General Public License
'along with this program. If not, see <https://www.gnu.org/licenses/>.
'
''' <summary>
''' Zeigt die ausgewählten Lösungen an
''' </summary>
Partial Public Class SolutionDialog
    Inherits System.Windows.Forms.Form

    'Das Problem
    Private mProblem As BlueM.Opt.Common.Problem

    'Properties
    '**********
    Private ReadOnly Property checkedSolutions() As Integer()
        Get
            Dim solution_IDs As Integer()
            ReDim solution_IDs(-1)
            For Each row As DataGridViewRow In Me.DataGridView1.Rows
                If (row.Cells(0).Value = "True") Then
                    ReDim Preserve solution_IDs(solution_IDs.Length)
                    solution_IDs(solution_IDs.Length - 1) = row.HeaderCell.Value
                End If
            Next
            Return solution_IDs
        End Get
    End Property

    'Events
    '******
    Public Event SelectedSolutionsSimulationRequested(ByVal checkedSolutions() As Integer)
    Public Event SelectedSolutionsChanged(ByVal selectedSolutions() As Integer)
    Public Event SelectedSolutionsCleared()


    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="prob">Das Problem</param>
    Public Sub New(ByRef prob As BlueM.Opt.Common.Problem)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        'Problem speichern
        Me.mProblem = prob

        'Spalten einrichten
        '==================

        Dim column As DataGridViewColumn
        Dim cellstyle As DataGridViewCellStyle

        'Allgemein
        '---------
        cellstyle = Me.DataGridView1.DefaultCellStyle.Clone()
        cellstyle.Format = "G5"

        'Ziele
        '--------
        For Each feature As Common.ObjectiveFunction In Me.mProblem.List_ObjectiveFunctions
            column = New DataGridViewTextBoxColumn()
            If (feature.isPrimObjective) Then
                cellstyle.BackColor = Color.LightGreen
                column.HeaderText = feature.Description & " (*)"
                column.HeaderCell.ToolTipText = "Primary objective function"
            Else
                cellstyle.BackColor = Color.LightBlue
                column.HeaderText = feature.Description
                column.HeaderCell.ToolTipText = "Secondary objective function"
            End If
            column.ReadOnly = True
            column.Name = feature.Description
            column.DefaultCellStyle = cellstyle.Clone()
            Me.DataGridView1.Columns.Add(column)
        Next

        'Constraints
        '-----------
        cellstyle.BackColor = Color.LightCoral

        For Each Constraint As Common.Constraintfunction In Me.mProblem.List_Constraintfunctions
            column = New DataGridViewTextBoxColumn()
            column.ReadOnly = True
            column.HeaderText = Constraint.Bezeichnung
            column.HeaderCell.ToolTipText = "Constraint"
            column.Name = Constraint.Bezeichnung
            column.DefaultCellStyle = cellstyle.Clone()
            Me.DataGridView1.Columns.Add(column)
        Next

        'OptParameter
        '------------
        cellstyle.BackColor = Color.LightGray

        For Each OptPara As BlueM.Opt.Common.OptParameter In Me.mProblem.List_OptParameter
            column = New DataGridViewTextBoxColumn()
            column.ReadOnly = True
            column.HeaderText = OptPara.Bezeichnung
            column.HeaderCell.ToolTipText = "Optimization parameter"
            column.Name = OptPara.Bezeichnung
            column.DefaultCellStyle = cellstyle.Clone()
            Me.DataGridView1.Columns.Add(column)
        Next

        'Handler einrichten
        '==================
        AddHandler Me.SelectedSolutionsSimulationRequested, AddressOf Form1.simulateSelectedSolutions
        AddHandler Me.SelectedSolutionsCleared, AddressOf Form1.clearSelectedSolutions
        AddHandler Me.SelectedSolutionsChanged, AddressOf Form1.updateSelectedSolutions

    End Sub

    ''' <summary>
    ''' Ein Individuum zur Lösungsauswahl hinzufügen
    ''' </summary>
    ''' <param name="ind">das ausgewählte Individuum</param>
    Public Sub addSolution(ByVal ind As Common.Individuum)

        Dim i As Integer
        Dim cellvalues() As Object
        Dim row As DataGridViewRow

        'Daten zusammenstellen
        '---------------------
        ReDim cellvalues(Me.DataGridView1.ColumnCount - 1)

        cellvalues(0) = True

        i = 1

        'Ziele
        For Each featurevalue As Double In ind.Objectives
            cellvalues(i) = featurevalue * Me.mProblem.List_ObjectiveFunctions(i - 1).Direction
            i += 1
        Next

        'Constraints
        For Each constraintvalue As Double In ind.Constraints
            cellvalues(i) = constraintvalue
            i += 1
        Next

        'OptParameter
        For Each optpara As Double In ind.OptParameter_RWerte
            cellvalues(i) = optpara
            i += 1
        Next

        'Zeile erstellen
        row = New DataGridViewRow()
        row.CreateCells(Me.DataGridView1, cellvalues)
        row.HeaderCell.Value = ind.ID.ToString()

        'Zeile hinzufügen
        Me.DataGridView1.Rows.Add(row)

        'Spalten anpassen
        Call Me.DataGridView1.AutoResizeColumns()

    End Sub

    'Automatisches speichern von Zellenänderungen
    '********************************************
    Private Sub dataGridView1_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DataGridView1.CurrentCellDirtyStateChanged

        If (DataGridView1.IsCurrentCellDirty) Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    'Nicht angehakte Lösungen aus Lösungsauwahl entfernen
    '****************************************************
    Private Sub ToolStripButton_unselect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton_unselect.Click

        'Zeilen löschen
        For Each row As DataGridViewRow In Me.DataGridView1.Rows
            If (row.Cells(0).Value = "False") Then
                Me.DataGridView1.Rows.Remove(row)
            End If
        Next

        'Event auslösen
        RaiseEvent SelectedSolutionsChanged(Me.checkedSolutions)

    End Sub

    'Lösungsauswahl zurücksetzen
    '***************************
    Private Sub ToolStripButton_Clear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton_Clear.Click

        'Alle Zeilen löschen
        Call Me.DataGridView1.Rows.Clear()

        'Lösungsdialog verstecken
        Call Me.Hide()

        'Event auslösen
        RaiseEvent SelectedSolutionsCleared()

    End Sub

    'Ausgewählte Lösungen simulieren
    '*******************************
    Private Sub ToolStripButton_Simulate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton_Wave.Click

        'Cursor
        Cursor = Cursors.WaitCursor

        'Event auslösen
        RaiseEvent SelectedSolutionsSimulationRequested(Me.checkedSolutions)

        'Cursor
        Cursor = Cursors.Default

    End Sub

    'Form resize
    '***********
    Private Sub SolutionDialog_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize

        Me.DataGridView1.Width = Me.ClientSize.Width
        Me.DataGridView1.Height = Me.ClientSize.Height - 25       'Minus Höhe der Toolbar

    End Sub

    'Form schließen
    '**************
    Private Sub SolutionDialog_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        'verhindern, dass das Formular komplett gelöscht wird
        e.Cancel = True

        'Formular verstecken
        Call Me.Hide()

    End Sub

End Class
