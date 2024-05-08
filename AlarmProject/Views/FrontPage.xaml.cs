namespace SessionTrackerProject.Views;
using SessionTrackerProject.Models;
using SessionTrackerProject.Views.Controls;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

public partial class FrontPage : ContentPage
{
    //counter is for the bugfix.
    private int counter = 0;
    public FrontPage()
    {
        InitializeComponent();
        AppShell.SetNavBarIsVisible(this, false);
        LoadSession();
    }
    /// <summary>
    /// Shows the initial prompt when there is nothing in the session list.
    /// </summary>
    private void UpdateEmptyWarning()
    {
        lbl_EmptyWarning.Opacity = SessionRepository.Sessions.Count == 0 ? 1 : 0;
    }
    /// <summary>
    /// Fetches the <see cref="SessionRepository.Sessions"/> and adds it to an <see cref="ObservableCollection{T}"/> and assign it as an ItemSource for the Maui UI
    /// </summary>
    private void LoadSession()
    {
        var Sessions = new ObservableCollection<Session>(SessionRepository.GetSessions());
        //SessionList is the name of the ListView
        SessionList.ItemsSource = Sessions;
        SessionScheduler.SetSchedule();
        UpdateEmptyWarning();
    }
    //When the page appears, it loads the the data from SessionRepository
    protected override void OnAppearing()
    {
        counter = 0;
        base.OnAppearing();
        LoadSession();
    }
    //Event when back button is pressed
    private async void btn_AddSession_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddSessionPage), animate: true);
    }
    //Event when an item is selected within the list view
    private async void SessionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (SessionList.SelectedItem != null)
        {
            await Shell.Current.GoToAsync($"{nameof(EditSessionPage)}?Id={((Session)(SessionList.SelectedItem)).SessionID}", animate: true);
        }
    }
    //Event when the item is tapped, this is so that it rids off the bug of double clicking. This is called automatically after the event SessionList_ItemSelected.
    private void SessionList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        SessionList.SelectedItem = null;
    }
    //Event when delete is tapped. Delete is shown when an item in the listview is long pressed
    private void Delete_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var session = menuItem.CommandParameter as Session;
        SessionRepository.DeleteSession(session.SessionID);
        counter = 0;
        LoadSession();
        UpdateEmptyWarning();
        SessionScheduler.SetSchedule();
    }
    //Event when the switch of an object in listview is pressed.
    private void Switch_IsEnabled_Toggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch toggleSwitch && toggleSwitch.BindingContext is Session session)
        {
            SessionRepository.UpdateIsEnabledOnly(session.SessionID, e.Value);
            if (session.IsEnabled && counter > SessionRepository.Sessions.Count - 1) DisplayToast(session);
            counter++;
        }
        SessionScheduler.SetSchedule();
    }
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
                    if(session.SessionRepeat.Count > 1 && session.SessionRepeat.Contains(today)) 
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
    /// Responsible in showing the information based on the <see cref="Session"/> object passed. It shows the user the time left for the task to notify inluding the repeat days.
    /// </summary>

    private void btn_Info_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("SESSION INFO", "A session is a scheduled time dedicated to studying. When creating a session, you'll specify key details such as the study start time, duration, and the material to study (which can be attached as a PDF file). Once set, you'll receive notifications when the session begins, and the Smyt will track the time you spend studying.", "OK");
    }
}
