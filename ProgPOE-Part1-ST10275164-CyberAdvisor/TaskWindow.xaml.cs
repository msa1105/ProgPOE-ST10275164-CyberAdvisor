using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            var newTask = new CyberTask
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                DueDate = DueDatePicker.SelectedDate ?? DateTime.Now.AddDays(1),
                IsCompleted = false
            };
            _tasks.Add(newTask);
            ActivityLogger.Log(ActivityType.Task, $"Created: {newTask.Title}");
            LoadTasks();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.Title = TitleTextBox.Text;
                selectedTask.Description = DescriptionTextBox.Text;
                selectedTask.DueDate = DueDatePicker.SelectedDate ?? selectedTask.DueDate;
                ActivityLogger.Log(ActivityType.Task, $"Updated: {selectedTask.Title}");
                LoadTasks();
            }
        }

        private void ToggleCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.IsCompleted = !selectedTask.IsCompleted;
                ActivityLogger.Log(ActivityType.Task, $"Toggled Complete: {selectedTask.Title}");
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

    // Helper class to convert boolean to emoji for the UI
    public class BoolToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "✅" : "❌";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}