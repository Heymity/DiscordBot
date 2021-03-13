 using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Trivia
{
    class DefaultAnswer
    {
        public string Content { get; private set; }
        public bool IsCorrect { get; private set; }

        public DefaultAnswer(string content, bool isCorrect)
        {
            Content = content;
            IsCorrect = isCorrect;
        }
    }
}
