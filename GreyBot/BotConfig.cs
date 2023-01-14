using System.Text.Json;

namespace GreyBot
{
    public class BotConfig
    {
        public string? BotToken { get; set; }
        public ulong? DevGuildId { get; set; }

        public static BotConfig GetFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<BotConfig>(json)
                ?? throw new NullReferenceException();
        }
    }
}
