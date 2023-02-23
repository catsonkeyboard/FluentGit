﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Dragablz;
using FluentGit.Infrastructure.MVVM;
using FluentGit.Pages.RepositoryContent;
using FluentGit.Pages.RepositoryInit;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;

namespace FluentGit;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private string _applicationTitle;

    public IInterTabClient InterTabClient => new InterTabClient();

    [ObservableProperty]
    private ObservableCollection<HeaderedItemViewModel> _items;

    public Func<HeaderedItemViewModel> Factory
    {
        get
        {
            return
                () =>
                {
                    var dateTime = DateTime.Now;
                    var contentViewModel = _serviceProvider.GetService<RepositoryInitViewModel>();
                    return new CustomHeaderedItemViewModel()
                    {
                        Header = "New Repository",
                        Content = contentViewModel.View,
                    };
                };
        }
    }

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _applicationTitle = "FluentGit";
        Items = new ObservableCollection<HeaderedItemViewModel>();
        Items.Add(new HeaderedItemViewModel
        { 
            Header = "HOME", 
            Content = "Home" });
        Items.Add(new HeaderedItemViewModel
        {
            Header = "jdksaghdfsg",
            Content = "dfsdfs"
        });
    }
    
    private void OnToggleThemeClicked(object sender, RoutedEventArgs e)
    {
        var currentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();

        Wpf.Ui.Appearance.Theme.Apply(currentTheme == Wpf.Ui.Appearance.ThemeType.Light ? Wpf.Ui.Appearance.ThemeType.Dark : Wpf.Ui.Appearance.ThemeType.Light);
    }
    
}