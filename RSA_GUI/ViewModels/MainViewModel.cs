using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RSA_GUI.Models;

namespace RSA_GUI.ViewModels
{
    public class MainViewModel : HeadViewModel
    {
        private RSAEncryption _rsa = new RSAEncryption();

        public MainViewModel()
        {
            
        }
        public string Encrypted
        {
            get => _encrypted;
            set
            {
                _encrypted = value;
                NotifyPropertyChanged();
            }
        }
        public string Decrypted { 
            get => _decrypted; 
            set
            {
                _decrypted = value;
                NotifyPropertyChanged();
            }
        }
        public string PublicKey
        {
            get => FormatKey(_rsa.PublicKey);
            set
            {
                try
                {
                    _rsa.SetKey(value, Keys.Public);
                }
                catch (Exception ex)
                {
                    InvokeError(ex.Message);
                }
                NotifyPropertyChanged();
            }
        }
        public string PrivateKey {
            get => FormatKey(_rsa.PrivateKey);
            set
            {
                try
                {
                    _rsa.SetKey(value, Keys.Private);
                }
                catch (Exception ex)
                {
                    InvokeError(ex.Message);
                }
                NotifyPropertyChanged();
            }
        }
        public string KeyLength
        {
            get 
            {
                return _rsa.KeySize.ToString(); 
            }
            set
            {
                try
                {
                    _rsa.KeySize = int.Parse(value);
                }
                catch (FormatException)
                {
                    InvokeError("Размер указан неверным образом");
                }
                catch (ArgumentException ex)
                {
                    InvokeError(ex.Message);
                }
            }
        }


        public ICommand Encrypt
        {
            get
            {
                return new DelegeteCommand((o) => 
                {
                    try
                    {
                        Encrypted = _rsa.Encrypt(Decrypted);
                    }
                    catch (Exception e)
                    {
                        InvokeError(e);
                    }                 
                }, (o) => _rsa.IsKeysSet());
            }
        }
        public ICommand Decrypt
        {
            get 
            {
                return new DelegeteCommand((o) => 
                {
                    try
                    {
                        Decrypted = _rsa.Decrypt(Encrypted);
                    }
                    catch(Exception e)
                    {
                        InvokeError(e);
                    }
                }, (o) => _rsa.IsKeysSet());
            }
        }

        public ICommand GenerateKeys
        {
            get
            {
                return new DelegeteCommand(async (o) =>
                {
                    await _rsa.GenerateKeyPair();

                    NotifyPropertyChanged(nameof(PublicKey));
                    NotifyPropertyChanged(nameof(PrivateKey));
                    NotifyPropertyChanged(nameof(Decrypt));
                    NotifyPropertyChanged(nameof(Encrypt));

                }, (o) => true);
            }
        }

        private string FormatKey((BigInteger, BigInteger) key)
        {
            return key.Item1.ToString() + " " + key.Item2.ToString();
        }
    }

    public class DelegeteCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<object?> _action;
        private Func<object?, bool> _canExecute;

        public DelegeteCommand(Action<object?> action, Func<object?, bool> func)
        {
            _action = action;
            _canExecute = func;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            _action.Invoke(parameter);
        }
    }


}
