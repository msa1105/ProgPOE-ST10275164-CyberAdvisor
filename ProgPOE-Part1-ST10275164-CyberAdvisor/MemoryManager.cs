// --- MemoryManager.cs (Refactored Toolkit Version) ---

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class MemoryManager
    {
        // This is now the single entry point for memory extraction.
        // It's called on EVERY user input to ensure nothing is missed.
        public void ProcessInput(BotUser user, string input)
        {
            ExtractJobInfo(user, input);
            ExtractSkillLevel(user, input);
            ExtractDevices(user, input);
            ExtractServices(user, input);
            ExtractAge(user, input);
            // We don't store the interaction history here anymore, MainWindow does that via ActivityLogger.
        }

        // The private extraction methods are now more focused and robust.
        #region Private Extraction Methods

        private void ExtractJobInfo(BotUser user, string input)
        {
            var match = Regex.Match(input, @"(?:i work as|my job is|i'm an?)\s+([a-zA-Z\s]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var job = match.Groups[1].Value.Trim();
                user.RememberInfo("job", job);
                if (IsTechJob(job))
                {
                    user.RememberInfo("tech_level", "advanced");
                }
            }
        }

        private void ExtractSkillLevel(BotUser user, string input)
        {
            if (Regex.IsMatch(input, @"(i'm|i am)\s+(new to this|a beginner|just starting)", RegexOptions.IgnoreCase))
            {
                user.RememberInfo("skill_level", "beginner");
            }
            else if (Regex.IsMatch(input, @"(i'm|i am)\s+(experienced|an expert|advanced)", RegexOptions.IgnoreCase))
            {
                user.RememberInfo("skill_level", "advanced");
            }
        }

        private void ExtractDevices(BotUser user, string input)
        {
            var devices = new[] { "iphone", "android", "laptop", "pc", "mac", "computer", "tablet", "ipad", "windows"};
            foreach (var device in devices)
            {
                if (Regex.IsMatch(input, $@"\b(i use|i have|my)\s+(an? )?({device})\b", RegexOptions.IgnoreCase))
                {
                    var currentDevices = user.GetRememberedInfo("devices") ?? "";
                    if (!currentDevices.Contains(device))
                    {
                        user.RememberInfo("devices", (currentDevices + " " + device).Trim());
                    }
                }
            }
        }

        private void ExtractServices(BotUser user, string input)
        {
            var services = new[] { "facebook", "instagram", "twitter", "linkedin", "gmail", "outlook", "tiktok" };
            foreach (var service in services)
            {
                if (Regex.IsMatch(input, $@"\b(i use|i'm on)\s+({service})\b", RegexOptions.IgnoreCase))
                {
                    var currentServices = user.GetRememberedInfo("services") ?? "";
                    if (!currentServices.Contains(service))
                    {
                        user.RememberInfo("services", (currentServices + " " + service).Trim());
                    }
                }
            }
        }

        private void ExtractAge(BotUser user, string input)
        {
            var ageMatch = Regex.Match(input, @"i'm\s+(\d{1,2})\s*years?\s*old", RegexOptions.IgnoreCase);
            if (ageMatch.Success)
            {
                user.RememberInfo("age", ageMatch.Groups[1].Value);
            }
        }

        private bool IsTechJob(string job)
        {
            var techJobs = new[] { "developer", "programmer", "engineer", "it", "tech", "computer", "software", "data", "cybersecurity" };
            return techJobs.Any(tech => job.ToLower().Contains(tech));
        }

        #endregion

        #region Public Response Generation Methods

        /// <summary>
        /// Generates a personalized introductory phrase based on stored memories and the current topic.
        /// </summary>
        public string GeneratePersonalizedResponse(BotUser user, string topic)
        {
            var responses = new List<string>();
            var job = user.GetRememberedInfo("job");
            var skillLevel = user.GetRememberedInfo("skill_level");

            // Personalize based on skill level
            if (skillLevel == "beginner")
            {
                responses.Add("Since you mentioned you're new to this, let me break it down simply:");
            }
            else if (skillLevel == "advanced" || (job != null && IsTechJob(job)))
            {
                responses.Add($"Given your tech background, here's a more detailed perspective on {topic.ToLower()}:");
            }

            // Personalize based on devices for relevant topics
            var devices = user.GetRememberedInfo("devices");
            if (devices != null && topic == "TwoFactorAuth")
            {
                if (devices.Contains("iphone")) responses.Add("For your iPhone, setting this up in your Apple ID settings is a great start.");
                if (devices.Contains("android")) responses.Add("On your Android, securing your Google account with this is crucial.");
            }

            // If there are multiple possible personalizations, pick one at random.
            return responses.Any() ? responses[new Random().Next(responses.Count)] : "";
        }

        #endregion
    }
}