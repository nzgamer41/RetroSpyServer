﻿
using GameSpyLib.Database.DatabaseModel.MySql;
using GameSpyLib.Database.Entity;
using GameSpyLib.Extensions;
using GameSpyLib.Logging;
using GameSpyLib.RetroSpyConfig;
using Serilog.Events;
using StackExchange.Redis;
using System;
using System.Linq;


namespace GameSpyLib.Common
{
    public abstract class ServerManagerBase : IDisposable
    {
        public readonly string RetroSpyVersion = "0.5";
        public string ServerName { get; protected set; }
        public static ConfigManager Config { get; protected set; }
        public static LogWriter LogWriter { get; protected set; }
        public static ConnectionMultiplexer Redis { get; protected set; }
        public DatabaseEngine DBEngine;

        protected bool Disposed = false;

        public ServerManagerBase(string serverName)
        {
            ServerName = serverName;
            LogWriter = new LogWriter(serverName);
            StringExtensions.ShowRetroSpyLogo(RetroSpyVersion);
            LoadDatabaseConfig();
            LoadServerConfig();
        }

        public void LoadServerConfig()
        {
            LogWriter.ToLog(LogEventLevel.Information, StringExtensions.FormatServerTableHeader("-----------", "--------------", "------"));
            LogWriter.ToLog(LogEventLevel.Information, StringExtensions.FormatServerTableContext("Server Name", "Host Name", "Port"));
            LogWriter.ToLog(LogEventLevel.Information, StringExtensions.FormatServerTableHeader("-----------", "--------------", "------"));
            // Add all servers
            foreach (ServerConfig cfg in ConfigManager.Config.Servers)
            {
                StartServer(cfg);
            }
            LogWriter.ToLog(LogEventLevel.Information, StringExtensions.FormatServerTableHeader("-----------", "--------------", "------"));
            LogWriter.ToLog(LogEventLevel.Information, " Server is successfully started! ");
        }


        protected abstract void StartServer(ServerConfig cfg);

        protected abstract void StopServer();

        //public abstract bool IsServerRunning();

        private void LoadDatabaseConfig()
        {
            DatabaseConfig dbConfig = ConfigManager.Config.Database;
            DBEngine = (DatabaseEngine)Enum.Parse(typeof(DatabaseEngine), dbConfig.Type);

            // Determine which database is using and create the database connection

            switch (DBEngine)
            {
                case DatabaseEngine.MySql:
                    string mySqlConnStr =
                        string.Format(
                            "Server={0};Database={1};Uid={2};Pwd={3};Port={4};SslMode={5};SslCert={6};SslKey={7};SslCa={8}",
                            dbConfig.RemoteAddress, dbConfig.DatabaseName, dbConfig.UserName, dbConfig.Password,
                            dbConfig.RemotePort, dbConfig.SslMode, dbConfig.SslCert, dbConfig.SslKey, dbConfig.SslCa);
                    retrospyContext.RetroSpyMySqlConnStr = mySqlConnStr;
                    using (var db = new retrospyContext())
                    {
                        db.Users.Where(u => u.Userid == 0);
                    }
                    break;
                case DatabaseEngine.SQLite:
                    string SQLiteConnStr = "Data Source=" + dbConfig.DatabaseName + ";Version=3;New=False";

                    break;
                default:
                    throw new Exception("Unknown database engine!");
            }
            LogWriter.Log.Information($"Successfully connected to the {dbConfig.Type}!");

            RedisConfig redisConfig = ConfigManager.Config.Redis;
            Redis = ConnectionMultiplexer.Connect(redisConfig.RemoteAddress + ":" + redisConfig.RemotePort.ToString());
            LogWriter.Log.Information($"Successfully connected to Redis!");

        }


        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)。
                StopServer();
            }

            // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
            // TODO: 将大型字段设置为 null。
            Disposed = true;
        }

        //TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~ServerManagerBase()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
    }
}
