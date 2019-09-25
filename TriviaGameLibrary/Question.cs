using System.Collections.Generic;

namespace TriviaGameLibrary
{
    public struct Question
    {
        public string Category { get; set;}
        public string Type { get; set;}
        public string Difficulty { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; } 
    }
}