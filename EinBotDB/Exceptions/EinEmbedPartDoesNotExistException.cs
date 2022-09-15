namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class EinEmbedPartDoesNotExistException : Exception
{
    private int embedPartId;

    public EinEmbedPartDoesNotExistException()
    {
    }

    public EinEmbedPartDoesNotExistException(int embedPartId) : base($"EinEmbedPart with id {embedPartId} does not exist.")
    {
        this.embedPartId = embedPartId;
    }

    public EinEmbedPartDoesNotExistException(string? message) : base(message)
    {
    }

    public EinEmbedPartDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected EinEmbedPartDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}