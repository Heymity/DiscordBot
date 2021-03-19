namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public class BaseAnswer
    {
        public string Content { get; private set; }
        public bool IsCorrect { get; private set; }

        public BaseAnswer(string content, bool isCorrect)
        {
            Content = content;
            IsCorrect = isCorrect;
        }
    }
}
