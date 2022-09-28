using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA_GUI.Models
{
    public enum Keys
    {
        Private,
        Public
    }
    public class RSAEncryption
    {
        // криптографический алгоритм, с открытым ключем, основывающийся на вычислительной сложности
        // факторизации больших целых чисел
        // e, n / d, n
        public (BigInteger, BigInteger) PrivateKey { get; private set; } = (0, 0);
        public (BigInteger, BigInteger) PublicKey { get; private set; } = (0, 0);
        // размер в байтах
        public int KeySize 
        { 
            get
            {
                return _keySize;
            }
            set 
            {
                if (value > 10 && value < 200)
                {
                    _keySize = value;
                }
                else throw new ArgumentException("Размер ключа вне диапазона допустимых значений");
            }
        }

        private int _keySize = 32;

        private readonly List<BigInteger> _primes = new List<BigInteger>();
        private List<int> _firstPrimes = new List<int>() { 2 };

        public RSAEncryption()
        {

        }

        public void SetKey(string keys, Keys keyType)
        {
            var key = ReadNumbers(keys);
            if(key.Count != 2)
            {
                throw new Exception("Введенный приватный ключ имеет неверный формат");
            }
            else
            {
                if(keyType == Keys.Private) 
                {
                    PrivateKey = (key[0], key[1]);
                }
                else if(keyType == Keys.Public)
                {
                    PublicKey = (key[0], key[1]);
                }
                
            }
            
        }

        public bool IsKeysSet()
        {
            return PublicKey != (0, 0) && PrivateKey != (0, 0) && KeySize != 0;
        }

        private void Reset()
        {
            PrivateKey = (0, 0);
            PublicKey = (0, 0);
            _primes.Clear();
        }

        private void FindFirstPrimes()
        {
            for (int i = 2; i < 1000; i++)
            {
                if(_firstPrimes.Any(x => i % x == 0)){
                    continue;
                }
                else _firstPrimes.Add(i);            
            }
        }

        //алгоритм проверки числа на простоту посредством одноименного алгоритма
        // n - число, простоту которого необходимо доказать
        private bool MillerRabinAlgorithm(BigInteger n)
        {
            // условия
            // a^d mod n = 1
            // r exist, r < s, a^(2^r*d) mod n = n - 1
            //цикл нахождения представления числа в виде произведения (2^s)*d, где d нечетно  
            int s = 0;
            BigInteger d = n - 1;

            while (d % 2 == 0)
            {
                s++;
                d /= 2;
            }

            //число необходимых подтверждений простоты числа (r)
            int r = (int)BigInteger.Log(n, 2);

            // цикл перебора количества чисел потенциальных свидетелей простоты входного числа

            for(int i = 0; i < r; i++)
            {
                //генерация случайного числа меньше входного a
                BigInteger possiblePrimeWitness;
                do
                {
                    possiblePrimeWitness = GetRandomSizedInt(KeySize);
                }
                while (possiblePrimeWitness < 2 || possiblePrimeWitness > n-2);

                //условие простоты числа, если неверно - число составное
                BigInteger buf = BigInteger.ModPow(possiblePrimeWitness, d, n);
                if ((buf == 1 || buf == n - 1)
                    || IsRExist(s, possiblePrimeWitness, d, n))
                {
                    continue;
                }
                return false;
            }
            return true;

            // проверка существования r, удовлетворяющее второму условию алгоритма
            bool IsRExist(int power, BigInteger a, BigInteger oddRemainder, BigInteger n2)
            {
                BigInteger powerbuffer = oddRemainder;
                for(int r = 0; r < power; r++)
                {
                    BigInteger buffer = BigInteger.ModPow(a, powerbuffer, n2);
                    //проверка числа на соответсвие 2 условию алгоритма миллера рабина
                    if (buffer == 1)
                    {
                        return false;
                    }
                    else 
                    if(buffer == n - 1)
                    {
                        return true;
                    }
                    powerbuffer *= 2;
                }
                return false;
            }

        }

        // алгоритм, который находит мультипликативно обратное к открытой экспоненте
        private BigInteger ExtendedEuclidAlgorithm(BigInteger eulerFunction, // n
            BigInteger publicExponent) // a
        {
            
            // ax = eulerFunction
            // d * public exponent - 1 делится нацело на euler function
            // at = mod n
            BigInteger t = 0;
            BigInteger newt = 1;
            BigInteger r = eulerFunction;
            BigInteger newr = publicExponent;

            // алгоритм стырен, но не понят
            while(newr != 0)
            {
                BigInteger quotient = r / newr;
                (t, newt) = (newt, t - quotient * newt);
                (r, newr) = (newr, r - quotient * newr);
            }

            //if(r > 1)
            //{
                //throw new ArithmeticException();
            //}
            if (t < 0)
            {
                t += eulerFunction;
            }
            return t;
        }

        // генерация и установка ключей
        public async Task GenerateKeyPair()
        {
            Reset();

            if(_firstPrimes.Count < 2)
            {
                FindFirstPrimes();
            }
            if(KeySize <= 20)
            {
                FindPrimes(3);
            }
            else
            {
                await FindPrimesAsync();
            }

            BigInteger pPrime = _primes[0]; //29
            BigInteger qPrime = _primes[1]; //_primes[1];11
            BigInteger publicExponent = _primes[2]; //_primes[2];
            BigInteger module_n = pPrime * qPrime;
            BigInteger eulerFunction = (pPrime - 1) * (qPrime - 1);
            BigInteger privateExponent = ExtendedEuclidAlgorithm(eulerFunction, publicExponent);
            PublicKey = (publicExponent, module_n);
            PrivateKey = (privateExponent, module_n);

            if (eulerFunction % publicExponent == 0) throw new Exception();
        }

        // найти определенное количество возможно простых чисел
        private void FindPrimes(int count)
        {
            while (_primes.Count <= count)
            {
                AddPossiblePrimeNumberToList();
            }
        }
        private async Task FindPrimesAsync()
        {
            var tasks = Enumerable.Range(1, 16)
               .Select(x => Task.Run(AddPossiblePrimeNumberToList));
            await Task.Factory.
               ContinueWhenAny(tasks.ToArray(), (dummytask) => FindPrimes(3));
        }
        //получение случайного целого числа заданной длины
        private BigInteger GetRandomSizedInt(int bytesCount)
        {
            byte[] byteBigIntRepresentation = new byte[bytesCount + 1];
            new Random().NextBytes(byteBigIntRepresentation);
            byteBigIntRepresentation[^1] = 0;
            return new BigInteger(byteBigIntRepresentation);
        }

        //получение возможно простого числа
        private void AddPossiblePrimeNumberToList()
        {
            bool IsPrime = false;
            BigInteger number = 0;
            while (!IsPrime )
            {
                number = GetRandomSizedInt(KeySize);
                number += number.IsEven ? 1 : 0;
                if(!_firstPrimes.Any(x => number % x == 0))
                {
                    IsPrime = MillerRabinAlgorithm(number);
                }
                
            }
            if (!_primes.Contains(number))
            {
                _primes.Add(number);
            }
            else AddPossiblePrimeNumberToList();
        }

        public string Encrypt(string toEncrypt)
        {
            var chunks = ChunkMessageInUnicode(toEncrypt.Trim(), PublicKey.Item2);
            StringBuilder sb = new StringBuilder();
            foreach (BigInteger chunk in chunks)
            {
                var c = BigInteger.ModPow(chunk, PublicKey.Item1, PublicKey.Item2);
                sb.Append(c.ToString() + " ");
            }
            var s = sb.ToString().Trim();
            return s;
        }
       
        public string Decrypt(string toDecrypt)
        {
            // получение чисел из строки
            var chunks = ReadNumbers(toDecrypt.Trim());        
            // расшифровка до чисел
            List<BigInteger> decrypted = new List<BigInteger>();

            foreach (BigInteger chunk in chunks)
            {
                decrypted.Add(BigInteger.ModPow(chunk, PrivateKey.Item1, PrivateKey.Item2));
            }
            return ConvertToString(decrypted);

        }

        // метод чанкинга, разбивает текст на чанки, которые могут быть обработаны данным ключем
        private List<BigInteger> ChunkMessageInUnicode(string message, BigInteger n)
        {
            if(message == String.Empty)
            {
                throw new ArgumentException("Вы ввели пустое сообщение для шифрования");
            }
            //строка раскладывается на массив байт
            byte[] unicodeMessage = Encoding.Unicode.GetBytes(message);
            //определение максимального размера чанка в битах
            int chunkSizeInBits = (int)Math.Ceiling(BigInteger.Log(n, 2));
            //значение чанка не может быть больше, чем n - 1 
            List<BigInteger> chunks = new List<BigInteger>();
            //представление строки как массива бит
            BitArray stringInBits = new BitArray(unicodeMessage);
            // буферное число, которое можно зашифровать 
            BigInteger buffer = 1;
            int bitCount = 0;

            for (int i = 0; i < stringInBits.Count; i++)
            {
                //проверяет, не является ли полученное значение большим, чем n-1

                if (((buffer << 1) + (stringInBits[i] ? 1 : 0) << 1) + 1 > n - 1)
                {
                    chunks.Add((buffer << 1) + 1);
                    buffer = 1;
                    bitCount = 0;
                    i--;
                }
                else
                {
                    buffer <<= 1;
                    buffer += (stringInBits[i] ? 1 : 0);
                    bitCount++;
                }
            }
            if (bitCount > 0) chunks.Add((buffer << 1) + 1);

            return chunks;
        }

        // метод, который читает массив чисел 
        private List<BigInteger> ReadNumbers(string message)
        {
            if(message == String.Empty)
            {
                throw new Exception("Вы ввели пустую строку для дешифрования");
            }
            try
            {
                return message.Split(' ').Select(x => BigInteger.Parse(x)).ToList();
            }
            catch (FormatException)
            {
                throw new Exception("Невозможно преобразовать зашифрованное сообщение в формат, необходимый в программе");
            }          
        }
        // преобразует массив считанных чисел в одну строку - дешифрованное сообщение
        private string ConvertToString(List<BigInteger> numberArray)
        {
            List<bool> bitResult = new List<bool>();
            // конвертирует одно число и добавляет чистые биты в массив
            for(int i = 0; i < numberArray.Count; i++)
            {
                bitResult.AddRange(ConvertOneNumber(numberArray[i]));
            }
            bitResult.Reverse();
            List<byte> chars = new List<byte>();
            byte buffer = 0;
            int bitCount = 0;

            for(int j = 0; j<bitResult.Count; j++)
            {
                bitCount++;
                buffer = (byte)((buffer << 1) + (bitResult[j] ? 1 : 0));
                
                if (bitCount > 7)
                {
                    chars.Add(buffer);
                    buffer = 0;
                    bitCount = 0;
                }
            }
            chars.Reverse();
            return Encoding.Unicode.GetString(chars.ToArray());

            // функция, которая парсит отдельное число

            List<bool> ConvertOneNumber(BigInteger num)
            {
                List<bool> significant = new List<bool>();
                var bytes = num.ToByteArray(true, false);
                BitArray temp = new BitArray(bytes);
                int startPosition = temp.Length - 2;
                while (!temp[startPosition+1])
                {
                    startPosition--;
                }
                for(; startPosition > 0; startPosition--)
                {
                    significant.Add(temp[startPosition]);
                }
                return significant;

            }
        }

    }
}
