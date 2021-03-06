﻿using System;
using System.Collections.Generic;
using GameSpyLib.Extensions;

namespace Chat.Entity.Structure.ChatCommand
{
    public enum GetKeyType
    {
        GetChannelAllUserKeyValue,
        GetChannelSpecificUserKeyValue
    }
    public class GETCKEY : ChatChannelCommandBase
    {
        public string NickName { get; protected set; }
        public string Cookie { get; protected set; }
        public List<string> Keys { get; protected set; }

        public GetKeyType RequestType { get; protected set; }

        public GETCKEY()
        {
            Keys = new List<string>();
        }

        public override bool Parse(string recv)
        {
            if (!base.Parse(recv))
            {
                return false;
            }

            if (_cmdParams.Count != 4)
            {
                return false;
            }

            if (_longParam == null)
            {
                return false;
            }

            NickName = _cmdParams[1];

            if (NickName == "*")
            {
                RequestType = GetKeyType.GetChannelAllUserKeyValue;
            }
            else
            {
                RequestType = GetKeyType.GetChannelSpecificUserKeyValue;
            }

            Cookie = _cmdParams[2];

            Keys = StringExtensions.ConvertKeyStrToList(_longParam);

            return true;
        }
    }
}
