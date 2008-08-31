﻿Public Class Individuum_PES
    Inherits Individuum

    Private mOptParas() As OptParameter  '06a Parameterarray für PES

    ''' <summary>
    ''' Die Optimierungsparameter des Individuums
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PES_OptParas() As OptParameter()
        Get
            Return Me.mOptParas
        End Get
        Set(ByVal value As OptParameter())
            Me.mOptParas = value
        End Set
    End Property

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="type">Frei definierbarer String</param>
    ''' <param name="id">Eindeutige Nummer</param>
    Public Sub New(ByVal type As String, ByVal id As Integer)

        'Basisindividuum instanzieren
        Call MyBase.New(type, id)

        'zusätzliche Individuum_PES-Eigenschaften:
        '-----------------------------------------
        Dim i As Integer

        'Parameterarray für PES
        ReDim Me.PES_OptParas(Individuum.mProblem.NumParams - 1)
        For i = 0 To Me.PES_OptParas.GetUpperBound(0)
            Me.PES_OptParas(i) = New OptParameter()
        Next

    End Sub

    ''' <summary>
    ''' Kopiert ein Individuum
    ''' </summary>
    ''' <returns>Individuum</returns>
    Public Overrides Function Clone() As Individuum

        Dim i As Integer

        Clone = New Individuum_PES(Me.mType, Me.mID)

        'Feature-Werte
        Call Array.Copy(Me.Features, Clone.Features, Me.Features.Length)

        'Constraint-Werte
        If (Not Me.Constraints.GetLength(0) = -1) Then
            Array.Copy(Me.Constraints, Clone.Constraints, Me.Constraints.Length)
        End If

        'Kennzeichnung ob Dominiert
        Clone.Dominated = Me.Dominated

        'Nummer der Pareto Front
        Clone.Front = Me.Front

        'Für crowding distance
        Clone.Distance = Me.Distance

        'Array für PES Parameter
        For i = 0 To Me.PES_OptParas.GetUpperBound(0)
            CType(Clone, Individuum_PES).PES_OptParas(i) = Me.PES_OptParas(i).Clone
        Next

        Return Clone

    End Function

    ''' <summary>
    ''' Erzeugt ein neues (leeres) Individuum von der gleichen Klasse
    ''' </summary>
    ''' <returns>Das neue Individuum</returns>
    Public Overrides Function Create(Optional ByVal type As String = "tmp", Optional ByVal id As Integer = 0) As Individuum
        Dim ind As New Individuum_PES(type, id)
        Return ind
    End Function

End Class
