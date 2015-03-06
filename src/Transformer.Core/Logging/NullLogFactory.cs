using System;

namespace Transformer.Core.Logging
{
    /// <summary>
    /// Creates a NLogLogger, that logs all messages with NLog.
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class NLogFactory : ILogFactory
    {
        public ILog GetLogger(Type type)
        {
            return new NLogLogger(type);
        }

        public ILog GetLogger(string typeName)
        {
            return new NLogLogger(typeName);
        }

        public void DisableLogging()
        {
            NLog.LogManager.DisableLogging();
        }

        public void EnableLogging()
        {
            NLog.LogManager.EnableLogging();
        }
    }
}