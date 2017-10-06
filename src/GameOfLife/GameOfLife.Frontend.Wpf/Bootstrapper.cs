using System.Windows;
using Autofac;
using GameOfLife.Api;
using GameOfLife.Frontend.Wpf.Mocks;
using GameOfLife.Frontend.Wpf.ViewModels;
using GameOfLife.Frontend.Wpf.Views;
using Prism.Autofac;

namespace GameOfLife.Frontend.Wpf
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            builder.RegisterType<GameManagerMock>().AsImplementedInterfaces();
        }

        protected override DependencyObject CreateShell()
        {
            var shell = new ShellView ();
            shell.Show();
            return shell;
        }
    }
}