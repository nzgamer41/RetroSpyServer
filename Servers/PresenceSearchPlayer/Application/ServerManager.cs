﻿using GameSpyLib.Common;
using GameSpyLib.Extensions;
using GameSpyLib.Logging;
using System.Net;

namespace PresenceSearchPlayer
{
    /// <summary>
    /// A factory that create the instance of servers
    /// </summary>
    public class ServerManager : ServerManagerBase
    {
        private GPSPServer Server = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">Server name in XML config file</param>
        public ServerManager(string serverName) : base(serverName)
        {
        }

        /// <summary>
        /// Starts a specific server
        /// </summary>
        /// <param name="cfg">The configuration of the specific server to run</param>
        protected override void StartServer(GameSpyLib.RetroSpyConfig.ServerConfig cfg)
        {
            if (cfg.Name == ServerName)
            {
                Server = new GPSPServer(cfg.Name, DBEngine, IPAddress.Parse(cfg.ListeningAddress), cfg.ListeningPort);
                LogWriter.ToLog(Serilog.Events.LogEventLevel.Information,
                    StringExtensions.FormatServerTableContext(cfg.Name, cfg.ListeningAddress, cfg.ListeningPort.ToString()));
            }
        }

        /// <summary>
        /// Stop a specific server
        /// </summary>
        /// <param name="cfg">The configuration of the specific server to stop</param>
        protected override void StopServer()
        {
            Server?.Stop();
        }

        private bool _disposed = false;

        protected override void Dispose(bool disposingManagedResources)
        {
            if (_disposed)
            {
                return;
            }

            if (disposingManagedResources)
            {
                LogWriter.Log.Dispose();
            }

            base.Dispose(disposingManagedResources);
        }
    }
}
