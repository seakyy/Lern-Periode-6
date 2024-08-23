using System.Text;

namespace bigrams
{
    internal class Program
    {
        static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        static void Main(string[] args)
        {
            string inFileDE = @"faust.txt";
            string inTextDE = File.ReadAllText(inFileDE);
            char mostFrequentDE = getMostFrequent(inTextDE);

            string cipherFile = @"cipher.txt";
            string cipherText = File.ReadAllText(cipherFile);
            char mostFrequentCipher = getMostFrequent(cipherText);

            int offset = Array.IndexOf(alphabet, mostFrequentDE) - Array.IndexOf(alphabet, mostFrequentCipher);

            string translation = translate(cipherText.ToUpper(), offset);
            Console.WriteLine(translation);
            
        }

        static char getMostFrequent(string text) {
        
           text = text.ToUpper();
            var frequency = new Dictionary<char, int>();
            foreach (var c in text)
            {
                if (char.IsLetter(c))
                {

                    if (frequency.ContainsKey(c))

                        frequency[c]++;
                    else frequency[c] = 1;
                }
                
            }
            return frequency.OrderByDescending(kvp => kvp.Value).First().Key;


        }

        static string translate(string text, int offset) { 

            var translated = new StringBuilder();
            foreach (var c in text) {

                if (char.IsLetter(c)) { 

                 int originalPos = Array.IndexOf(alphabet, c);
                    int newPos = (originalPos + offset + alphabet.Length) % alphabet.Length;
                    translated.Append(alphabet[newPos]);
                }

                else { translated.Append(c); 
                }

            }
        
          return translated.ToString();
        }


    }
}
