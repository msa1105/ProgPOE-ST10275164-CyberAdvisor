using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class ResponseManager
    {
        private readonly Dictionary<string, List<string>> keywordResponses;
        private readonly Dictionary<string, List<string>> sentimentResponses;
        private readonly Random random;
        private readonly List<string> fallbackResponses;

        // Delegate for response filtering based on user data
        public delegate bool ResponseFilter(BotUser user, string topic);
        public delegate string ResponseModifier(string response, BotUser user, string sentiment);

        public ResponseManager()
        {
            random = new Random();
            keywordResponses = new Dictionary<string, List<string>>();
            sentimentResponses = new Dictionary<string, List<string>>();
            fallbackResponses = new List<string>();
            InitializeKeywordResponses();
            InitializeSentimentResponses();
            InitializeFallbackResponses();
        }

        private void InitializeKeywordResponses()
        {
            keywordResponses["password"] = new List<string>
            {
                "🔐 A strong password should be at least 12 characters long with a mix of letters, numbers, and symbols!",
                "🔒 Never reuse passwords across accounts - each should be unique like a fingerprint!",
                "🛡️ Consider using a password manager to generate and store complex passwords securely!",
                "⚡ Pro tip: Use passphrases like 'Coffee$Morning2024!' - they're easier to remember but hard to crack!"
            };
            keywordResponses["phishing"] = new List<string>
            {
                "🎣 Phishing emails often create urgency - 'Act now or lose your account!' Don't fall for it!",
                "🔍 Always check the sender's email carefully - scammers use look-alike addresses!",
                "🚫 Legitimate companies won't ask for passwords via email. When in doubt, contact them directly!",
                "🛡️ Hover over links before clicking - does the URL match what you'd expect?"
            };
            keywordResponses["privacy"] = new List<string>
            {
                "🕵️ Your privacy is precious! Review app permissions regularly - why does a flashlight app need your contacts?",
                "🔒 Use privacy-focused browsers and search engines to reduce data tracking!",
                "📱 Social media privacy settings change often - check them monthly!",
                "🌐 Consider using a VPN to encrypt your internet traffic and protect your browsing habits!"
            };
            keywordResponses["scam"] = new List<string>
            {
                "⚠️ If it sounds too good to be true, it probably is! Trust your instincts!",
                "📞 Scammers often pressure you to act quickly - legitimate businesses give you time to think!",
                "💰 Never send money, gift cards, or personal info to someone you've never met in person!",
                "🔍 Research unknown contacts online - many scam phone numbers and emails are reported by others!"
            };
            keywordResponses["malware"] = new List<string>
            {
                "🦠 Keep your antivirus updated and run regular scans - prevention is better than cure!",
                "⬇️ Only download software from official sources - third-party sites often bundle malware!",
                "🔄 Keep all your software updated - security patches fix vulnerabilities that malware exploits!",
                "🚫 Be suspicious of unexpected pop-ups claiming your computer is infected - they're often malware themselves!"
            };
            keywordResponses["wifi"] = new List<string>
            {
                "📶 Public WiFi is like a postcard - anyone can read what you're sending!",
                "🔐 Use a VPN on public networks to encrypt your data and stay invisible to hackers!",
                "🏠 Set up a guest network at home to keep your main devices separate from visitors!",
                "⚠️ Avoid online banking or shopping on public WiFi - wait until you're on a secure network!"
            };
            keywordResponses["2fa"] = new List<string>
            {
                "🛡️ Two-Factor Authentication is like having two locks on your door - much safer!",
                "📱 Use authenticator apps instead of SMS when possible - they're more secure!",
                "🔑 Enable 2FA on all important accounts: email, banking, social media, and work accounts!",
                "💡 Keep backup codes in a safe place - you'll need them if you lose your phone!"
            };
        }

        private void InitializeSentimentResponses()
        {
            sentimentResponses["worried"] = new List<string>
            {
                "I understand your concerns - cybersecurity can feel overwhelming, but you're taking the right steps by learning!",
                "It's completely natural to feel worried about online threats. Let's break this down into manageable steps.",
                "Your caution is actually a strength! Awareness is the first line of defense against cyber threats.",
                "Don't worry - with the right knowledge and habits, you can significantly reduce your risk online."
            };
            sentimentResponses["frustrated"] = new List<string>
            {
                "I can sense your frustration. Let me try explaining this in a simpler way.",
                "Cybersecurity can be confusing at first, but don't give up! You're building important skills.",
                "I understand this might be overwhelming. Let's focus on one thing at a time.",
                "Take a deep breath - even cybersecurity experts started where you are now!"
            };
            sentimentResponses["curious"] = new List<string>
            {
                "I love your curiosity! That's exactly the right attitude for staying cyber safe.",
                "Great question! Your eagerness to learn will serve you well in cybersecurity.",
                "Your curiosity is fantastic - it shows you're thinking critically about online safety!",
                "Excellent! The more you know, the better protected you'll be online."
            };
            sentimentResponses["overwhelmed"] = new List<string>
            {
                "Let's slow down and focus on just the basics for now. You don't need to learn everything at once!",
                "I can see this might be a lot to process. How about we start with just one simple step?",
                "Don't worry about mastering everything immediately - cybersecurity is a journey, not a race!",
                "Let's break this down into smaller, manageable pieces. What would you like to focus on first?"
            };
            sentimentResponses["confident"] = new List<string>
            {
                "That's the spirit! Your confidence will help you make better security decisions.",
                "Excellent attitude! Confidence combined with knowledge makes you a hard target for cybercriminals.",
                "I'm glad you're feeling confident! Keep that energy as you continue learning.",
                "Perfect! With that mindset, you'll be a cybersecurity champion in no time!"
            };
        }

        private void InitializeFallbackResponses()
        {
            fallbackResponses.Add("I'm not sure I understand that. Can you try rephrasing your question?");
            fallbackResponses.Add("Hmm, that's not something I'm familiar with. Could you ask about password safety, phishing, or privacy instead?");
            fallbackResponses.Add("I'd love to help, but I didn't quite catch that. Try asking about cybersecurity topics like scams or malware!");
            fallbackResponses.Add("Let me think... I'm not sure about that one. How about asking me about WiFi safety or two-factor authentication?");
            fallbackResponses.Add("That's outside my expertise area. I'm great with cybersecurity topics though - try asking about those!");
        }

        public string GetKeywordResponse(string keyword, BotUser user, string sentiment = "neutral")
        {
            // Find matching keyword
            var matchingKeyword = keywordResponses.Keys.FirstOrDefault(k =>
                keyword.ToLower().Contains(k) || k.Contains(keyword.ToLower()));

            if (matchingKeyword != null)
            {
                var responses = keywordResponses[matchingKeyword];
                var baseResponse = responses[random.Next(responses.Count)];

                // Add personal touch if user has shown interest in this topic before
                if (user.HasInterest(matchingKeyword))
                {
                    baseResponse = $"Since you're interested in {matchingKeyword}, here's another tip: " + baseResponse;
                }

                return ModifyResponseForSentiment(baseResponse, sentiment, user);
            }

            return null;
        }

        public string GetSentimentResponse(string sentiment, BotUser user)
        {
            if (sentimentResponses.ContainsKey(sentiment))
            {
                var responses = sentimentResponses[sentiment];
                return responses[random.Next(responses.Count)];
            }
            return "";
        }

        public string GetFallbackResponse(BotUser user)
        {
            var response = fallbackResponses[random.Next(fallbackResponses.Count)];

            // Personalize based on user's interests
            if (user.InterestsTopics.Any())
            {
                var interest = user.InterestsTopics.First();
                response += $" Since you've been interested in {interest}, would you like to know more about that?";
            }

            return response;
        }

        private string ModifyResponseForSentiment(string response, string sentiment, BotUser user)
        {
            return sentiment switch
            {
                "worried" or "frustrated" or "overwhelmed" => $"Hey {user.Name}, take it easy. " + response,
                "curious" => $"{user.Name}, " + response,
                "confident" or "happy" => $"Awesome, {user.Name}! " + response,
                _ => response
            };
        }

        public List<string> FindKeywords(string input)
        {
            var foundKeywords = new List<string>();
            var inputLower = input.ToLower();

            foreach (var keyword in keywordResponses.Keys)
            {
                if (inputLower.Contains(keyword))
                {
                    foundKeywords.Add(keyword);
                }
            }

            return foundKeywords;
        }

        // Method using delegates for advanced filtering
        public string GetFilteredResponse(string input, BotUser user, ResponseFilter filter)
        {
            var keywords = FindKeywords(input);

            foreach (var keyword in keywords)
            {
                if (filter(user, keyword))
                {
                    return GetKeywordResponse(keyword, user);
                }
            }

            return GetFallbackResponse(user);
        }
    }
}