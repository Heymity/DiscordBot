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

        public Span<DefaultAnswer> GetAnswers() => new Span<DefaultAnswer>(Answers.ToArray());
        
        public DefaultAnswer GetCorrectAnswer() => Answers.Where((DefaultAnswer ans) => ans.IsCorrect == true).FirstOrDefault();

        public string GetQuestion() => Content;

        public bool IsCorrect(DefaultAnswer ans) => ans.IsCorrect;
    }
}
