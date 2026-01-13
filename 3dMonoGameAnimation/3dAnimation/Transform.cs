using Microsoft.Xna.Framework;

namespace _3dAnimation;

public class Transform
{
    public VectorFrame Translation = new VectorFrame();
    public QuaternionFrame Rotation = new QuaternionFrame();
    public ScalarFrame Scalar = new ScalarFrame();

    public Matrix GetLocalMatrix()
    {
        Vector3 translation = Translation?.GetVector3() ?? Vector3.Zero;
        Vector3 scale = Scalar?.GetVector3() ?? Vector3.One;
        Quaternion rotation = Rotation?.GetQuaternion() ?? Quaternion.Identity;

        rotation.Normalize();

        Matrix scaleMatrix = Matrix.CreateScale(scale);
        Matrix rotationMatrix = Matrix.CreateFromQuaternion(rotation);
        Matrix translationMatrix = Matrix.CreateTranslation(translation);

        return scaleMatrix * rotationMatrix * translationMatrix;
    }
}