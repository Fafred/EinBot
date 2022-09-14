namespace EinBot.Currency.CurrencyInteractions;

using System;
using System.Runtime.Serialization;

[Serializable]
internal class CollectionTypeIsPerKeyException : Exception
{
    public CollectionTypeIsPerKeyException()
    {
    }

    public CollectionTypeIsPerKeyException(string? message) : base(message)
    {
    }

    public CollectionTypeIsPerKeyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CollectionTypeIsPerKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}