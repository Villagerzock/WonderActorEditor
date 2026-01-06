namespace WonderActorEditor.compression.pack;

public class PackFile : IPackComponent
{
    private byte[]? _bytes;

    public byte[]? Bytes
    {
        get { return _bytes; }
    }
    public PackFile? GetAsPackFile()
    {
        return this;
    }

    public PackFolder? GetAsPackFolder()
    {
        return null;
    }
}