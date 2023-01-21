using System.Text;

namespace GreyBot.Utils
{
    internal class DataStringViewer<T> where T : class
    {
        public DataStringViewer(IEnumerable<T> data)
            => Data = data.ToArray();

        public DataStringViewer(T[] data)
            => Data = data;

        public T[] Data { get; set; }

        public string GetView(int startIndex, int endIndex, Func<T, string> predicate, Action? completion = null) 
            => GetView(new StringBuilder(), startIndex, endIndex, predicate, completion);

        public string GetView(StringBuilder stringBuilder, int startIndex, int endIndex, Func<T, string> predicate, Action? completion = null)
        {
            if (Data.Length < startIndex)
                return stringBuilder.ToString();

            endIndex = endIndex > Data.Length ? Data.Length : endIndex;

            foreach (var model in Data[startIndex..endIndex])
                stringBuilder.Append(predicate.Invoke(model));

            completion?.Invoke();

            return stringBuilder.ToString();
        }
    }
}
