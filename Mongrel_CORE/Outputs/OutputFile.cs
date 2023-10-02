namespace Mongrel.Outputs;

public abstract class OutputFile : IDisposable
{
    public abstract void WriteLocation(Locations location);

    public abstract void Dispose();

}