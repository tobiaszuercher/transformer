using System;

using Microsoft.Build.Utilities;
using PowerDeploy.Transformer.Logging;
using Transformer.Core.Logging;
using Transformer.MsBuild;

namespace PowerDeploy.MsBuild
{
    /// <summary>
    /// Uses a cmdlet to log to
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class BuildLogFactory : ILogFactory
    {
        private readonly TaskLoggingHelper _taskLogHelper;

        public BuildLogFactory(TaskLoggingHelper taskLogHelper)
        {
            _taskLogHelper = taskLogHelper;
        }

        public ILog GetLogger(Type type)
        {
            return new MsBuildLogger(_taskLogHelper);
        }

        public ILog GetLogger(string typeName)
        {
            return new MsBuildLogger(_taskLogHelper);
        }
    }
}
