﻿using System.Windows;
using Autofac;
using GameOfLife.Frontend.Wpf.Model;
using GameOfLife.Frontend.Wpf.Views;
using Prism.Autofac;
using Prism.Events;
using GameOfLife.Backend;
using GameOfLife.Frontend.Wpf.Mocks;

namespace GameOfLife.Frontend.Wpf
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            //builder.RegisterType<GameManagerMock>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<GameManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventAggregator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PlayerProvider>().SingleInstance();
        }

        protected override DependencyObject CreateShell()
        {
            var shell = new ShellView();
            shell.Show();
            return shell;
        }
    }
}