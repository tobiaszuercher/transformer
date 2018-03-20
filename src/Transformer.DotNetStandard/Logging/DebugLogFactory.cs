using System;

namespace Transformer.Core.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class DebugLogFactory : ILogFactory
    {
        public ILog GetLogger(Type type)
        {
            return new DebugLogger(type);
        }

        public ILog GetLogger(string typeName)
        {
            return new DebugLogger(typeName);
        }

        public void DisableLogging()
        {
            throw new NotImplementedException();
        }

        public void EnableLogging()
        {
            throw new NotImplementedException();
        }
    }
}