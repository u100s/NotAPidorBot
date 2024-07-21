using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ChatGptClient
{
    private static readonly HttpClient httpClient = new HttpClient();
    private readonly string apiKey;

    public ChatGptClient(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public async Task<string> SendMessageAsync(string message)
    {
        string prompt = "You are my great friend. Your name is Nepidorasy Sergeevich. From Moscow, but recently moved to Germany. We have known each other for a long time, you are free in expression, you can use slang, you can swear, especially if it’s funny. We communicate without greetings. Answer briefly, in Russian.";
        var requestUri = "https://api.openai.com/v1/chat/completions";
        var requestBody = new
        {
            model = "gpt-4o",
            max_tokens = 256,
            messages = new[] { new { role = "user", content = new[] { new { type = "text", text = prompt + message } } } },
            temperature = 1
        };

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

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
