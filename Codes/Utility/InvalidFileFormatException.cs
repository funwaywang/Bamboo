using System;
using System.Runtime.Serialization;

namespace Bamboo
{
    [Serializable]
    internal class InvalidFileFormatException : Exception
    {
        public InvalidFileFormatException() : base("Invalid File Format.")
        {

        }
    }
}