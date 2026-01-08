using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;

namespace WonderActorEditor.rendering;

public class Model
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Uv;
    }
    
    public Vertex[] vertices;
    public uint[] indices;

    public void upload()
    {
        ResourceFactory factory = Program.gd.ResourceFactory;
        DeviceBuffer vertexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(vertices.Length * Unsafe.SizeOf<Vertex>()),
            BufferUsage.VertexBuffer
        ));

        DeviceBuffer indexBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)(indices.Length * Unsafe.SizeOf<uint>()),
            BufferUsage.IndexBuffer
        ));
        
        
    }
}