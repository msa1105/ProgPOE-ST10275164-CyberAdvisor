// --- ActivityLog.cs ---
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public enum ActivityType { Chat, Task, Quiz, System }

    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public ActivityType Type { get; set; }
        public string Description { get; set; }

        public string FormattedString => $"[{Timestamp:HH:mm:ss}] [{Type}] {Description}";
    }

    public static class ActivityLogger
    {
        private static readonly List<ActivityLogEntry> _log = new List<ActivityLogEntry>();

        public static void Log(ActivityType type, string description)
        {
            _log.Add(new ActivityLogEntry { Type = type, Description = description });
        }

        public static List<ActivityLogEntry> GetLogEntries()
        {
            // Return a copy in reverse chronological order
            return _log.OrderByDescending(e => e.Timestamp).ToList();
        }

        public static void Clear()
        {
            _log.Clear();
        }
    }
}