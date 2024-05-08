using UraniumUI.Material.Controls;
namespace SessionTrackerProject.Views.Controls;

public partial class AddSession : ContentView
{
    //Events to be handled
    /// <summary>
    /// Event when the back button is pressed.
    /// </summary>
    public event EventHandler<EventArgs> OnSave;
    /// <summary>
    /// Event when the done or save button is pressed.
    /// </summary>
    public event EventHandler<EventArgs> OnCancel;
    /// <summary>
    /// Event when the attaching a file.
    /// </summary>
    public event EventHandler<EventArgs> OnAttach;
    /// <summary>
    /// Event when displaying the information on the field SET TIME
    /// </summary>
    public event EventHandler<EventArgs> InfoSetTime;
    /// <summary>
    /// Event when displaying the information on the field FILE
    /// </summary>
    public event EventHandler<EventArgs> InfoAttachFile;
    /// <summary>
    /// Event when displaying the information on the field READ TIME
    /// </summary>
    public event EventHandler<EventArgs> InfoReadTime;
    /// <summary>
    /// Event when displaying the information on the field LABEL
    /// </summary>
    public event EventHandler<EventArgs> InfoLabel;
    /// <summary>
    /// Event when displaying the information on the field REPEAT
    /// </summary>
    public event EventHandler<EventArgs> InfoRepeat;
    /// <summary>
    /// Shared page view when adding and updating a study session
    /// </summary>
    public AddSession()
    {
        InitializeComponent();
    }
    /// <summary>
    /// The "Done" or "Save" Button
    /// </summary>
    public Button? Button_Done
    {
        get { return btn_Done; }
        set { btn_Done = value; }
    }
    /// <summary>
    /// File Path of the PDF file is saved here.
    /// </summary>
    public string FilePathToOpen { get; set; }
    /// <summary>
    /// The <see cref="Button"/> back in the front end when adding / editing the study session
    /// </summary>
    public Button? Button_Back
    {
        get { return btn_Back; }
        set { btn_Back = value; }
    }
    /// <summary>
    /// The <see cref="Frame"/> for the the ClockField. The Frame that is containing the "TAP" text
    /// </summary>
    public Frame? Frame_tapFrameTime
    {
        get { return lbl_tapFrameTime; }
        set { lbl_tapFrameTime = value;}
    }
    /// <summary>
    /// The <see cref="Label"/> for the the ClockField. The text "TAP"
    /// </summary>
    public Label? Label_tapTextTime
    {
        get { return lbl_tapTextTime; }
        set { lbl_tapTextTime = value;} 
    }
    /// <summary>
    /// The <see cref="TimePickerField"/> entry for the the ClockField.
    /// </summary>
    public TimePickerField? TimePickerField_ClockField
    {
        get
        {
            if (ClockField != null)
                return ClockField;
            else return null;
        }
        set { ClockField = value; }
    }
    /// <summary>
    /// The <see cref="Label"/> text "ADD / EDIT SESSION" when editing or adding a study session
    /// </summary>
    public Label? Label_lblSetTask
    {
        get { return lbl_SetTask;}
        set { lbl_SetTask = value; }
    }
    /// <summary>
    /// The <see cref="Button"/> to add the PDF file
    /// </summary>
    public Button? Frame_tapFrameFile
    {
        get { return lbl_tapFrameFile; }
        set { lbl_tapFrameFile = value; }
    }
    /// <summary>
    /// The <see cref="Button"/> to add the PDF file
    /// </summary>
    public Label? Label_tapTextFile
    {
        get { return lbl_tapTextFile; }
        set { lbl_tapTextFile = value; }
    }
    /// <summary>
    /// The <see cref="Frame"/> next to the Repeat field enlosing the text "TAP"
    /// </summary>
    public Frame? Frame_tapFrameRepeat
    {
        get { return lbl_tapFrameRepeat; }
        set { lbl_tapFrameRepeat = value; }
    }
    /// <summary>
    /// The <see cref="Label"/> next to the Repeat field, the text "TAP"
    /// </summary>
    public Label? Label_tapTextRepeat
    {
        get { return lbl_tapTextRepeat; }
        set { lbl_tapTextRepeat = value; }
    }
    public string FilePath { get; set; }
    /// <summary>
    /// The <see cref="Entry"/> for the label.
    /// </summary>
    public Entry? Entry_SessionLabel
    {
        get { return lblEntry; }
        set { lblEntry = value; }
    }
    /// <summary>
    /// The <see cref="Entry"/> for the read time.
    /// </summary>
    public Entry? Entry_ReadTime
    {
        get { return lblEntry_ReadTime; }
        set { lblEntry_ReadTime = value; }
    }

    /// <summary>
    /// The <see cref="MultiplePickerField"/> for the choosing the day/s of week to repeat the study session.
    /// </summary>
    public MultiplePickerField? MultiplePickerField_DayOfWeek
    {
        get { return DayOfWeekPickerField; }
        set { DayOfWeekPickerField = value; }
    }
    /// <summary>
    /// The <see cref="DayOfWeek"/> list containing the choosen day/s of week to repeat the study session.
    /// </summary>
    public List<DayOfWeek> TaskRepeat
    {
        get { return DayOfWeekParser(DayOfWeekPickerField.SelectedItems as List<string>); }
        set { TaskRepeat = value; }
    }
    //Helper function
    private List<DayOfWeek> DayOfWeekParser(List<string> list)
    {
        var daysOfWeek = new List<DayOfWeek>();
        if (list != null)
        {
            foreach (var item in list)
            {
                if (item is string day && Enum.TryParse(day, true, out DayOfWeek dayOfWeek))
                    daysOfWeek.Add(dayOfWeek);
            }
        }
        return daysOfWeek;
    }
    /// <summary>
    /// The <see cref="DateTime"/> data chosen in Clock field."
    /// </summary>
    public DateTime att_SessionTime
    {
        get
        {
            TimeSpan? nullableTimeSpan = TimePickerField_ClockField.Time;
            TimeSpan time;

            if (nullableTimeSpan.HasValue)
            {
                time = nullableTimeSpan.Value;
            }
            else
            {
                time = TimeSpan.Zero;
                return DateTime.Now;
            }
            return DateTime.Today.Add(time);
        }
        set { att_SessionTime = value; }
    }

    private void btn_Back_Clicked(object sender, EventArgs e)
    {
        OnCancel?.Invoke(sender, e);
    }

    private void btn_Done_Clicked(object sender, EventArgs e)
    {
        OnSave?.Invoke(sender, e);
    }

    private void lbl_tapFrameFile_Clicked(object sender, EventArgs e)
    {
        OnAttach?.Invoke(sender, e);
    }

    private void btn_InfoSetTime_Clicked(object sender, EventArgs e)
    {
        InfoSetTime?.Invoke(sender, e);
    }

    private void btn_InfoAttachFile_Clicked(object sender, EventArgs e)
    {
        InfoAttachFile?.Invoke(sender, e);
    }

    private void btn_InfoReadTime_Clicked(object sender, EventArgs e)
    {
        InfoReadTime?.Invoke(sender, e);
    }

    private void btn_InfoLabel_Clicked(object sender, EventArgs e)
    {
        InfoLabel?.Invoke(sender, e);
    }

    private void btn_InfoRepeat_Clicked(object sender, EventArgs e)
    {
        InfoRepeat?.Invoke(sender, e);
    }
}