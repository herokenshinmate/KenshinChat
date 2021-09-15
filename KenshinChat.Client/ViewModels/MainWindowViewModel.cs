using KenshinChat.Client.Commands;
using KenshinChat.Client.Enums;
using KenshinChat.Client.Models;
using KenshinChat.Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KenshinChat.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public CurrentUser currentUser { get; set; }

        public ObservableCollection<ChatMessage> HubMessages { get; set; }

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public ObservableCollection<User> HubUsers
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        //Services
        private IUserService userService;
        private IHubService hubService;

        //Utilities
        private TaskFactory ctxTaskFactory;

        #region Getters / Setters

        private string _chatBox;
        public string ChatBox
        {
            get { return _chatBox; }
            set { _chatBox = value; OnPropertyChanged(); }
        }

        private string _UserNameField;
        public string UserNameField
        {
            get { return _UserNameField; }
            set { _UserNameField = value; OnPropertyChanged(); }
        }

        private string _PasswordField;
        public string PasswordField
        {
            get { return _PasswordField; }
            set { _PasswordField = value; OnPropertyChanged(); }
        }

        private ViewState _viewState;
        public ViewState CurrentViewState
        {
            get { return _viewState; }
            set { _viewState = value; OnPropertyChanged(); }
        }

        private string _Register_UsernameField;
        public string Register_UsernameField
        {
            get { return _Register_UsernameField; }
            set { _Register_UsernameField = value; OnPropertyChanged(); }
        }

        private string _Register_PasswordField;
        public string Register_PasswordField
        {
            get { return _Register_PasswordField; }
            set { _Register_PasswordField = value; OnPropertyChanged(); }
        }

        private string _Register_RePasswordField;
        public string Register_RePasswordField
        {
            get { return _Register_RePasswordField; }
            set { _Register_RePasswordField = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
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

        private ICommand _registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand = new RelayCommandAsync(() => Register(), o => CanRegister()));
            }
        }

        private ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand = new RelayCommandAsync(() => SendMessage(), o => true));
            }
        }

        private ICommand _toRegisterCommand;
        public ICommand ToRegisterCommand => _toRegisterCommand ??= new RelayCommand(p => GoToRegisterView());

        #endregion

        public MainWindowViewModel(IUserService _userService, IHubService _hubService) 
        {
            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            userService = _userService;
            hubService = _hubService;
            CurrentViewState = ViewState.Login;
            HubMessages = new ObservableCollection<ChatMessage>();

            //Events
            hubService.RecieveMessage += NewMessage;
            hubService.UsersUpdate += UpdateUsers;
            hubService.ReceiveChatLog += ReceiveChatLog;
        }

        #region Login
        /// <summary>
        /// Handles log in
        /// </summary>
        /// <returns></returns>
        private async Task Login()
        {
            try
            {
                LoginResponse response = await userService.AttemptLogin(new UserRequest
                {
                    Id = "1",
                    UserName = UserNameField,
                    Password = PasswordField
                });
                
                if(response.Success)
                {
                    currentUser = new CurrentUser
                    {
                        UserId = response.UserId,
                        Username = response.Username,
                        AccessToken = response.AccessToken
                    };

                    if (await hubService.ConnectAsync(currentUser.AccessToken))
                    {
                        await hubService.RegisterToHub(currentUser);
                        CurrentViewState = ViewState.Home;
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect to the hub");
                    }
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage);
                }

            }
            catch (Exception) { }
        }

        /// <summary>
        /// Checks if we can log in
        /// </summary>
        /// <returns></returns>
        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserNameField) && !string.IsNullOrEmpty(PasswordField);
        }
        #endregion

        #region Register

        private async Task Register()
        {
            try
            {
                LoginResponse response = await userService.AttemptRegister(new UserRequest
                {
                    Id = "1",
                    UserName = Register_UsernameField,
                    Password = Register_PasswordField
                });

                if(response.Success)
                {
                    currentUser = new CurrentUser
                    {
                        UserId = response.UserId,
                        Username = response.Username,
                        AccessToken = response.AccessToken
                    };

                    if(await hubService.ConnectAsync(currentUser.AccessToken))
                    {
                        CurrentViewState = ViewState.Home;
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect to the hub");
                    }
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage);
                }
            }
            catch (Exception) { }
        }

        private bool CanRegister()
        {
            if (!string.IsNullOrEmpty(Register_UsernameField) && !string.IsNullOrEmpty(Register_PasswordField) && !string.IsNullOrEmpty(Register_RePasswordField))
                if (Register_PasswordField == Register_RePasswordField)
                    return true;

            return false;
        }

        #endregion

        #region SendMessage
        private async Task SendMessage()
        {
            if (!string.IsNullOrEmpty(ChatBox))
            {
                await hubService.SendMessage(new ChatMessage
                {
                    Author = currentUser.Username,
                    Message = ChatBox,
                    AuthorId = currentUser.UserId,
                    Time = DateTime.Now,
                    IsOriginNative = false
                });

                ChatBox = "";
            }
        }
        #endregion

        #region Events

        public void NewMessage(ChatMessage msg)
        {
            if (msg.AuthorId == currentUser.UserId)
                msg.IsOriginNative = true;
            ctxTaskFactory.StartNew(() => HubMessages.Add(msg));
            //HubMessages.Add(msg);
        }

        public void UpdateUsers(List<User> users)
        {     
            if (users.Count > 1)
            {
                User itemToRemove = users.FirstOrDefault(u => u.UserId == currentUser.UserId);
                if (itemToRemove != null)
                    users.Remove(itemToRemove);

                ctxTaskFactory.StartNew(() => HubUsers = new ObservableCollection<User>(users));
            }
        }

        public void ReceiveChatLog(List<ChatMessage> log)
        {
            ctxTaskFactory.StartNew(() =>
            {
                log.OrderByDescending(x => x.Time);
                log.ForEach(x => HubMessages.Add(x));
            });
        }

        #endregion

        private void GoToRegisterView()
        {
            CurrentViewState = ViewState.Register;
        }
    }
}
