using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArgumentsStyleLiteral

namespace Gyldendal.Api.CoreData.Common.Logging
{
    public class Logger : ILogger
    {
        private readonly LoggingManager.Logger _logger;

        public Logger()
        {
            _logger = new LoggingManager.Logger(new LoggingManagerConfig());
        }
        
        public void Info(string message, bool forced = false, [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Info(message, forced, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Warning(string message, bool forced = false, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Warning(message, forced, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Debug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Debug(message, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Error(string message, Exception ex, bool isCritical = false, 
            [CallerMemberName] string methodName = "", string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Error(message, ex, isCritical, methodName, transactionId: transactionId, isGdprSafe: isGdprSafe);
        }

        public void Critical(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Critical(message, ex, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Fatal(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false)
        {
            _logger.Fatal(message, ex, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public string GetPropertyNameAndValue(params Expression<Func<object>>[] propertyLambda)
        {
            return _logger.GetPropertyNameAndValue(propertyLambda);
        }

        public void SetCurrentThreadContext(string context)
        {
            _logger.SetCurrentThreadContext(context);
        }

        public void ShutdownLogger()
        {
            _logger.ShutdownLogger();
        }
    }
}
