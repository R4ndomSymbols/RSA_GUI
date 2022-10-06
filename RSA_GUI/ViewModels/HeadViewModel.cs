using RSA_GUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RSA_GUI.ViewModels
{
    public class HeadViewModel : INotifyPropertyChanged
    {
        private HeadViewModel _cvm;
        private UserControl _cus;
        protected string _encrypted = string.Empty;
        protected string _decrypted = string.Empty;

        private DelegeteCommand _encryptCommand;
        private DelegeteCommand _decryptCommand;

        public HeadViewModel CurrentPageView
        {
            get
            {
                return _cvm;
            }
            set
            {
                _cvm = value;
                NotifyPropertyChanged();
            }
        }
        public UserControl CurrentUserControl
        {
            get => _cus;
            set
            {
                _cus = value;
                NotifyPropertyChanged();
            }
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
        public string Decrypted
        {
            get => _decrypted;
            set
            {
                _decrypted = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand ChangeToRsaVM
        {
            get
            {
                return new DelegeteCommand((o) =>
                {
                    //CurrentPageView = new MainViewModel();
                    CurrentUserControl = new RSAControl();
                }, (o) => true);
            }
        }
        public ICommand ChangeToAesVM
        {
            get
            {
                return new DelegeteCommand(
                (o) => CurrentUserControl = new AESControl(),            
                (o) => true);
            }
        }

        public ICommand ChangeToFourSquaresVM
        {
            get
            {
                return new DelegeteCommand(
                (o) => CurrentUserControl = new FourSquaresControl(),
                (o) => true);
            }
        }


        public virtual ICommand Encrypt { get; }
        public virtual ICommand Decrypt { get; }
        public UserControl Cus { get => _cus; set => _cus = value; }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void InvokeError(string message)
        {
            MessageBox.Show(message);
        }
        protected void InvokeError(Exception e)
        {
            InvokeError(e.Message);
        }
    }
}
