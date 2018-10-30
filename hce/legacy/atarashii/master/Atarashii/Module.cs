using System;

namespace Atarashii
{
    /// <summary>
    ///     Abstract type representing an Atarashii module.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Name of the module to be used in contexts such as output subjects.
        /// </summary>
        protected abstract string Identifier { get; }
        
        /// <summary>
        /// Optional Output type for writing messages to.
        /// </summary>
        private Output Output { get; }

        protected Module(Output output = null)
        {
            Output = output;
        }

        /// <summary>
        ///     Invokes the Output.Write with the inbound exception message, then throws the exception.
        /// </summary>
        /// <param name="exception">
        ///    Exception to write and throw.
        /// </param>
        /// <exception cref="Exception">
        ///    Exception which will be thrown.
        /// </exception>
        protected void WriteAndThrow(Exception exception)
        {
            WriteError(exception.Message);
            throw exception;
        }

        /// <summary>
        ///     Writes a Type.Success message w/ Module Name as the subject.
        /// </summary>
        /// <param name="message">
        ///    Message to write to the Output instance.
        /// </param>
        protected void WriteSuccess(string message)
        {
            Output?.WriteSuccess(Identifier, message);
        }

        /// <summary>
        ///     Writes a Type.Info message w/ Module Name as the subject.
        /// </summary>
        /// <param name="message">
        ///    Message to write to the Output instance.
        /// </param>
        protected void WriteInfo(string message)
        {
            Output?.WriteInfo(Identifier, message);
        }

        /// <summary>
        ///     Writes a Type.Error message w/ Module Name as the subject.
        /// </summary>
        /// <param name="message">
        ///    Message to write to the Output instance.
        /// </param>
        protected void WriteError(string message)
        {
            Output?.WriteError(Identifier, message);
        }
    }
}