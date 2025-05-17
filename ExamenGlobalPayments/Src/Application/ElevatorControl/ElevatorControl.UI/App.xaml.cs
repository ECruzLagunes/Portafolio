using ElevatorControl.IoC;
using ElevatorControl.UI.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ElevatorControl.UI
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            
            services.AddElevatorDependencies(); 
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();

            ServiceProvider = services.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}