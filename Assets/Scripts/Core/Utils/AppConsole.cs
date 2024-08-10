using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils
{
    public static class AppConsole
    {
        public delegate void LogMessageHandler(MessageInfo message);
        public static event LogMessageHandler OnLogMessage;

        private static readonly List<MessageInfo> _messages = new();

        public static void LogMessage(float time, string text, string stackTrace, LogType logType)
        {
            lock (_messages)
            {
                var message = new MessageInfo(time, text, stackTrace, logType);

                _messages.Add(message);

                if (OnLogMessage != null)
                    OnLogMessage(message);
            }
        }

        public static void ReadLastMessages(List<MessageInfo> outMessages, int maxMessagesCount = -1)
        {
            lock (_messages)
            {
                var messagesStartI = 0;
                var messagesCount = _messages.Count;

                if (maxMessagesCount >= 0)
                {
                    messagesStartI = Mathf.Max(_messages.Count - maxMessagesCount, 0);
                    messagesCount = _messages.Count - messagesStartI;
                }

                for (var i = messagesStartI; i < messagesStartI + messagesCount; i++)
                    outMessages.Add(_messages[i]);
            }
        }

        public class MessageInfo
        {
            public readonly float Time;
            public readonly string Message;
            public readonly string StackTrace;
           
            public readonly LogType LogType;
            
            public MessageInfo(float time, string message, string stackTrace, LogType logType)
            {
                Time = time;
                Message = message;
                StackTrace = stackTrace;
                LogType = logType;
            }
        }
    }
}