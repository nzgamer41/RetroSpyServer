﻿using Chat.Entity.Structure;
using Chat.Entity.Structure.ChatCommand;
using Chat.Server;
using GameSpyLib.Common.BaseClass;
using GameSpyLib.Common.Entity.Interface;

namespace Chat.Handler.CommandHandler
{
    /// <summary>
    /// error code condition is complicated
    /// there are 2 types error code
    /// 1.irc numeric error code
    /// 2.self defined code
    /// we do not want to send self defined code, so if we find errorCode < noerror
    /// we just return.
    /// if error code bigger than noerror we need to process it in ConstructResponse()
    ///we also need to check the error code != noerror in ConstructResponse()
    /// </summary>
    public abstract class ChatCommandHandlerBase : CommandHandlerBase
    {
        protected ChatError _errorCode;
        protected ChatCommandBase _cmd;
        /// <summary>
        /// Generic response buffer
        /// if some handler have multiple response
        /// only use this for sending error message
        /// </summary>
        protected string _sendingBuffer;

        new protected ChatSession _session;

        public ChatCommandHandlerBase(ISession session, ChatCommandBase cmd) : base(session)
        {
            _errorCode = ChatError.NoError;
            _cmd = cmd;
            _session = (ChatSession)session.GetInstance();
        }

        //if we use this structure the error response should also write to _sendingBuffer
        public override void Handle()
        {
            base.Handle();

            CheckRequest();

            if (_errorCode != ChatError.NoError)
            {
                if (_errorCode == ChatError.IRCError)
                {
                    SendErrorMessage();
                }
                return;
            }

            DataOperation();
            if (_errorCode != ChatError.NoError)
            {
                if (_errorCode == ChatError.IRCError)
                {
                    SendErrorMessage();
                }
                return;
            }

            ConstructResponse();

            if (_errorCode != ChatError.NoError)
            {
                if (_errorCode == ChatError.IRCError)
                {
                    SendErrorMessage();
                }
                return;
            }

            Response();
        }
        public virtual void CheckRequest()
        { }
        public virtual void DataOperation()
        { }
        public virtual void ConstructResponse()
        { }
        public virtual void Response()
        {
            if (_sendingBuffer == null || _sendingBuffer == "" || _sendingBuffer.Length < 3)
            {
                return;
            }
            base._session.SendAsync(_sendingBuffer);
        }

        private void SendErrorMessage()
        {
            if (_sendingBuffer == null || _sendingBuffer == "" || _sendingBuffer.Length < 3)
            {
                return;
            }
            base._session.SendAsync(_sendingBuffer);
        }
    }
}
