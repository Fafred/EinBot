namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class KeyAlreadyPresentInTableException : Exception
{
    public KeyAlreadyPresentInTableException()
    {
    }

    public KeyAlreadyPresentInTableException(int tableId, string key) : base($"Table with id {tableId} already contains the key {key}") { }

    public KeyAlreadyPresentInTableException(string tableName, string key) : base($"Table with name {tableName} already contains the key {key}") { }

    public KeyAlreadyPresentInTableException(ulong roleId, string key) : base($"Table with role id {roleId} already contains the key {key}") { }

    public KeyAlreadyPresentInTableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected KeyAlreadyPresentInTableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}