using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CommandLineFlashcardApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DeckManager deckManager = new DeckManager();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu(deckManager);
            }
        }
        private static bool MainMenu(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Main Menu");
            Console.Write("Current Deck:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {ToTitleCase(deckManager.currentDeckName)}\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("\t t) Take Test");
            Console.WriteLine("\t d) Switch Decks\n");

            Console.WriteLine("\t p) View Flashcards");
            Console.WriteLine("\t a) Add FlashCard");
            Console.WriteLine("\t r) Remove FlashCard\n");

            Console.WriteLine("\t n) New Deck");
            Console.WriteLine("\t b) Rename Deck");
            Console.WriteLine("\t m) Delete Deck\n");

            Console.WriteLine("\t s) Print Scores");
            Console.WriteLine("\t c) Clear Scores\n");

            Console.WriteLine("\t u) Backup Decks");
            Console.WriteLine("\t j) Set Default Deck\n");
            
            Console.WriteLine("\t e) Exit");
            
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "d":
                    switchDecks(deckManager);
                    return true;
                case "t":
                    runTest(deckManager);
                    return true;
                case "p":
                    printFlashcards(deckManager);
                    return true;
                case "a":
                    addFlashcard(deckManager);
                    return true;
                case "r":
                    removeFlashcard(deckManager);
                    return true;
                case "n":
                    newDeck(deckManager);
                    return true;
                case "m":
                    deleteDeck(deckManager);
                    return true;
                case "b":
                    renameDeck(deckManager);
                    return true;
                case "j":
                    setDefaultDeckName(deckManager);
                    return true;
                case "s":
                    printScores(deckManager);
                    return true;
                case "c":
                    clearScores(deckManager);
                    return true;
                case "u":
                    backupDecks(deckManager);
                    return true;
                case "e":
                    deckManager.saveDeck();
                    deckManager.saveScores();
                    deckManager.saveSettings();
                    return false;
                default:
                    return true;
            }
        }
        private static void printHeading(string headingText)
        {
            Console.WriteLine("");
            Console.WriteLine(headingText);
            Console.WriteLine(new String('*', headingText.Length));
            Console.WriteLine();
        }
        public static string ToTitleCase(string inputString)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(inputString.ToLower());
        }
        public static void printWarning(string input)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(input);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void switchDecks(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Available Decks");

            // List available decks
            for(int i = 0; i < deckManager.getAvailableDecksAsArray().Length; i++)
            {
                Console.WriteLine($"\t {i + 1}) {ToTitleCase(deckManager.getAvailableDecksAsArray()[i])}");
            }

            Console.Write("\nWhich deck do you want to switch to? ");
            try
            {
                int selection = int.Parse(Console.ReadLine());

                //load deck selection
                deckManager.loadDeck(deckManager.getAvailableDecksAsArray()[selection - 1]);

            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                {
                    Console.WriteLine("\nYou didn't choose a valid deck. Press any key to try again..");
                    Console.ReadLine();
                    switchDecks(deckManager);
                    return;
                }
                throw;
            }
            
        }
        private static void runTest(DeckManager deckManager)
        {
            // Initialise Test
            int score = 0;
            List<int> incorrectAnswersList = new List<int>();
            Flashcard[] flashcardArray = deckManager.getRandomisedFlashcardsArray();
            
            if (flashcardArray.Length != 0)
            {
                Console.Clear();
                Console.Write("\nPress any key when you are ready to begin..");
                Console.ReadLine();
                Console.Clear();

                // Questions
                for (int i = 0; i < flashcardArray.Length; i++)
                {
                    printHeading($"Question {i + 1}");
                    Console.Write($"\t Question: {flashcardArray[i].question} ");
                    Console.ReadLine();
                    Console.Write($"\n\n\t Answer: {flashcardArray[i].answer}\n\n\nPress the spacebar (Or NumPad 0) if you got it right!");

                    ConsoleKey input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Spacebar || input == ConsoleKey.NumPad0)
                    {
                        score += 1;
                    }
                    else
                    {
                        incorrectAnswersList.Add(i);
                    }
                    Console.Clear();
                }

                // Test Results
                int scorePercentage = 0;
                try
                {
                    scorePercentage = Convert.ToInt32(100.0 / flashcardArray.Length * score);
                }
                catch (DivideByZeroException) {}

                deckManager.addScore($"{ToTitleCase(deckManager.currentDeckName)} : {score}/{flashcardArray.Length} : {scorePercentage}%");
                printHeading("Test Results");
                Console.WriteLine("\t You have completed the test!\n");
                Console.WriteLine($"\t Percentage: {scorePercentage}%\n");
                Console.WriteLine($"\t Your final score was {score} out of {flashcardArray.Length}\n");

                // Incorrect questions
                if (incorrectAnswersList.Count != 0)
                {
                    printHeading("Incorrect Flashcards");
                    for (int i = 0; i < incorrectAnswersList.Count; i++)
                    {
                        Console.WriteLine($"\t Question {i + 1}: {flashcardArray[incorrectAnswersList[i]].question}");
                    }
                    Console.WriteLine("");
                }
                Console.Write("\nPress any key to return to the main menu..");
                Console.ReadLine();
            
            }
            else
            {
                Console.Clear();
                Console.Write("\nYou have no flashcards to test. Choose 'Add New Flashcard' from the main menu..");
                Console.ReadLine();
                Console.Clear();
            }
        }
        private static void addFlashcard(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Add A New Question");
            Console.Write("\t Enter your new question: ");
            string newQuestion = Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t Answer: ");
            string newAnswer = Console.ReadLine();
            deckManager.addFlashcard(newQuestion, newAnswer);
        }
        private static void removeFlashcard(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Remove Flashcard");

            // List available flashcards
            Flashcard[] flashCardsArray = deckManager.getCurrentDeckAsArray();
            if (flashCardsArray.Length != 0)
            {

                for (int i = 0; i < flashCardsArray.Length; i++)
                {
                    Console.WriteLine($"\t {i + 1}) Question {i + 1}: {flashCardsArray[i].question}");
                    Console.WriteLine($"\t\t Answer: {flashCardsArray[i].answer}\n");
                }

                Console.Write("What Flashcard would you like to remove? (0 to return to main menu): ");
                try
                {
                    int response = int.Parse(Console.ReadLine());
                    // If user types 0 then exit to main menu
                    if (response == 0)
                    {
                        return;
                    }
                    printWarning("\nAre you sure? Press Y to confirm (any other key to return to main menu)..");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        deckManager.removeFlashcard(response - 1);
                    }

                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nYou didn't choose a valid flashcard. Press any key to try again..");
                        Console.ReadLine();
                        removeFlashcard(deckManager);
                        return;
                    }
                    throw;
                }
            }
            else
            {
                Console.Clear();
                Console.Write("\nYou have no flashcards to remove. Choose 'Add New Flashcard' from the main menu..");
                Console.ReadLine();
                Console.Clear();
            }

            
        }
        private static void newDeck(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("New Deck");
            Console.Write("\t Name your new deck: ");
            string newDeckName = Console.ReadLine();
            deckManager.newDeck(newDeckName);
        }
        private static void deleteDeck(DeckManager deckManager)
        {
            if (deckManager.getAvailableDecksAsArray().Length > 0)
            {
                Console.Clear();
                printHeading("Delete Deck");

                for (int i = 0; i < deckManager.getAvailableDecksAsArray().Length; i++)
                {
                    Console.WriteLine($"\t {i + 1}) {ToTitleCase(deckManager.getAvailableDecksAsArray()[i])}");
                }

                Console.Write("\nWhich deck do you want to delete? (0 to exit to main menu) ");

                try
                {
                    int selection = int.Parse(Console.ReadLine());
            
                    if (selection == 0)
                    {
                        return;
                    }
                    else if (deckManager.getAvailableDecksAsArray()[selection - 1].Equals("sample"))
                    {
                        Console.Write("\nUnable to delete the sample deck as it is protected. Press any key to return to the main menu..");
                        Console.ReadLine();
                        return;

                    }
                    printWarning($"\nAre you sure you wish to delete {ToTitleCase(deckManager.getAvailableDecksAsArray()[selection - 1])}? Press Y to confirm...");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        deckManager.removeDeck(deckManager.getAvailableDecksAsArray()[selection - 1]);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nYou didn't choose a valid deck. Press any key to try again..");
                        Console.ReadLine();
                        deleteDeck(deckManager);
                        return;
                    }
                    throw;
                }

            }
            else
            {
                Console.Clear();
                printHeading("New Deck");
                Console.Write("Name your new deck: ");
                string newDeckName = Console.ReadLine();
                deckManager.newDeck(newDeckName);
            }

        }
        private static void renameDeck(DeckManager deckManager)
        {
            // As long as there exists a deck
            if (deckManager.getAvailableDecksAsArray().Length > 0)
            {
                Console.Clear();
                printHeading("Rename Deck");


                // List available decks
                for (int i = 0; i < deckManager.getAvailableDecksAsArray().Length; i++)
                {
                    Console.WriteLine($"\t {i + 1}) {ToTitleCase(deckManager.getAvailableDecksAsArray()[i])}");
                }

                Console.Write("\nWhich deck do you want to rename? (0 to exit to main menu) ");

                try
                {
                    int selection = int.Parse(Console.ReadLine());

                    if (selection == 0)
                    {
                        return;
                    }
                    try
                    {
                        Console.Write("\nWhat would you like to rename the deck to? ");
                        string newDeckName = Console.ReadLine();

                        printWarning($"\nAre you sure you wish to rename {ToTitleCase(deckManager.getAvailableDecksAsArray()[selection - 1])} to {ToTitleCase(newDeckName)}? Press Y to confirm...");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            deckManager.renameDeck(deckManager.getAvailableDecksAsArray()[selection - 1], newDeckName);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                        {
                            Console.WriteLine("\nYou didn't choose a valid deck. Press any key to try again..");
                            Console.ReadLine();
                            renameDeck(deckManager);
                            return;
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nYou didn't choose a valid deck. Press any key to try again..");
                        Console.ReadLine();
                        renameDeck(deckManager);
                        return;
                    }
                    throw;
                }
            }
            else
            {
                Console.Clear();
                printHeading("New Deck");
                Console.Write("Name your new deck: ");
                string newDeckName = Console.ReadLine();
                deckManager.newDeck(newDeckName);
            }
        }
        private static void backupDecks(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Backup Decks");
            Console.WriteLine($"\t {deckManager.backupDecks()}");
            Console.ReadLine();
        }
        private static void setDefaultDeckName(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Set Default Deck");

            // Show available decks
            for (int i = 0; i < deckManager.getAvailableDecksAsArray().Length; i++)
            {
                Console.WriteLine($"\t {i + 1}) {ToTitleCase(deckManager.getAvailableDecksAsArray()[i])}");
            }

            Console.Write("\nWhich deck do you want to be the default? ");

            try
            {
                int selection = int.Parse(Console.ReadLine());
                deckManager.setDefaultDeckName(deckManager.getAvailableDecksAsArray()[selection - 1]);
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentException || ex is IndexOutOfRangeException)
                {
                    Console.WriteLine("\nYou didn't choose a valid deck. Press any key to try again..");
                    Console.ReadLine();
                    setDefaultDeckName(deckManager);
                    return;
                }
                throw;
            }

        }
        private static void printFlashcards(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("All Flashcards");
            Flashcard[] flashCardsArray = deckManager.getCurrentDeckAsArray();

            if (flashCardsArray.Length != 0)
            {

                for (int i = 0; i < flashCardsArray.Length; i++)
                {
                    Console.WriteLine($"\t Question {i + 1}: {flashCardsArray[i].question}");
                    Console.WriteLine($"\t\t Answer: {flashCardsArray[i].answer}\n");
                }
                Console.Write("\nPress any key to return to the main menu..");
                Console.ReadLine();
            }
            else
            {
                Console.Clear();
                Console.Write("\nYou have no flashcards to view. Choose 'Add New Flashcard' from the main menu..");
                Console.ReadLine();
                Console.Clear();
            }
        }
        private static void printScores(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Previous Scores");
            Dictionary<string, string> scoresDictionary = deckManager.getScoresDictionary();

            if (scoresDictionary.Count != 0)
            {
                foreach (KeyValuePair<string, string> score in scoresDictionary)
                {
                    Console.WriteLine($"\t {score.Key} : {score.Value}");
                }
            }
            else
            {
                Console.WriteLine("None. Take a test now to submit a score.\n");     
            }
            Console.Write("\nPress any key to return to the main menu..");
            Console.ReadLine();
        }
        private static void clearScores(DeckManager deckManager)
        {
            Console.Clear();
            printHeading("Clear Scores");
            printWarning("\nAre you sure? Press Y to confirm (any other key to return to main menu)..");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                deckManager.clearScores();
            }
        }
    }
}
