﻿using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    public interface IQuestion<T> where T : BaseAnswer
    {
        public int Points { get; set; }

        string GetQuestion();

        T GetCorrectAnswer();

        int GetCorrectAnswerIndex();

        IReadOnlyList<T> GetAnswers();

        int GetAnswersLenght();

        bool IsCorrect(T ans);
    }
}
