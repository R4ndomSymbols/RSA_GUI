using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA_GUI
{
    public class AESViewModel : HeadViewModel
    {
        private AESEncryption _aes = new AESEncryption();

        public string Key
        {
            get
            {
                return _aes.Key;
            }
            set
            {
                try
                {
                    _aes.Key = value;
                }
                catch (Exception e)
                {
                    InvokeError(e);
                }
            }
        }

        public string IV
        {
            get => _aes.IVVector;
            set
            {
                try
                {
                    _aes.IVVector = value;
                }
                catch (Exception e)
                {
                    InvokeError(e);
                }
            }
        }

        public string KeySize
        {
            get => _aes.KeySize.ToString();
            set
            {
                try
                {
                    if (int.TryParse(value, out int t))
                    {
                        _aes.KeySize = t;
                    }
                    else
                    {
                        throw new ArgumentException("Ключ имеет неверный формат");
                    }
                }
                catch (Exception e)
                {
                    InvokeError(e.Message);
                }
            }
        }
    }
}
