using KenshinChat.Client.Commands;
using KenshinChat.Client.Enums;
using KenshinChat.Client.Models;
using KenshinChat.Client.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;
using NetVips;

namespace KenshinChat.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public CurrentUser currentUser { get; set; }
        private const int MAX_IMAGE_WIDTH = 60;
        private const int MAX_IMAGE_HEIGHT = 60;

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
        private IDialogService dialogService;

        //Utilities
        private TaskFactory ctxTaskFactory;

        #region Getters / Setters

        private string _chatBox;
        public string ChatBox
        {
            get { return _chatBox; }
            set 
            { 
                _chatBox = value;
                if (!string.IsNullOrEmpty(_chatBox))
                    hubService.UpdateIsTyping(true);
                else
                    hubService.UpdateIsTyping(false);
                OnPropertyChanged(); 
            }
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

        private string _register_ProfilePic;
        public string Register_ProfilePic
        {
            get { return _register_ProfilePic; }
            set
            {
                _register_ProfilePic = value;
                OnPropertyChanged();
            }
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

        private ICommand _selectProfilePicCommand;
        public ICommand SelectProfilePicCommand
        {
            get
            {
                return _selectProfilePicCommand ?? (_selectProfilePicCommand =
                    new RelayCommand((o) => SelectProfilePic()));
            }
        }

        #endregion

        public MainWindowViewModel(IUserService _userService, IHubService _hubService, IDialogService _dialogService) 
        {
            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            userService = _userService;
            hubService = _hubService;
            dialogService = _dialogService;
            CurrentViewState = ViewState.Login;
            HubMessages = new ObservableCollection<ChatMessage>();

            //Events
            hubService.RecieveMessage += NewMessage;
            hubService.GetAllUsers += GetAllUsers;
            hubService.UpdateUser += UpdateUser;
            hubService.ReceiveChatLog += ReceiveChatLog;
            hubService.AddNewUser += AddNewUser;
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

                        //Add the profile picture after we send our data to the hub
                        currentUser.ProfilePicture = response.ProfilePicture;

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
                    ProfilePicture = GetByteProfilePicture(),
                    UserName = Register_UsernameField,
                    Password = Register_PasswordField
                });

                if(response.Success)
                {
                    currentUser = new CurrentUser
                    {
                        UserId = response.UserId,
                        ProfilePicture = response.ProfilePicture,
                        Username = response.Username,
                        AccessToken = response.AccessToken
                    };

                    if(await hubService.ConnectAsync(currentUser.AccessToken))
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

        private bool CanRegister()
        {
            if (!string.IsNullOrEmpty(Register_UsernameField) && !string.IsNullOrEmpty(Register_PasswordField))
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

        public async void GetAllUsers(List<User> users)
        {     
            if (users.Count > 1)
            {
                //Remove self
                User itemToRemove = users.FirstOrDefault(u => u.UserId == currentUser.UserId);
                if (itemToRemove != null)
                    users.Remove(itemToRemove);

                //Get profile pictures
                foreach (User user in users)
                {
                    user.ProfilePicture = await userService.GetProfilePicture(currentUser.AccessToken, user.UserId);
                }

                await ctxTaskFactory.StartNew(() => HubUsers = new ObservableCollection<User>(users));
            }
        }

        public async void AddNewUser(User newUser)
        {
            newUser.ProfilePicture = await userService.GetProfilePicture(currentUser.AccessToken, newUser.UserId);
            await ctxTaskFactory.StartNew(() => HubUsers.Add(newUser));
        }

        public void UpdateUser(User userToUpdate)
        {
            User oldUser = HubUsers.FirstOrDefault(u => u.UserId == userToUpdate.UserId);
            if(oldUser != null)
            {
                HubUsers.Remove(oldUser);

                oldUser.IsOnline = userToUpdate.IsOnline;
                oldUser.IsTyping = userToUpdate.IsTyping;
                oldUser.Username = userToUpdate.Username;

                HubUsers.Add(oldUser);
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

        private void SelectProfilePic()
        {
            var imgPath = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (!string.IsNullOrEmpty(imgPath))
            {
                ConvertToSize(imgPath);
                Register_ProfilePic = imgPath;
            }
        }

        private Image ConvertToSize(string imgPath)
        {
            using Image image = Image.Thumbnail(imgPath, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT, crop: NetVips.Enums.Interesting.Attention);
            return image;
        }

        private byte[] GetByteProfilePicture()
        {
            byte[] pic = null;
            if (!string.IsNullOrEmpty(Register_ProfilePic)) pic = File.ReadAllBytes(Register_ProfilePic);
            return pic;
        }
    }
}
