using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json; //For json serialization / deserialization (local storage)
using System.Threading.Tasks;

namespace SessionTrackerProject.Models
{
    /// <summary>
    /// This class is for data management for the <see cref="Session"/> object. Here is where adding, updating, deleting, etc. is hand
    /// </summary>
    public static class SessionRepository
    {
        /// <summary>
        /// Storage in code for the list of <see cref="Session"/> / study sessions.
        /// </summary>
        public static List<Session> Sessions = new List<Session>();
        /// <summary>
        /// Filepath in phone is inside the hidden folder: <b>AppData</b>
        /// </summary>
        private static string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sessions.json");

        /// <summary>
        /// Saves the <see cref="Session"/> collection into the appdata directory locally of the android device in <b>.json format</b> by using <see cref="JsonSerializerOptions"></see>
        /// </summary>
        public static void SaveSessions()
        {
            var options = new JsonSerializerOptions { WriteIndented = true }; //For json formatting
            var jsonString = JsonSerializer.Serialize(Sessions, options);
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// Loads the <see cref="Session"/> collection from the appdata directory locally of the android device in <b>.json format</b> by using <see cref="JsonSerializerOptions"></see>
        /// </summary>
        public static void LoadSessions()
        {
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                Sessions = JsonSerializer.Deserialize<List<Session>>(jsonString);
            }
        }

        //Helper function
        public static DateTime SetDate(int Hour, int Minute)
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Hour, Minute, 0);
        }
        /// <summary>
        /// Returns the <see cref="Session"/> list.
        /// </summary>
        public static List<Session> GetSessions()
        {
            LoadSessions();
            return Sessions;
        }
        /// <summary>
        /// Returns the <see cref="Session"/> that matches the sessID, if nothing matches, it returns null.
        /// </summary>
        public static Session GetSessionByID(int SessionID)
        {
            var session = Sessions.FirstOrDefault(x => x.SessionID == SessionID);
            if (session != null)
            {
                return new Session(session);
            }
            return null;
        }
        /// <summary>
        /// Updates the <see cref="Session"/> that is currently in the list then saves the data by calling <see cref="SaveSessions()"/> as well as displaying a notification on the upcoming study sessions using <see cref="SessionScheduler.ShowNotif()"/>
        /// </summary>
        public static void UpdateSession(int SessionID, Session sess)
        {
            if (SessionID != sess.SessionID) return;
            var SessionToUpdate = Sessions.FirstOrDefault(x => x.SessionID == SessionID);
            if (SessionToUpdate != null)
            {
                SessionToUpdate.SessionLabel = sess.SessionLabel;
                SessionToUpdate.SessionTime = sess.SessionTime;
                SessionToUpdate.SessionRepeat = sess.SessionRepeat;
                SessionToUpdate.IsEnabled = sess.IsEnabled;
                SessionToUpdate.FileName = sess.FileName;
                SessionToUpdate.ReadTime = sess.ReadTime;
                SessionToUpdate.RemainingTime = SessionToUpdate.ReadTime;
                SessionToUpdate.FilePath = sess.FilePath;
                LocalNotificationCenter.Current.Cancel(1338);
                SessionScheduler.ShowNotif();
                SaveSessions();
            }
            else return;
        }
        /// <summary>
        /// Adds an <see cref="Session"/> object to the list <see cref="Sessions"/>, then saves the data by calling <see cref="SaveSessions()"/> as well as displaying a notification on the upcoming study sessions using <see cref="SessionScheduler.ShowNotif()"/>
        /// </summary>
        public static void AddSession(Session sess)
        {
            var maxID = Sessions.Count > 0 ? Sessions.Max(x => x.SessionID) : 0;
            sess.SessionID = maxID + 1;
            Sessions.Add(sess);
            SaveSessions();
            LocalNotificationCenter.Current.Cancel(1338);
            SessionScheduler.ShowNotif();
        }
        /// <summary>
        /// Deletes an <see cref="Session"/> object from the list <see cref="Sessions"/>, then saves the data by calling <see cref="SaveSessions()"/> as well as displaying a notification on the upcoming study sessions using <see cref="SessionScheduler.ShowNotif()"/>
        /// </summary>
        public static void DeleteSession(int sessID)
        {
            var session = Sessions.FirstOrDefault(x=> x.SessionID == sessID);
            if (session != null) Sessions.Remove(session);
            else return;
            SaveSessions();
            LocalNotificationCenter.Current.Cancel(1338);
            SessionScheduler.ShowNotif();
        }
        /// <summary>
        /// Updates the <see cref="Session.IsEnabled"/> property of <see cref="Session"/> object from the list <see cref="Sessions"/>, then saves the data by calling <see cref="SaveSessions()"/> as well as displaying a notification on the upcoming study sessions using <see cref="SessionScheduler.ShowNotif()"/>
        /// </summary>
        public static void UpdateIsEnabledOnly(int sessionID, bool isToggled)
        {
            var session = Sessions.FirstOrDefault(x => x.SessionID == sessionID);
            if(session != null)
            {
                session.IsEnabled = isToggled;
                SaveSessions();
                LocalNotificationCenter.Current.Cancel(1338);
                SessionScheduler.ShowNotif();
            }
        }

    }

}