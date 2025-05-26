using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class MemoryManager // Reference: https://stackoverflow.com/questions/16199015/regex-to-match-age-in-a-sentence
                               // Reference: https://stackoverflow.com/questions/13206839/how-to-use-a-delegate-in-c-sharp


    {
        private  Dictionary<string, Action<BotUser, string>> memoryPatterns;
        private  List<string> personalityTraits;

        // Delegate for memory processing
        public delegate void MemoryProcessor(BotUser user, string information);
        public delegate string MemoryRecaller(BotUser user, string context);

        public MemoryManager()
        {
            InitializeMemoryPatterns();
            personalityTraits = new List<string> { "cautious", "curious", "tech-savvy", "beginner", "experienced" };
        }

        private void InitializeMemoryPatterns() // Reference: https://stackoverflow.com/questions/2099340/using-a-dictionary-with-list-values

        {
            memoryPatterns = new Dictionary<string, Action<BotUser, string>>
            {
                ["interested in"] = (user, input) => ExtractInterest(user, input),
                ["my job"] = (user, input) => ExtractJobInfo(user, input),
                ["i work"] = (user, input) => ExtractJobInfo(user, input),
                ["i am"] = (user, input) => ExtractPersonalInfo(user, input),
                ["i have"] = (user, input) => ExtractPossessionInfo(user, input),
                ["i use"] = (user, input) => ExtractUsageInfo(user, input),
                ["my age"] = (user, input) => ExtractAge(user, input),
                ["years old"] = (user, input) => ExtractAge(user, input)
            };
        }

        public void ProcessInput(BotUser user, string input)
        {
            var inputLower = input.ToLower();

            foreach (var pattern in memoryPatterns)
            {
                if (inputLower.Contains(pattern.Key))
                {
                    pattern.Value(user, input);
                }
            }

            // Store the interaction in history
            user.AddToHistory($"User: {input}");
        }

        private void ExtractInterest(BotUser user, string input)
        {
            var match = Regex.Match(input.ToLower(), @"interested in (\w+(?:\s+\w+)?)");
            if (match.Success)
            {
                var interest = match.Groups[1].Value.Trim();
                user.AddInterest(interest);
                user.RememberInfo("last_interest", interest);
            }
        }

        private void ExtractJobInfo(BotUser user, string input)
        {
            var patterns = new[]
            {
                @"my job (?:is|as) (\w+(?:\s+\w+)*)",
                @"i work (?:as|in) (\w+(?:\s+\w+)*)",
                @"i am (?:a|an) (\w+(?:\s+\w+)*)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(input.ToLower(), pattern);
                if (match.Success)
                {
                    var job = match.Groups[1].Value.Trim();
                    user.RememberInfo("job", job);

                    // Determine tech-savviness based on job
                    if (IsTechJob(job))
                    {
                        user.RememberInfo("tech_level", "advanced");
                    }
                    break;
                }
            }
        }

        private void ExtractPersonalInfo(BotUser user, string input)
        {
            var traits = new[] { "new to", "beginner", "experienced", "expert", "learning", "studying" };

            foreach (var trait in traits)
            {
                if (input.ToLower().Contains(trait))
                {
                    user.RememberInfo("skill_level", trait);
                    break;
                }
            }
        }

        private void ExtractPossessionInfo(BotUser user, string input)
        {
            var devices = new[] { "iphone", "android", "laptop", "computer", "tablet", "ipad" };

            foreach (var device in devices)
            {
                if (input.ToLower().Contains(device))
                {
                    var currentDevices = user.GetRememberedInfo("devices") ?? "";
                    if (!currentDevices.Contains(device))
                    {
                        user.RememberInfo("devices", currentDevices + " " + device);
                    }
                }
            }
        }

        private void ExtractUsageInfo(BotUser user, string input)
        {
            var services = new[] { "facebook", "instagram", "twitter", "linkedin", "gmail", "outlook" };

            foreach (var service in services)
            {
                if (input.ToLower().Contains(service))
                {
                    var currentServices = user.GetRememberedInfo("services") ?? "";
                    if (!currentServices.Contains(service))
                    {
                        user.RememberInfo("services", currentServices + " " + service);
                    }
                }
            }
        }

        private void ExtractAge(BotUser user, string input) // Reference: https://stackoverflow.com/questions/16199015/regex-to-match-age-in-a-sentence

        {
            var ageMatch = Regex.Match(input, @"(\d{1,2})\s*years?\s*old");
            if (ageMatch.Success)
            {
                user.RememberInfo("age", ageMatch.Groups[1].Value);
            }
        }

        private bool IsTechJob(string job)
        {
            var techJobs = new[] { "developer", "programmer", "engineer", "it", "tech", "computer", "software", "data", "cyber" };
            return techJobs.Any(tech => job.Contains(tech));
        }

        public string GeneratePersonalizedResponse(BotUser user, string topic)
        {
            var responses = new List<string>();

            // Check user's job for personalization
            var job = user.GetRememberedInfo("job");
            if (!string.IsNullOrEmpty(job))
            {
                if (IsTechJob(job))
                {
                    responses.Add($"Given your background in {job}, you probably already know some of this, but here's a refresher:");
                }
                else
                {
                    responses.Add($"As someone working in {job}, this cybersecurity tip might be especially relevant:");
                }
            }

            // Check user's interests
            if (user.HasInterest(topic))
            {
                responses.Add($"Since you've shown interest in {topic} before, here's something more advanced:");
            }

            // Check skill level
            var skillLevel = user.GetRememberedInfo("skill_level");
            if (!string.IsNullOrEmpty(skillLevel))
            {
                if (skillLevel.Contains("beginner") || skillLevel.Contains("new"))
                {
                    responses.Add("Since you mentioned you're new to this, let me explain it simply:");
                }
                else if (skillLevel.Contains("experienced"))
                {
                    responses.Add("Given your experience, here's a more detailed explanation:");
                }
            }

            // Check devices for relevant advice
            var devices = user.GetRememberedInfo("devices");
            if (!string.IsNullOrEmpty(devices) && topic.Contains("mobile") || topic.Contains("phone"))
            {
                if (devices.Contains("iphone"))
                {
                    responses.Add("For your iPhone specifically:");
                }
                else if (devices.Contains("android"))
                {
                    responses.Add("For your Android device:");
                }
            }

            return responses.Any() ? responses[new Random().Next(responses.Count)] : "";
        }

        public string RecallPreviousContext(BotUser user, string currentTopic)
        {
            var contextResponses = new List<string>();

            // Reference previous interests
            if (user.InterestsTopics.Any())
            {
                var lastInterest = user.InterestsTopics.Last();
                if (lastInterest != currentTopic.ToLower())
                {
                    contextResponses.Add($"Earlier you were interested in {lastInterest}, and now {currentTopic} - you're building a solid security foundation!");
                }
            }

            // Reference interaction count
            if (user.InteractionCount > 10)
            {
                contextResponses.Add($"We've had quite a conversation, {user.Name}! You're really committed to learning about cybersecurity.");
            }

            // Reference session time
            var sessionTime = DateTime.Now - user.SessionStartTime;
            if (sessionTime.TotalMinutes > 30)
            {
                contextResponses.Add($"You've been learning for a while now, {user.Name}. That dedication will pay off!");
            }

            return contextResponses.Any() ? contextResponses[new Random().Next(contextResponses.Count)] : "";
        }

        // Using delegates for advanced memory operations
        public void ProcessMemoryWithDelegate(BotUser user, string input, MemoryProcessor processor)
        {
            processor(user, input);
        }

        public string RecallWithDelegate(BotUser user, string context, MemoryRecaller recaller)
        {
            return recaller(user, context);
        }
    }
}