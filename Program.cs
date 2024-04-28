using System;
using System.IO;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Compression;
using Translations;
using Encryption;
using DB;
using JollyWrapper;

class Menu
{
    private bool finished = false;
    private DatabaseConnection database = new();
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

    protected string ChooseLanguage()
    {
        while(true)
        {
            Console.WriteLine("Choose language:");
            string file_location = "./Translations/";
            string[] files = Directory.GetFiles(file_location);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {files[i]}");
            }
            bool correct_value = int.TryParse(Console.ReadLine() ?? "0", out int choice);
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
        string file = ChooseLanguage();

        TranslationSet set = new(file);
        set.CreateTranslations();
        MorseTranslate morse = new(ref set);

        Console.WriteLine("Enter text to be translated: ");
        string input = Console.ReadLine() ?? "";
        database.InsertConversion(input);
        input = input.ToUpper();
        if (set.CheckForInvalidCharacters(input))
        {
            Console.WriteLine("Invalid characters found in input they will be removed");
            set.FixInvalidCharacters(input);
        }
        string translation = morse.TranslateIntoMorse(input);
        Console.WriteLine("Would you like to encrypt the message? (y/n): ");
        if(Console.ReadLine() == "y")
        {
            Console.WriteLine("Enter passkey: ");
            string passkey = Console.ReadLine() ?? "";
            Encryptions encryption = new(passkey);
            translation = translation.Replace("·", ".");
            Console.WriteLine(translation);
            translation = encryption.EncryptMessage(translation);
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
        input = input.Replace(".", "·");
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
                
                Translations.TranslationSet.RandomValues random_letter = set.GetRandomMorseCharacter();
                
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