﻿namespace Chat.Entity.Structure.ChatCommand
{
    public class LISTLIMIT : ChatCommandBase
    {
        public int MaxNumberOfChannels { get; protected set; }
        public string Filter { get; protected set; }

        public override bool Parse(string recv)
        {
            if (!base.Parse(recv))
            {
                return false;
            }

            if (_cmdParams.Count != 2)
            {
                return false;
            }
            int max;
            if (!int.TryParse(_cmdParams[0], out max))
            {
                return false;
            }
            MaxNumberOfChannels = max;

            Filter = _cmdParams[1];

            return true;
        }
    }
}
