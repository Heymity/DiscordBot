using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Trivia
{
    interface IQuestion<T>
    {
        string GetQuestion();

        T GetCorrectAnswer();

        int GetCorrectAnswerIndex();

        IReadOnlyList<T> GetAnswers();

        int GetAnswersLenght();

        bool IsCorrect(T ans);
    }
}
