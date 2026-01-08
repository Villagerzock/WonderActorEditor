using System.Numerics;

namespace WonderActorEditor.rendering;


// Not to confuse with Actor.cs this is the Actor class for the Editor, Actor.cs is the Actor you edit this right here is just for having Actors in a render that we can display
public class EditorActor
{
    public Vector3 Position
    {
        set
        {
            _transformCache = null;
            field = value;
        }
        get;
    } = new();

    public Quaternion Rotation
    {
        get;
        set
        {
            _transformCache = null;
            field = value;
        }
    } = Quaternion.Identity;

    public Vector3 Scale
    {
        get;
        set
        {
            _transformCache = null;
            field = value;
        }
    } = new();

    public Model? Model;

    private Matrix4x4? _transformCache;

    public Matrix4x4 Transform
    {
        get
        {
            if (_transformCache != null)
            {
                return _transformCache.Value;
            }

            Matrix4x4 modelMatrix = Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) *
                                    Matrix4x4.CreateTranslation(Position);
            _transformCache = modelMatrix;
            return modelMatrix;
        }
    }
}