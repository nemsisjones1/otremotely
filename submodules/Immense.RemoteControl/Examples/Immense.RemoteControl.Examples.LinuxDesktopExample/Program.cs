using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Examples.LinuxDesktopExample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Immense.RemoteControl.Desktop.Startup;
using Immense.RemoteControl.Desktop.Services;
using Immense.RemoteControl.Desktop.Shared.Startup;
using Immense.RemoteControl.Desktop.Shared.Services;

var services = new ServiceCollection();
services.AddRemoteControlLinux(config =>
{
    config.AddBrandingProvider<BrandingProvider>();
});

services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
    // Add file logger, etc.
});
// Add other services.

var provider = services.BuildServiceProvider();

var result = await provider.UseRemoteControlClient(
    args,
    "The remote control client for Remotely.",
    serverUri: "https://localhost:7024");

if (!result.IsSuccess)
{
    Console.WriteLine($"Remote control failed with message: {result.Reason}");
}

var shutdownService = provider.GetRequiredService<IShutdownService>();
Console.CancelKeyPress += async (s, e) =>
{
    await shutdownService.Shutdown();
};

var appState = provider.GetRequiredService<IAppState>();
Console.WriteLine("Unattended session ready at: ");
Console.WriteLine($"https://localhost:7024/RemoteControl/Viewer?mode=Unattended&sessionId={appState.SessionId}&accessKey={appState.AccessKey}");

Console.WriteLine("Press Ctrl + C to exit.");
var dispatcher = provider.GetRequiredService<IAvaloniaDispatcher>();
try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, dispatcher.AppCancellationToken);
}
catch (TaskCanceledException)
{
    // Ok.
}
