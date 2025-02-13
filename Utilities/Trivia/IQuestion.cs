﻿using System.Collections.Generic;

namespace DiscordBot.Utilities.Trivia
{
    public interface IQuestion<T> where T : BaseAnswer
    {
        public int Points { get; set; }

        public string GetQuestion();

        public IReadOnlyList<T> GetAnswers();

        T GetCorrectAnswer();

        int GetCorrectAnswerIndex();

        int GetAnswersLenght();

        bool IsCorrect(T ans);
    }
}