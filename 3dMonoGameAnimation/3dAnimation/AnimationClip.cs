using System;
using System.Collections.Generic;

namespace _3dAnimation;

public class AnimationClip
{
    private readonly string _name;
    public string Name => _name;

    public float[] FramesTimer = Array.Empty<float>();

    public Dictionary<int, Transform>[] JoinByFrameTransform
        = Array.Empty<Dictionary<int, Transform>>();

    public AnimationClip(string name)
    {
        _name = name;
    }

    public void AddTranslation(int frame, int jointId, VectorFrame translation)
    {
        var t = GetOrCreateTransform(frame, jointId);
        t.Translation = translation;
        t.HasTranslation = true;
    }

    public void AddRotation(int frame, int jointId, QuaternionFrame rotation)
    {
        var t = GetOrCreateTransform(frame, jointId);
        t.Rotation = rotation;
        t.HasRotation = true;
    }

    public void AddScalar(int frame, int jointId, ScalarFrame scalar)
    {
        var t = GetOrCreateTransform(frame, jointId);
        t.Scalar = scalar;
        t.HasScalar = true;
    }

    public void AddFrameTimer(int frame, float value)
    {
        EnsureFrameIndex(ref FramesTimer, frame);
        FramesTimer[frame] = value;
    }

    private Transform GetOrCreateTransform(int frame, int jointId)
    {
        EnsureFrameIndex(ref JoinByFrameTransform, frame);

        var frameDict = JoinByFrameTransform[frame];
        if (frameDict == null)
        {
            frameDict = new Dictionary<int, Transform>();
            JoinByFrameTransform[frame] = frameDict;
        }

        if (!frameDict.TryGetValue(jointId, out var transform))
        {
            transform = new Transform();
            frameDict[jointId] = transform;
        }

        return transform;
    }

    private static void EnsureFrameIndex<T>(ref T[] array, int frame)
    {
        if (array.Length <= frame)
        {
            Array.Resize(ref array, frame + 1);
        }
    }
}