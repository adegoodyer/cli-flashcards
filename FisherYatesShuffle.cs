using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineFlashcardApp
{
    class FisherYatesShuffle
    {
        // https://www.dotnetperls.com/fisher-yates-shuffle

        private static Random random = new Random();

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for(int i = 0; i < array.Length - 1; i++)
            {
                int r = i + random.Next(n - 1);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
    }
}
