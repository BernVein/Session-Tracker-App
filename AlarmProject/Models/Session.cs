using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;

namespace SessionTrackerProject.Models
{
    [Serializable]
    public class Session

    {
        /// <summary>
        /// Unique ID assigned to a study session
        /// </summary>
        public int SessionID { get; set; }
        /// <summary>
        /// Local filepath of the pdf file to be read
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// File name of the pdf file to be read
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Time for the study session
        /// </summary>
        public DateTime SessionTime { get; set; }
        /// <summary>
        /// A string label for a personal note on what the Study session will be
        /// </summary>
        public string SessionLabel { get; set; }
        /// <summary>
        /// A <see cref="DayOfWeek"/> list for the study session. These are the days where the study session will be notified again.
        /// </summary>
        public List<DayOfWeek> SessionRepeat { get; set; }
        /// <summary>
        /// Disables/Enables the study session notification
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// The time the user plans to read and study the pdf file. The field should be in <b>minutes.</b>
        /// </summary>
        public int ReadTime { get; set; }
        /// <summary>
        /// The remaining time in <b>minutes</b> on the study time for the current study session.
        /// </summary>
        public int RemainingTime { get; set; }
        public Session() {}

        public Session(int sessionID, DateTime sessTime, string sessLabel, List<DayOfWeek> SessRep, bool IsEnabled, string fileName, string filePath, int readTime)
        {
            SessionTime = sessTime;
            SessionLabel = sessLabel;
            SessionRepeat = SessRep;
            this.IsEnabled = IsEnabled;
            this.SessionID = sessionID;
            FileName = fileName;
            FilePath = filePath;
            ReadTime = RemainingTime = readTime;
        }

        public Session(DateTime SessTime, string SessLabel, List<DayOfWeek> SessRep, bool IsEnabled, string fileName, string filePath, int readTime)
        {
            SessionTime = SessTime;
            SessionLabel = SessLabel;
            SessionRepeat = SessRep;
            this.IsEnabled = IsEnabled;
            FilePath = filePath;
            FileName = fileName;
            ReadTime = RemainingTime = readTime;
        }
        public Session(Session session)
        {
            SessionTime = session.SessionTime;
            SessionLabel = session.SessionLabel;
            SessionRepeat = session.SessionRepeat;
            IsEnabled = session.IsEnabled;
            SessionID = session.SessionID;
            FileName = session.FileName;
            FilePath = session.FilePath;
            ReadTime = session.ReadTime;
            RemainingTime = session.RemainingTime;
        }
    }
}