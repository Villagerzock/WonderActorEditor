using SixLabors.ImageSharp.Processing;

namespace WonderActorEditor;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Veldrid;

public static class TextureLoader
{
    public static Texture LoadTexture(GraphicsDevice gd, ResourceFactory factory, string path)
    {
        using Image<Rgba32> image = Image.Load<Rgba32>(path);
        image.Mutate(x => x.Flip(FlipMode.Vertical)); // je nach UV nötig

        var tex = factory.CreateTexture(TextureDescription.Texture2D(
            (uint)image.Width, (uint)image.Height, mipLevels: 1, arrayLayers: 1,
            PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled));

        var pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);

        gd.UpdateTexture(tex, pixels, 0, 0, 0, (uint)image.Width, (uint)image.Height, 1, 0, 0);
        return tex;
    }
}
