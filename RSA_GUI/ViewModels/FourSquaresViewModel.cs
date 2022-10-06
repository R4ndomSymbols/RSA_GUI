using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RSA_GUI.Models;

namespace RSA_GUI.ViewModels
{
    public class FourSquaresViewModel : HeadViewModel
    {
        private FourSquaresEncryption _fse = new FourSquaresEncryption();

        public FourSquaresViewModel()
        {

        }

        public override ICommand Decrypt
        {
            get => 
            new DelegeteCommand((o) =>
            {
                Decrypted = _fse.Decrypt(Encrypted);
            },
            (o) => _fse.IsConfigured());
        }

        public override ICommand Encrypt
        {
            get => 
            new DelegeteCommand((o) =>
            {
                Encrypted = _fse.Encrypt(Decrypted);
            },
            (o) => _fse.IsConfigured());

        }



        public string Keys
        {
            get => _fse.Keys.Item1 + " " + _fse.Keys.Item2;
            set
            {
                try
                {
                    var t = value.ToUpper().Trim().Split(' ');
                    _fse.Keys = (t[0], t[1]);
                    NotifyPropertyChanged(nameof(Encrypt));
                    NotifyPropertyChanged(nameof(Decrypt));
                }
                catch (IndexOutOfRangeException)
                {
                    InvokeError("Неверный формат ключа");
                }
                catch (ArgumentException ex)
                {
                    InvokeError(ex);
                }
            }
        }
    }
}
