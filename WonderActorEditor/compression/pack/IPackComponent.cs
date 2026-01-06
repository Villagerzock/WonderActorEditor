namespace WonderActorEditor.compression.pack;

public interface IPackComponent
{
    PackFile? GetAsPackFile();
    PackFolder? GetAsPackFolder();
}