namespace WonderActorEditor.compression.pack;

public class PackFolder : IPackComponent
{
    private IPackComponent[] children;

    public IPackComponent? GetChild(int i)
    {
        return children[i];
    }
    
    public PackFile? GetAsPackFile()
    {
        return null;
    }

    public PackFolder? GetAsPackFolder()
    {
        return this;
    }
}