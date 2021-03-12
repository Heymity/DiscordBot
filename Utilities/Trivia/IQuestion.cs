﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utilities.Trivia
{
    interface IQuestion<T>
    {
        string GetQuestion();

        T GetCorrectAnswer();

        Span<T> GetAnswers();

        bool IsCorrect(T ans);
    }
}
