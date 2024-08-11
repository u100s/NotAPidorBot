using System.Text;
using Newtonsoft.Json;

namespace NotAPidorBot.Models.ChatGpt;
public class Client
{
    private static readonly HttpClient httpClient = new HttpClient();
    private readonly string _apiKey;

    public Client(string apiKey)
    {
        this._apiKey = apiKey;
    }

    public async Task<string> SendMessagesContextAsync(RequestBody request)
    {
        var requestUri = "https://api.openai.com/v1/chat/completions";

        var jsonRequestBody = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await httpClient.PostAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error: {response.StatusCode}, Details: {await response.Content.ReadAsStringAsync()}");
        }

        try
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Response>(jsonResponse);
            return responseObject.choices[0].message.content.Trim();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}