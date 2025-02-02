﻿using CommunityToolkit.Mvvm.Messaging;
using FluentGit.Infrastructure.Message;
using FluentGit.Infrastructure.MVVM;
using FluentGit.Pages.RepositoryContent;
using FluentGit.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.IO;

namespace FluentGit.Pages.RepositoryInit
{
    public partial class RepositoryInitViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private String? _newRepositoryUrl;

        [ObservableProperty]
        private String? _newRepositoryName;

        [ObservableProperty]
        private String? _newRepositoryPath;

        [ObservableProperty]
        private bool _progressBarVisible = false;

        public event EventHandler<BaseViewModel> CloneCompleteEvent;

        public RepositoryInitViewModel(IServiceProvider serviceProvider,RepositoryInitView view) : base(view) 
        {
            _serviceProvider = serviceProvider;
        }


        [RelayCommand]
        public void ChooseDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var ok = dialog.ShowDialog();
            if (ok == System.Windows.Forms.DialogResult.OK)
            {
                NewRepositoryPath = dialog.SelectedPath;
            }
        }

        [RelayCommand]
        public void OpenRepository()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var ok = dialog.ShowDialog();
            if (ok == System.Windows.Forms.DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                var gitPath = Path.Combine(path, ".git");
                var gitService = new GitService(gitPath);
                var viewModelFunc = _serviceProvider.GetService<Func<GitService, RepositoryContentViewModel>>();
                if (viewModelFunc != null)
                {
                    var viewModel = viewModelFunc.Invoke(gitService);
                    //WeakReferenceMessenger.Default.Send(new CloneCompleteMessage(viewModel));
                    CloneCompleteEvent?.Invoke(this, viewModel);
                }
            }
        }

        [RelayCommand]
        public void Clone()
        {
            if (String.IsNullOrEmpty(NewRepositoryUrl)
                || String.IsNullOrEmpty(NewRepositoryName)
                || String.IsNullOrEmpty(NewRepositoryPath)) return;
            ProgressBarVisible = true;
            var repositoryAsync = _serviceProvider.GetService<GitRepositoryAsync>();
            if (repositoryAsync != null)
            {
                var cloneTask = repositoryAsync
                    .Clone(NewRepositoryUrl, NewRepositoryPath + "\\" + NewRepositoryName);
                cloneTask
                    .GetAwaiter()
                    .OnCompleted(() =>
                    {
                        //clone complete
                        var gitService = new GitService(cloneTask.Result);
                        ProgressBarVisible = false;
                        var viewModelFunc = _serviceProvider.GetService<Func<GitService, RepositoryContentViewModel>>();
                        if (viewModelFunc != null)
                        {
                            var viewModel = viewModelFunc.Invoke(gitService);
                            WeakReferenceMessenger.Default.Send(new CloneCompleteMessage(viewModel));
                        }
                    });
            }
        }
    }
}
