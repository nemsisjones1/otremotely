﻿using Avalonia.Controls;
using Immense.RemoteControl.Desktop.Services;
using System.Windows.Input;
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using Immense.RemoteControl.Desktop.Shared.Reactive;

namespace Immense.RemoteControl.Desktop.ViewModels;

public interface IHostNamePromptViewModel
{
    string Host { get; set; }
    ICommand OKCommand { get; }
}

public class HostNamePromptViewModel : BrandedViewModelBase, IHostNamePromptViewModel
{
    public HostNamePromptViewModel(
        IBrandingProvider brandingProvider,
        IAvaloniaDispatcher dispatcher,
        ILogger<HostNamePromptViewModel> logger)
        : base(brandingProvider, dispatcher, logger)
    {
        OKCommand = new RelayCommand<Window>(x => x?.Close());
    }

    public string Host
    {
        get => Get<string>() ?? "https://";
        set => Set(value);
    }

    public ICommand OKCommand { get; }
}
