namespace Assignment1_ONT412
{
    // Proxy Design Pattern: ICallOperation Interface
    // Subject interface, defining the common interface for RealSubject and Proxy.
    public interface ICallOperation
    {
        Call? MakeCall(string caller, string receiver);
        void DropCall(Call call);
        Call? ReturnCall(Call call);
    }
}
