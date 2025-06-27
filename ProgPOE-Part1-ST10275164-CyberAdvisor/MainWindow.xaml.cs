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
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public partial class MainWindow : Window
    {
        #region Fields and Properties

        // Core Logic Components
        private BotUser currentUser;
        private readonly SentimentAnalyzer sentimentAnalyzer;
        private readonly ResponseManager responseManager;
        private readonly NluEngine nluEngine;

        // UI Components & State Management
        private readonly DispatcherTimer typingIndicatorTimer;
        private const string PlaceholderText = "Ask me anything, or type 'help' for commands...";

        // State fields for managing special conversational modes
        private bool _isQuizActive = false;
        private List<QuizQuestion> _quizQuestions;
        private int _currentQuizQuestionIndex;
        private int _quizScore;
        private int _logPageIndex = 0;

        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();

            // Initialize all the bot's core components
            sentimentAnalyzer = new SentimentAnalyzer();
            responseManager = new ResponseManager();
            nluEngine = new NluEngine();

            // Setup UI elements and timers
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

            // Reset all stateful modes
            _isQuizActive = false;
            _logPageIndex = -1; // Set to -1 to require a fresh "show log" command

            // Set placeholder and kick off welcome sequence
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

        private async void SendButton_Click(object sender, RoutedEventArgs e) => await HandleUserInput();
        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) await HandleUserInput(); }

        /// <summary>
        /// This is the primary entry point for all user input.
        /// It routes the input to the correct handler based on the application's current state.
        /// </summary>
        private async Task HandleUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput) || userInput == PlaceholderText) return;

            AddUserMessageToChat(userInput);
            ResetInputBox();

            ShowTypingIndicator(true);
            await Task.Delay(new Random().Next(400, 800)); // Simulate bot "thinking"

            // --- STATEFUL ROUTING ---
            // The bot's response depends on its current mode (e.g., in a quiz or not).
            if (_isQuizActive)
            {
                await ProcessQuizInputAsync(userInput);
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

        /// <summary>
        /// Processes user input when the bot is in a normal conversational state.
        /// It uses the NLU engine to understand the user's goal and dispatches to the correct handler.
        /// </summary>
        private async Task<string> ProcessNluIntentAsync(string input)
        {
            ActivityLogger.Log(ActivityType.Chat, $"User: {input}");
            DetectedIntent intent = nluEngine.Process(input);
            string response = "";

            switch (intent.Name)
            {
                case "GetInfo":
                    response = responseManager.GetKeywordResponse(intent.Topic, currentUser);
                    break;
                case "StartQuiz":
                    await StartQuizAsync();
                    break; // The StartQuiz method handles its own chat messages.
                case "CreateTask":
                    response = HandleCreateTaskIntent(intent);
                    break;
                case "ListTasks":
                    response = HandleListTasksIntent();
                    break;
                case "ViewLog":
                    _logPageIndex = 0; // Reset to the first page
                    response = FormatLogPage();
                    break;
                case "ViewMoreLog":
                    if (_logPageIndex == -1) response = "Please ask to see the log first before asking for more.";
                    else { _logPageIndex++; response = FormatLogPage(); }
                    break;
                case "Greeting":
                    response = $"Hello {currentUser.Name}! How can I assist you with your cybersecurity today?";
                    _logPageIndex = -1; // Reset log paging on new topics
                    break;
                case "ThankYou":
                    response = $"You're welcome, {currentUser.Name}! Stay safe online.";
                    break;
                case "Help":
                    ShowHelp();
                    break; // The ShowHelp method handles its own chat message.
                case "Fallback":
                default:
                    _logPageIndex = -1; // Reset log paging on unrecognized input
                    response = responseManager.GetFallbackResponse(currentUser);
                    break;
            }

            // Handle name-setting as a special case that can happen at any time
            var nameMatch = Regex.Match(input, @"(?:my name is|call me|i am)\s+([A-Za-z]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success)
            {
                currentUser.Name = nameMatch.Groups[1].Value.Trim();
                UpdateUserNameDisplay();
                response = $"Got it! Nice to meet you, {currentUser.Name}. What can I help you with?";
            }

            if (!string.IsNullOrEmpty(response)) ActivityLogger.Log(ActivityType.Chat, $"Bot: {response.Split('\n').First()}");
            return response;
        }

        private string HandleCreateTaskIntent(DetectedIntent intent)
        {
            if (intent.Entities.TryGetValue("task", out var taskDesc) && intent.Entities.TryGetValue("time", out var timeDesc))
            {
                if (DateTimeParser.TryParse(timeDesc, out DateTime dueDate))
                {
                    var newTask = new CyberTask { Title = taskDesc, Description = "Created via chat.", DueDate = dueDate };
                    currentUser.Tasks.Add(newTask);
                    ActivityLogger.Log(ActivityType.Task, $"Created via chat: {taskDesc}");
                    return $"✅ Reminder set! I'll remind you to '{taskDesc}' on {dueDate:g}.";
                }
                return $"🤔 I had trouble understanding that date. Please try again with a clear format, like 'tomorrow at 5pm' or 'on July 25th at noon'.";
            }
            return responseManager.GetFallbackResponse(currentUser);
        }

        private string HandleListTasksIntent()
        {
            var pendingTasks = currentUser.Tasks.Where(t => !t.IsCompleted).ToList();
            if (!pendingTasks.Any()) return "You have no pending tasks or reminders.";

            var response = "Here are your current reminders:\n";
            response += string.Join("\n", pendingTasks.OrderBy(t => t.DueDate).Select(t => $"• {t.Title} (Due: {t.DueDate:g})"));
            return response;
        }

        #endregion

        #region In-Chat Feature Logic (Quiz & Log)

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
            _isQuizActive = true;
            _quizScore = 0;
            _currentQuizQuestionIndex = 0;
            _quizQuestions = InitializeQuestions();
            ActivityLogger.Log(ActivityType.Quiz, "Quiz started.");
            await AddBotMessageWithDelay("🚀 Starting the quiz! Type 'stop quiz' at any time to end it.", "suggestion", 200);
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
                string description = _quizScore >= 9 ? "Excellent! You're a cybersecurity expert!" : _quizScore >= 7 ? "Great job! Solid understanding." : "A good start, but keep learning!";
                var summary = $"🏁 Quiz Complete! Your final score is: {_quizScore}/{_quizQuestions.Count}\n\n{description}";
                await AddBotMessageWithDelay(summary, "summary", 500);
                ActivityLogger.Log(ActivityType.Quiz, $"Quiz finished with score: {_quizScore}/{_quizQuestions.Count}");
            }
            _isQuizActive = false; // This is the crucial step to exit the state
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
            return new List<QuizQuestion> { /* ... same 10 questions as before ... */ };
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

        private void ClearChatButton_Click(object sender, RoutedEventArgs e) => StartNewSession();
        private void ShowStatsButton_Click(object sender, RoutedEventArgs e) => ShowExitSummary(false);
        private void TasksButton_Click(object sender, RoutedEventArgs e) { var taskWindow = new TaskWindow(currentUser.Tasks) { Owner = this }; taskWindow.ShowDialog(); }
        private async void QuizButton_Click(object sender, RoutedEventArgs e) => await StartQuizAsync();
        private void ActivityLogButton_Click(object sender, RoutedEventArgs e) { _logPageIndex = 0; AddBotMessageToChat(FormatLogPage(), "summary"); }

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

        private void ResetInputBox() { InputTextBox.Text = ""; }
        private void ShowTypingIndicator(bool show) { TypingIndicator.Visibility = show ? Visibility.Visible : Visibility.Collapsed; if (show) typingIndicatorTimer.Start(); else typingIndicatorTimer.Stop(); }
        private void TypingIndicatorTimer_Tick(object sender, EventArgs e) => TypingDots.Text = TypingDots.Text.Length >= 3 ? "." : TypingDots.Text + ".";
        private void ScrollToBottom() => ChatScrollViewer.ScrollToEnd();
        private void UpdateUserNameDisplay() { if (currentUser.Name != "Guest") UserNameDisplay.Text = $"Hello, {currentUser.Name}!"; }

        private void ShowExitSummary(bool isExiting)
        {
            var summary = $"User: " +
                $"{currentUser.Name}" +
                $"\nInteractions: " +
                $"{currentUser.InteractionCount}" +
                $"\nDuration: " +
                $"{(DateTime.Now - currentUser.SessionStartTime):mm\\:ss}";

            if (currentUser.InterestsTopics.Any()) summary += $"\nTopics: {string.Join(", ", currentUser.InterestsTopics)}";
            AddBotMessageToChat(summary, "summary");
            if (isExiting) { AddBotMessageToChat($"Goodbye, {currentUser.Name}!", "neutral"); Task.Delay(3000).ContinueWith(_ => Dispatcher.Invoke(Close)); }
        }

        private void PlayWelcomeAudio()
        {
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    
                    // 'Copy to Output Directory' property is set to 'Copy if newer'.
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

        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e) { if (InputTextBox.Text == PlaceholderText) { InputTextBox.Text = ""; InputTextBox.Foreground = Brushes.White; } }
        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e) { if (string.IsNullOrWhiteSpace(InputTextBox.Text)) { InputTextBox.Text = PlaceholderText; InputTextBox.Foreground = Brushes.Gray; } }

        #endregion
    }
}