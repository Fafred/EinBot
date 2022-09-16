namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class TableAlreadyExistsWithNameException : Exception
{
    public TableAlreadyExistsWithNameException()
    {
    }

    public TableAlreadyExistsWithNameException(string? message) : base(message)
    {
    }

    public TableAlreadyExistsWithNameException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TableAlreadyExistsWithNameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}