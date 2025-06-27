using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MessageBox = System.Windows.MessageBox;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public partial class TaskWindow : Window
    {
        private readonly List<CyberTask> _tasks;

        public TaskWindow(List<CyberTask> tasks)
        {
            InitializeComponent();
            _tasks = tasks;
            LoadTasks();
        }

        private void LoadTasks()
        {
            TasksListView.ItemsSource = null;
            TasksListView.ItemsSource = _tasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate);
        }

        private void TasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                TitleTextBox.Text = selectedTask.Title;
                DescriptionTextBox.Text = selectedTask.Description;
                DueDatePicker.SelectedDate = selectedTask.DueDate;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a title for the task.", "Title Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newTask = new CyberTask
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                DueDate = DueDatePicker.SelectedDate, // Can be null
                IsCompleted = false
            };
            _tasks.Add(newTask);
            ActivityLogger.Log(ActivityType.Task, $"Added: {newTask.Title}");
            LoadTasks();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.Title = TitleTextBox.Text;
                selectedTask.Description = DescriptionTextBox.Text;
                // This correctly allows clearing a date by setting it to null.
                selectedTask.DueDate = DueDatePicker.SelectedDate;
                ActivityLogger.Log(ActivityType.Task, $"Updated: {selectedTask.Title}");
                LoadTasks();
            }
        }

        private void ToggleCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.IsCompleted = !selectedTask.IsCompleted;
                string status = selectedTask.IsCompleted ? "Completed" : "Marked as not complete";
                ActivityLogger.Log(ActivityType.Task, $"{status}: {selectedTask.Title}");
                LoadTasks();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                _tasks.Remove(selectedTask);
                ActivityLogger.Log(ActivityType.Task, $"Deleted: {selectedTask.Title}");
                LoadTasks();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            TasksListView.SelectedItem = null;
        }
    }

    // Helper class for UI binding remains unchanged.
    public class BoolToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? "✅" : "❌";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}