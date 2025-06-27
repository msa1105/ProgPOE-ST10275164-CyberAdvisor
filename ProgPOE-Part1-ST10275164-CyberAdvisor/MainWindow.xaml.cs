using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public partial class MainWindow : Window
    {
        #region Fields and Properties

        // Core Logic Components
        private BotUser currentUser;
        private readonly NluEngine nluEngine;
        private readonly MemoryManager memoryManager;
        private readonly ResponseManager responseManager;
        private readonly SentimentAnalyzer sentimentAnalyzer;

        // UI Components & State Management
        private readonly DispatcherTimer typingIndicatorTimer;
        private const string PlaceholderText = "Ask me anything, or type 'help' for commands...";

        private bool _isQuizActive = false;
        private List<QuizQuestion> _quizQuestions;
        private int _currentQuizQuestionIndex;
        private int _quizScore;
        private CyberTask _taskWaitingForReminder;
        private int _logPageIndex = 0;

        #endregion

        #region Initialization and Session Management

        public MainWindow()
        {
            InitializeComponent();
            nluEngine = new NluEngine();
            memoryManager = new MemoryManager();
            responseManager = new ResponseManager();
            sentimentAnalyzer = new SentimentAnalyzer();
            typingIndicatorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            typingIndicatorTimer.Tick += TypingIndicatorTimer_Tick;
            StartNewSession();
        }

        private void StartNewSession()
        {
            currentUser = new BotUser();
            ChatPanel.Children.Clear();
            ActivityLogger.Clear();
            ActivityLogger.Log(ActivityType.System, "New session started.");

            _isQuizActive = false;
            _logPageIndex = -1;
            _taskWaitingForReminder = null;

            InputTextBox.Text = PlaceholderText;
            InputTextBox.Foreground = Brushes.Gray;
            PlayWelcomeAudio();
            ShowWelcomeSequence();
        }

        private async void ShowWelcomeSequence()
        {
            await AddBotMessageWithDelay("🛡️ Welcome to CyberAdvisor! I'm your enhanced AI security mentor.", "neutral", 500);
            await AddBotMessageWithDelay("First, what should I call you? (e.g., 'my name is Alex')", "neutral", 1200);
        }

        #endregion

        #region Main User Input Loop

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await HandleUserInput();
        }

        private async void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await HandleUserInput();
            }
        }

        private async Task HandleUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput) || userInput == PlaceholderText) return;

            AddUserMessageToChat(userInput);
            ResetInputBox();

            ShowTypingIndicator(true);
            await Task.Delay(new Random().Next(400, 800));

            // Main conversational state routing
            if (_isQuizActive)
            {
                await ProcessQuizInputAsync(userInput);
            }
            else if (_taskWaitingForReminder != null)
            {
                await ProcessReminderConfirmationAsync(userInput);
            }
            else
            {
                string botResponse = await ProcessNluIntentAsync(userInput);
                if (!string.IsNullOrEmpty(botResponse))
                {
                    await AddBotMessageWithDelay(botResponse, "neutral", 0);
                }
            }

            ShowTypingIndicator(false);
        }

        #endregion

        #region NLU Intent Processing

        private async Task<string> ProcessNluIntentAsync(string input)
        {
            memoryManager.ProcessInput(currentUser, input);
            DetectedIntent intent = nluEngine.Process(input);
            string response = "";

            switch (intent.Name)
            {
                case "CreateTask":
                    response = HandleCreateTaskIntent(intent);
                    break;
                case "GetInfo":
                    string personalization = memoryManager.GeneratePersonalizedResponse(currentUser, intent.Topic);
                    string mainResponse = responseManager.GetKeywordResponse(intent.Topic, currentUser);
                    response = $"{personalization} {mainResponse}".Trim();
                    break;
                case "ListTasks":
                    ActivityLogger.Log(ActivityType.System, "User listed their current tasks.");
                    response = HandleListTasksIntent();
                    break;
                case "StartQuiz":
                    await StartQuizAsync();
                    // The response is now handled inside StartQuizAsync to ensure it always appears
                    break;
                case "RecallMemory":
                    ActivityLogger.Log(ActivityType.System, "User requested a memory recall.");
                    response = HandleRecallMemoryIntent();
                    break;
                case "AcknowledgeInfo":
                    ActivityLogger.Log(ActivityType.System, "User provided personal info, memory updated.");
                    response = "Thanks, I'll remember that for our conversation!";
                    break;

               
                case "ViewLog":
                    _logPageIndex = 0; // Reset to the first page
                    response = FormatLogPage();
                    break;
                case "ViewMoreLog":
                    if (_logPageIndex == -1)
                        response = "Please ask to see the log first.";
                    else
                    {
                        _logPageIndex++;
                        response = FormatLogPage();
                    }
                    break;
               

                case "Greeting":
                    response = $"Hello {currentUser.Name}! How can I assist you today?";
                    _logPageIndex = -1;
                    break;
                case "ThankYou":
                    response = $"You're welcome, {currentUser.Name}! Stay safe online.";
                    break;
                case "Help":
                    ShowHelp();
                    break;
                case "Fallback":
                default:
                    _logPageIndex = -1;
                    response = responseManager.GetFallbackResponse(currentUser);
                    break;
            }

            var nameMatch = Regex.Match(input, @"(?:my name is|call me|i am)\s+([A-Za-z]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success)
            {
                currentUser.Name = nameMatch.Groups[1].Value.Trim();
                UpdateUserNameDisplay();
                if (intent.Name == "Fallback" || intent.Name == "Greeting")
                {
                    response = $"Got it! Nice to meet you, {currentUser.Name}.";
                }
            }

            return response;
        }

        #endregion

        #region Task and Reminder Logic

        private string HandleCreateTaskIntent(DetectedIntent intent)
        {
            if (!intent.Entities.TryGetValue("task", out var taskDesc))
            {
                return responseManager.GetFallbackResponse(currentUser);
            }

            if (intent.Entities.TryGetValue("time", out var timeDesc))
            {
                if (DateTimeParser.TryParse(timeDesc, out DateTime dueDate))
                {
                    var newTask = new CyberTask { Title = taskDesc, Description = "Created via chat.", DueDate = dueDate };
                    currentUser.Tasks.Add(newTask);
                    ActivityLogger.Log(ActivityType.Task, $"Reminder set: '{taskDesc}' for {dueDate:g}");
                    return $"✅ Got it! I will remind you to '{taskDesc}' on {dueDate:g}.";
                }
                else
                {
                    _taskWaitingForReminder = new CyberTask { Title = taskDesc, Description = "Created via chat." };
                    return $"✅ Task '{taskDesc}' added. I had trouble understanding the date. When should I remind you?";
                }
            }
            else
            {
                _taskWaitingForReminder = new CyberTask { Title = taskDesc, Description = "Created via chat." };
                return $"✅ Task '{taskDesc}' has been added. Would you like to set a reminder for it?";
            }
        }

        private async Task ProcessReminderConfirmationAsync(string userInput)
        {
            string response;
            DetectedIntent intent = nluEngine.Process(userInput);

            if (intent.Name == "Deny")
            {
                currentUser.Tasks.Add(_taskWaitingForReminder);
                ActivityLogger.Log(ActivityType.Task, $"Task added: '{_taskWaitingForReminder.Title}' (no reminder).");
                response = "Okay, I've added it to your list with no reminder.";
            }
            else
            {
                if (DateTimeParser.TryParse(userInput, out DateTime dueDate))
                {
                    _taskWaitingForReminder.DueDate = dueDate;
                    currentUser.Tasks.Add(_taskWaitingForReminder);
                    ActivityLogger.Log(ActivityType.Task, $"Reminder set: '{_taskWaitingForReminder.Title}' for {dueDate:g}");
                    response = $"✅ Excellent, reminder set for {dueDate:g}.";
                }
                else
                {
                    response = "Great! When would you like to be reminded? (e.g., 'in 3 days', 'tomorrow at noon')";
                    await AddBotMessageWithDelay(response, "neutral", 0);
                    return;
                }
            }

            _taskWaitingForReminder = null;
            await AddBotMessageWithDelay(response, "neutral", 0);
        }

        private string HandleListTasksIntent()
        {
            var pendingTasks = currentUser.Tasks.Where(t => !t.IsCompleted).ToList();
            if (!pendingTasks.Any()) return "You have no pending tasks or reminders.";

            var response = "Here are your current reminders:\n";
            response += string.Join("\n", pendingTasks.OrderBy(t => t.DueDate.HasValue ? t.DueDate.Value : DateTime.MaxValue)
                                                        .Select(t => $"• {t.Title} (Due: {(t.DueDate.HasValue ? t.DueDate.Value.ToString("g") : "N/A")})"));
            return response;
        }

        #endregion

        #region Quiz, Log, Memory, and Other Feature Logic

        private string HandleRecallMemoryIntent()
        {
            if (!currentUser.PersonalInfo.Any()) return "You haven't told me anything personal about yourself yet.";

            var response = "Here's what I remember about you:\n";
            foreach (var entry in currentUser.PersonalInfo)
            {
                var key = char.ToUpper(entry.Key[0]) + entry.Key.Substring(1).Replace("_", " ");
                response += $"\n• {key}: {entry.Value}";
            }
            return response;
        }

        private async Task ProcessQuizInputAsync(string input)
        {
            DetectedIntent quizTimeIntent = nluEngine.Process(input);
            if (quizTimeIntent.Name == "StopQuiz")
            {
                await EndQuizAsync(interrupted: true);
            }
            else
            {
                await ProcessQuizAnswerAsync(input);
            }
        }

        private async Task StartQuizAsync()
        {
            const int QuizLength = 10;
            var masterQuestionBank = InitializeQuestions();
            _quizQuestions = masterQuestionBank.OrderBy(q => Guid.NewGuid()).Take(QuizLength).ToList();

            _isQuizActive = true;
            _quizScore = 0;
            _currentQuizQuestionIndex = 0;
            ActivityLogger.Log(ActivityType.Quiz, $"Quiz started with {QuizLength} random questions.");

            await AddBotMessageWithDelay("🚀 Starting a random quiz! Type 'stop quiz' at any time to end it.", "suggestion", 200);
            await AskNextQuestionAsync();
        }

        private async Task AskNextQuestionAsync()
        {
            if (_currentQuizQuestionIndex >= _quizQuestions.Count) { await EndQuizAsync(); return; }

            var q = _quizQuestions[_currentQuizQuestionIndex];
            var questionText = $"❓ Question {_currentQuizQuestionIndex + 1}/{_quizQuestions.Count}:\n{q.QuestionText}\n\n";
            questionText += string.Join("\n", q.Options.Select((opt, i) => $"{i + 1}. {opt}"));
            questionText += "\n\nType the number of your answer.";

            await AddBotMessageWithDelay(questionText, "neutral", 500);
        }

        private async Task ProcessQuizAnswerAsync(string userAnswer)
        {
            var currentQuestion = _quizQuestions[_currentQuizQuestionIndex];
            if (int.TryParse(userAnswer, out int answerIndex) && answerIndex > 0 && answerIndex <= currentQuestion.Options.Count)
            {
                if ((answerIndex - 1) == currentQuestion.CorrectAnswerIndex)
                {
                    _quizScore++;
                    await AddBotMessageWithDelay($"✅ Correct! {currentQuestion.Explanation}", "happy", 200);
                }
                else
                {
                    await AddBotMessageWithDelay($"❌ Incorrect. The correct answer was {currentQuestion.CorrectAnswerIndex + 1}. {currentQuestion.Explanation}", "worried", 200);
                }
                _currentQuizQuestionIndex++;
                await Task.Delay(1500);
                await AskNextQuestionAsync();
            }
            else
            {
                await AddBotMessageWithDelay($"Please enter a valid number between 1 and {currentQuestion.Options.Count}, or type 'stop quiz' to exit.", "error", 200);
            }
        }

        private async Task EndQuizAsync(bool interrupted = false)
        {
            if (interrupted)
            {
                await AddBotMessageWithDelay("Quiz stopped. Let me know when you want to start again!", "neutral", 200);
                ActivityLogger.Log(ActivityType.Quiz, "Quiz stopped by user.");
            }
            else
            {
                string description = _quizScore >= 8 ? "Excellent! You're a cybersecurity expert!" : _quizScore >= 5 ? "Great job! A solid understanding." : "A good start, but keep learning!";
                var summary = $"🏁 Quiz Complete! Your final score is: {_quizScore}/{_quizQuestions.Count}\n\n{description}";
                await AddBotMessageWithDelay(summary, "summary", 500);
                ActivityLogger.Log(ActivityType.Quiz, $"Quiz finished with score: {_quizScore}/{_quizQuestions.Count}");
            }
            _isQuizActive = false;
        }

        private string FormatLogPage()
        {
            const int PageSize = 5;
            var allEntries = ActivityLogger.GetLogEntries();
            if (!allEntries.Any()) return "📜 There is no activity to show yet.";

            var pagedEntries = allEntries.Skip(_logPageIndex * PageSize).Take(PageSize).ToList();
            if (!pagedEntries.Any())
            {
                _logPageIndex = -1;
                return "📜 You've reached the end of your activity log.";
            }

            int totalPages = (int)Math.Ceiling((double)allEntries.Count / PageSize);
            var response = $"📜 Activity Log (Page {_logPageIndex + 1} of {totalPages}):\n\n";
            response += string.Join("\n", pagedEntries.Select(e => e.FormattedString));

            if ((_logPageIndex + 1) * PageSize < allEntries.Count)
            {
                response += "\n\nType 'more' or 'next' to see the next page.";
            }
            return response;
        }

        private List<QuizQuestion> InitializeQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion { QuestionText = "What is the most important factor for a strong password?", Options = new List<string> { "Length", "Complexity (using !@#$)", "Using your pet's name" }, CorrectAnswerIndex = 0, Explanation = "Length is the single most important factor. A long passphrase is much harder to crack than a short, complex one." },
                new QuizQuestion { QuestionText = "A 'password manager' is a type of malware.", Options = new List<string> { "True", "False" }, CorrectAnswerIndex = 1, Explanation = "False. A password manager is a secure tool that helps you create and store unique, strong passwords for all your accounts." },
                new QuizQuestion { QuestionText = "Which method of 2FA is generally considered the most secure?", Options = new List<string> { "SMS (text message)", "Authenticator App", "Hardware Security Key" }, CorrectAnswerIndex = 2, Explanation = "Hardware keys are the gold standard as they are immune to phishing. Authenticator apps are a strong second choice." },
                new QuizQuestion { QuestionText = "What is 'Biometric' authentication?", Options = new List<string> { "Using your location to log in", "Using a physical characteristic like a fingerprint or face", "Using a password you've memorized" }, CorrectAnswerIndex = 1, Explanation = "Biometrics use something you 'are' (like a fingerprint) to verify your identity. It's often used to unlock phones and laptops." },
                new QuizQuestion { QuestionText = "What type of malware disguises itself as a legitimate program?", Options = new List<string> { "Virus", "Worm", "Trojan" }, CorrectAnswerIndex = 2, Explanation = "A Trojan Horse tricks you into installing it by pretending to be a useful piece of software, like a game or utility." },
                new QuizQuestion { QuestionText = "Ransomware's primary goal is to:", Options = new List<string> { "Steal your passwords", "Encrypt your files and demand payment", "Slow down your computer" }, CorrectAnswerIndex = 1, Explanation = "Ransomware holds your data hostage by encrypting it and demands a ransom for its release." },
                new QuizQuestion { QuestionText = "The best defense against ransomware is:", Options = new List<string> { "A strong firewall", "Regular, offline backups", "A fast internet connection" }, CorrectAnswerIndex = 1, Explanation = "If you have backups, you can restore your files without paying the ransom, rendering the attack useless." },
                new QuizQuestion { QuestionText = "A 'keylogger' is a type of spyware that records your...", Options = new List<string> { "Screen", "Keystrokes", "Webcam" }, CorrectAnswerIndex = 1, Explanation = "Keyloggers capture everything you type, including passwords and private messages, making them extremely dangerous." },
                new QuizQuestion { QuestionText = "A 'zero-day' vulnerability is:", Options = new List<string> { "A security flaw with zero impact", "A flaw exploited by hackers before the developer has a patch for it", "A flaw found on the first day a program is released" }, CorrectAnswerIndex = 1, Explanation = "It's called a 'zero-day' because the developers have had zero days to fix it, making it extremely dangerous." },
                new QuizQuestion { QuestionText = "You receive an email from your bank asking you to click a link to verify your account. What should you do?", Options = new List<string> { "Click the link and log in", "Ignore the email", "Open your browser and manually type your bank's website address to log in" }, CorrectAnswerIndex = 2, Explanation = "Never click links in unexpected emails. Go directly to the official website to verify any account issues." },
                new QuizQuestion { QuestionText = "'Smishing' is a type of phishing attack conducted via:", Options = new List<string> { "Email", "Phone Call", "SMS (Text Message)" }, CorrectAnswerIndex = 2, Explanation = "Smishing combines 'SMS' and 'phishing'. It's a very common way for scammers to send malicious links." },
                new QuizQuestion { QuestionText = "A phishing email will often create a sense of...", Options = new List<string> { "Calm and patience", "Urgency and fear", "Curiosity and excitement" }, CorrectAnswerIndex = 1, Explanation = "Scammers want you to panic and act without thinking, so they use urgent language like 'account suspended' or 'act now'." },
                new QuizQuestion { QuestionText = "What is 'social engineering'?", Options = new List<string> { "A type of coding language", "Manipulating people to give up confidential information", "A social media marketing technique" }, CorrectAnswerIndex = 1, Explanation = "Social engineering is the art of psychological manipulation. Phishing is a common form of it." },
                new QuizQuestion { QuestionText = "An attacker calls you pretending to be from tech support and asks for remote access to your computer. This is an example of:", Options = new List<string> { "Vishing", "A Denial-of-Service attack", "Ransomware" }, CorrectAnswerIndex = 0, Explanation = "Vishing (Voice Phishing) uses phone calls to trick people into giving up access or information." },
                new QuizQuestion { QuestionText = "A VPN (Virtual Private Network) will:", Options = new List<string> { "Make your internet faster", "Encrypt your internet traffic", "Block all viruses" }, CorrectAnswerIndex = 1, Explanation = "A VPN's main purpose is to create a secure, encrypted tunnel for your data, protecting your privacy from eavesdroppers." },
                new QuizQuestion { QuestionText = "Is it safe to do online banking on public Wi-Fi without a VPN?", Options = new List<string> { "Yes, if the website is HTTPS", "No, it's never safe", "Only if the Wi-Fi has a password" }, CorrectAnswerIndex = 1, Explanation = "No. An attacker on the same network can intercept your data. Always use a VPN on public networks for sensitive tasks." },
                new QuizQuestion { QuestionText = "What does a firewall primarily do?", Options = new List<string> { "Scans for viruses", "Monitors and filters network traffic", "Backs up your files" }, CorrectAnswerIndex = 1, Explanation = "A firewall acts as a barrier, controlling what traffic is allowed into or out of your network based on security rules." },
                new QuizQuestion { QuestionText = "The padlock icon in your browser's address bar signifies what?", Options = new List<string> { "The website is safe from malware", "The website is owned by a trusted company", "Your connection to the website is encrypted (HTTPS)" }, CorrectAnswerIndex = 2, Explanation = "The padlock means your connection is encrypted, preventing eavesdropping. It does not guarantee the site itself is trustworthy." },
                new QuizQuestion { QuestionText = "The most secure Wi-Fi encryption standard is:", Options = new List<string> { "WEP", "WPA2", "WPA3" }, CorrectAnswerIndex = 2, Explanation = "WPA3 is the latest and most secure standard. WEP is ancient and completely insecure." },
                new QuizQuestion { QuestionText = "Keeping your software updated is a critical security practice.", Options = new List<string> { "True", "False" }, CorrectAnswerIndex = 0, Explanation = "Updates often contain patches for security vulnerabilities that attackers can exploit. It's one of the easiest and most important security habits." },
                new QuizQuestion { QuestionText = "You find a USB stick on the ground. What should you do?", Options = new List<string> { "Plug it into your computer to find the owner", "Plug it into an isolated, non-critical computer", "Destroy it or turn it in to a lost and found without plugging it in" }, CorrectAnswerIndex = 2, Explanation = "Never plug in unknown USB drives. They can be loaded with malware designed to automatically infect any computer they're connected to." },
                new QuizQuestion { QuestionText = "What is the 'Principle of Least Privilege'?", Options = new List<string> { "Giving a user the minimum levels of access needed to perform their job functions", "Always using the least expensive security software", "Privileging security over user convenience" }, CorrectAnswerIndex = 0, Explanation = "This principle limits the damage that can result from a compromised account. A user with fewer permissions can do less harm." },
                new QuizQuestion { QuestionText = "What is a 'data breach'?", Options = new List<string> { "A type of network cable", "An intentional system shutdown", "An incident where sensitive information is stolen or released" }, CorrectAnswerIndex = 2, Explanation = "In a data breach, confidential data like usernames, passwords, and credit card numbers are exposed to unauthorized individuals." },
                new QuizQuestion { QuestionText = "You can check if your email has been exposed in a known data breach using which website?", Options = new List<string> { "CanIBeHacked.com", "HaveIBeenPwned.com", "IsMyDataSafe.org" }, CorrectAnswerIndex = 1, Explanation = "HaveIBeenPwned.com is a reputable, free service that aggregates data from hundreds of breaches, allowing you to check your exposure." },
                new QuizQuestion { QuestionText = "What does 'end-to-end encryption' (E2EE) mean?", Options = new List<string> { "The data is encrypted only on the sender's device", "The data is encrypted on the server", "Only the sender and intended recipient can read the message" }, CorrectAnswerIndex = 2, Explanation = "E2EE ensures that no one in between, not even the company providing the service, can decipher the messages." },
                new QuizQuestion { QuestionText = "Is it safe to share your password with a close friend or family member?", Options = new List<string> { "Yes, if you trust them", "No, passwords should never be shared" }, CorrectAnswerIndex = 1, Explanation = "Passwords should be treated like toothbrushes: never share them. If someone needs access, use features like guest accounts or family sharing plans." },
                new QuizQuestion { QuestionText = "What is 'Adware'?", Options = new List<string> { "Software that helps you make advertisements", "Software that automatically displays or downloads unwanted advertising material", "A hardware device for blocking ads" }, CorrectAnswerIndex = 1, Explanation = "Adware is a type of malware that bombards you with pop-ups and ads, often tracks your browsing habits, and can slow down your computer." },
                new QuizQuestion { QuestionText = "If you receive a 'friend request' from someone you don't know on social media, you should:", Options = new List<string> { "Accept it to be friendly", "Ignore or delete the request", "Accept it, but restrict their access" }, CorrectAnswerIndex = 1, Explanation = "Accepting requests from strangers can expose your personal information to scammers or fake accounts. It's safest to only connect with people you know." },
                new QuizQuestion { QuestionText = "The term 'digital footprint' refers to:", Options = new List<string> { "The size of your hard drive", "The trail of data you leave behind when you use the internet", "The number of devices you own" }, CorrectAnswerIndex = 1, Explanation = "Your digital footprint includes social media posts, browsing history, and online purchases. It's important to be mindful of what you share." },
                new QuizQuestion { QuestionText = "A 'Denial-of-Service' (DoS) attack aims to:", Options = new List<string> { "Steal data from a server", "Make a website or service unavailable to legitimate users", "Delete a user's account" }, CorrectAnswerIndex = 1, Explanation = "A DoS attack floods a server with so much traffic that it becomes overwhelmed and cannot respond to normal requests." }
            };
        }

        #endregion

        #region UI Handlers and Helper Methods

        private void ShowHelp()
        {
            var helpText = "Here are some things you can do:\n\n" +
                           "💬 Ask about topics like: 'password safety', 'what is phishing?', 'info on VPNs'\n\n" +
                           "✔️ Manage tasks: 'remind me to update my pc tomorrow at 2pm', 'show my tasks'\n\n" +
                           "❓ Take a quiz: 'start a quiz' or 'test my knowledge'";
            AddBotMessageToChat(helpText, "suggestion");
        }

        private void ClearChatButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewSession();
        }

        private void ShowStatsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowExitSummary(false);
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            new TaskWindow(currentUser.Tasks) { Owner = this }.ShowDialog();
        }

        private async void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            // This now sets a response and starts the quiz, ensuring a consistent experience.
            string response = "🚀 Starting a random quiz! Good luck! Type 'stop quiz' at any time to end it.";
            await AddBotMessageWithDelay(response, "suggestion", 0);
            await StartQuizAsync();
        }

        private void ActivityLogButton_Click(object sender, RoutedEventArgs e)
        {
            _logPageIndex = 0;
            AddBotMessageToChat(FormatLogPage(), "summary");
        }

        private void AddUserMessageToChat(string message)
        {
            var border = new Border { Style = (Style)FindResource("UserBubbleStyle") };
            var textBlock = new TextBlock { Text = message, Foreground = Brushes.White, FontSize = 14, TextWrapping = TextWrapping.Wrap };
            border.Child = textBlock;
            ChatPanel.Children.Add(border);
            ScrollToBottom();
        }

        private void AddBotMessageToChat(string message, string sentiment)
        {
            var border = new Border { Style = (Style)FindResource("BotBubbleStyle"), Background = new SolidColorBrush(sentimentAnalyzer.GetSentimentColor(sentiment)) };
            var textBlock = new TextBlock { Text = $"{sentimentAnalyzer.GetSentimentEmoji(sentiment)} {message}", Foreground = Brushes.White, FontSize = 14, TextWrapping = TextWrapping.Wrap };
            border.Child = textBlock;
            ChatPanel.Children.Add(border);
            ScrollToBottom();
        }

        private async Task AddBotMessageWithDelay(string message, string sentiment, int delay)
        {
            if (delay > 0)
            {
                ShowTypingIndicator(true);
                await Task.Delay(delay);
            }
            AddBotMessageToChat(message, sentiment);
            ShowTypingIndicator(false);
        }

        private void ResetInputBox()
        {
            InputTextBox.Text = "";
        }

        private void ShowTypingIndicator(bool show)
        {
            TypingIndicator.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (show)
                typingIndicatorTimer.Start();
            else
                typingIndicatorTimer.Stop();
        }

        private void TypingIndicatorTimer_Tick(object sender, EventArgs e)
        {
            TypingDots.Text = TypingDots.Text.Length >= 3 ? "." : TypingDots.Text + ".";
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }

        private void UpdateUserNameDisplay()
        {
            if (currentUser.Name != "Guest")
            {
                UserNameDisplay.Text = $"Hello, {currentUser.Name}!";
            }
        }

        private void ShowExitSummary(bool isExiting)
        {
            var summary = $"User: {currentUser.Name}\n" +
                          $"Interactions: {currentUser.InteractionCount}\n" +
                          $"Duration: {(DateTime.Now - currentUser.SessionStartTime):mm\\:ss}";
            if (currentUser.InterestsTopics.Any())
            {
                summary += $"\nTopics: {string.Join(", ", currentUser.InterestsTopics.Distinct())}";
            }
            AddBotMessageToChat(summary, "summary");
            if (isExiting)
            {
                AddBotMessageToChat($"Goodbye, {currentUser.Name}!", "neutral");
                Task.Delay(3000).ContinueWith(_ => Dispatcher.Invoke(Close));
            }
        }

        private void PlayWelcomeAudio()
        {
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    using (var player = new SoundPlayer("Assets/welcome.wav"))
                    {
                        player.Load();
                        player.Play();
                    }
                }
                catch (Exception ex)
                {
                    ActivityLogger.Log(ActivityType.System, $"Audio error: {ex.Message}");
                }
            }
        }

        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text == PlaceholderText)
            {
                InputTextBox.Text = "";
                InputTextBox.Foreground = Brushes.White;
            }
        }

        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                InputTextBox.Text = PlaceholderText;
                InputTextBox.Foreground = Brushes.Gray;
            }
        }

        #endregion
    }
}