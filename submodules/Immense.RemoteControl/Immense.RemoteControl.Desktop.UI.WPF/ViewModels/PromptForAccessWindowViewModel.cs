﻿using Immense.RemoteControl.Desktop.Shared.Reactive;
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.UI.WPF.Services;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.UI.WPF.ViewModels;

public interface IPromptForAccessWindowViewModel : IBrandedViewModelBase
{
    string OrganizationName { get; set; }
    bool PromptResult { get; set; }
    string RequesterName { get; set; }
    ICommand SetResultNoCommand { get; }
    ICommand SetResultYesCommand { get; }

    void SetResultNo(Window? promptWindow);
    void SetResultYes(Window? promptWindow);
}

public class PromptForAccessWindowViewModel : BrandedViewModelBase, IPromptForAccessWindowViewModel
{
    public PromptForAccessWindowViewModel(
        string requesterName,
        string organizationName,
        IBrandingProvider brandingProvider,
        IWindowsUiDispatcher dispatcher,
        ILogger<PromptForAccessWindowViewModel> logger)
        : base(brandingProvider, dispatcher, logger)
    {
        if (!string.IsNullOrWhiteSpace(requesterName))
        {
            RequesterName = requesterName;
        }

        if (!string.IsNullOrWhiteSpace(organizationName))
        {
            OrganizationName = organizationName;
        }

        SetResultNoCommand = new RelayCommand<Window>(SetResultNo);
        SetResultYesCommand = new RelayCommand<Window>(SetResultYes);
    }

    public string OrganizationName
    {
        get => Get<string>() ?? "your IT provider";
        set => Set(value);
    }

    public bool PromptResult { get; set; }
    public string RequesterName
    {
        get => Get<string>() ?? "a technician";
        set => Set(value);
    }

    public ICommand SetResultNoCommand { get; }
    public ICommand SetResultYesCommand { get; }


    public void SetResultNo(Window? promptWindow)
    {
        PromptResult = false;
        promptWindow?.Close();
    }

    public void SetResultYes(Window? promptWindow)
    {
        PromptResult = true;
        promptWindow?.Close();
    }
}
