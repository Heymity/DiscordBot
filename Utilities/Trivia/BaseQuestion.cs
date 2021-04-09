using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public class BaseQuestion : IQuestion<BaseAnswer>
    {
        private string _content;
        public string Content { get => _content; set => _content = value; }

        public List<BaseAnswer> answers;
        public List<BaseAnswer> Answers { get => answers; set => answers = value; }

        private int _points = 1;
        public int Points { get => _points; set => _points = value; }

        public string GetQuestion() => Content;

        public IReadOnlyList<BaseAnswer> GetAnswers() => Answers;

        public BaseQuestion(string content, List<BaseAnswer> answers, int points)
        {
            Points = points;
            Content = content;
            Answers = answers;
        }

        public BaseQuestion()
        {
            Points = 0;
            Content = "";
            Answers = new List<BaseAnswer>();
        }

        public int GetAnswersLenght() => Answers.Count;

        public BaseAnswer GetCorrectAnswer() => Answers.Where((BaseAnswer ans) => ans.IsCorrect == true).FirstOrDefault();

        public bool IsCorrect(BaseAnswer ans) => ans.IsCorrect;

        public int GetCorrectAnswerIndex()
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers[i].IsCorrect) return i;
            }

            return -1;
        }
    }
}