using System;
using System.Linq;
using System.Collections.Generic;
using TriviaGameLibrary;

namespace TriviaGameConsole
{
    class Game 
    {
        public void Run()
        {
            Console.Clear();
            var tgm = new TriviaGameManager();
            bool isPlaying = true;

            // Game loop
            do
            {
                // reset score
                int score = 0;
                // list categories and prompt for selection
                int categorySelected = CategorySelection(tgm);
                // load 10 questions from the selected category
                var questions = LoadQuestions(tgm, categorySelected);
                // ask each question, keeping score
                foreach (var question in questions)
                {
                    if (AskQuestion(question))
                    {
                        score++;
                        Console.WriteLine("\nCorrect!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("\nIncorrect");
                        Console.ReadKey();
                    }
                }
                // game over, show score
                Console.Clear();
                Console.WriteLine("Game Over");
                Console.WriteLine($"You answered {score} questions correctly.");
                // ask to play again
                Console.WriteLine("Would you like to play again? Y/N");
                isPlaying = !(Console.ReadKey().Key == ConsoleKey.N);
            } while (isPlaying);

            Environment.Exit(0);
        }

        private int CategorySelection(TriviaGameManager tgm)
        {
            string response = string.Empty;
            int categoryID;

            do
            {
                Console.Clear();
                Console.WriteLine("Select from the categories below...");
                Console.WriteLine("________________________________________\n");
                // get categories
                var categories = tgm.GetCategoriesAsync().Result;
                // list categories
                foreach (var category in categories)
                {
                    Console.WriteLine($"{category.Key}. {category.Value}");
                }
                // prompt for catagory choice
                Console.Write("\nPlease enter the category number: ");
                response = Console.ReadLine();
                
            } while (!int.TryParse(response, out categoryID));

            
            return categoryID;
        }

        private List<Question> LoadQuestions(TriviaGameManager tgm, int categoryID)
        {
            var questions = tgm.GetQuestionsAsync(10, categoryID).Result;
            return questions;
        } 

        private bool AskQuestion(Question question)
        {
            string response = string.Empty;
            int correctAnswerID;
            int selection;    

            do
            {
                Console.Clear();
                Console.WriteLine(question.QuestionText);
                Console.WriteLine();

                // randomly assign correct answer
                var length = question.IncorrectAnswers.Count + 1;
                var rng = new Random();
                correctAnswerID = rng.Next(1, length + 1);
                int shift = 0;
                for (int i = 0; i < length; i++)
                {
                    if (i + 1 == correctAnswerID)
                    {
                        shift = 1;
                        Console.WriteLine($"{i + 1}. {question.CorrectAnswer}");
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}. {question.IncorrectAnswers[i - shift]}");
                    }
                }

                Console.WriteLine("\nSelect an answer from above: ");
                response = Console.ReadLine();
                
            } while (!int.TryParse(response, out selection));

            return selection == correctAnswerID;
        }
    }
}

