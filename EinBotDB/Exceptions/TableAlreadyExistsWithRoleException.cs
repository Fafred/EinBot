namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class TableAlreadyExistsWithRoleException : Exception
{
    private ulong newRoleId;

    public TableAlreadyExistsWithRoleException()
    {
    }

    public TableAlreadyExistsWithRoleException(ulong newRoleId)
    {
        this.newRoleId = newRoleId;
    }

    public TableAlreadyExistsWithRoleException(string? message) : base(message)
    {
    }

    public TableAlreadyExistsWithRoleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TableAlreadyExistsWithRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}