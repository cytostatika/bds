using System;
namespace Utilities
{
    [Serializable]
    public class FunctionCall
    {
        public readonly string funcName;
        public readonly object funcInput;
        public readonly Type actorType;

        public FunctionCall(string funcName, object funcInput, Type actorType)
        {
            this.funcName = funcName;
            this.funcInput = funcInput;
            this.actorType = actorType;
        }
    }
}