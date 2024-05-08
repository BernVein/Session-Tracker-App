using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using SessionTrackerProject.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.LocalNotification; //Used for notification
using Plugin.LocalNotification.AndroidOption;

namespace SessionTrackerProject.Models
{
    /// <summary>
    /// The class that manages and schedules the study sessions.
    /// </summary>
    public static class SessionScheduler
    {
        /// <summary>
        /// The <see cref="Session"/> object that's in the current study session.
        /// </summary>
        public static Session SessionSubject;

        static SessionScheduler()
        {
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }
        //Event on when the notification is tapped.
        private static async void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            var sessionId = int.Parse(e.Request.ReturningData);
            SessionSubject = SessionRepository.Sessions.FirstOrDefault(a => a.SessionID == sessionId);
            if (SessionSubject != null)
            {
                //Handles the event where the user decides to dismiss the study session.
                if (e.ActionId == 101)
                {
                    SessionRepository.DeleteSession(sessionId);
                    ShowNotif();
                }
                //Handles the event where the user decides to start the study session.
                if (e.IsTapped && e.Request.NotificationId != 1338 || e.ActionId == 100)
                {
                    await Shell.Current.GoToAsync(nameof(PDFReaderPage), true);
                }
                else
                    await Shell.Current.GoToAsync(nameof(FrontPage), true);
            }
            else
            {
                LocalNotificationCenter.Current.ClearAll();
                return;
            }
            //If the user interacts with the notification in any way, the notifications dissapears.
            
        }
        /// <summary>
        /// Responsible in scheduling all study sessions / Sessions and its notifications from <see cref="Session.SessionTime"/>. The notification will be repeated in every <b>5 minutes</b> if the notification is not being interacted.
        /// </summary>
        public static void SetSchedule()
        {
            SessionRepository.LoadSessions();
            foreach (Session session in SessionRepository.Sessions)
            {
                if (session.IsEnabled && session.SessionRepeat.Contains(DateTime.Now.DayOfWeek))
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = session.SessionID,
                        Title = "SESSION SCHEDULED",
                        Subtitle = session.SessionLabel,
                        Description = $"You have to study and read {RemovePdfExtension(session.FileName)} for {session.ReadTime} minutes. Tap here to start the study session.",
                        //BadgeNumber is for the circular symbol above the application.
                        BadgeNumber = 42,
                        CategoryType = NotificationCategoryType.Status,
                        //Returned data if the notification is triggered.
                        ReturningData = session.SessionID.ToString(),
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.Date + session.SessionTime.TimeOfDay,
                            RepeatType = NotificationRepeat.TimeInterval,
                            //Notification will trigger every 5 minutes if there is not interaction in the notification.
                            NotifyRepeatInterval = TimeSpan.FromMinutes(5)
                        },
                        Android = new AndroidOptions
                        {
                            //Notification cannot be swiped.
                            Ongoing = true,
                        },
                    };
                    //Shows the notification based on the parameters above.
                    LocalNotificationCenter.Current.Show(request);

                }
            }
        }
        /// <summary>
        /// Shows the notification for the count of upcoming tasks.
        /// </summary>
        public static void ShowNotif()
        {
            if(CountSessionsForToday(SessionRepository.Sessions) > 0)
            {
                var request = new NotificationRequest
                {
                    NotificationId = 1338,
                    Title = "YOU HAVE SESSION/S TODAY",
                    Description = $"Upcoming study session/s: {CountSessionsForToday(SessionRepository.Sessions)}",
                    BadgeNumber = 42,
                    CategoryType = NotificationCategoryType.Alarm,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now,
                        RepeatType = NotificationRepeat.Daily,
                    },
                    Android = new AndroidOptions
                    {
                        Ongoing = true,
                        LaunchAppWhenTapped = true,
                    },
                };
                LocalNotificationCenter.Current.Show(request);
            }

        }
        //Helper function
        private static string RemovePdfExtension(string fileName)
        {
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return fileName.Substring(0, fileName.Length - 4);
            }
            return fileName;
        }
        //Helper function
        private static int CountSessionsForToday(List<Session> sessions)
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            TimeSpan now = DateTime.Now.TimeOfDay;
            int count = 0;
            foreach (var session in sessions)
            {
                if (session.SessionRepeat.Contains(today) && session.IsEnabled && session.SessionTime.TimeOfDay > now)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
