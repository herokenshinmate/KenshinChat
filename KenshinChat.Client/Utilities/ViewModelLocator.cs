using KenshinChat.Client.Services;
using KenshinChat.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace KenshinChat.Client.Utilities
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
            container.RegisterType<ILoginService, LoginService>();
        }

        public MainWindowViewModel MainVM
        {
            get { return container.Resolve<MainWindowViewModel>(); }
        }
    }
}
