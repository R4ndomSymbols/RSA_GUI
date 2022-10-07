using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA_GUI.Models
{
    public class FourSquaresEncryption
    {
        private char[][] fourSquares = new char[12].Select(x => new char[12]).ToArray();
        private char _startSymbol = (char)int.Parse("0410", NumberStyles.HexNumber);
        private char _endSymbol = (char)int.Parse("042F", NumberStyles.HexNumber);
        private List<char> alphabet;
        private (int, int) sq1 = (0, 0); // x y
        private (int, int) sq2 = (6, 0);
        private (int, int) sq3 = (0, 6);
        private (int, int) sq4 = (6, 6);
        private (string, string) _keys = (string.Empty, string.Empty);
        public (string, string) Keys
        {
            get => _keys;
            set
            {
                Reset();
                AddAlphabetSquare(sq1);
                AddAlphabetSquare(sq4);

                if (value.Item1.Any(x => !alphabet.Contains(x)) || value.Item2.Any(x => !alphabet.Contains(x)))
                {
                    throw new ArgumentException("Недопустимый ключ (ключи)");
                }
                else if (value.Item1.Length >= alphabet.Count() || value.Item2.Length >= alphabet.Count())
                {
                    throw new ArgumentException("Cлишком длинный ключ (ключи)");
                }
                _keys = (FormatKey(value.Item1), FormatKey(value.Item2));
                
                AddCodeSquare(sq2, _keys.Item1);
                AddCodeSquare(sq3, _keys.Item2);

            }
        }

        public FourSquaresEncryption()
        {
            alphabet = new List<char>();
            for(int i = (int)_startSymbol; i<= (int)_endSymbol; i++)
            {
                alphabet.Add((char)i);
            }
            alphabet.AddRange(new char[] { ' ', '.', ',', '?' });


        }
        public string Decrypt(string toDecrypt)
        {
            string correct = toDecrypt;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < correct.Length; i += 2)
            {
                result.Append(Getpair((correct[i], correct[i + 1]), (sq2, sq3)));
            }
            return result.ToString();
        }

        public string Encrypt(string toEncrypt)
        {
            string corrected = FormatInput(toEncrypt);
            corrected += corrected.Length % 2 != 0 ? "А" : "";
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < corrected.Length; i += 2)
            {
                result.Append(Getpair((corrected[i], corrected[i + 1]), (sq1, sq4)));
            }
            return result.ToString();
        }

        public bool IsConfigured()
        {
            return Keys != ("", "");
        }
        private void AddAlphabetSquare((int, int) startPoint) // x y
        {
            int count = 0;
            for (int i = startPoint.Item1; i < startPoint.Item1 + 6; i++)
            {
                for (int j = startPoint.Item2; j < startPoint.Item2 + 6; j++)
                {
                    fourSquares[i][j] = alphabet[count++];
                }
            }
        }
        private void AddCodeSquare((int, int) startPoint, string word) // x y
        {
            int index = 0;
            int inAlphabetIndex = 0;
            string correctWord = word.ToUpper();
            char temp = _startSymbol;
            for (int i = startPoint.Item1; i < startPoint.Item1 + 6; i++)
            {
                for (int j = startPoint.Item2; j < startPoint.Item2 + 6; j++)
                {
                    if (index < correctWord.Length)
                    {
                        fourSquares[i][j] = correctWord[index];
                        index++;
                    }
                    else
                    {
                        if (correctWord.Contains(alphabet[inAlphabetIndex]))
                        {
                            j--;
                        }
                        else
                        {
                            fourSquares[i][j] = alphabet[inAlphabetIndex];
                        }
                        inAlphabetIndex++;
                    }
                }
            }
        }

        private string Getpair((char, char) pair, ((int, int), (int, int)) squaresPair)
        {
            (int, int) first = FindInSquare(squaresPair.Item1, pair.Item1);
            (int, int) second = FindInSquare(squaresPair.Item2, pair.Item2);
            return fourSquares[second.Item1][first.Item2].ToString() + fourSquares[first.Item1][second.Item2].ToString();
        }

        private (int, int) FindInSquare((int, int) startPoint, char c)
        {
            for (int i = startPoint.Item1; i < startPoint.Item1 + 6; i++)
            {
                for (int j = startPoint.Item2; j < startPoint.Item2 + 6; j++)
                {
                    if (c == fourSquares[i][j])
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }

        private void Reset()
        {
            fourSquares = new char[12].Select(x => new char[12]).ToArray();
        }

        private string FormatInput(string toFormat)
        {
            return string.Concat(toFormat.ToUpper().Select(x => !alphabet.Contains(x)  ? "" : x.ToString()));
        }

        private string FormatKey(string key)
        {
            return string.Concat(key.Distinct());
        }


    }
}
