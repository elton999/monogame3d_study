using System.Numerics;
using System.Transactions;

namespace _3dAnimation;

public class Frame
{
    public float[] mValue;
}

public class ScalarFrame : Frame
{
    public ScalarFrame()
    {
        mValue = new float[]{ 1f, 1f, 1f};
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
        mValue = new float[] { 0f, 0f, 0f, 1f };
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
        mValue = new float[4];
    }

    public Quaternion GetQuaternion()
    {
        return new Quaternion(mValue[0], mValue[1], mValue[2], mValue[3]);
    }
}
