using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TriviaGameLibrary;

namespace TriviaGameConsole
{
    class Game 
    {
        private int CenterX => Console.WindowWidth / 2;
        private int CenterY => Console.WindowHeight / 2;

        public async void Run()
        {
            var tgmTask = new TriviaGameManager().IntializeAsync();
            bool isPlaying = true;
            int questionsAsked = 10;

            // Welcome Screen
            PrintWelcomeScreen();
            Console.ReadKey();

            var tgm = await tgmTask;

            // Get the trivia categories
            var categoriesTask = tgm.GetCategoriesAsync();

            // Game loop
            while (isPlaying)
            {
                // reset score
                int score = 0;

                // list categories and prompt for selection
                Console.Clear();
                var categories = LoadingLoop(categoriesTask);
                int categorySelected = CategorySelection(categories);

                // load 10 questions from the selected category
                var questionsTask = tgm.GetQuestionsAsync(questionsAsked, categorySelected);
                Console.Clear();
                var questions = LoadingLoop(questionsTask);

                // ask each question, keeping score
                foreach (var question in questions)
                {
                    if (AskQuestion(question))
                    {
                        score++;
                        Console.Write("\nCorrect!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.Write("\nIncorrect");
                        Console.ReadKey();
                    }
                }

                // game over, show score
                Console.Clear();
                Console.WriteLine("Game Over");
                Console.WriteLine($"You answered {score} out of {questionsAsked} questions correctly.");

                // ask to play again
                Console.WriteLine("Would you like to play again? Y/N");
                isPlaying = !(Console.ReadKey().Key == ConsoleKey.N);
            }

            Environment.Exit(0);
        }

        private void PrintWelcomeScreen()
        {
            string title = "Trivia Game";
            string prompt = "Press ENTER to continue...";

            Console.Clear();

            Console.SetCursorPosition(CenterX - (title.Length / 2), CenterY);
            Console.Write(title);
            
            Console.SetCursorPosition(CenterX - (prompt.Length / 2), CenterY + 2);
            Console.Write(prompt);
        }

        private T LoadingLoop<T>(Task<T> task)
        {
            if (task.IsCompletedSuccessfully)
            {
                return task.Result;
            }

            var message = "Loading .....";
            int i = 0;

            while (!task.IsCompleted)
            {
                i = i > 5 ? 0 : i;
                Console.SetCursorPosition(CenterX - (message.Length / 2), CenterY);
                Console.Write(string.Concat(message.Take(8 + i)));
                i++;
                Thread.Sleep(250);
            }

            // clear the loading text
            Console.SetCursorPosition(CenterX - (message.Length / 2), CenterY);
            Console.Write(message.Take(8 + i));

            return task.Result;
        }

        private int CategorySelection(Dictionary<int, string> categories)
        {
            int categoryID = 0;

            Console.Clear();
            Console.WriteLine("Select from the categories below...");
            Console.WriteLine("________________________________________\n");
            
            // list categories
            for (int i = 0; i < categories.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {categories.ElementAt(i).Value}");
            }

            // prompt for catagory choice
            var isValidResponse = false;
            while (!isValidResponse)
            {
                Console.SetCursorPosition(Console.WindowLeft + 10, Console.WindowHeight - 5);
                Console.Write("\nPlease enter the category number: ");
                var response = Console.ReadLine();
                isValidResponse = int.TryParse(response, out categoryID) 
                                    && categoryID > 0 
                                    && categoryID <= categories.Count();
                
                Console.SetCursorPosition(Console.WindowLeft + 10, Console.WindowHeight - 5);
                Console.Write("                                                        ");
            }

            return categories.ElementAt(categoryID - 1).Key;
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