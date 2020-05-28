using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CommandLineFlashcardApp
{
    class DeckManager
    {
        // Application properties
        private string decksDirectory = $"{Directory.GetCurrentDirectory()}\\Decks";
        private string scoresDataLocation = @"scores.json";

        // Settings
        private string settingsLocation = $"{Directory.GetCurrentDirectory()}\\settings.json";
        private Dictionary<string, string> settingsDictionary = new Dictionary<string, string>();

        // Current deck
        public string currentDeckName { get; set; }
        private string currentDeckLocation;
        private List<Flashcard> currentDeck = new List<Flashcard>();
        private List<string> availableDecks = new List<string>();
        private Dictionary<string, string> scoresDictionary = new Dictionary<string, string>();

        public DeckManager()
        {
            // Initialize variables
            currentDeckLocation = getDefaultDeckLocation();

            // Load settings
            if (File.Exists(settingsLocation))
            {
                loadSettings();
            }
            else
            {

            }

            // Load default deck
            if (File.Exists(getDefaultDeckLocation()))
            {
                loadDeck(getDefaultDeckName());
            }

            // Populate list of available decks
            populateAvailableDecksArray();

            // Deserialize scores data and load into memory
            if (File.Exists(@"scores.json"))
            {
                loadScores();
            }
        }
        public Flashcard[] getCurrentDeckAsArray()
        {
            return currentDeck.ToArray();
        }
        public Flashcard[] getRandomisedFlashcardsArray()
        {
            // TODO - return randomised array
            return getCurrentDeckAsArray();
        }
        public void populateAvailableDecksArray()
        {
            availableDecks.Clear();
            string[] files = Directory.GetFiles(decksDirectory);
            foreach (string file in files)
            {
                availableDecks.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
        public string[] getAvailableDecksAsArray()
        {
            return availableDecks.ToArray();
        }
        public void addFlashcard(string newQuestion, string newAnswer)
        {
            currentDeck.Add(new Flashcard(newQuestion, newAnswer));
            saveDeck();
        }
        public void removeFlashcard(int flashcardNumber)
        {
            currentDeck.RemoveAt(flashcardNumber);
        }
        public void addScore(string score)
        {
            scoresDictionary.Add(DateTime.Now.ToString(), score);
        }
        public Dictionary<string, string> getScoresDictionary()
        {
            return new Dictionary<string, string>(scoresDictionary.Reverse());
        }
        public async void saveScores()
        {
            using (FileStream fs = File.Create(@"scores.json"))
            {
                await JsonSerializer.SerializeAsync(fs, scoresDictionary);
            }
        }
        public void loadScores()
        {
            try
            {
                string jsonString = File.ReadAllText(scoresDataLocation);
                scoresDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
        public void clearScores()
        {
            scoresDictionary.Clear();
        }
        public string generateDeckPath(string deckName)
        {
            return $"{decksDirectory}\\{deckName.ToLower()}.json";
        }
        public async void saveDeck()
        {
            using (FileStream fs = File.Create(currentDeckLocation))
            {
                await JsonSerializer.SerializeAsync(fs, currentDeck);
            }
        }
        public async void loadDeck (string deckName)
        {
            // Create correct path
            string deckPath = generateDeckPath(deckName);

            // Load deck
            using (FileStream fs = File.OpenRead(deckPath))
            {
                currentDeck = await JsonSerializer.DeserializeAsync<List<Flashcard>>(fs);
            }

            // Change current deck name
            currentDeckName = deckName;
            currentDeckLocation = deckPath;
        }
        public string backupDecks ()
        {
            string userMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string backupFolder = $"{userMyDocumentsFolder}\\FlashCardsBackup";
            string datedBackupFolder = $"{backupFolder}\\{DateTime.Now.ToString("ddd-dd-MMMM-yyyy--HH-mm-ss")}";

            // If backup folder doesn't exist then create
            if (!Directory.Exists($"{userMyDocumentsFolder}\\FlashCardsBackup"))
            {
                Directory.CreateDirectory(backupFolder);
            }

            // Create new dated directory in backup folder
            Directory.CreateDirectory(datedBackupFolder);

            //Copy decks folder to dated directory
            FileSystem.CopyDirectory(decksDirectory, datedBackupFolder);


            return $"\nThe folder {decksDirectory}\nhas been backed up to { datedBackupFolder}";

        }
        public void newDeck (string newDeckName)
        {
            currentDeckName = newDeckName;
            currentDeckLocation = generateDeckPath(newDeckName);
            currentDeck.Clear();
            availableDecks.Add(newDeckName.ToLower());
            saveDeck();
        }
        public void renameDeck (string existingDeckName, string newDeckName)
        {
            if (File.Exists(generateDeckPath(existingDeckName)))
            {
                File.Move(generateDeckPath(existingDeckName), generateDeckPath(newDeckName.ToLower()));

                if (currentDeckName.Equals(existingDeckName))
                {
                    currentDeckName = existingDeckName;
                    currentDeckLocation = generateDeckPath(existingDeckName);
                }
                populateAvailableDecksArray();
            }
        }
        public void removeDeck(string removeDeckName)
        {
            // Generate path for deck to be deleted
            string deckPath = generateDeckPath(removeDeckName);

            // Delete deck
            File.Delete(deckPath);

            // Re-populate Available decks array
            populateAvailableDecksArray();

            // Check deleted deck wasn't default
            if (getDefaultDeckName().Equals(removeDeckName.ToLower()))
            {
                setDefaultDeckName("sample");
            }

            // Load default deck
            loadDeck(getDefaultDeckName());
        }
        public string getDefaultDeckName()
        {
            string value = "";
            // Try and get default deck name from dictionary, else return "sample"
            if (!settingsDictionary.TryGetValue("defaultDeck", out value))
                {
                value = "sample";
                }
            return value;
        }
        public void setDefaultDeckName(string deckName)
        {
            settingsDictionary["defaultDeck"] = deckName.ToLower();
            saveSettings();
        }
        public string getDefaultDeckLocation()
        {

            return generateDeckPath(getDefaultDeckName());
        }
        public async void saveSettings()
        {
            using (FileStream fs = File.Create(settingsLocation))
            {
                await JsonSerializer.SerializeAsync(fs, settingsDictionary);
            }
        }
        public void loadSettings()
        {
            try
            {
                string jsonString = File.ReadAllText(settingsLocation);
                settingsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
    }
}
