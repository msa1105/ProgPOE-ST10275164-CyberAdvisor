// --- SentimentAnalyzer.cs ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Color = System.Windows.Media.Color; // NEW: Added for WPF Color

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class SentimentAnalyzer
    {
        private readonly Dictionary<string, List<string>> sentimentKeywords;

        public SentimentAnalyzer()
        {
            sentimentKeywords = new Dictionary<string, List<string>>
            {
                ["worried"] = new List<string> { "worried", "concerned", "anxious", "scared", "afraid", "nervous", "panic", "stress" },
                ["curious"] = new List<string> { "curious", "interested", "wonder", "learn", "know more", "tell me", "explain", "how does" },
                ["frustrated"] = new List<string> { "frustrated", "annoyed", "angry", "mad", "upset", "irritated", "confused", "don't understand" },
                ["happy"] = new List<string> { "great", "awesome", "excellent", "wonderful", "amazing", "love", "like", "good", "nice" },
                ["confident"] = new List<string> { "confident", "sure", "ready", "prepared", "understand", "got it", "clear", "easy" },
                ["overwhelmed"] = new List<string> { "overwhelmed", "too much", "complicated", "difficult", "hard", "complex", "lost" }
            };
        }

        public string DetectSentiment(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "neutral";

            input = input.ToLower();
            var sentimentScores = new Dictionary<string, int>();

            foreach (var sentiment in sentimentKeywords)
            {
                int score = sentiment.Value.Count(keyword => input.Contains(keyword));
                if (score > 0)
                {
                    sentimentScores[sentiment.Key] = score;
                }
            }

            return sentimentScores.Any() ? sentimentScores.OrderByDescending(x => x.Value).First().Key : "neutral";
        }

        // MODIFIED: This now returns a WPF Color directly.
        public Color GetSentimentColor(string sentiment)
        {
            return sentiment switch
            {
                "worried" => Colors.IndianRed,
                "frustrated" => Colors.DarkRed,
                "overwhelmed" => Colors.Orange,
                "curious" => Colors.RoyalBlue,
                "happy" => Colors.MediumSeaGreen,
                "confident" => Colors.CadetBlue,
                "summary" => Colors.SlateGray,
                "suggestion" => Colors.Teal,
                "context" => Colors.DarkSlateGray,
                "error" => Colors.Firebrick,
                _ => Color.FromRgb(94, 129, 172) // Default bot color
            };
        }

        public string GetSentimentEmoji(string sentiment)
        {
            return sentiment switch
            {
                "worried" => "😟",
                "frustrated" => "😤",
                "overwhelmed" => "😵‍💫",
                "curious" => "🤔",
                "happy" => "😊",
                "confident" => "😎",
                "summary" => "📊",
                "suggestion" => "🔗",
                "context" => "💭",
                "error" => "⚠️",
                _ => "🤖"
            };
        }
    }
}