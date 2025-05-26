using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class SentimentAnalyzer
    {
        private readonly Dictionary<string, List<string>> sentimentKeywords;

        public SentimentAnalyzer()
        {
            sentimentKeywords = new Dictionary<string, List<string>> // Reference: https://stackoverflow.com/questions/2099340/using-a-dictionary-with-list-values

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

            // Check for sentiment keywords with priority (more specific sentiments first)
            var sentimentScores = new Dictionary<string, int>();

            foreach (var sentiment in sentimentKeywords)
            {
                int score = sentiment.Value.Count(keyword => input.Contains(keyword));
                if (score > 0)
                {
                    sentimentScores[sentiment.Key] = score;
                }
            }

            if (sentimentScores.Any())
            {
                return sentimentScores.OrderByDescending(x => x.Value).First().Key;
            }

            return "neutral";
        }

        public ConsoleColor GetSentimentColor(string sentiment)
        {
            return sentiment switch
            {
                "worried" => ConsoleColor.Red,
                "frustrated" => ConsoleColor.DarkRed,
                "overwhelmed" => ConsoleColor.DarkYellow,
                "curious" => ConsoleColor.Blue,
                "happy" => ConsoleColor.Green,
                "confident" => ConsoleColor.Cyan,
                _ => ConsoleColor.White
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
                _ => "🤖"
            };
        }
    }
}