namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class TableAlreadyExistsException : Exception
{
    private ulong roleId;

    public TableAlreadyExistsException()
    {
    }

    public TableAlreadyExistsException(ulong roleId) : base($"Table with role id {roleId} already exists.")
    {
        this.roleId = roleId;
    }

    public TableAlreadyExistsException(string tableName) : base($"Table with name {tableName} already exists.")
    {
    }

    public TableAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TableAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}