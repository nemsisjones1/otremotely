﻿using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations
{
    public class HubEventHandler : IHubEventHandler
    {
        private readonly IHubContext<AgentHub> _serviceHub;
        private readonly ILogger<HubEventHandler> _logger;

        public HubEventHandler(
            IHubContext<AgentHub> serviceHub,
            ILogger<HubEventHandler> logger)
        {
            _serviceHub = serviceHub;
            _logger = logger;
        }

        public Task ChangeWindowsSession(RemoteControlSession session, string viewerConnectionId, int targetWindowsSession)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }   

            return _serviceHub.Clients
                .Client(ex.AgentConnectionId)
                .SendAsync("ChangeWindowsSession",
                    viewerConnectionId,
                    ex.UnattendedSessionId,
                    ex.AccessKey,
                    ex.UserConnectionId,
                    ex.RequesterUserName,
                    ex.OrganizationName,
                    ex.OrganizationId,
                    targetWindowsSession);
        }

        public Task InvokeCtrlAltDel(RemoteControlSession session, string viewerConnectionId)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _serviceHub.Clients.Client(ex.AgentConnectionId).SendAsync("CtrlAltDel");
        }

        public Task NotifyDesktopSessionAdded(RemoteControlSession sessionInfo)
        {
            return Task.CompletedTask;
        }

        public Task NotifyDesktopSessionDisposed(RemoteControlSession sessionInfo)
        {
            return Task.CompletedTask;
        }

        public Task NotifyDesktopSessionRemoved(RemoteControlSession sessionInfo)
        {
            return Task.CompletedTask;
        }

        public Task NotifyRemoteControlEnded(RemoteControlSession sessionInfo)
        {
            return Task.CompletedTask;
        }

        public Task NotifyRemoteControlStarted(RemoteControlSession sessionInfo)
        {
            return Task.CompletedTask;
        }

        public Task NotifySessionChanged(RemoteControlSession session, SessionSwitchReasonEx reason, int currentSessionId)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            _logger.LogDebug("Windows session changed during remote control.  " +
                "Reason: {reason}.  " +
                "Current Session ID: {sessionId}.  " +
                "Session Info: {@sessionInfo}",
                reason,
                currentSessionId,
                session);

            return _serviceHub.Clients
                .Client(ex.AgentConnectionId)
                .SendAsync("RestartScreenCaster",
                    ex.ViewerList,
                    ex.UnattendedSessionId,
                    ex.AccessKey,
                    ex.UserConnectionId,
                    ex.RequesterUserName,
                    ex.OrganizationName,
                    ex.OrganizationId);
        }

        public Task RestartScreenCaster(RemoteControlSession session, HashSet<string> viewerList)
        {

            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _serviceHub.Clients
                     .Client(ex.AgentConnectionId)
                     .SendAsync("RestartScreenCaster",
                            viewerList,
                            ex.UnattendedSessionId,
                            ex.AccessKey,
                            ex.UserConnectionId,
                            ex.RequesterName,
                            ex.OrganizationName,
                            ex.OrganizationId);
        }
    }
}
