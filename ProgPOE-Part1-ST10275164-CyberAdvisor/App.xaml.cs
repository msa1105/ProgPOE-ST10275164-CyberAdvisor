using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Application entry point and global event handling
    /// </summary>
    public partial class App : Application
    {
        private const string LOG_FILE_NAME = "CyberAdvisor_ErrorLog.txt";
        private const string APP_NAME = "CyberAdvisor";
        private const string APP_VERSION = "2.0.0";

        /// <summary>
        /// Application startup event - called when the application starts
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Set up global exception handling
            SetupExceptionHandling();

            // Initialize application settings
            InitializeApplicationSettings();

            // Log application startup
            LogApplicationEvent($"Application started - Version {APP_VERSION}");

            // Call base startup
            base.OnStartup(e);

            // Show splash screen or perform additional initialization if needed
            ShowApplicationInfo();
        }

        /// <summary>
        /// Application exit event - called when the application is shutting down
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            // Log application shutdown
            LogApplicationEvent("Application shutting down");

            // Perform cleanup operations
            CleanupResources();

            // Call base exit
            base.OnExit(e);
        }

        /// <summary>
        /// Set up global exception handling for the application
        /// </summary>
        private void SetupExceptionHandling()
        {
            // Handle unhandled exceptions in the main UI thread
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Handle unhandled exceptions in background threads
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Handle unhandled exceptions in async/await scenarios
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Handle unhandled exceptions in the UI thread
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                // Log the exception
                LogException(e.Exception, "UI Thread Exception");

                // Show user-friendly error message
                string errorMessage = "An unexpected error occurred in CyberAdvisor.\n\n" +
                                    "The application will attempt to continue running.\n" +
                                    "If problems persist, please restart the application.\n\n" +
                                    $"Error: {e.Exception.Message}";

                MessageBox.Show(errorMessage,
                              "CyberAdvisor Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);

                // Mark as handled to prevent application crash
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // Last resort - log to system and show basic error
                System.Diagnostics.EventLog.WriteEntry("Application",
                    $"CyberAdvisor Fatal Error: {ex.Message}",
                    System.Diagnostics.EventLogEntryType.Error);

                MessageBox.Show("A critical error occurred. The application will now close.",
                              "Critical Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);

                // Force shutdown if we can't handle the error
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Handle unhandled exceptions in background threads
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                LogException(ex, "Background Thread Exception");

                // Since this is a background thread exception, we need to marshal to UI thread  
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    string errorMessage = "A background process encountered an error.\n\n" +
                                        "The application may be unstable. Consider restarting.\n\n" +
                                        $"Error: {ex.Message}";

                    MessageBox.Show(errorMessage,
                                  "Background Process Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                }));
            }
            catch (Exception logEx)
            {
                // Emergency logging  
                try
                {
                    File.AppendAllText("emergency_log.txt",
                        $"{DateTime.Now}: Emergency log - {logEx.Message}\n");
                }
                catch
                {
                    // If we can't even log, there's nothing more we can do  
                }
            }
        }

        /// <summary>
        /// Handle unhandled exceptions in async tasks
        /// </summary>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                LogException(e.Exception, "Async Task Exception");

                // Mark as observed to prevent application termination
                e.SetObserved();

                // Show warning to user on UI thread
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    string errorMessage = "An asynchronous operation encountered an error.\n\n" +
                                        "The operation has been cancelled, but the application should continue normally.\n\n" +
                                        $"Error: {e.Exception.GetBaseException().Message}";

                    MessageBox.Show(errorMessage,
                                  "Async Operation Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }));
            }
            catch (Exception ex)
            {
                // Log emergency info
                LogApplicationEvent($"Failed to handle async exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize application-wide settings and configurations
        /// </summary>
        private void InitializeApplicationSettings()
        {
            try
            {
                // Set application properties
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                // Create logs directory if it doesn't exist
                string logsDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    APP_NAME, "Logs");

                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Set culture if needed (for internationalization)
                // System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            catch (Exception ex)
            {
                LogApplicationEvent($"Failed to initialize application settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Show application information on startup (optional)
        /// </summary>
        private void ShowApplicationInfo()
        {
            try
            {
                // You can show a splash screen or startup message here if desired
                // For now, we'll just log the startup
                LogApplicationEvent($"CyberAdvisor v{APP_VERSION} initialized successfully");
            }
            catch (Exception ex)
            {
                LogApplicationEvent($"Error showing application info: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup resources when the application exits
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                // Cleanup any global resources here
                // For example: dispose of global objects, save settings, etc.

                LogApplicationEvent("Application cleanup completed");
            }
            catch (Exception ex)
            {
                LogApplicationEvent($"Error during cleanup: {ex.Message}");
            }
        }

        /// <summary>
        /// Log exceptions to file
        /// </summary>
        private void LogException(Exception ex, string context)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}\n" +
                                $"Exception: {ex.GetType().Name}\n" +
                                $"Message: {ex.Message}\n" +
                                $"Stack Trace: {ex.StackTrace}\n" +
                                $"Inner Exception: {ex.InnerException?.Message ?? "None"}\n" +
                                new string('-', 80) + "\n";

                string logPath = GetLogFilePath();
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // If logging fails, we can't do much more
                // Could try Windows Event Log as last resort
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("Application",
                        $"CyberAdvisor logging failed: {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch
                {
                    // Ultimate fallback - do nothing rather than crash
                }
            }
        }

        /// <summary>
        /// Log general application events
        /// </summary>
        private void LogApplicationEvent(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                string logPath = GetLogFilePath();
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // Silent failure for application events
            }
        }

        /// <summary>
        /// Get the path for the log file
        /// </summary>
        private string GetLogFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, APP_NAME, "Logs");

            // Ensure directory exists
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            // Create log file name with date
            string fileName = $"{DateTime.Now:yyyy-MM-dd}_{LOG_FILE_NAME}";
            return Path.Combine(appFolder, fileName);
        }

        /// <summary>
        /// Static method to get application version (can be called from anywhere)
        /// </summary>
        public static string GetApplicationVersion()
        {
            return APP_VERSION;
        }

        /// <summary>
        /// Static method to get application name (can be called from anywhere)
        /// </summary>
        public static string GetApplicationName()
        {
            return APP_NAME;
        }

        /// <summary>
        /// Method to show about dialog (can be called from MainWindow)
        /// </summary>
        public static void ShowAboutDialog()
        {
            string aboutMessage = $"{APP_NAME} v{APP_VERSION}\n\n" +
                                 "Your personal cybersecurity mentor with enhanced AI capabilities.\n\n" +
                                 "Features:\n" +
                                 "• Personalized cybersecurity advice\n" +
                                 "• Sentiment analysis and adaptive responses\n" +
                                 "• Memory of user preferences and interests\n" +
                                 "• Interactive learning experience\n\n" +
                                 "© 2024 Student ID: ST10275164";

            MessageBox.Show(aboutMessage,
                          $"About {APP_NAME}",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }
    }
}