namespace EinBot.Currency.CurrencyInteractions.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
internal class CollectionTypeIsPerUserException : Exception
{
    public CollectionTypeIsPerUserException()
    {
    }

    public CollectionTypeIsPerUserException(string? message) : base(message)
    {
    }

    public CollectionTypeIsPerUserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CollectionTypeIsPerUserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}