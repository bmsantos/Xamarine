﻿using System;

namespace Xamarine.Hosting.Swan.Attributes
{
    /// <summary>
    ///     Represents an attribute to select which properties are copyable between objects.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CopyableAttribute : Attribute
    {
    }
}
