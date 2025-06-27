using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class DetectedIntent
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
    }

    public class IntentDefinition
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public List<string> Patterns { get; set; }
    }

    public class NluEngine
    {
        private readonly List<IntentDefinition> _intentDefinitions;
        private readonly List<string> _taskTriggerWords = new List<string> { "task", "remind", "reminder", "to-do" };
        private readonly Regex _timeEntityRegex;

        public NluEngine()
        {
            // This powerful Regex finds common time-related phrases.
            _timeEntityRegex = new Regex(
                @"(in\s+\d+\s+(?:day|week|hour|minute)s?|tomorrow|next\s+\w+|on\s+\w+(\s+\d+(st|nd|rd|th)?)?|at\s+\d{1,2}(:\d{2})?\s*(am|pm)?)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            _intentDefinitions = InitializeIntents();
        }

        /// <summary>
        /// The main NLU processing pipeline. It now checks for complex tasks first.
        /// </summary>
        // Inside NluEngine.cs
        public DetectedIntent Process(string input)
        {
            string lowerInput = input.ToLower().Trim();

            // --- 1. Entity-First Task Detection (Highest Priority) ---
            var taskIntent = ExtractTaskIntent(lowerInput);
            if (taskIntent != null)
            {
                return taskIntent;
            }

            // --- 2. Standard Intent Pattern Matching ---
            // We create a prioritized list of intent names to check first.
            var prioritizedIntents = new List<string>
    {
        "ListTasks", "StartQuiz", "StopQuiz", "ViewLog", "ViewMoreLog", "RecallMemory", "Help"
    };

            // Check high-priority intents first
            foreach (var intentName in prioritizedIntents)
            {
                var intentDef = _intentDefinitions.FirstOrDefault(i => i.Name == intentName);
                if (intentDef != null)
                {
                    foreach (var pattern in intentDef.Patterns)
                    {
                        var match = Regex.Match(lowerInput, pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            return CreateDetectedIntent(intentDef, match);
                        }
                    }
                }
            }

            // Check remaining intents (including GetInfo, AcknowledgeInfo, Greeting, etc.)
            foreach (var intentDef in _intentDefinitions.Where(i => !prioritizedIntents.Contains(i.Name)))
            {
                foreach (var pattern in intentDef.Patterns)
                {
                    var match = Regex.Match(lowerInput, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        // Special check to prevent "what is my job" from being AcknowledgeInfo
                        if (intentDef.Name == "AcknowledgeInfo" && lowerInput.StartsWith("what"))
                        {
                            continue; // Skip this match, let it fall through to GetInfo or Fallback
                        }
                        return CreateDetectedIntent(intentDef, match);
                    }
                }
            }


            return new DetectedIntent { Name = "Fallback" };
        }

        // Helper method to reduce code duplication
        private DetectedIntent CreateDetectedIntent(IntentDefinition intentDef, Match match)
        {
            var detectedIntent = new DetectedIntent { Name = intentDef.Name, Topic = intentDef.Topic };
            foreach (var groupName in match.Groups.Keys.Cast<string>().Where(g => !int.TryParse(g, out _)))
            {
                detectedIntent.Entities[groupName] = match.Groups[groupName].Value.Trim();
            }
            return detectedIntent;
        }

        /// <summary>
        /// The new, flexible task detection logic.
        /// </summary>
        private DetectedIntent ExtractTaskIntent(string input)
        {
            if (!_taskTriggerWords.Any(trigger => input.Contains(trigger)))
            {
                return null; // Not a task intent.
            }

            string remainingText = input;
            string timePhrase = null;

            var timeMatch = _timeEntityRegex.Match(remainingText);
            if (timeMatch.Success)
            {
                timePhrase = timeMatch.Value;
                remainingText = remainingText.Replace(timePhrase, "").Trim();
            }
            
            var cleanupPatterns = new[] { "remind me to", "set a reminder for", "add task for", "add task", "new task:", "reminder", "remind me" };
            foreach (var pattern in cleanupPatterns)
            {
                remainingText = Regex.Replace(remainingText, $@"\b{pattern}\b", "", RegexOptions.IgnoreCase);
            }
            
            string taskDescription = Regex.Replace(remainingText.Trim(), @"\s+", " ").Trim(':', ' ', ',');

            if (!string.IsNullOrWhiteSpace(taskDescription))
            {
                var intent = new DetectedIntent
                {
                    Name = "CreateTask",
                    Entities = { ["task"] = taskDescription }
                };

                if (timePhrase != null)
                {
                    intent.Entities["time"] = timePhrase;
                }
                
                return intent;
            }
            
            return null;
        }

        /// <summary>
        /// Initializes all non-task related intents.
        /// </summary>
        private List<IntentDefinition> InitializeIntents()
        {
            var intents = new List<IntentDefinition>();

            // --- Information Intents (Hyper-Specific) ---
            AddTopicIntent(intents, "Password", new List<string>
            {
                "password",
                "passcode",
                "pass phrase",
                "credential"
            });
            AddTopicIntent(intents, "TwoFactorAuth", new List<string>
            {
                "2fa",
                "two factor",
                "mfa",
                "multi-factor",
                "authenticator",
                "verification code",
                "otp"
            });
            AddTopicIntent(intents, "Phishing", new List<string>
            {
                "phishing",
                "phish",
                "fake email",
                "smishing",
                "vishing"
            });
            AddTopicIntent(intents, "Malware", new List<string>
            {
                "malware",
                "virus",
                "spyware",
                "ransomware",
                "trojan",
                "antivirus"
            });
            AddTopicIntent(intents, "VPN", new List<string>
            {
                "vpn",
                "virtual private network"
            });
            AddTopicIntent(intents, "WiFiSecurity", new List<string>
            {
                "wifi",
                "wi-fi",
                "public wifi",
                "hotspot",
                "wpa2",
                "wpa3"
            });
            AddTopicIntent(intents, "DataBreach", new List<string>
            {
                "data breach",
                "hacked",
                "leaked",
                "compromised",
                "have i been pwned"
            });
            AddTopicIntent(intents, "Encryption", new List<string>
            {
                "encryption",
                "encrypt",
                "end-to-end",
                "e2ee",
                "bitlocker"
            });

            // --- Other Functional Intents ---
            intents.Add(new IntentDefinition
            {
                Name = "AcknowledgeInfo",
                Patterns = new List<string>
                {
                    // This regex is a "catch-all" for common ways people provide facts.
                    // It looks for phrases like "i am", "my job is", "i use", "i like", etc.
                    @"\b(i am|i'm|my job is|i work as|i use|i have|i like|i enjoy|i live in|i'm on|i am new to this|i am a beginner|i am experienced)\b"
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "ListTasks",
                Patterns = new List<string>
                {
                    @"(show|list|see|view|what are)\s+(my\s+)?(tasks|reminders|to-dos|todos)",
                    @"(display|get)\s+(my\s+)?(tasks|reminders|to-dos|todos)",
                    @"(what's|what is)\s+(my\s+)?(task list|reminder list|to-do list|todo list)"
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "StartQuiz",
                Patterns = new List<string>
                {
                    @"(start|take|begin)\s+quiz",
                    @"test my knowledge",
                    @"(i want to|let's)\s+do\s+quiz",
                    @"(i want to|let's)\s+take\s+quiz"
                    
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "StopQuiz",
                Patterns = new List<string>
                {
                    @"(stop|end|quit|exit)\s+quiz"
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "Confirm",
                Patterns = new List<string>
                {
                    @"\b(yes|yep|sure|ok|yeah)\b",
                    @"\b(agree|sounds good|go ahead)\b",
                    @"\b(please proceed|continue|let's do it)\b",
                    @"\b(affirmative|absolutely|definitely)\b"

                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "Deny",
                Patterns = new List<string>
                {
                    @"\b(no|nope|nah|don't)\b",
                    @"\b(not interested|not now|never)\b",
                    @"\b(leave me alone|go away|stop)\b"

                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "ViewLog",
                Patterns = new List<string>
            {
       
                    @"\b(show|view|display|list|see)\b.*\b(log|history|activity)\b"
            }
            });
            intents.Add(new IntentDefinition
            {
                Name = "ViewMoreLog",
                Patterns = new List<string>
                {
                    @"show\s+more",
                    @"next\s+page",
                    @"\b(more|next)\b"
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "Greeting",
                Patterns = new List<string>
                {
                    @"\b(hi|hello|hey)\b",
                    @"\b(good\s+morning|good\s+afternoon|good\s+evening)\b",
                    @"\b(how are you|how's it going|what's up)\b",
                    @"\b(welcome|nice to meet you|pleased to meet you)\b"

                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "ThankYou",
                Patterns = new List<string>
                {
                    @"\b(thanks|thank you)\b",
                    @"\b(appreciate|grateful)\b",
                    @"\b(thank you for your help|thanks for your assistance)\b",
                    @"\b(thanks for your help|thanks for your assistance)\b"
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "Help",
                Patterns = new List<string>
                {
                    @"\b(help|options|commands)\b",
                    @"what can you do",
                    @"what are your capabilities",
                    @"how can you assist me",
                }
            });
            intents.Add(new IntentDefinition
            {
                Name = "RecallMemory",
                Patterns = new List<string>
                {
                    @"what do you know about me",
                    @"what do you remember",
                    @"tell me about my info",
                    @"what have i told you",
                    @"what do you remember about me",
                    @"what do you know about my (job|skills|devices|age|services)",
                    @"what's my (job|skills|devices|age|services)"
                }
            });

            return intents;
        }

        private void AddTopicIntent(List<IntentDefinition> intents, string topicName, List<string> keywords)
        {
            string keywordPattern = string.Join("|", keywords.Select(Regex.Escape));
            string conversationalPattern = $@"(what is|what are|tell me about|how do i|info on)\s+.*?({keywordPattern})";
            string simplePattern = $@"\b({keywordPattern})s?\b";

            intents.Add(new IntentDefinition
            {
                Name = "GetInfo",
                Topic = topicName,
                Patterns = new List<string> { conversationalPattern, simplePattern }
            });
        }
    }
}