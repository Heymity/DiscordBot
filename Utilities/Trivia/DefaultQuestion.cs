using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Utilities.Trivia
{
    class DefaultQuestion : IQuestion<DefaultAnswer>
    {

        public string Content {get; private set;}
        public List<DefaultAnswer> Answers { get; private set; }

        public DefaultQuestion(string content, List<DefaultAnswer> answers)
        {
            Content = content;
            Answers = answers;
        }

        public IReadOnlyList<DefaultAnswer> GetAnswers() => Answers;

        public int GetAnswersLenght() => Answers.Count;

        public DefaultAnswer GetCorrectAnswer() => Answers.Where((DefaultAnswer ans) => ans.IsCorrect == true).FirstOrDefault();

        public string GetQuestion() => Content;

        public bool IsCorrect(DefaultAnswer ans) => ans.IsCorrect;

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
