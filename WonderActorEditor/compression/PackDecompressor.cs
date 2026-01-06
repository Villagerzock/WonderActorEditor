using System.Text;

namespace WonderActorEditor.compression;

public class PackDecompressor
{
    public static string? GetHeader(byte[] file)
    {
        if (file.Length >= 4)
        {
            byte[] header = file[..4];
            return Encoding.ASCII.GetString(header);
        };
        return null;
    }
    public static bool HasYaz0(byte[] file)
    {
        string? header = GetHeader(file);
        if (header == null)
        {
            return false;
        }
        Console.WriteLine(header);
        if (header == "Yaz0")
        {
            return true;
        }
        return false;
    }

    public static byte[] DecompressPack(byte[] file)
    {
        if (HasYaz0(file))
        {
            Span<byte> decompressed = Yaz0Library.Yaz0.Decompress(file);
            file = decompressed.ToArray();
        }
        Console.WriteLine(GetHeader(file));
        return file;
    }
}