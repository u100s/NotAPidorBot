using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NotAPidorBot.Models.GPTReactions;
public class ChatGptClient
{
    private static readonly HttpClient httpClient = new HttpClient();
    private readonly string _apiKey;

    public ChatGptClient(string apiKey)
    {
        this._apiKey = apiKey;
    }

    public async Task<string> SendMessagesContextAsync(GptContext context)
    {
        var requestUri = "https://api.openai.com/v1/chat/completions";
        var requestBody = new ChatGPTRequestBody(context);

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
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
            var responseObject = JsonConvert.DeserializeObject<ChatGptResponse>(jsonResponse);
            return responseObject.choices[0].message.content.Trim();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}



public class ChatGptResponse
{
    public string id { get; set; }
    public string model { get; set; }
    public string created { get; set; }
    public Choice[] choices { get; set; }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
