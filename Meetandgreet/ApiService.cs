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
        private static readonly HttpClient _httpClient = new HttpClient
        {
            // FIX: Point this to the ROOT address, not the swagger.json file
            BaseAddress = new Uri("http://localhost:5105/")
        };

        // This makes the method accessible to your LoginPage
        public static async Task<List<User>> GetAllUser()
        {
            try
            {
                // Update this string to match your API's "Get All" endpoint
                return await _httpClient.GetFromJsonAsync<List<User>>("api/Dates/UserSelector");
            }
            catch
            {
                return new List<User>(); // Returns empty list if API is down
            }
        }

        public static async Task<bool> RegisterUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertAUser", user);
            return response.IsSuccessStatusCode;
        }
    }
}

