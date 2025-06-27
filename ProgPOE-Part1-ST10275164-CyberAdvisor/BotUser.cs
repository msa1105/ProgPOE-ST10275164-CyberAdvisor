// --- BotUser.cs ---
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class BotUser
    {
        public string Name { get; set; } = "Guest";
        public Dictionary<string, string> PersonalInfo { get; set; } = new Dictionary<string, string>();
        public List<string> InterestsTopics { get; set; } = new List<string>();
        public List<string> ConversationHistory { get; set; } = new List<string>();
        public DateTime SessionStartTime { get; set; } = DateTime.Now;
        public string LastSentiment { get; set; } = "neutral";
        public int InteractionCount { get; set; } = 0;

        // NEW: Property to hold user's tasks
        public List<CyberTask> Tasks { get; set; } = new List<CyberTask>();

        public void AddInterest(string topic)
        {
            if (!InterestsTopics.Contains(topic.ToLower()))
            {
                InterestsTopics.Add(topic.ToLower());
            }
        }

        public void RememberInfo(string key, string value)
        {
            PersonalInfo[key.ToLower()] = value;
        }

        public string GetRememberedInfo(string key)
        {
            return PersonalInfo.ContainsKey(key.ToLower()) ? PersonalInfo[key.ToLower()] : null;
        }

        public void AddToHistory(string interaction)
        {
            ConversationHistory.Add($"[{DateTime.Now:HH:mm:ss}] {interaction}");
            InteractionCount++;
        }

        public bool HasInterest(string topic)
        {
            return InterestsTopics.Any(interest => interest.Contains(topic.ToLower()));
        }
    }
}