using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSA_GUI.Models
{

    public class AESEncryption
    {
        
        private Aes? _aes = Aes.Create();

        public int KeySize
        {
            get
            {
                return _aes.KeySize;
            }
            set
            {
                if (_aes.LegalKeySizes.Any((x) => value >= x.MinSize && value <= x.MaxSize))
                {
                    _aes.KeySize = value;
                }
                else throw new ArgumentException("Такой размер ключа недопустим");
            }
        }

        public string Key
        {
            get
            {
                return InHex(_aes.Key);
            }
            set
            {
                try
                {
                    _aes.Key = Encoding.Unicode.GetBytes(value);
                }
                catch (CryptographicException)
                {
                    throw new ArgumentException("Такой ключ не является валидным");
                }
            }
        }
        public string IVVector
        {
            get
            {
                return InHex(_aes.IV);
            }
            set
            {
                try
                {
                    _aes.IV = Encoding.Unicode.GetBytes(value);
                }
                catch (CryptographicException)
                {
                    throw new ArgumentException("Такой вектор не является валидным");
                }
            }
        }


        public string Decrypt(string toDecrypt)
        {
            var slittedBytes = toDecrypt.Split(' ');
            byte[] convertedBytes = slittedBytes.Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
            return Encoding.Unicode.GetString(
                _aes.DecryptCbc(convertedBytes, _aes.IV));
        }
        public string Encrypt(string toEncrypt)
        {
            var rawResult = _aes.EncryptCbc(Encoding.Unicode.GetBytes(toEncrypt), _aes.IV);

            return string.Join(" ", rawResult.Select(x => $"{x:X}"));

        }
        private string InHex(byte[] toConvert)
        {
            return string.Join(" ", toConvert.Select(x => $"{x:X}"));
        }
    }
}
