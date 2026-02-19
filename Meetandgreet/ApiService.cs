using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDates;



    using System.Net.Http;
    using System.Net.Http.Json; // Makes sending/receiving JSON easy
namespace Meetandgreet
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService() 
        {
            _httpClient = new HttpClient();
            // Replace this with your actual API URL (usually found when you run the API project)
            _httpClient.BaseAddress = new Uri("http://localhost:5105/swagger/v1/swagger.json");
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            // This sends your model as JSON to your API Controller
            var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertAUser", user);
            return response.IsSuccessStatusCode;
        }
    }
}

