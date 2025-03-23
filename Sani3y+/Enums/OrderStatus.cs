namespace Sani3y_.Enums
{
    public enum OrderStatus
    {
        WaitingForAcceptance,  // Craftsman needs to accept or reject
        UnderImplementation,   // Craftsman is working on it
        Completed,             // User marks it as done
        Canceled               // User cancels the request
    }
}
