namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public class BaseAnswer
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }

        public BaseAnswer(string content, bool isCorrect)
        {
            Content = content;
            IsCorrect = isCorrect;
        }

        public BaseAnswer()
        {
            Content = "";
            IsCorrect = false;
        }
    }
}