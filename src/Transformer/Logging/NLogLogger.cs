using System;
using NLog;

namespace Transformer.Logging
{
    public class NLogLogger : ILog
    {
        private readonly NLog.Logger log;

        public NLogLogger(string typeName)
        {
            log = NLog.LogManager.GetLogger(typeName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogger"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public NLogLogger(Type type)
        {
            log = NLog.LogManager.GetLogger(type.Name);
        }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log.Debug(message.ToString(), exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            log.Debug(format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message.ToString(), exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            log.Error(format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message.ToString(), exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            log.Fatal(format, args);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            log.Info(message.ToString(), exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.Info(format, args);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message.ToString(), exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            log.Warn(format, args);
        }
    }
}