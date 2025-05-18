using ElevatorControl.Application.Interface;
using ElevatorControl.Application.Services;
using ElevatorControl.IoC;
using ElevatorControl.UI.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ElevatorControl.UI
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ElevatorController>();
            services.AddSingleton<IElevatorService,ElevatorService>();
            services.AddSingleton<MainWindow>();
            var sp = services.BuildServiceProvider();

            var mainWindow = sp.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}