using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Gyldendal.Api.CoreData.Common.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Log Information message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="forced">If true it will Log info message even if info Logging is disabled.</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="sourceFilePath">Calling method's class file path</param>
        /// <param name="sourceLineNumber">Line number from which this method is called</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Info(string message, bool forced = false, [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false);


        /// <summary>
        /// Log Warning message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="forced">If true it will Log info message even if info Logging is disabled.</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="sourceFilePath">Calling method's class file path</param>
        /// <param name="sourceLineNumber">Line number from which this method is called</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Warning(string message, bool forced = false, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", 
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false);

        /// <summary>
        /// Log Debug message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="sourceFilePath">Calling method's class file path</param>
        /// <param name="sourceLineNumber">Line number from which this method is called</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Debug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", 
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false);

        /// <summary>
        /// Log Error message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged</param>
        /// <param name="isCritical">Log Message as Critical if this flag is true</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Error(string message, Exception ex, bool isCritical = false, [CallerMemberName] string methodName = "", 
            string transactionId = null, bool isGdprSafe = false);

        /// <summary>
        /// Log Critical message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="sourceFilePath">Calling method's class file path</param>
        /// <param name="sourceLineNumber">Line number from which this method is called</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Critical(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", 
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false);

        /// <summary>
        /// Log Fatal message
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="ex">Exception to be logged</param>
        /// <param name="methodName">Calling method name</param>
        /// <param name="sourceFilePath">Calling method's class file path</param>
        /// <param name="sourceLineNumber">Line number from which this method is called</param>
        /// <param name="transactionId">To track request's complete cycle</param>
        /// <param name="isGdprSafe">Specifies the logged message is safe or not in context of GDPR</param>
        void Fatal(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0, string transactionId = null, bool isGdprSafe = false);
        
        /// <summary>
        /// Provide this method with all the parameters as Expression and it will construct a string with parameter/variable name and value for logging
        /// </summary>
        /// <param name="propertyLambda">The expression containing parameters</param>
        /// <returns>A string contain name and value of parameters/variables provided</returns>
        string GetPropertyNameAndValue(params Expression<Func<object>>[] propertyLambda);

        /// <summary>
        /// Set Thread Context which will be saved to current thread and will be used
        /// with all the logging calls within current thread.
        /// </summary>
        /// <param name="context">Context</param>
        void SetCurrentThreadContext(string context);

        void ShutdownLogger();
    }
}
