using ModelDates;
using System;
using System.Collections.Generic;
using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
// Makes sending/receiving JSON easy
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
namespace Meetandgreet
{
    public class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            // FIX: Pointed directly to your active VS Dev Tunnel address
            BaseAddress = new Uri("https://fdgrrqj8-5105.euw.devtunnels.ms/")
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

        public static async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                // This must match the route in your Controller exactly
                string url = $"api/Dates/GetUser/GetUser/{userId}";

                return await _httpClient.GetFromJsonAsync<User>(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user: {ex.Message}");
                return null;
            }
        }
        public static async Task<bool> RegisterUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertAUser", user);

            // IF THE REGISTRATION FAILS, WE CAPTURE THE SECRET REASON HERE:
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("API ERROR REASON: " + errorContent);
            }

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                // Make sure this route matches your Delete endpoint in your Controller
                var response = await _httpClient.DeleteAsync($"api/Dates/DeleteUser/DeleteUser/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<List<City>> GetCitiesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<City>>("api/Dates/CitySelector");
            }
            catch
            {
                return new List<City>();
            }
        } 

        public static async Task<Preferences> GetPreferencesByUserIdAsync(int userId)
        {
            try
            {
                // 1. Updated to match the doubled path: api/Dates/PreferencesSelector/PreferencesSelector
                string url = $"api/Dates/PreferencesSelector/PreferencesSelector?userId={userId}";
                System.Diagnostics.Debug.WriteLine($"Fetching Preferences from: {url}");

                // 2. Use a direct request
                var response = await _httpClient.GetAsync(url);

                // 3. Log the result
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Preferences>();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to fetch preferences. Status: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR in GetPreferencesAsync: {ex.Message}");
                return null;
            }
        }

        public static async Task<List<User>> GetDiscoveryFeedAsync(User currentUser, Preferences prefs)
        {
            if (prefs == null) return new List<User>();

            try
            {
                string url = $"api/Dates/GetDiscoveryFeed" +
                             $"?currentUserId={currentUser.Id}" +
                             $"&preferredGenderId={prefs.PreferredGender.Id}" +
                             $"&minAge={prefs.AgeMin}" +
                             $"&maxAge={prefs.AgeMax}" +
                             $"&maxDistance={prefs.DistanceMax}";

                // ADDED: Print the EXACT URL to the Output window
                System.Diagnostics.Debug.WriteLine("--- SENDING API REQUEST ---");
                System.Diagnostics.Debug.WriteLine("URL: " + url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    return new List<User>();
                }

                var results = await response.Content.ReadFromJsonAsync<List<User>>();
                System.Diagnostics.Debug.WriteLine($"API returned {results?.Count ?? 0} users.");

                return results ?? new List<User>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return new List<User>();
            }
        }
        public static async Task<bool> LikeUserAsync(int likerId, int likedId)
        {
            try
            {
                // We use a POST request because we are changing data in the database
                // We pass the IDs as query parameters to match the controller method
                string url = $"api/Dates/LikeUser?likerId={likerId}&likedId={likedId}";

                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    // Read the JSON response to see if a match was found
                    var result = await response.Content.ReadFromJsonAsync<MatchResult>();
                    return result?.matchFound ?? false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error liking user: {ex.Message}");
            }

            return false;
        }

        // Add this small helper class at the bottom of ApiService.cs 
        // or in your Models folder to handle the API response
        public class MatchResult
        {
            public bool matchFound { get; set; }
        }

        public static async Task<List<Matches>> GetMatchesForUserAsync(int userId)
        {
            try
            {
                // Swagger shows the route is doubled: "api/Dates/GetMatchesForUser/GetMatchesForUser/{userId}"
                string url = $"api/Dates/GetMatchesForUser/GetMatchesForUser/{userId}";

                System.Diagnostics.Debug.WriteLine($"Fetching Matches from: {url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Matches>>();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to fetch matches. Status: {response.StatusCode}");
                    return new List<Matches>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetMatchesForUserAsync: {ex.Message}");
                return new List<Matches>();
            }
        }

        public static async Task<List<Messages>> GetMessagesByMatchIdAsync(int matchId)
        {
            try
            {
                // Updated URL to match the doubled route pattern: api/Dates/GetMessages/GetMessages/{matchId}
                string url = $"api/Dates/GetMessages/GetMessages/{matchId}";

                System.Diagnostics.Debug.WriteLine($"Fetching Messages from: {url}");

                return await _httpClient.GetFromJsonAsync<List<Messages>>(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching messages: {ex.Message}");
                return new List<Messages>();
            }
        }

        public static async Task<bool> SendMessageAsync(Messages msg)
        {
            try
            {
                // Must match the doubled route pattern: api/Dates/SendMessage/SendMessage
                string url = "api/Dates/SendMessage/SendMessage";

                var response = await _httpClient.PostAsJsonAsync(url, msg);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }
    } 
}

