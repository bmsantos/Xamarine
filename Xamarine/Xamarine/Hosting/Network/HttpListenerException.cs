﻿using System;

#if !NET47
namespace Xamarine.Hosting.Network
{
    /// <summary>
    ///     Represents an HTTP Listener's exception.
    /// </summary>
    public class HttpListenerException : Exception
    {
        internal HttpListenerException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Gets the error code.
        /// </summary>
        public int ErrorCode { get; }
    }
}
#endif
