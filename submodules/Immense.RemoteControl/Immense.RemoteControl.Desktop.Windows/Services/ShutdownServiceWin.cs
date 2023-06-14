using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.UI.WPF.Services;
using Immense.RemoteControl.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Desktop.Windows.Services;

public class ShutdownServiceWin : IShutdownService
{
    private readonly IDesktopHubConnection _hubConnection;
    private readonly IWindowsUiDispatcher _dispatcher;
    private readonly IAppState _appState;
    private readonly ILogger<ShutdownServiceWin> _logger;
    private readonly SemaphoreSlim _shutdownLock = new(1, 1);

    public ShutdownServiceWin(
        IDesktopHubConnection hubConnection,
        IWindowsUiDispatcher dispatcher,
        IAppState appState,
        ILogger<ShutdownServiceWin> logger)
    {
        _hubConnection = hubConnection;
        _dispatcher = dispatcher;
        _appState = appState;
        _logger = logger;
    }

    public async Task Shutdown()
    {
        using var _ = _logger.Enter(LogLevel.Information);

        try
        {
            if (!await _shutdownLock.WaitAsync(0))
            {
                // We've made our best effort to shutdown gracefully, but WPF will
                // sometimes hang indefinitely.  In that case, we'll forcefully close.
                _logger.LogInformation(
                    "Shutdown was called more than once. Forcing process exit.");
                Environment.FailFast("Process hung during shutdown. Forcefully quitting on second call.");
                return;
            }

            _logger.LogInformation("Starting process shutdown.");

            _logger.LogInformation("Disconnecting viewers.");
            await TryDisconnectViewers();

            _logger.LogInformation("Shutting down UI dispatchers.");
            await _dispatcher.Shutdown();

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while shutting down.");
            Environment.Exit(1);
        }
        finally
        {
            _shutdownLock.Release();
        }
    }

    private async Task TryDisconnectViewers()
    {
        try
        {
            if (_hubConnection.IsConnected && _appState.Viewers.Any())
            {
                await _hubConnection.DisconnectAllViewers();
                await _hubConnection.Disconnect();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending shutdown notice to viewers.");
        }
    }
}