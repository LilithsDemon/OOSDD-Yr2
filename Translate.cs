using System;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace Translations
{
    class TranslationSet
    {
        public Dictionary<char, string> Translations { get; } = new();
        private string[] _lines;
        public TranslationSet(string file) => _lines = File.ReadAllLines(file); // On creation, read all lines from the file

        public void CreateTranslations()
        {
            foreach (string line in _lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue; // Skip empty lines and comments
                char character = Convert.ToChar(line.Substring(0, 1)); // Get the first character of the line
                string morse = line.Substring(2); // Get the morse code from the line
                KeyValuePair<char, string> translation = new(character, morse);
                //Console.WriteLine($"Adding translation: {translation.Key} -> {translation.Value}");
                if (Translations.ContainsValue(translation.Value)) continue; // If the value is already in the dictionary, skip it

                Translations.Add(translation.Key, translation.Value);
            }
            Translations.Add(' ', "/");
        }

        public bool CheckForInvalidCharacters(string input) => input.Any(c => !Translations.ContainsKey(c)); // Check if the input contains any characters that are not in the dictionary

        public string FixInvalidCharacters(string input) // Remove any characters that are not in the dictionary
        {
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i].ToString().Any(Translations.ContainsKey));
                input.Remove(i);
            }
            return input;
        }

        public record RandomValues(char Key, string Value); // A record that contains a random letter and it's morse code
        public RandomValues GetRandomMorseCharacter() // Get a random letter and it's morse code
        {
            Random random = new();
            int index = random.Next(0, Translations.Count);
            return new RandomValues(Translations.ElementAt(index).Key, Translations.ElementAt(index).Value);
        }

        public char GetKey(string morse) // Get the key from the value
        {
            List<char> keys = (from kvp in Translations where kvp.Value == morse select kvp.Key).ToList();
            try 
            {
                return keys[0];
            }
            catch
            {
                return ' ';
            }
        }
    }

    class MorseTranslate
    {
        private readonly TranslationSet _set;
        public MorseTranslate(ref TranslationSet set) => _set = set;

        public string[] TranslateIntoMorse(string[] input) // Translate a string array into morse code
        {
            string[] output = new string[input.Length];
            for(int i = 0; i < input.Length; i++)
            {
                if(char.Parse(input[i]) == ' ') output.Append("  ");
                if(_set.Translations.TryGetValue(char.Parse(input[i]), out string? morse)) output.Append(morse);
                else Console.WriteLine($"No Translation found for {input[i]}");
            }
            return output;
        }

        public string TranslateIntoMorse(string input) // Method overloading to either give a string or string array
        {
            string output = "";
            input.Replace(".", "·"); // Makes sure any . are replaced with · as the translation doesn't support .
            for(int i = 0; i < input.Length; i++)
            {
                if (_set.Translations.TryGetValue(input[i], out string? morse)) output += morse + "  ";
                else Console.WriteLine($"No translation found for {input[i]}");
            }
            return output;
        }

        public string TranslateFromMorse(string[] input) // Translate a string array from morse code
        {
            string output = "";
            for(int i = 0; i < input.Length; i++)
            {
                string value = input[i];
                output += _set.GetKey(value);
            }
            return output;
        }

    }
}