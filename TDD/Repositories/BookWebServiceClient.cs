using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TDD.objects;

namespace TDD.Repositories
{
    public class BookWebServiceClient : IBookWebService
    {
        private readonly HttpClient _httpClient;

        public BookWebServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Book> FindBookByIsbn(string isbn)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"https://api.example.com/livres/{isbn}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                Book book = JsonSerializer.Deserialize<Book>(jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return book;
            }

            return null;
        }
    }
}