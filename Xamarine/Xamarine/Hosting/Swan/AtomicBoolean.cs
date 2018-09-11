﻿using Xamarine.Hosting.Swan.Abstractions;

namespace Xamarine.Hosting.Swan
{
    /// <summary>
    ///     Fast, atomic boolean combining interlocked to write value and volatile to read values.
    /// </summary>
    public sealed class AtomicBoolean : AtomicTypeBase<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AtomicBoolean" /> class.
        /// </summary>
        /// <param name="initialValue">if set to <c>true</c> [initial value].</param>
        public AtomicBoolean(bool initialValue = default)
            : base(initialValue ? 1 : 0)
        {
            // placeholder
        }

        /// <inheritdoc />
        protected override bool FromLong(long backingValue)
        {
            return backingValue != 0;
        }

        /// <inheritdoc />
        protected override long ToLong(bool value)
        {
            return value ? 1 : 0;
        }
    }
}
