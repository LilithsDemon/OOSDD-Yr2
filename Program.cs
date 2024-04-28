using System;
using System.IO;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Translations;
using Encryption;
using DB;
using JollyWrapper;

class Menu
{
    private bool finished = false; // IF the user wants to exit the program
    private DatabaseConnection database = new(); // Database connection for logging conversions
    public void StartMenu()
    {
        Console.Clear();
        while (!finished)
        {
            Console.WriteLine("1. Translate into Morse code");
            Console.WriteLine("2. Translate from Morse code");
            Console.WriteLine("3. Training");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            bool correct_value = int.TryParse(Console.ReadLine() ?? "0", out int choice);
            Console.Clear();
            if(!correct_value) 
            {   
                Console.WriteLine("Please enter a numerical value");
                continue;
            }
            switch (choice)
            {
                case 1:
                    TranslateIntoMorse();
                    break;
                case 2:
                    TranslateFromMorse();
                    break;
                case 3:
                    Training();
                    break;
                case 4:
                    finished = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

    }

    // Reads all files in the Translations folder and allows the user to choose a language
    protected string ChooseLanguage()
    {
        while(true)
        {
            Console.WriteLine("Choose language:");
            string file_location = "./Translations/"; // Location of the translation files
            string[] files = Directory.GetFiles(file_location); // Get all files in the directory
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {files[i]}"); // Display the files to the user
            }
            bool correct_value = int.TryParse(Console.ReadLine() ?? "0", out int choice); // Get the user's choice
            if(!correct_value) 
            {   
                Console.WriteLine("Please enter a numerical value");
                continue;
            }
            if(choice < 1 || choice > files.Length)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }
            else
            {
                return files[choice - 1];
            }
        }
    }

    public void TranslateIntoMorse()
    {
        Console.Clear();
        string file = ChooseLanguage(); // Get's the chosen language file

        TranslationSet set = new(file);
        set.CreateTranslations();
        MorseTranslate morse = new(ref set);

        Console.WriteLine("Enter text to be translated: ");
        string input = Console.ReadLine() ?? "";
        database.InsertConversion(input); // Log the conversion into the database
        input = input.ToUpper(); // Only capital letters in the translation files
        if (set.CheckForInvalidCharacters(input)) // Checks for any characters that are not in the translation files
        {
            Console.WriteLine("Invalid characters found in input they will be removed");
            set.FixInvalidCharacters(input); // Removes any invalid characters
        }
        string translation = morse.TranslateIntoMorse(input);
        Console.WriteLine("Would you like to encrypt the message? (y/n): ");
        if(Console.ReadLine() == "y")
        {
            Console.WriteLine("Enter passkey: ");
            string passkey = Console.ReadLine() ?? "";
            Encryptions encryption = new(passkey); // Creates new encryption object with the passkey
            translation = translation.Replace("·", "."); // Replaces the · with . for the encryption as it doesn't support ·
            translation = encryption.EncryptMessage(translation); // Encrypts the message
        }
        Console.WriteLine($"Output: {translation}");
    }

    public void TranslateFromMorse()
    {
        Console.Clear();
        string file = ChooseLanguage();

        TranslationSet set = new(file);
        set.CreateTranslations();
        MorseTranslate morse = new(ref set);

        Console.WriteLine("Enter code to be translated: ");
        string input = Console.ReadLine() ?? "";
        database.InsertConversion(input);
        Console.WriteLine("Is the code encrypted (y/n): ");
        if(Console.ReadLine() == "y")
        {
            Console.WriteLine("Enter passkey: ");
            string passkey = Console.ReadLine() ?? "";
            Encryptions encryption = new(passkey);
            input = encryption.DecryptMessage(input);
        }
        input = input.Replace(".", "·"); // Replaces . with · for the translation as it doesn't support .

        // This splits the inputs into words and then each individual morse character
        string[] split_words = input.Split('/');
        List<string> split_morse = new List<string>();
        foreach(string word in split_words)
        {
            string[] split_word = word.Split("  ");
            foreach(string morse_chars in split_word)
            {
                split_morse.Add(morse_chars);
            }
            split_morse.Add("/");
        }

        string translation = morse.TranslateFromMorse(split_morse.ToArray());
        Console.WriteLine($"Morse: {translation}");
    }

    // Allows a user to make sure they know each character in morse code
    public void Training()
    {
        Console.Clear();
        string file = ChooseLanguage();

        TranslationSet set = new(file);
        set.CreateTranslations();
        while(true)
        {
            Console.WriteLine("Press 'n' to quit or press 'y' to get a new morse character: (y/n): ");
            string input = Console.ReadLine() ?? "";
            if(input == "n") 
            {
                Console.Clear();
                return;
            }
            else 
            {
                
                Translations.TranslationSet.RandomValues random_letter = set.GetRandomMorseCharacter(); // A record that contains a random letter and it's morse code
                
                Console.WriteLine($"Covert {random_letter.Value} to it's letter: ");
                input = (Console.ReadLine() ?? "").ToUpper();
                if(Convert.ToString(random_letter.Key) == input)
                {
                    Console.WriteLine("Correct!");
                }
                else
                {
                    Console.WriteLine($"Incorrect! the correct letter was: {random_letter.Key}");
                }
            }
        }
    }

}

class Program
{
    static void Main(string[] args)
    {
        Menu menu = new Menu();
        menu.StartMenu();
    }
}