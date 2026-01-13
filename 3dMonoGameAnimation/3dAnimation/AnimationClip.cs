using System;

namespace _3dAnimation;

public class AnimationClip
{
    private string _name;

    public string Name => _name;
    public float[] FramesTimer = new float[] { };
    public Transform[][] JoinByFrameTransform = new Transform[][] { };
    
    public AnimationClip(string name)
    {
        _name = name;
    }

    public void AddScalar(int frame, int jointId, ScalarFrame scalar)
    {
        ResizeFrameArray(frame, jointId);

        JoinByFrameTransform[frame - 1][jointId].Scalar = scalar;
    }

    public void AddTranslation(int frame, int jointId, VectorFrame translation)
    {
        ResizeFrameArray(frame, jointId);

        JoinByFrameTransform[frame - 1][jointId].Translation = translation;
    }

    public void AddRotation(int frame, int jointId, QuaternionFrame rotation)
    {
        ResizeFrameArray(frame, jointId);

        JoinByFrameTransform[frame - 1][jointId].Rotation = rotation;
    }

    public void AddFrameTimer(int frame, float value)
    {
        if (FramesTimer.Length < frame)
        {
            Array.Resize(ref FramesTimer, frame);
        }

        FramesTimer[frame - 1] = value;
    }

    private void ResizeFrameArray(int frame, int jointId)
    {
        if (JoinByFrameTransform.Length < frame)
        {
            Array.Resize(ref JoinByFrameTransform, frame);
        }

        if (JoinByFrameTransform[frame - 1] == null)
        {
            JoinByFrameTransform[frame - 1] = new Transform[] { };
        }

        if (JoinByFrameTransform[frame - 1].Length < jointId + 1)
        {
            Array.Resize(ref JoinByFrameTransform[frame - 1], jointId + 1);
        }

        if (JoinByFrameTransform[frame - 1][jointId] == null)
        {
            JoinByFrameTransform[frame - 1][jointId] = new Transform();
        }
    }
}
