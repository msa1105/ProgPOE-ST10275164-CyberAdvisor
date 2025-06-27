// --- DateTimeParser.cs (New File) ---
using System;
using System.Text.RegularExpressions;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public static class DateTimeParser
    {
        /// <summary>
        /// Tries to parse a natural language string into a DateTime object.
        /// Examples: "tomorrow at 5pm", "in 3 days", "on friday at 10am"
        /// </summary>
        /// <param name="input">The natural language string.</param>
        /// <param name="result">The parsed DateTime object.</param>
        /// <returns>True if parsing was successful, otherwise false.</returns>
        public static bool TryParse(string input, out DateTime result)
        {
            input = input.ToLower().Trim();
            result = DateTime.MinValue;
            DateTime baseDate = DateTime.Today;

            // Handle simple relative terms
            if (input.Contains("tomorrow")) baseDate = baseDate.AddDays(1);
            else if (input.Contains("today")) { /* No change needed */ }
            else
            {
                // Handle "in X days"
                var daysMatch = Regex.Match(input, @"in (\d+) days?");
                if (daysMatch.Success)
                {
                    baseDate = baseDate.AddDays(int.Parse(daysMatch.Groups[1].Value));
                }

                // Handle days of the week (e.g., "on friday")
                for (int i = 0; i < 7; i++)
                {
                    var dayOfWeek = DateTime.Today.AddDays(i).DayOfWeek.ToString().ToLower();
                    if (input.Contains(dayOfWeek))
                    {
                        baseDate = DateTime.Today.AddDays(i);
                        break;
                    }
                }
            }

            // Now, parse the time
            var timeMatch = Regex.Match(input, @"(\d{1,2})(:\d{2})?\s*(am|pm)?");
            if (!timeMatch.Success)
            {
                // If no time is specified, default to 9 AM
                result = baseDate.AddHours(9);
                return true;
            }

            int hour = int.Parse(timeMatch.Groups[1].Value);
            int minute = timeMatch.Groups[2].Success ? int.Parse(timeMatch.Groups[2].Value.TrimStart(':')) : 0;
            string ampm = timeMatch.Groups[3].Value;

            if (ampm == "pm" && hour < 12)
            {
                hour += 12;
            }
            else if (ampm == "am" && hour == 12) // Midnight case
            {
                hour = 0;
            }

            result = baseDate.AddHours(hour).AddMinutes(minute);
            return true;
        }
    }
}