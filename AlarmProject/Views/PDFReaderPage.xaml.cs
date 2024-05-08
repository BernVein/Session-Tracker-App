using Microsoft.Maui.Controls;
using SessionTrackerProject.Models;
using System.Net;
using System;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification; //Used for notification

namespace SessionTrackerProject.Views
{
    public partial class PDFReaderPage : ContentPage
    {
        /// <summary>
        /// The time tracker.
        /// </summary>
        private IDispatcherTimer _timer;

        public PDFReaderPage()
        {
            InitializeComponent();
            AppShell.SetNavBarIsVisible(this, false);
            //Event when the notification is tapped.
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
            //Required initialization for the PDF viewer.
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, View) =>
            {
#if __ANDROID__
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
#endif
            });
            var androidFilePath = SessionScheduler.SessionSubject.FilePath;
            var encodedFilePath = WebUtility.UrlEncode(androidFilePath);
            //Opens the PDF file with the PDF viewer
            pdfviewer.Source = $"file:///android_asset/pdfjs/web/viewer.html?file={encodedFilePath}";
            //Initilizes a timer
            _timer = Application.Current.Dispatcher.CreateTimer();
            //Interval of the timer will be 1 minute
            _timer.Interval = TimeSpan.FromMinutes(1);
            //Each interval, Timer_Tick is called.
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        //Event when the notification is tapped.
        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            //If the notificaiton is tapped and if it matches the notification ID, and if the user chose the action button "Open"
            if (e.IsTapped && e.Request.NotificationId == 1337 || e.ActionId == 100)
            {
                //Required initialization for the PDF viewer.
                Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, View) =>
                {
#if __ANDROID__
                    handler.PlatformView.Settings.AllowFileAccess = true;
                    handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                    handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
#endif
                });

                var androidFilePath = SessionScheduler.SessionSubject.FilePath;
                var encodedFilePath = WebUtility.UrlEncode(androidFilePath);
                pdfviewer.Source = $"file:///android_asset/pdfjs/web/viewer.html?file={encodedFilePath}";
            }
            //If the notificaiton is tapped and if it matches the notification ID, and if the user chose the action button "Dismiss"
            else if (e.ActionId == 101 && e.Request.NotificationId == 1337)
            {
                if(SessionScheduler.SessionSubject != null)
                {
                    SessionRepository.DeleteSession(SessionScheduler.SessionSubject.SessionID);
                    LocalNotificationCenter.Current.Cancel(1338);
                    SessionScheduler.ShowNotif();
                }
                
            }
            //Cancels the notification if the user interacted with the notification
            LocalNotificationCenter.Current.ClearAll();
        }

        /// <summary>
        /// Will deduct the remaining time of <see cref="SessionScheduler.SessionSubject.RemainingTime"/> by one each minute. If the remaining time is 0, it will display a notification saying you've finish the study session.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (SessionScheduler.SessionSubject.RemainingTime > 0)
            {
                SessionScheduler.SessionSubject.RemainingTime--;
                SessionRepository.SaveSessions();
            }

            if (SessionScheduler.SessionSubject.RemainingTime == 0)
            {
                _timer.Stop();

                var request = new NotificationRequest
                {
                    NotificationId = SessionScheduler.SessionSubject.SessionID + 1,
                    Title = "TASK ON HAND",
                    Subtitle = SessionScheduler.SessionSubject.SessionLabel,
                    Description = $"You have successfully finished studying and reading {SessionScheduler.SessionSubject.FileName}. Good job!",
                    BadgeNumber = 42,
                    CategoryType = NotificationCategoryType.Alarm,
                    //Returned data when the notification is being interacted
                    ReturningData = SessionScheduler.SessionSubject.SessionID.ToString(),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now,
                        RepeatType = NotificationRepeat.Daily,
                    },
                };
                //Show the notification based on the params above
                LocalNotificationCenter.Current.Show(request);
            }
        }
        //Event when the back button is pressed. It will show a notification on the remaining time of the study session, and if the notification is tapped, it will resume the time and will open the PDF reader
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _timer.Stop();
            if(SessionScheduler.SessionSubject.RemainingTime > 0)
            {
                var request = new NotificationRequest
                {
                    NotificationId = 1337,
                    Title = "PENDING TASK ON HAND",
                    Subtitle = SessionScheduler.SessionSubject.SessionLabel,
                    Description = $"You have {SessionScheduler.SessionSubject.RemainingTime} minutes left to study and read {SessionScheduler.SessionSubject.FileName}. Tap here to resume reading.",
                    BadgeNumber = 42,
                    CategoryType = NotificationCategoryType.Status,
                    ReturningData = SessionScheduler.SessionSubject.SessionID.ToString(),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now,
                        RepeatType = NotificationRepeat.TimeInterval,
                        //Notifies every 5 mins until the user opens or dismisses the task
                        NotifyRepeatInterval = TimeSpan.FromMinutes(5)
                    },
                    Android = new AndroidOptions
                    {
                        //Notification can't be removed
                        Ongoing = true
                    },
                };
                LocalNotificationCenter.Current.Show(request);
            }
        }
    }
}
