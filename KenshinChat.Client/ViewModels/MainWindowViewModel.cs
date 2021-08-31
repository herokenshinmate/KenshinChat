using KenshinChat.Client.Commands;
using KenshinChat.Client.Models;
using KenshinChat.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KenshinChat.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string userNameField;
        public string UserNameField
        {
            get { return userNameField; }
            set { userNameField = value; OnPropertyChanged(); }
        }

        private string passwordField;
        public string PasswordField
        {
            get { return passwordField; }
            set { passwordField = value; OnPropertyChanged(); }
        }

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { isLoggedIn = value; OnPropertyChanged(); }
        }

        //Services
        private ILoginService loginService;

        /// <summary>
        /// Log In ommand
        /// </summary>
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommandAsync(() => Login(), o => CanLogin()));
            }
        }

        public MainWindowViewModel(ILoginService _loginService) 
        {
            loginService = _loginService;
        }

        /// <summary>
        /// Handles log in
        /// </summary>
        /// <returns></returns>
        private async Task Login()
        {
            try
            {
                var response = await loginService.AttemptLogin(new User
                {
                    UserName = UserNameField,
                    Password = PasswordField
                });
                MessageBox.Show("LOG IN FAILED DICKHEAD");

            }
            catch (Exception) { }
        }

        /// <summary>
        /// Checks if we can log in
        /// </summary>
        /// <returns></returns>
        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(userNameField) && !string.IsNullOrEmpty(passwordField);
        }
    }
}
