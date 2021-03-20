using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Utilities.Trivia
{
    [System.Serializable]
    public class BaseQuestion : IQuestion<BaseAnswer>
    {
        public string Content {get; private set;}
        public List<BaseAnswer> Answers { get; private set; }

        public BaseQuestion(string content, List<BaseAnswer> answers)
        {
            Content = content;
            Answers = answers;
        }

        public IReadOnlyList<BaseAnswer> GetAnswers() => Answers;

        public int GetAnswersLenght() => Answers.Count;

        public BaseAnswer GetCorrectAnswer() => Answers.Where((BaseAnswer ans) => ans.IsCorrect == true).FirstOrDefault();

        public string GetQuestion() => Content;

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
