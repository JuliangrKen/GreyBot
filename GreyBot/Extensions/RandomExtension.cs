using Discord;

namespace GreyBot.Extensions
{
    internal static class RandomExtension
    {
        public static Color NextColor(this Random random)
            => new(random.Next(byte.MaxValue), random.Next(byte.MaxValue), random.Next(byte.MaxValue));
    }
}
