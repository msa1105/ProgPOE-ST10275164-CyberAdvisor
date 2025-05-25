using System;
using System.Media;
using System.Threading;
using Figgle;
using ProgPOE_Part1_ST10275164_CyberAdvisor;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static BotUser currentUser = new BotUser();
    static SentimentAnalyzer sentimentAnalyzer = new SentimentAnalyzer();
    static ResponseManager responseManager = new ResponseManager();
    static MemoryManager memoryManager = new MemoryManager();

    // Delegates for response processing
    static ResponseManager.ResponseFilter userInterestFilter = (user, topic) => user.HasInterest(topic);
    static ResponseManager.ResponseModifier personalizedModifier = (response, user, sentiment) =>
        $"[{sentiment.ToUpper()}] {response}";

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        PlayWelcomeAudio();      //plays welcome audio
        DisplayAsciiLogo();      //displays the ASCII art title
        AskForUserName();        //asks for users name
        ShowEnhancedWelcome();   //shows new enhanced welcome

        //this starts the chatbot loop
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n💬 {currentUser.Name}, ask me anything (type 'exit' to quit): ");
            Console.ResetColor();

            string userInput = Console.ReadLine();

            if (userInput?.ToLower() == "exit")
            {
                ShowExitSummary();
                break;
            }

            ProcessUserInput(userInput);
        }
    }

    //plays welcome greeting audio
    static void PlayWelcomeAudio()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("C:\\Users\\alexa\\source\\repos\\ProgPOE-Part1-ST10275164-CyberAdvisor\\ProgPOE-Part1-ST10275164-CyberAdvisor\\Assets\\welcome.wav"))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️ Error playing audio: " + ex.Message);
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🔊 Audio playback is not supported on this platform.");
            Console.ResetColor();
        }
    }

    //this displays the ASCII logo
    static void DisplayAsciiLogo()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(FiggleFonts.Slant.Render("CyberAdvisor"));
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("                     ═══ Enhanced AI Security Mentor ═══");
        Console.ResetColor();
        Thread.Sleep(100); // slight pause
    }

    //this asks the users name
    static void AskForUserName()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("🤖 What's your name? ");
        Console.ResetColor();

        string nameInput = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(nameInput))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("❌ Please enter a valid name: ");
            Console.ResetColor();
            nameInput = Console.ReadLine();
        }

        currentUser.Name = nameInput.Trim();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✨ Welcome, {currentUser.Name}! I'm your enhanced cybersecurity mentor.");
        Console.ResetColor();
    }

    static void ShowEnhancedWelcome()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n🛡️ NEW FEATURES:");
        Console.WriteLine("   • I can remember what interests you");
        Console.WriteLine("   • I adapt to your emotions and mood");
        Console.WriteLine("   • I provide personalized cybersecurity advice");
        Console.WriteLine("   • I give varied responses to keep things interesting");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n🎯 Try asking about: passwords, phishing, privacy, scams, malware, wifi, or 2fa");
        Console.WriteLine("💡 Tell me about yourself to get personalized advice!");
        Console.ResetColor();
    }

    static void ProcessUserInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            BotReply("🤔 I didn't catch that. Could you try again?", ConsoleColor.Yellow);
            return;
        }

        try
        {
            // Process memory and extract information
            memoryManager.ProcessInput(currentUser, input);

            // Detect sentiment
            string sentiment = sentimentAnalyzer.DetectSentiment(input);
            currentUser.LastSentiment = sentiment;

            // Find keywords in input
            var keywords = responseManager.FindKeywords(input);

            // Generate response based on input
            string response = GenerateIntelligentResponse(input, keywords, sentiment);

            // Display response with appropriate styling
            var sentimentColor = sentimentAnalyzer.GetSentimentColor(sentiment);
            var sentimentEmoji = sentimentAnalyzer.GetSentimentEmoji(sentiment);

            BotReply($"{sentimentEmoji} {response}", sentimentColor);

            // Add context or follow-up if appropriate
            AddContextualFollowUp(keywords, sentiment);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            BotReply($"⚠️ Oops! I encountered an error: {ex.Message}", ConsoleColor.Red);
            BotReply("🔄 Let's try that again, shall we?", ConsoleColor.Yellow);
            Console.ResetColor();
        }
    }

    static string GenerateIntelligentResponse(string input, List<string> keywords, string sentiment)
    {
        string response = "";

        // Handle sentiment-based responses first
        if (sentiment != "neutral")
        {
            var sentimentResponse = responseManager.GetSentimentResponse(sentiment, currentUser);
            if (!string.IsNullOrEmpty(sentimentResponse))
            {
                response = sentimentResponse + " ";
            }
        }

        // Handle specific patterns and conversations
        if (HandleSpecificPatterns(input, ref response))
        {
            return response;
        }

        // Handle keyword-based responses
        if (keywords.Any())
        {
            var keywordResponse = responseManager.GetKeywordResponse(keywords.First(), currentUser, sentiment);
            if (!string.IsNullOrEmpty(keywordResponse))
            {
                // Add personalized context
                var personalizedIntro = memoryManager.GeneratePersonalizedResponse(currentUser, keywords.First());
                if (!string.IsNullOrEmpty(personalizedIntro))
                {
                    response += personalizedIntro + " ";
                }

                response += keywordResponse;

                // Remember user's interest in this topic
                currentUser.AddInterest(keywords.First());

                return response;
            }
        }

        // Fallback to original responses for backward compatibility
        if (HandleLegacyResponses(input, ref response))
        {
            return response;
        }

        // Final fallback
        response += responseManager.GetFallbackResponse(currentUser);
        return response;
    }

    static bool HandleSpecificPatterns(string input, ref string response)
    {
        input = input.ToLower().Trim();

        // Personal information sharing
        if (input.Contains("my name is") || input.Contains("i'm ") || input.Contains("i am "))
        {
            response = $"Nice to meet you! I'll remember that about you, {currentUser.Name}. What would you like to learn about cybersecurity?";
            return true;
        }

        // Interest declarations
        if (input.Contains("interested in") || input.Contains("want to learn about"))
        {
            response = "Excellent! I love your enthusiasm for learning. I'll keep that in mind for our future conversations. What specific questions do you have?";
            return true;
        }

        // Memory recall requests
        if (input.Contains("what do you remember") || input.Contains("what did i tell you"))
        {
            var interests = string.Join(", ", currentUser.InterestsTopics);
            var info = currentUser.PersonalInfo.Any() ?
                string.Join(", ", currentUser.PersonalInfo.Select(kv => $"{kv.Key}: {kv.Value}")) : "nothing specific";

            response = $"Let me think... I remember you're interested in: {(interests.Any() ? interests : "We haven't discussed specific interests yet")}. " +
                      $"Other details: {info}. We've chatted {currentUser.InteractionCount} times in this session!";
            return true;
        }

        // Greeting responses
        if (input.Contains("hello") || input.Contains("hi ") || input.StartsWith("hi"))
        {
            var greetings = new[] {
                $"Hello there, {currentUser.Name}! Ready to boost your cybersecurity knowledge?",
                $"Hi {currentUser.Name}! What cybersecurity topic shall we explore today?",
                $"Hey {currentUser.Name}! Great to see you again. What's on your mind?",
                $"Hello! I'm excited to help you stay cyber safe, {currentUser.Name}!"
            };
            response = greetings[new Random().Next(greetings.Length)];
            return true;
        }

        // How are you responses
        if (input.Contains("how are you") || input.Contains("how you doing"))
        {
            var responses = new[] {
                $"I'm functioning perfectly and ready to help you stay secure online, {currentUser.Name}!",
                $"I'm doing great, thanks for asking! My circuits are buzzing with cybersecurity tips for you!",
                $"Excellent! I've been processing the latest security threats to better protect you, {currentUser.Name}!",
                $"I'm wonderful! Each conversation makes me better at helping people like you stay cyber safe!"
            };
            response = responses[new Random().Next(responses.Length)];
            return true;
        }

        return false;
    }

    static bool HandleLegacyResponses(string input, ref string response)
    {
        // Maintain backward compatibility with original responses
        if (input.Contains("do you") || input.Contains("how do you") || input.Contains("purpose"))
        {
            response = $"I'm here to be your personal cybersecurity mentor, {currentUser.Name}! I learn about you and adapt my advice to your needs.";
            return true;
        }

        if (input.Contains("ask you") || input.Contains("ask"))
        {
            response = "You can ask me about password safety, phishing, privacy, scams, malware, WiFi security, 2FA, and much more! I'll remember what interests you most.";
            return true;
        }

        if (input.Contains("help") || input.Contains("options"))
        {
            var helpResponse = "🔐 Try asking about: passwords, phishing, privacy, scams, malware, wifi, or 2fa\n" +
                              "💡 Tell me about yourself for personalized advice!\n" +
                              "🧠 I can remember your interests and adapt to your learning style!";

            if (currentUser.InterestsTopics.Any())
            {
                helpResponse += $"\n🎯 Based on our chat, you might also like to know more about: {string.Join(", ", currentUser.InterestsTopics)}";
            }

            response = helpResponse;
            return true;
        }

        if (input.Contains("thank"))
        {
            var thankResponses = new[] {
                $"You're absolutely welcome, {currentUser.Name}! Learning about cybersecurity is one of the best investments you can make!",
                $"My pleasure, {currentUser.Name}! I'm here whenever you need cybersecurity guidance!",
                $"Anytime, {currentUser.Name}! Your security is my priority. Feel free to ask me anything else!",
                $"Happy to help, {currentUser.Name}! Remember, staying cyber safe is a journey, not a destination!"
            };
            response = thankResponses[new Random().Next(thankResponses.Length)];
            return true;
        }

        if (input.Contains("bye") || input.Contains("see you") || input.Contains("later"))
        {
            ShowExitSummary();
            Environment.Exit(0);
            return true;
        }

        return false;
    }

    static void AddContextualFollowUp(List<string> keywords, string sentiment)
    {
        // Add follow-up suggestions based on context
        if (keywords.Any())
        {
            var contextualAdvice = memoryManager.RecallPreviousContext(currentUser, keywords.First());
            if (!string.IsNullOrEmpty(contextualAdvice))
            {
                Thread.Sleep(1000); // Brief pause before follow-up
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                BotReply($"💭 {contextualAdvice}", ConsoleColor.DarkCyan);
            }
        }

        // Suggest related topics
        if (keywords.Contains("password") && !currentUser.HasInterest("2fa"))
        {
            Thread.Sleep(500);
            BotReply("🔗 Since you're interested in passwords, you might also want to learn about two-factor authentication (2FA)!", ConsoleColor.Blue);
        }
        else if (keywords.Contains("phishing") && !currentUser.HasInterest("scam"))
        {
            Thread.Sleep(500);
            BotReply("🔗 Phishing is closely related to scams - would you like to know about other types of scams too?", ConsoleColor.Blue);
        }
        else if (keywords.Contains("wifi") && !currentUser.HasInterest("privacy"))
        {
            Thread.Sleep(500);
            BotReply("🔗 WiFi security ties into privacy protection - interested in learning more about online privacy?", ConsoleColor.Blue);
        }
    }

    static void ShowExitSummary()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n" + new string('═', 60));
        Console.WriteLine("              📊 SESSION SUMMARY");
        Console.WriteLine(new string('═', 60));

        Console.WriteLine($"👤 User: {currentUser.Name}");
        Console.WriteLine($"💬 Total interactions: {currentUser.InteractionCount}");
        Console.WriteLine($"⏱️ Session duration: {DateTime.Now - currentUser.SessionStartTime:mm\\:ss}");
        Console.WriteLine($"😊 Last detected mood: {currentUser.LastSentiment}");

        if (currentUser.InterestsTopics.Any())
        {
            Console.WriteLine($"🎯 Topics explored: {string.Join(", ", currentUser.InterestsTopics)}");
        }

        if (currentUser.PersonalInfo.Any())
        {
            Console.WriteLine($"🧠 Personal details remembered: {currentUser.PersonalInfo.Count} items");
        }

        Console.WriteLine("\n🛡️ Keep practicing good cybersecurity habits!");
        Console.WriteLine($"👋 Goodbye, {currentUser.Name}! Stay cyber safe!");
        Console.WriteLine(new string('═', 60));
        Console.ResetColor();
    }

    //Enhanced bot reply with typing effect and colors
    static void BotReply(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write("\n🤖 ");

        foreach (char c in message)
        {
            Console.Write(c);
            Thread.Sleep(c == ' ' ? 20 : 15); // Slightly faster typing, pause at spaces
        }

        Console.WriteLine();
        Console.ResetColor();
    }
}