using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KaRecipes.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        
        private ICommand closingCommand;
        private ICommand aboutCommand;
        private ICommand helpCommand;
        public MainWindowViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand) : base(getCommand) 
        {
        }
        public ICommand ClosingCommand
        {
            get
            {
                if (closingCommand == null)
                {
                    closingCommand = getCommand(
                        o =>
                        {
                            Environment.Exit(0);
                        },
                        o => true
                        );
                }
                return closingCommand;
            }
        }
        public ICommand AboutCommand
        {
            get
            {
                if (aboutCommand == null)
                {
                    aboutCommand = getCommand(
                        o =>
                        {
                            string currentVersion = "?";
                            //if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                            //{
                            //    currentVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                            //}
                            //else
                            //{
                            //    currentVersion = "???";
                            //}
                            string applicationInfo = $"-----\nKARecipes v.{currentVersion}\n-----\n\nProgram do obsługi receptur na liniach SMA\n\n2021 Kongsberg Automotive";
                            ShowInfo(applicationInfo, "Informacje o aplikacji");
                        },
                        o => true
                        );
                }
                return aboutCommand;
            }
        }
        public ICommand HelpCommand
        {
            get
            {
                if (helpCommand == null)
                {
                    helpCommand = getCommand(
                        o =>
                        {
                            ShowInfo("Brak pomocy dla aktualnej wersji aplikacji.", "Pomoc");
                        },
                        o => true
                        );
                }
                return helpCommand;
            }
        }
    }

}
