// --- QuizQuestion.cs ---
using System.Collections.Generic;

namespace ProgPOE_Part1_ST10275164_CyberAdvisor
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } // Feedback for the user
    }
}