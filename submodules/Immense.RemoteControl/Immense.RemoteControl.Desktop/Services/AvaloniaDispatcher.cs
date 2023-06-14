﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Desktop.Services;

public interface IAvaloniaDispatcher
{
    CancellationToken AppCancellationToken { get; }

    Application? CurrentApp { get; }

    Window? MainWindow { get; }

    Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal);
    Task InvokeAsync(Func<Task> func, DispatcherPriority priority = DispatcherPriority.Normal);
    Task<T> InvokeAsync<T>(Func<Task<T>> func, DispatcherPriority priority = DispatcherPriority.Normal);
    void Post(Action action, DispatcherPriority priority = DispatcherPriority.Normal);
    void Shutdown();
    void StartUnattended();
    void StartAttended();
}

internal class AvaloniaDispatcher : IAvaloniaDispatcher
{
    private static readonly CancellationTokenSource _appCts = new();
    private AppBuilder? _appBuilder;
    private static Application? _currentApp;
    private readonly ILogger<AvaloniaDispatcher> _logger;

    public AvaloniaDispatcher(ILogger<AvaloniaDispatcher> logger)
    {
        _logger = logger;
    }

    public CancellationToken AppCancellationToken => _appCts.Token;

    public Application? CurrentApp => _currentApp ?? _appBuilder?.Instance;

    public Window? MainWindow
    {
        get
        {
            if (CurrentApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
            {
                return app.MainWindow;
            }
            return null;
        }
    }

    public Task InvokeAsync(Func<Task> func, DispatcherPriority priority = DispatcherPriority.Normal)
    {

        return Dispatcher.UIThread.InvokeAsync(func, priority);
    }

    public Task<T> InvokeAsync<T>(Func<Task<T>> func, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(func, priority);
    }

    public Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(action, priority);
    }

    public void Post(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        Dispatcher.UIThread.Post(action, priority);
    }

    public void Shutdown()
    {
        _appCts.Cancel();
        if (_currentApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            if (!lifetime.TryShutdown())
            {
                Environment.Exit(0);
            }
        }
    }

    public void StartUnattended()
    {
        try
        {
            var args = Environment.GetCommandLineArgs();
            var argString = string.Join(", ", args);
            _logger.LogInformation("Starting dispatcher in unattended mode with args: [{args}].", argString);
            _appBuilder = BuildAvaloniaApp();
            _appBuilder.Start(MainImpl, args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting background app.");
            throw;
        }
    }

    public void StartAttended()
    {
        try
        {
            var args = Environment.GetCommandLineArgs();
            _appBuilder = BuildAvaloniaApp();
            _appBuilder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting foreground app.");
            throw;
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    private static void MainImpl(Application app, string[] args)
    {
        _currentApp = app;
        app.Run(_appCts.Token);
    }
}
