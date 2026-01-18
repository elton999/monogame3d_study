using Microsoft.Xna.Framework;

namespace _3dAnimation;

public enum InterpolationType
{
    LINEAR,
    STEP,
    CATMULLROMSPLINE,
    CUBICSPLINE
}

public class Frame
{
    public float[] mValue;
    public InterpolationType Interpolation;
    public float[] InTangent;
    public float[] OutTangent;
}

public class ScalarFrame : Frame
{
    public ScalarFrame()
    {
        mValue = new float[]{ 1f, 1f, 1f};
    }

    public ScalarFrame(Vector3 vector)
    {
        mValue = new float[] { vector.X, vector.Y, vector.Z };
    }

    public Vector3 GetVector3()
    {
        return new Vector3(mValue[0], mValue[1], mValue[2]);
    }
}

public class VectorFrame : Frame
{
    public VectorFrame()
    {
        mValue = new float[] { 0f, 0f, 0f };
    }

    public VectorFrame(Vector3 vector)
    {
        mValue = new float[] { vector.X, vector.Y, vector.Z };
    }

    public Vector3 GetVector3()
    {
        return new Vector3(mValue[0], mValue[1], mValue[2]);
    }
}

public class QuaternionFrame : Frame
{
    public QuaternionFrame()
    {
        mValue = new float[] { 0f, 0f, 0f, 1f };
    }

    public QuaternionFrame(Vector4 vector)
    {
        mValue = new float[] { vector.X, vector.Y, vector.Z, vector.W };
    }

    public Quaternion GetQuaternion()
    {
        return new Quaternion(mValue[0], mValue[1], mValue[2], mValue[3]);
    }
}
