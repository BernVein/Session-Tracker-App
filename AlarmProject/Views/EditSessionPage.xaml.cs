using SessionTrackerProject.Models;
using Microsoft.Maui.Storage;
using System.Linq;
using System.Collections;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SessionTrackerProject.Views;
[QueryProperty(nameof(SessionID), "Id")]
public partial class EditSessionPage : ContentPage
{
    /// <summary>
    /// This object will be the fetched object chosen in <see cref="ListView"/>
    /// </summary>
    private Session session;
    private string fileName;
    private string filePath;
    private int counter = 0;
    private List<DayOfWeek> temp;
    public EditSessionPage()
    {
        InitializeComponent();
        AppShell.SetNavBarIsVisible(this, false);
        InitializeUI();
    }
    public string SessionID
    {
        set
        {
            //Custom bugfix for uranium UI. Used a counter because Uranium UI refreshes the page everytime the user interacts with the fields. I restricted the refreshes here to only once so that it wont set the fields again.
            if(counter == 0)
            {
                counter++;
                session = SessionRepository.GetSessionByID(int.Parse(value));
                ctrl_AddSession.TimePickerField_ClockField.Time = DateTimeToNullableTimeSpan(session.SessionTime);
                ctrl_AddSession.Label_tapTextFile.Text = session.FileName;
                ctrl_AddSession.FilePath = session.FilePath;
                ctrl_AddSession.Entry_SessionLabel.Text = session.SessionLabel;
                ctrl_AddSession.Entry_ReadTime.Text = session.ReadTime.ToString();
                filePath = session.FilePath;
                temp = session.SessionRepeat;
            }

        }
    }

    //Event when back button is pressed
    private void btn_Back_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..", animate: true);
    }

    private void InitializeUI()
    {
        ctrl_AddSession.Label_tapTextRepeat.Opacity = 1;
        ctrl_AddSession.Frame_tapFrameRepeat.Opacity = 1;
        ctrl_AddSession.MultiplePickerField_DayOfWeek.Opacity = 0;

        ctrl_AddSession.Frame_tapFrameTime.Opacity = 0;
        ctrl_AddSession.Label_tapTextTime.Opacity = 0;

        ctrl_AddSession.Frame_tapFrameFile.BackgroundColor = Colors.Transparent;

        ctrl_AddSession.Label_lblSetTask.Text = "EDIT TASK";
        ctrl_AddSession.MultiplePickerField_DayOfWeek.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "SelectedItems")
            {
                UpdateUIInformation();
            }
        };

        ctrl_AddSession.Frame_tapFrameFile.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "FileName")
            {
                UpdateUIInformation();
            }
        };
    }
    /// <summary>
    /// Sets the UI for when editing a study sess. General idea is the texts "TAP" and the frame enclosing those text will dissapear when the user interacts with it. This is done via modifying their opacities.
    /// </summary>
    private void UpdateUIInformation()
    {
        if (ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems != null)
        {
            ctrl_AddSession.Label_tapTextRepeat.Opacity = 0;
            ctrl_AddSession.Frame_tapFrameRepeat.Opacity = 0;
            ctrl_AddSession.MultiplePickerField_DayOfWeek.Opacity = 1;
        }
        else
        {
            ctrl_AddSession.MultiplePickerField_DayOfWeek.Opacity = 0;
            ctrl_AddSession.Label_tapTextRepeat.Opacity = 1;
            ctrl_AddSession.Frame_tapFrameRepeat.Opacity = 1;
        }

        if (!string.IsNullOrEmpty(session.FileName))
        {
            ctrl_AddSession.Frame_tapFrameFile.BackgroundColor = Colors.Transparent;
        }
    }
    //Event when this page appears; Also initializes the UI
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ctrl_AddSession.Label_lblSetTask.Text = "EDIT SESSION";
        ctrl_AddSession.Label_lblSetTask.FontSize = 43;
        ctrl_AddSession.Button_Done.Text = "SAVE";
    }
    //Event when the user attaches the PDF File
    private async void ctrl_AddSession_OnAttach(object sender, EventArgs e)
    {
        var res = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a file to be attached.",
            FileTypes = FilePickerFileType.Pdf
        });
        if (res == null) return;
        fileName = res.FileName;
        filePath = res.FullPath;
        ctrl_AddSession.Frame_tapFrameFile.Opacity = 0;
        ctrl_AddSession.Label_tapTextFile.Text = res.FileName;
    }
    //Event when the "Save" button is pressed
    private void btn_Done_Clicked(object sender, EventArgs e)
    {
        //Custom field validations
        if (ctrl_AddSession.Label_tapTextFile.Text == "TAP")
        {
            DisplayAlert("ERROR", "You have to attach a file.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(ctrl_AddSession.Entry_ReadTime.Text))
        {
            DisplayAlert("ERROR", "Read Time entry is required.", "OK");
            return;
        }
        string readTimeText = ctrl_AddSession.Entry_ReadTime.Text;
        if (readTimeText == "0" || readTimeText.StartsWith("0") || readTimeText.StartsWith("."))
        {
            DisplayAlert("ERROR", "Read time cannot be 0 or start with 0 or '.'", "OK");
            return;
        }
        UpdateDetails();
        //Show pop-up
        DisplayToast(session);
        Shell.Current.GoToAsync("..", true);
    }
    /// <summary>
    /// Responsible in showing the information based on the <see cref="Session"/> object passed. It shows the user the time left for the task to notify inluding the repeat days.
    /// </summary>
    private void DisplayToast(Session session)
    {
        DayOfWeek today = DateTime.Now.DayOfWeek;
        TimeSpan now = DateTime.Now.TimeOfDay;

        if (session.IsEnabled && session.SessionRepeat.Contains(today))
        {
            if (session.SessionTime.TimeOfDay > now)
            {
                TimeSpan difference = session.SessionTime.TimeOfDay - now;
                string daysOfWeek = string.Join(", ", session.SessionRepeat);
                string message = $"{difference.Hours} hours and {difference.Minutes} mins from now - {daysOfWeek}";
                Toast.Make($"Study session is set to {message}", ToastDuration.Long).Show();
            }
            else
            {
                DayOfWeek tom = DateTime.Now.AddDays(value: 1).DayOfWeek;
                if (!session.SessionRepeat.Contains(tom))
                {
                    if (session.SessionRepeat.Count > 1 && session.SessionRepeat.Contains(today))
                        session.SessionRepeat.Remove(today);
                    session.SessionRepeat.Add(tom);
                }
                TimeSpan difference = TimeSpan.FromDays(1) - now + session.SessionTime.TimeOfDay;
                string daysOfWeek = string.Join(", ", session.SessionRepeat);
                string message = $"{difference.Hours} hours and {difference.Minutes} mins from now - {daysOfWeek}";
                Toast.Make($"Study session is set to {message}", ToastDuration.Long).Show();
            }
        }
        else
        {
            string daysOfWeek = string.Join(", ", session.SessionRepeat);
            Toast.Make($"Your study session will repeat on {daysOfWeek}", ToastDuration.Long).Show();
        }
    }
    /// <summary>
    /// Updates the information of the fetched <see cref="Session"/> object based on the new input from the user.
    /// </summary>
    private void UpdateDetails()
    {
        session.SessionTime = ctrl_AddSession.att_SessionTime;
        List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
        //If the user doesn't choose anything in th repeat field, it will default to the current day of the week.
        if (ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems == null || ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems.Count == 0)
        {
            session.SessionRepeat = temp;
        }
        else
        {
            var selectedItemsList = ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems.Cast<string>().ToList();
            dayOfWeekList = DayOfWeekParser(selectedItemsList);
            session.SessionRepeat = dayOfWeekList;
        }
        //Parsing the data the user has inputed
        session.FileName = ctrl_AddSession.Label_tapTextFile.Text;
        session.FilePath = filePath;
        session.ReadTime = int.Parse(Regex.Replace(ctrl_AddSession.Entry_ReadTime.Text, @"\..*", ""));
        session.RemainingTime = session.ReadTime;
        //If there is no label, the label will be "No Label"
        if (string.IsNullOrEmpty(ctrl_AddSession.Entry_SessionLabel.Text))
            session.SessionLabel = "No Label";
        else session.SessionLabel = ctrl_AddSession.Entry_SessionLabel.Text;
        SessionRepository.UpdateSession(session.SessionID, session);
    }
    //Helper function
    private TimeSpan? DateTimeToNullableTimeSpan(DateTime time) {return time.TimeOfDay;}
    //Helper function
    private List<DayOfWeek> DayOfWeekParser(IList list)
    {
        var daysOfWeek = new List<DayOfWeek>();
        if (list != null)
        {
            foreach (var item in list)
            {
                var dayString = item?.ToString();
                if (!string.IsNullOrEmpty(dayString) && Enum.TryParse(dayString, true, out DayOfWeek dayOfWeek))
                    daysOfWeek.Add(dayOfWeek);
            }
        }
        return daysOfWeek;
    }
    //Event when displaying the information on the field SET TIME
    private void ctrl_AddSession_InfoSetTime(object sender, EventArgs e)
    {
        DisplayAlert("REQUIRED FIELD", "This sets the time for when you plan the study session. A notification will be displayed when the set time arrives. This field is mandatory.", "OK");
    }
    //Event when displaying the information on the field FILE
    private void ctrl_AddSession_InfoAttachFile(object sender, EventArgs e)
    {
        DisplayAlert("REQUIRED FIELD", "This allows you to attach the file that you intend to study. This field is mandatory.", "OK");
    }
    //Event when displaying the information on the field READ TIME
    private void ctrl_AddSession_InfoReadTime(object sender, EventArgs e)
    {
        DisplayAlert("REQUIRED FIELD", "The 'Read Time' is the duration you plan to spend studying and reading the file. This will be tracked. If you leave the reading page, a notification will be displayed showing the remaining time. Additionally, a notification will be displayed when the reading time is up. Values should not start with 0, it also cannot be 0, don't start with '.', and refrain from using decimal points, if you do, it will just get the whole number. This field is mandatory.", "OK");
    }

    //Event when displaying the information on the field LABEL
    private void ctrl_AddSession_InfoLabel(object sender, EventArgs e)
    {
        DisplayAlert("OPTIONAL FIELD", "The 'Label' is your personal note about what the study session involves. This field is optional.", "OK");
    }
    //Event when displaying the information on the field REPEAT
    private void ctrl_AddSession_InfoRepeat(object sender, EventArgs e)
    {
        DisplayAlert("OPTIONAL FIELD", "You can select specific days of the week for your study session to repeat. For example, if you choose Monday and Tuesday, a notification will appear every Monday and Tuesday at the time you set. If you don't choose a day, the task will default to repeat on the current day you're setting the task. This field is optional.", "OK");
    }

}
