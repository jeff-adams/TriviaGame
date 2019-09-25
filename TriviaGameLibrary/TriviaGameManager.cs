using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TriviaGameLibrary
{
    public class TriviaGameManager
    {
        private const string apiURLRequestBase = "https://opentdb.com/api.php?";
        private const string apiTokenRequest = "https://opentdb.com/api_token.php?command=request";
        private const string apiCategoryRequest = "https://opentdb.com/api_category.php";

        private string apiToken;
        private readonly HttpClient client; 

        public TriviaGameManager()
        {
            client = new HttpClient();
            apiToken = GetAPITokenAsync().Result;
        }

        public async Task<Dictionary<int, string>> GetCategoriesAsync()
        {
            var categories = new Dictionary<int, string>();

            using var jsonResult = await GetJsonDocumentAsync(apiCategoryRequest);
            foreach(var category in jsonResult.RootElement.GetProperty("trivia_categories").EnumerateArray())
            {
                categories.Add(category.GetProperty("id").GetInt32(), 
                               category.GetProperty("name").GetString());
            }

            return categories;
        }

        public async Task<List<Question>> GetQuestionsAsync(int amount, int categoryID = 0)
        {
            // clamp the amount of questions to at least 1 and the maximum of 50
            amount = Math.Clamp(amount, 1, 50);

            // if no categoryID is given then questions are from random categories
            var questionURL = categoryID > 0
                            ? $"{apiURLRequestBase}amount={amount}&category={categoryID}&type=multiple&encode=base64"
                            : $"{apiURLRequestBase}amount={amount}&type=multiple&encode=base64";


            using var jsonResult = await GetJsonDocumentAsync(questionURL);
            var questions = new List<Question>();
            foreach (var item in jsonResult.RootElement.GetProperty("results").EnumerateArray())
            {   
                var question = new Question()
                {
                    Category = item.GetProperty("category").GetString(),
                    Type = item.GetProperty("type").GetString(),
                    Difficulty = item.GetProperty("difficulty").GetString(),
                    QuestionText = item.GetProperty("question").GetString().DecodeBase64(),
                    CorrectAnswer = item.GetProperty("correct_answer").GetString().DecodeBase64(),
                    IncorrectAnswers = item.GetProperty("incorrect_answers").EnumerateArray().Select(x => x.GetString().DecodeBase64()).ToList(),
                };

                questions.Add(question);
            }

            return questions;
        }

        private async Task<JsonDocument> GetJsonDocumentAsync(string url)
        {
            return await JsonDocument.ParseAsync(await client.GetStreamAsync(url));
        }

        private async Task<string> GetAPITokenAsync()
        {
            using var jsonResult = await GetJsonDocumentAsync(apiTokenRequest);
            return jsonResult.RootElement.GetProperty("token").GetString();
        }
    }
}
