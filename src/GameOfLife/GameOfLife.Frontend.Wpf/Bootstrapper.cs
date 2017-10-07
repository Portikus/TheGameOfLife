using System.Windows;
using Autofac;
using GameOfLife.Frontend.Wpf.Mocks;
using GameOfLife.Frontend.Wpf.Views;
using Prism.Autofac;
using Prism.Events;

namespace GameOfLife.Frontend.Wpf
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            builder.RegisterType<GameManagerMock>().AsImplementedInterfaces();
            builder.RegisterType<EventAggregator>().AsImplementedInterfaces().SingleInstance();
        }

        protected override DependencyObject CreateShell()
        {
            var shell = new ShellView();
            shell.Show();
            return shell;
        }
    }
}