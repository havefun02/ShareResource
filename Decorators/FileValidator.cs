﻿namespace ShareResource.Decorators
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FileValidator:Attribute
    {
    }
}
