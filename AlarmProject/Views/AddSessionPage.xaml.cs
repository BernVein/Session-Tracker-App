using SessionTrackerProject.Models;
using System.Globalization;
using System.Collections;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Core;
using System.Security.Claims;
using CommunityToolkit.Maui.Alerts;
namespace SessionTrackerProject.Views;

public partial class AddSessionPage : ContentPage
{
    private string fileName;
    private string filePath;
    
    public AddSessionPage()
	{
        InitializeComponent();
        AppShell.SetNavBarIsVisible(this, false);
        InitializeUI();
    }
    /// <summary>
    /// Sets the UI for when adding a study session. General idea is the texts "TAP" and the frame enclosing those text will dissapear when the user interacts with it. This is done via modifying their opacities.
    /// </summary>
    private void InitializeUI()
    {
        //Initially, the frame and the text "TAP" will be seen
        ctrl_AddSession.Frame_tapFrameTime.Opacity = 1;
        ctrl_AddSession.Label_tapTextTime.Opacity = 1;

        ctrl_AddSession.MultiplePickerField_DayOfWeek.Opacity = 0;
        ctrl_AddSession.Label_tapTextRepeat.Opacity = 1;
        ctrl_AddSession.Frame_tapFrameRepeat.Opacity = 1;

        ctrl_AddSession.Label_tapTextFile.Text = "TAP";
        ctrl_AddSession.Label_tapTextFile.TextColor = Color.FromHex("#FFFFFF");
        //When the user interacts, we will update the opacities using the UpdateUIInformation function.
        ctrl_AddSession.TimePickerField_ClockField.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Time")
            {
                UpdateUIInformation();
            }
        };

        ctrl_AddSession.MultiplePickerField_DayOfWeek.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "SelectedItems")
            {
                UpdateUIInformation();
            }
        };

        ctrl_AddSession.Label_tapTextFile.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Text")
            {
                UpdateUIInformation();
            }
        };
    }
    /// <summary>
    /// Handles the UI when the user interacts with the fields. Mainly by modifying the opacities.
    /// </summary>
    private void UpdateUIInformation()
    {
        if (ctrl_AddSession.TimePickerField_ClockField.Time != null)
        {
            ctrl_AddSession.Frame_tapFrameTime.Opacity = 0;
            ctrl_AddSession.Label_tapTextTime.Opacity = 0;
        }
        else
        {
            ctrl_AddSession.Frame_tapFrameTime.Opacity = 1;
            ctrl_AddSession.Label_tapTextTime.Opacity = 1;
        }

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

        if (ctrl_AddSession.Label_tapTextFile.Text == "TAP")
        {
            ctrl_AddSession.Label_tapTextFile.Text = "TAP";
            ctrl_AddSession.Label_tapTextFile.TextColor = Color.FromHex("#FFFFFF");
        }
        else
        {
            ctrl_AddSession.Label_tapTextFile.TextColor = Color.FromHex("#1A1A1A");
            ctrl_AddSession.Label_tapTextFile.Text = fileName;
        }
    }


    //Event when the back button is pressed.
    private void btn_Back_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..", animate: true);

    }
    //Event when the save button is pressed.
    private void ctrl_AddSession_OnSave(object sender, EventArgs e)
    {
        //Custom field validations for each required fields.
        if (ctrl_AddSession.Label_tapTextTime.Opacity == 1)
        {
            DisplayAlert("ERROR", "You have to set the time.", "OK");
            return;
        }
        if (ctrl_AddSession.Label_tapTextFile.Text == "TAP")
        {
            DisplayAlert("ERROR", "You have to attach a file.", "OK");
            return;
        }
        List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
        //If the user doesn't choose a day, it will be set to the current day
        if (ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems == null || ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems.Count == 0)
        {
            dayOfWeekList.Add(DateTime.Now.DayOfWeek);
        }
        else
        {
            var selectedItemsList = ctrl_AddSession.MultiplePickerField_DayOfWeek.SelectedItems.Cast<string>().ToList();
            dayOfWeekList = ConvertToDayOfWeekList(selectedItemsList);
        }

        if (string.IsNullOrEmpty(ctrl_AddSession.Entry_ReadTime.Text))
        {
            DisplayAlert("ERROR", "Read Time entry is required.", "OK");
            return;
        }
        string readTimeText = ctrl_AddSession.Entry_ReadTime.Text;
        //Field validation for valid read time inputs
        if (readTimeText == "0" || readTimeText.StartsWith("0") || readTimeText.StartsWith("."))
        {
            DisplayAlert("ERROR", "Read time cannot be 0 or start with 0 or '.'", "OK");
            return;
        }


        TimeSpan? nullableTimeSpan = ctrl_AddSession.TimePickerField_ClockField.Time;
        TimeSpan time;

        if (nullableTimeSpan.HasValue)
        {
            time = nullableTimeSpan.Value;
        }
        else
        {
            time = TimeSpan.Zero;
            DisplayAlert("ERROR", "Time has no value.", "OK");
            return;
        }
        //Parsing the data provided by user
        DateTime sessionTime = DateTime.Today.Add(time);
        int readTime = int.Parse(Regex.Replace(ctrl_AddSession.Entry_ReadTime.Text, @"\..*", ""));
        bool isEnabled = true;
        string sessionLabel;
        if (string.IsNullOrEmpty(ctrl_AddSession.Entry_SessionLabel.Text))
            sessionLabel = "No Label";
        else sessionLabel = ctrl_AddSession.Entry_SessionLabel.Text;
        //The session data created based on the parsed data
        Session sessionToAdd = new Session(sessionTime, sessionLabel, dayOfWeekList, isEnabled, fileName, filePath, readTime);
        SessionRepository.AddSession(sessionToAdd);
        //Display a pop-up notification on the set data by the user.
        DisplayToast(sessionToAdd);
        Shell.Current.GoToAsync("..");
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


    //Helper function
    public static List<DayOfWeek> ConvertToDayOfWeekList(IEnumerable<string> selectedItems)
    {
        var dayOfWeekList = new List<DayOfWeek>();

        foreach (var selectedItem in selectedItems)
        {
            if (Enum.TryParse(selectedItem, true, out DayOfWeek dayOfWeek))
            {
                dayOfWeekList.Add(dayOfWeek);
            }
        }

        return dayOfWeekList;
    }
    //Event when the user attaches the PDF file.
    private async void ctrl_AddSession_OnAttach(object sender, EventArgs e)
    {
        var res = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a file to be attached.",
            FileTypes = FilePickerFileType.Pdf
        }) ;
        if (res == null) return;
        fileName = res.FileName;
        filePath = res.FullPath;
        ctrl_AddSession.Frame_tapFrameFile.Opacity = 0;
        ctrl_AddSession.Label_tapTextFile.Text = res.FileName;
        ctrl_AddSession.FilePathToOpen = filePath;
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
