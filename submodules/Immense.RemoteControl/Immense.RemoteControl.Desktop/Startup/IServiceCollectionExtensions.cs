using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Startup;
using Immense.RemoteControl.Desktop.Services;
using Immense.RemoteControl.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Immense.RemoteControl.Desktop.Services.Linux;

namespace Immense.RemoteControl.Desktop.Startup;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Linux and cross-platform remote control services to the service collection.
    /// All methods on <see cref="IRemoteControlClientBuilder"/> must be called to register
    /// required services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="clientConfig"></param>
    public static void AddRemoteControlLinux(
        this IServiceCollection services,
        Action<IRemoteControlClientBuilder> clientConfig)
    {
        services.AddRemoteControlXplat(clientConfig);

        services.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();
        services.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputLinux>();
        services.AddSingleton<IClipboardService, ClipboardServiceLinux>();
        services.AddSingleton<IAudioCapturer, AudioCapturerLinux>();
        services.AddSingleton<IChatUiService, ChatUiServiceLinux>();
        services.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
        services.AddScoped<IFileTransferService, FileTransferServiceLinux>();
        services.AddSingleton<ISessionIndicator, SessionIndicatorLinux>();
        services.AddSingleton<IShutdownService, ShutdownServiceLinux>();
        services.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceLinux>();
        services.AddSingleton<IAvaloniaDispatcher, AvaloniaDispatcher>();
        services.AddSingleton<IAppStartup, AppStartup>();
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddTransient<IMessageBoxViewModel, MessageBoxViewModel>();
    }
}
