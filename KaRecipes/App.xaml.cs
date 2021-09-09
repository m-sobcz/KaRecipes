using KaRecipes.UI.Models;
using KaRecipes.UI.ViewModels;
using KaRecipes.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KaRecipes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory());
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            ServiceCollection serviceCollection = new();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider.GetRequiredService<MainWindowViewModel>().ShowInformation += ShowInfo_ShowInformation;
            ServiceProvider.GetRequiredService<RecipesTabViewModel>().OpenFile += App_OpenFile;
            ServiceProvider.GetRequiredService<MainWindow>().Show();
        }


        private (string, string) App_OpenFile(object sender)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = AppContext.BaseDirectory;
            string fileContent="";
            if (openFileDialog.ShowDialog() == true)
            {
                fileContent = File.ReadAllText(openFileDialog.FileName);
                MessageBox.Show(fileContent);
                var fullFileName = openFileDialog.FileName;
                return (fullFileName, fileContent);
            }
            else return (null, null);
        }
        
        private void ShowInfo_ShowInformation(object sender, string text, string caption)
        {
            MessageBox.Show(text, caption);//! file content
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            //Models

            //ViewModels
            services.AddTransient<Func<Action<object>, Func<object, bool>, ICommand>>(sp => (execute, canExecute) => new RelayCommand(execute, canExecute));
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<RecipesTabViewModel>();
            services.AddSingleton<ParametersTabViewModel>();
            services.AddSingleton<DatabaseTabViewModel>();
            services.AddSingleton<DiagnosticsTabViewModel>();
            services.AddSingleton<SettingsTabViewModel>();
            //Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<RecipesTab>();
            services.AddSingleton<ParametersTab>();
            services.AddSingleton<DatabaseTab>();
            services.AddSingleton<DiagnosticsTab>();
            services.AddSingleton<SettingsTab>();
            //Logic

        }


        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Wystąpił nieobsłużony wyjątek: \n {e.Exception.Message}", "Nieobsłużony wyjątek", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }
}
