using ModelDates;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Meetandgreet
{
    public static class ApiService
    {
        public static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://krpx2rgs-5105.uks1.devtunnels.ms/")
        };

        // --- User Methods ---
        public static async Task<List<User>> GetAllUser()
        {
            try { return await _httpClient.GetFromJsonAsync<List<User>>("api/Dates/UserSelector"); }
            catch { return new List<User>(); }
        }

        public static async Task<User> GetUserByIdAsync(int userId)
        {
            try { return await _httpClient.GetFromJsonAsync<User>($"api/Dates/GetUser/GetUser/{userId}"); }
            catch { return null; }
        }

        public static async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertAUser", user);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        public static async Task<bool> InsertPreferencesAsync(Preferences p)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertAPreferences", p);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // THIS IS THE METHOD YOU WERE MISSING IN EDITPROFILE:
        public static async Task<string> UpdateUserAsync(User user)
        {
            try
            {
                // 1. Create an anonymous object that matches the structure of your UserUpdateDTO
                var updateDto = new
                {
                    Id = user.Id,
                    Bio = user.Bio,
                    Profilepic = user.ProfilePic
                };

                // 2. Send the DTO instead of the full 'user' object
                var response = await _httpClient.PutAsJsonAsync("api/Dates/UpdateAUser/UpdateUser", updateDto);

                // 3. Return the result
                return response.StatusCode.ToString() + ": " + await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        public static async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Dates/DeleteUser/DeleteUser/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // --- Discovery, Matches, & Messages ---
        public static async Task<List<City>> GetCitiesAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<City>>("api/Dates/CitySelector"); }
            catch { return new List<City>(); }
        }

        public static async Task<Preferences> GetPreferencesByUserIdAsync(int userId)
        {
            try { return await _httpClient.GetFromJsonAsync<Preferences>($"api/Dates/PreferencesSelector/{userId}"); }
            catch { return null; }
        }

        public static async Task<List<User>> GetDiscoveryFeedAsync(User currentUser, Preferences prefs)
        {
            try
            {
                var payload = new { UserId = currentUser.Id, Preferences = prefs };
                var response = await _httpClient.PostAsJsonAsync("api/Dates/GetDiscoveryFeed", payload);
                if (!response.IsSuccessStatusCode) return new List<User>();
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch { return new List<User>(); }
        }

        public static async Task<bool> LikeUserAsync(int likerId, int likedId)
        {
            try
            {
                // Change this line in your ApiService:
                var response = await _httpClient.PostAsync($"api/Dates/LikeUser/LikeUser?likerId={likerId}&likedId={likedId}", null);
                // This will throw an exception if the server returns a 404 or 500 error
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                // Add a breakpoint here!
                System.Diagnostics.Debug.WriteLine($"API ERROR: {ex.Message}");
                return false;
            }
        }

        public static async Task<List<Matches>> GetMatchesForUserAsync(int userId)
        {
            try { return await _httpClient.GetFromJsonAsync<List<Matches>>($"api/Dates/GetMatchesForUser/GetMatchesForUser/{userId}"); }
            catch { return new List<Matches>(); }
        }

        public static async Task<List<Messages>> GetMessagesByMatchIdAsync(int matchId)
        {
            try { return await _httpClient.GetFromJsonAsync<List<Messages>>($"api/Dates/GetMessages/GetMessages/{matchId}"); }
            catch { return new List<Messages>(); }
        }

        public static async Task<bool> SendMessageAsync(Messages msg)
        {
            try
            {
                // Create the simple object structure
                var dto = new
                {
                    msg.MatchID,
                    msg.SenderID,
                    msg.MessageText
                };

                var response = await _httpClient.PostAsJsonAsync("api/Dates/SendMessage/SendMessage", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public static async Task<GenderList> GetGenderListIdAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<GenderList>($"api/Dates/GenderSelector"); }
            catch { return null; }
        }

        public static async Task<bool> CheckIsManagerAsync(int userId)
        {
            try
            {
                // This now expects a true/false response
                return await _httpClient.GetFromJsonAsync<bool>($"api/Dates/IsManager/{userId}");
            }
            catch
            {
                // If an error occurs (or the API returns 404), return false
                return false;
            }
        }
        // --- Manager Methods ---
        public static async Task<bool> VerifyManagerPasswordAsync(int userId, string password)
        {
            try
            {
                // This must match the URL that worked in your browser
                return await _httpClient.GetFromJsonAsync<bool>($"api/Dates/VerifyManager/{userId}/{password}");
            }
            catch (Exception ex)
            {
                // Log the error to see exactly what is happening if it still fails
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        // --- Photo Management Methods ---
        // Change to match Swagger: /api/Dates/GetPhotos/GetPhotos/{userId}
        public static async Task<List<Photos>> GetPhotosByUserIdAsync(int userId)
        {
            try { return await _httpClient.GetFromJsonAsync<List<Photos>>($"api/Dates/GetPhotos/GetPhotos/{userId}"); }
            catch { return new List<Photos>(); }
        }

        // Change to match Swagger: /api/Dates/InsertPhoto/InsertPhoto
        public static async Task<bool> UploadPhotoAsync(int userId, string url)
        {
            var dto = new { UserId = userId, Url = url };
            var response = await _httpClient.PostAsJsonAsync("api/Dates/InsertPhoto/InsertPhoto", dto);
            return response.IsSuccessStatusCode;
        }

        // Change to match Swagger: /api/Dates/DeletePhoto/DeletePhoto/{photoId}
        public static async Task<bool> DeletePhotoAsync(int photoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Dates/DeletePhoto/DeletePhoto/{photoId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }



    }
}