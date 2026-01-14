using Microsoft.Xna.Framework;
using System.Linq;
using Matrix4x4 =  System.Numerics.Matrix4x4;

namespace _3dAnimation;

public class Transform
{
    private VectorFrame _translation = new VectorFrame();
    private QuaternionFrame _rotation = new QuaternionFrame();
    private ScalarFrame _scalar = new ScalarFrame();

    public VectorFrame Translation
    {
        get => _translation;
        set
        {
            HasTranslation = true;
            _translation = value;
        }
    }
    public QuaternionFrame Rotation
    {
        get => _rotation;
        set
        {
            HasRotation = true;
            _rotation = value;
        }
    }
    public ScalarFrame Scalar
    {
        get => _scalar;
        set
        {
            HasScalar = true;
            _scalar = value;
        }
    }

    public bool HasTranslation = false;
    public bool HasRotation = false;
    public bool HasScalar = false;

    public Matrix GetLocalMatrix()
    {
        Vector3 translation = Translation?.GetVector3() ?? Vector3.Zero;
        Vector3 scale = Scalar?.GetVector3() ?? Vector3.One;
        Quaternion rotation = Rotation?.GetQuaternion() ?? Quaternion.Identity;

        rotation.Normalize();

        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.ToNumerics());
        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation.ToNumerics());
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(translation.ToNumerics());

        return scaleMatrix * rotationMatrix * translationMatrix;
    }

    public Transform GetCopy()
    {
        var transform = new Transform();
        transform.Translation.mValue = Translation.mValue.ToList().ToArray();
        transform.Rotation.mValue = Rotation.mValue.ToList().ToArray();
        transform.Scalar.mValue = Scalar.mValue.ToList().ToArray();

        transform.HasTranslation = HasTranslation;
        transform.HasRotation = HasRotation;
        transform.HasScalar = HasScalar;

        return transform;
    }
}