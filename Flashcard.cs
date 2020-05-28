using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineFlashcardApp
{
    class Flashcard
    {
        public string question { get; set; }
        public string answer { get; set; }
        public Flashcard()
        {
            question = "";
            answer = "";
        }
        public Flashcard(string newQuestion, string newAnswer)
        {
            question = newQuestion;
            answer = newAnswer;
        }
    }
}
