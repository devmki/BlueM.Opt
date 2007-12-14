Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports IHWB.EVO.Kern

Public Class EVO_Einstellungen
    Inherits System.Windows.Forms.UserControl

    'Eigenschaften
    '#############

    Private settings As EVO.Kern.PES_Settings      'Sicherung s�mtlicher Einstellungen
    Public isSaved As Boolean = False               'Flag der anzeigt, ob die Einstellungen bereits gesichert wurden

    'Methoden
    '########

    'Konstruktor
    '***********
    Public Sub New()

        ' Dieser Aufruf ist f�r den Windows Form-Designer erforderlich.
        Call Me.InitializeComponent()

        ' F�gen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        'PES_Settings instanzieren
        Me.settings = New EVO.Kern.PES_Settings()
        'Standardm��ig Single-Objective Werte nehmen
        Call Me.settings.setStandard(EVO_MODUS.Single_Objective)
        'Form initialisieren
        Call Me.UserControl_Initialize()

    End Sub

    'Laden des Formulars    
    Private Sub EVO_Einstellungen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'EventHandler einrichten
        AddHandler Me.Button_Load.Click, AddressOf Form1.Load_EVO_Settings
        AddHandler Me.Button_Save.Click, AddressOf Form1.Save_EVO_Settings

    End Sub


    'Optimierungsmodus wurde ge�ndert
    '********************************
    Private Sub OptModus_Change()

        Select Case Me.settings.ty_EvoModus

            Case EVO_MODUS.Single_Objective
                'Vorgaben und Anzeige
                Label_OptModusValue.Text = "Single Objective"
                'Strategie
                ComboStrategie.Enabled = True
                'Sekund�re Population
                LabelInteract.Enabled = False
                TextInteract.Enabled = False
                LabelNMemberSecondPop.Enabled = False
                TextNMemberSecondPop.Enabled = False
                'Populationen
                CheckisPopul.Enabled = True

            Case EVO_MODUS.Multi_Objective
                'Vorgaben und Anzeige
                Label_OptModusValue.Text = "MultiObjective Pareto"
                'Strategie
                ComboStrategie.Enabled = False
                'Sekund�re Population
                LabelInteract.Enabled = True
                TextInteract.Enabled = True
                LabelNMemberSecondPop.Enabled = True
                TextNMemberSecondPop.Enabled = True
                'Populationen
                CheckisPopul.Enabled = False
                CheckisPopul.Checked = False
                GroupBox_Populationen.Enabled = False

        End Select

        Call FILLCOMBO_OPTELTERN(ComboOptEltern)
        Call FILLCOMBO_POPPENALTY(ComboPopPenalty)

    End Sub

    Public Sub SetFor_CES_PES(ByVal AnzGen As Integer, ByVal AnzEltern As Integer, ByVal AnzNachf As Integer)

        'Vorgaben und Anzeige
        Label_OptModusValue.Text = "MultiObjective Pareto"
        TextAnzGen.Text = CStr(AnzGen)
        TextAnzEltern.Text = CStr(AnzEltern)
        TextAnzNachf.Text = CStr(AnzNachf)

        System.Windows.Forms.Application.DoEvents()

    End Sub


    'UPGRADE_WARNING: Das Ereignis ComboOptEltern.SelectedIndexChanged kann ausgel�st werden, wenn das Formular initialisiert wird. Klicken Sie hier f�r weitere Informationen: 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="vbup2075"'
    Private Sub ComboOptEltern_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles ComboOptEltern.SelectedIndexChanged

        Select Case VB6.GetItemData(ComboOptEltern, ComboOptEltern.SelectedIndex)
            Case EVO_ELTERN.XY_Diskret, EVO_ELTERN.XY_Mitteln, EVO_ELTERN.Neighbourhood
                LabelRekombxy1.Enabled = True
                LabelRekombxy3.Enabled = True
                TextRekombxy.Enabled = True
            Case Else
                LabelRekombxy1.Enabled = False
                LabelRekombxy3.Enabled = False
                TextRekombxy.Enabled = False
        End Select

    End Sub

    'UPGRADE_WARNING: Das Ereignis CheckisPopul.CheckStateChanged kann ausgel�st werden, wenn das Formular initialisiert wird. Klicken Sie hier f�r weitere Informationen: 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="vbup2075"'
    Private Sub CheckisPopul_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles CheckisPopul.CheckStateChanged

        If (CheckisPopul.Checked) Then
            GroupBox_Populationen.Enabled = True
        Else
            GroupBox_Populationen.Enabled = False
        End If
    End Sub


    Private Sub FILLCOMBO_STRATEGIE(ByRef Cntrl As System.Windows.Forms.ComboBox)
        Cntrl.Items.Add(New VB6.ListBoxItem("'+' (Eltern+Nachfolger)", EVO_STRATEGIE.Plus))
        Cntrl.Items.Add(New VB6.ListBoxItem("',' (nur Nachfolger)", EVO_STRATEGIE.Komma))
        Cntrl.SelectedIndex = 0
    End Sub

    Private Sub FILLCOMBO_OPTPOPELTERN(ByRef Cntrl As System.Windows.Forms.ComboBox)
        Cntrl.Items.Add(New VB6.ListBoxItem("mit Rekombination", EVO_POP_ELTERN.Rekombination))
        Cntrl.Items.Add(New VB6.ListBoxItem("Mittelwerte", EVO_POP_ELTERN.Mittelwert))
        Cntrl.Items.Add(New VB6.ListBoxItem("Selektion", EVO_POP_ELTERN.Selektion))
        Cntrl.SelectedIndex = 0
    End Sub

    Private Sub FILLCOMBO_OPTELTERN(ByRef Cntrl As System.Windows.Forms.ComboBox)
        Cntrl.Items.Clear()
        Cntrl.Items.Add(New VB6.ListBoxItem("Selektion", EVO_ELTERN.Selektion))
        Cntrl.Items.Add(New VB6.ListBoxItem("Rekomb x/x, diskret", EVO_ELTERN.XX_Diskret))
        Cntrl.Items.Add(New VB6.ListBoxItem("Rekomb x/x, mitteln", EVO_ELTERN.XX_Mitteln))
        Cntrl.Items.Add(New VB6.ListBoxItem("Rekomb x/y, diskret", EVO_ELTERN.XY_Diskret))
        Cntrl.Items.Add(New VB6.ListBoxItem("Rekomb x/y, mitteln", EVO_ELTERN.XY_Mitteln))
        If (Me.settings.ty_EvoModus = EVO_MODUS.Multi_Objective) Then
            Cntrl.Items.Add(New VB6.ListBoxItem("Neighbourhood", EVO_ELTERN.Neighbourhood))
        End If
        Cntrl.SelectedIndex = 1
    End Sub

    Private Sub FILLCOMBO_OPTVORGABE(ByRef Cntrl As System.Windows.Forms.ComboBox)
        Cntrl.Items.Add(New VB6.ListBoxItem("Zuf�llig", EVO_STARTPARAMETER.Zufall))
        Cntrl.Items.Add(New VB6.ListBoxItem("Originalparameter", EVO_STARTPARAMETER.Original))
        Cntrl.SelectedIndex = 1
    End Sub

    Private Sub FILLCOMBO_POPPENALTY(ByRef Cntrl As System.Windows.Forms.ComboBox)
        Cntrl.Items.Clear()
        Select Case Me.settings.ty_EvoModus
            Case EVO.Kern.EVO_MODUS.Single_Objective
                Cntrl.Items.Add(New VB6.ListBoxItem("Mittelwert", EVO_POP_PENALTY.Mittelwert))
                Cntrl.Items.Add(New VB6.ListBoxItem("Schlechtester", EVO_POP_PENALTY.Schlechtester))
                Cntrl.SelectedIndex = 0
            Case EVO.Kern.EVO_MODUS.Multi_Objective
                Cntrl.Items.Add(New VB6.ListBoxItem("Crowding", EVO_POP_PENALTY.Crowding))
                Cntrl.Items.Add(New VB6.ListBoxItem("Spannweite", EVO_POP_PENALTY.Spannweite))
                Cntrl.SelectedIndex = 0
        End Select
    End Sub

    Private Sub UserControl_Initialize()
        Call FILLCOMBO_STRATEGIE(ComboStrategie)
        Call FILLCOMBO_STRATEGIE(ComboPopStrategie)
        Call FILLCOMBO_OPTPOPELTERN(ComboOptPopEltern)
        Call FILLCOMBO_OPTELTERN(ComboOptEltern)
        Call FILLCOMBO_OPTVORGABE(ComboOptVorgabe)
        Call FILLCOMBO_POPPENALTY(ComboPopPenalty)
    End Sub

    'Einstellungen aus Form einlesen
    '*******************************
    Private Sub readForm()

        settings.n_Eltern = Val(TextAnzEltern.Text)
        settings.n_Nachf = Val(TextAnzNachf.Text)
        settings.n_Gen = Val(TextAnzGen.Text)
        settings.ty_EvoStrategie = VB6.GetItemData(ComboStrategie, ComboStrategie.SelectedIndex)
        settings.Pop.is_POPUL = CheckisPopul.Checked
        settings.Pop.ty_PopEvoTyp = VB6.GetItemData(ComboPopStrategie, ComboPopStrategie.SelectedIndex)
        settings.Pop.ty_PopPenalty = VB6.GetItemData(ComboPopPenalty, ComboPopPenalty.SelectedIndex)
        settings.Pop.ty_OptPopEltern = VB6.GetItemData(ComboOptPopEltern, ComboOptPopEltern.SelectedIndex)
        If (settings.Pop.is_POPUL) Then
            settings.Pop.n_Runden = Val(TextAnzRunden.Text)
            settings.Pop.n_Popul = Val(TextAnzPop.Text)
            settings.Pop.n_PopEltern = Val(TextAnzPopEltern.Text)
        Else
            settings.Pop.n_Runden = 1
            settings.Pop.n_Popul = 1
            settings.Pop.n_PopEltern = 1
        End If
        settings.ty_OptEltern = VB6.GetItemData(ComboOptEltern, ComboOptEltern.SelectedIndex)
        settings.n_RekombXY = Val(TextRekombxy.Text)
        settings.DnStart = Val(TextDeltaStart.Text)
        settings.ty_StartPar = VB6.GetItemData(ComboOptVorgabe, ComboOptVorgabe.SelectedIndex)
        settings.is_DnVektor = CheckisDnVektor.Checked
        If (Val(TextInteract.Text) <= 0) Then
            settings.is_Interact = False
            settings.n_Interact = 1
        Else
            settings.is_Interact = True
            settings.n_Interact = Val(TextInteract.Text)
        End If
        settings.n_MemberSekPop = Val(TextNMemberSecondPop.Text)

    End Sub

    'Einstellungen in Form schreiben
    '*******************************
    Private Sub writeForm()

        Call OptModus_Change()

        Me.TextAnzEltern.Value = Me.settings.n_Eltern
        Me.TextAnzNachf.Value = Me.settings.n_Nachf
        Me.TextAnzGen.Value = Me.settings.n_Gen
        Me.ComboStrategie.SelectedItem = Me.settings.ty_EvoStrategie
        Me.CheckisPopul.Checked = Me.settings.Pop.is_POPUL
        Me.ComboPopStrategie.SelectedItem = Me.settings.Pop.ty_PopEvoTyp
        Me.ComboPopPenalty.SelectedItem = Me.settings.Pop.ty_PopPenalty
        Me.ComboOptPopEltern.SelectedItem = Me.settings.Pop.ty_OptPopEltern
        Me.TextAnzRunden.Value = Me.settings.Pop.n_Runden
        Me.TextAnzPop.Value = Me.settings.Pop.n_Popul
        Me.TextAnzPopEltern.Value = Me.settings.Pop.n_PopEltern
        Me.ComboOptEltern.SelectedItem = Me.settings.ty_OptEltern
        Me.TextRekombxy.Value = Me.settings.n_RekombXY
        Me.TextDeltaStart.Value = Me.settings.DnStart
        Me.ComboOptVorgabe.SelectedItem = Me.settings.ty_StartPar
        Me.CheckisDnVektor.Checked = Me.settings.is_DnVektor
        If (Me.settings.is_Interact) Then
            Me.TextInteract.Value = Me.settings.n_Interact
        Else
            Me.TextInteract.Value = 0
        End If
        Me.TextNMemberSecondPop.Value = Me.settings.n_MemberSekPop

        Call Application.DoEvents()

    End Sub

#Region "Schnittstelle"

    'Schnittstelle
    'XXXXXXXXXXXXX

    'Standardeinstellungen setzen
    '****************************
    Public Sub setStandard(ByVal modus As Kern.EVO_MODUS)
        Call Me.settings.setStandard(modus)
        Call Me.writeForm()
    End Sub

    'PES_Settings Property
    '*********************
    Public ReadOnly Property PES_Settings() As EVO.Kern.PES_Settings
        Get
            'Wenn Einstellungen noch nicht gespeichert, zuerst aus Form einlesen
            If (Not Me.isSaved) Then
                Call Me.readForm()
            End If
            PES_Settings = Me.settings
        End Get
    End Property

    'Speichern der PES_Settings in einer XML-Datei
    '*********************************************
    Public Sub saveSettings(ByVal filename As String)

        Call Me.readForm()

        Dim serializer As New XmlSerializer(GetType(EVO.Kern.PES_Settings))
        Dim writer As New StreamWriter(filename)
        serializer.Serialize(writer, Me.settings)
        writer.Close()

        Me.isSaved = True

    End Sub

    'Laden der PES_Settings aus einer XML-Datei
    '******************************************
    Public Sub loadSettings(ByVal filename As String)

        Dim serializer As New XmlSerializer(GetType(EVO.Kern.PES_Settings))

        ' If the XML document has been altered with unknown
        ' nodes or attributes, handle them with the
        ' UnknownNode and UnknownAttribute events.
        'AddHandler serializer.UnknownNode, AddressOf serializer_UnknownNode
        'AddHandler serializer.UnknownAttribute, AddressOf serializer_UnknownAttribute

        'XML-Datei einlesen
        Dim fs As New FileStream(filename, FileMode.Open)
        Me.settings = CType(serializer.Deserialize(fs), EVO.Kern.PES_Settings)
        fs.Close()

        'Geladene Settings in Form schreiben
        Call Me.writeForm()

    End Sub

#End Region 'Schnittstelle

End Class