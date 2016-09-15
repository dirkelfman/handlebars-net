using System;
using System.Diagnostics;

namespace HandlebarsDotNet.Compiler
{
    [DebuggerDisplay("undefined")]
    public  class UndefinedBindingResult
    {
        public UndefinedBindingResult()
        {
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}

