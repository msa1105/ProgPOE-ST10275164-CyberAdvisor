﻿// --- CyberTask.cs (Modified) ---
using System;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class CyberTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }

        // This is now a nullable DateTime, allowing a task to exist without a reminder.
        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}