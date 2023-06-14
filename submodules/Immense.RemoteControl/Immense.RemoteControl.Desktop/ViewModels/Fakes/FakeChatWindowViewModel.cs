﻿using Immense.RemoteControl.Desktop.Shared.Reactive;
using Immense.RemoteControl.Shared.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.ViewModels.Fakes;

public class FakeChatWindowViewModel : FakeBrandedViewModelBase, IChatWindowViewModel
{
    public ObservableCollection<ChatMessage> ChatMessages { get; } = new()
    {
        new ChatMessage("Designer", "This is a design-time test message.")
    };

    public string InputText
    {
        get => "Some text I'm going to send.";
        set { }
    }
    public string OrganizationName
    {
        get => "Design-Time Technicians";
        set { }
    }
    public string SenderName
    {
        get => "Test Tech";
        set { }
    }

    public string ChatSessionHeader => "Test Chat";

    public ICommand CloseCommand => new RelayCommand(() => { });

    public ICommand MinimizeCommand => new RelayCommand(() => { });

    public Task SendChatMessage()
    {
        return Task.CompletedTask;
    }
}
