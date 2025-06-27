// --- NluEngine.cs (Advanced Phrase Recognition Version) ---

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    // These classes remain unchanged.
    public class DetectedIntent { public string Name { get; set; } public string Topic { get; set; } public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>(); }
    public class IntentDefinition { public string Name { get; set; } public string Topic { get; set; } public List<string> Patterns { get; set; } }

    public class NluEngine
    {
        private readonly List<IntentDefinition> _intentDefinitions;

        public NluEngine()
        {
            _intentDefinitions = InitializeIntents();
        }

        private List<IntentDefinition> InitializeIntents()
        {
            var intents = new List<IntentDefinition>();

            // --- Information Intents (Hyper-Specific with Advanced Phrase Patterns) ---
            AddTopicIntent(intents, "Password",
                keywords: new List<string> { "password", "passcode", "pass phrase", "credential" },
                phrases: new List<string> { @"how\s+(strong|good|secure)\s+is\s+my\s+password", @"password\s+(safety|security|hygiene|best practices)", @"create\s+a\s+strong\s+password" });

            AddTopicIntent(intents, "TwoFactorAuth",
                keywords: new List<string> { "2fa", "two factor", "mfa", "multi-factor", "authenticator", "verification code", "otp" },
                phrases: new List<string> { @"should\s+i\s+use\s+2fa", @"what\s+is\s+an\s+authenticator\s+app", @"is\s+sms\s+2fa\s+safe" });

            AddTopicIntent(intents, "Phishing",
                keywords: new List<string> { "phishing", "phish", "fake email", "smishing", "vishing" },
                phrases: new List<string> { @"how\s+to\s+spot\s+a\s+phishing\s+email", @"i\s+got\s+a\s+weird\s+(email|text)", @"report\s+phishing" });

            AddTopicIntent(intents, "Malware",
                keywords: new List<string> { "malware", "virus", "spyware", "ransomware", "trojan", "antivirus" },
                phrases: new List<string> { @"how\s+to\s+remove\s+a\s+virus", @"my\s+computer\s+is\s+acting\s+weird", @"do\s+i\s+need\s+antivirus" });

            AddTopicIntent(intents, "VPN",
                keywords: new List<string> { "vpn", "virtual private network" },
                phrases: new List<string> { @"should\s+i\s+use\s+a\s+vpn", @"how\s+does\s+a\s+vpn\s+work", @"is\s+a\s+free\s+vpn\s+safe" });

            AddTopicIntent(intents, "WiFiSecurity",
                keywords: new List<string> { "wifi", "wi-fi", "public wifi", "hotspot", "wpa2", "wpa3" },
                phrases: new List<string> { @"is\s+public\s+wifi\s+safe", @"secure\s+my\s+home\s+network", @"airport\s+wifi\s+security" });

            // (You can continue adding more topics with specific phrases here)
            AddTopicIntent(intents, "DataBreach", new List<string> { "data breach", "hacked", "leaked", "compromised", "have i been pwned" });
            AddTopicIntent(intents, "Encryption", new List<string> { "encryption", "encrypt", "end-to-end", "e2ee", "bitlocker" });

            // --- Functional Intents (Added more pattern variations) ---
            intents.Add(new IntentDefinition { Name = "CreateTask", Patterns = new List<string> { @"remind me to (?<task>.+?)\s+(on|at|in)\s+(?<time>.+)", @"set a reminder for (?<task>.+?)\s+(on|at|in)\s+(?<time>.+)", @"add task (?<task>.+?) for (?<time>.+)" } });
            intents.Add(new IntentDefinition { Name = "ListTasks", Patterns = new List<string> { @"(show|list|see|view|what are|check)\s+(my\s+)?(tasks|reminders|to-dos)" } });
            intents.Add(new IntentDefinition { Name = "StartQuiz", Patterns = new List<string> { @"(start|take|begin|do|launch)\s+quiz", @"test my knowledge", @"give me a quiz" } });
            intents.Add(new IntentDefinition { Name = "StopQuiz", Patterns = new List<string> { @"(stop|end|quit|exit)\s+quiz" } }); // New intent for robustness
            intents.Add(new IntentDefinition { Name = "ViewLog", Patterns = new List<string> { @"(show|view)\s+(my\s+)?(activity|log|history)" } });
            intents.Add(new IntentDefinition { Name = "ViewMoreLog", Patterns = new List<string> { @"show\s+more", @"next\s+page", @"\b(more|next)\b" } });
            intents.Add(new IntentDefinition { Name = "Greeting", Patterns = new List<string> { @"\b(hi|hello|hey|yo|howdy|good morning|good afternoon)\b" } });
            intents.Add(new IntentDefinition { Name = "ThankYou", Patterns = new List<string> { @"\b(thanks|thank you|thx|cheers|appreciated)\b" } });
            intents.Add(new IntentDefinition { Name = "Help", Patterns = new List<string> { @"\b(help|options|commands|what can you do)\b" } });

            return intents;
        }

        private void AddTopicIntent(List<IntentDefinition> intents, string topicName, List<string> keywords, List<string> phrases = null)
        {
            var allPatterns = new List<string>();
            if (phrases != null)
            {
                allPatterns.AddRange(phrases); // Add specific phrase patterns first
            }

            // Add conversational keyword patterns
            string keywordPattern = string.Join("|", keywords.Select(Regex.Escape));
            allPatterns.Add($@"(what is|what are|tell me about|how do i|explain|info on)\s+.*?({keywordPattern})");
            // Add simple keyword match as the lowest priority
            allPatterns.Add($@"\b({keywordPattern})s?\b");

            intents.Add(new IntentDefinition
            {
                Name = "GetInfo",
                Topic = topicName,
                Patterns = allPatterns
            });
        }

        public DetectedIntent Process(string input)
        {
            input = input.ToLower().Trim();
            foreach (var intentDef in _intentDefinitions)
            {
                foreach (var pattern in intentDef.Patterns)
                {
                    var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        var detectedIntent = new DetectedIntent { Name = intentDef.Name, Topic = intentDef.Topic };
                        foreach (var groupName in match.Groups.Keys.Cast<string>().Where(g => !int.TryParse(g, out _)))
                        {
                            detectedIntent.Entities[groupName] = match.Groups[groupName].Value.Trim();
                        }
                        return detectedIntent;
                    }
                }
            }
            return new DetectedIntent { Name = "Fallback" };
        }
    }
}