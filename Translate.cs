using System;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using Compression;

namespace Translations
{
    class TranslationSet
    {
        public Dictionary<char, string> Translations { get; } = new();
        private string[] _lines;
        public TranslationSet(string file) => _lines = File.ReadAllLines(file);

        public void CreateTranslations()
        {
            foreach (string line in _lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                char character = Convert.ToChar(line.Substring(0, 1));
                string morse = line.Substring(2);
                KeyValuePair<char, string> translation = new(character, morse);
                //Console.WriteLine($"Adding translation: {translation.Key} -> {translation.Value}");
                if (Translations.ContainsValue(translation.Value)) continue; // If the value is already in the dictionary, skip it

                Translations.Add(translation.Key, translation.Value);
            }
            Translations.Add(' ', "/");
        }

        public bool CheckForInvalidCharacters(string input) => input.Any(c => !Translations.ContainsKey(c));

        public string FixInvalidCharacters(string input)
        {
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i].ToString().Any(Translations.ContainsKey));
                input.Remove(i);
            }
            return input;
        }

        public record RandomValues(char Key, string Value);
        public RandomValues GetRandomMorseCharacter()
        {
            Random random = new();
            int index = random.Next(0, Translations.Count);
            return new RandomValues(Translations.ElementAt(index).Key, Translations.ElementAt(index).Value);
        }

        public char GetKey(string morse)
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
        // 1 interface - 2 Different version, 1 for encrypted, 1 for not encrypted - method overloading

        private readonly TranslationSet _set;
        public MorseTranslate(ref TranslationSet set) => _set = set;

        public string[] TranslateIntoMorse(string[] input)
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
            input.Replace(".", "·");
            for(int i = 0; i < input.Length; i++)
            {
                if (_set.Translations.TryGetValue(input[i], out string? morse)) output += morse + "  ";
                else Console.WriteLine($"No translation found for {input[i]}");
            }
            return output;
        }

        public string TranslateFromMorse(string[] input)
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