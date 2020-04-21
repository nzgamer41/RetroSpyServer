﻿using Chat.Entity.Structure.ChatCommand;
using Chat.Handler.CommandHandler;
using GameSpyLib.Common.Entity.Interface;
using GameSpyLib.Logging;
using System;
using System.Collections.Generic;

namespace Chat.Handler.SystemHandler.ChatCommandManage
{
    public class ChatCommandManager
    {
        public Dictionary<string, Type> AvailableCommands;

        public ChatCommandManager()
        {
            AvailableCommands = new Dictionary<string, Type>();
        }

        public void Start()
        {
            AddCommand(new CRYPT(), typeof(CRYPTHandler));
            AddCommand(new USRIP(), typeof(USRIPHandler));
            AddCommand(new USER(), typeof(USERHandler));
            AddCommand(new NICK(), typeof(NICKHandler));
            AddCommand(new JOIN(), typeof(JOINHandler));
            AddCommand(new MODE(),typeof(MODEHandler));
            AddCommand(new NAMES(), typeof(NAMESHandler));
        }

        public ChatCommandManager AddCommand(ChatCommandBase cmd, Type cmdHandlerType)
        {
            AvailableCommands.TryAdd(cmd.CommandName, cmdHandlerType);
            return this;
        }

        public bool HandleCommands(IClient client, List<ChatCommandBase> cmds)
        {
            foreach (var cmd in cmds)
            {
                if (AvailableCommands.ContainsKey(cmd.CommandName))
                {
                    Type handlerType;
                    AvailableCommands.TryGetValue(cmd.CommandName, out handlerType);
                    ChatCommandHandlerBase handler =
                        (ChatCommandHandlerBase)Activator.CreateInstance(handlerType, client, cmd);
                    handler.Handle();
                }
                else
                {
                    LogWriter.ToLog(Serilog.Events.LogEventLevel.Error, $"{cmd.CommandName}Handler not implemented!");
                    continue;
                }
            }

            return true;
        }
    }
}
