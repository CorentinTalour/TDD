using System.Text.Json;
using TDD.Models;
using TDD.Repositories.Interfaces;

namespace TDD.Repositories.Implementations
{
    public class BookWebServiceClient : IBookWebService
    {
        private readonly HttpClient _httpClient;

        public BookWebServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Book?> FindBookByIsbn(string isbn)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"https://api.example.com/livres/{isbn}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                Book? book = JsonSerializer.Deserialize<Book>(jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return book;
            }

            return null;
        }
    }
}