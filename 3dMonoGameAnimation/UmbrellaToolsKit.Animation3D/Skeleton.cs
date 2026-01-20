using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D;

public class Skeleton : IFormattable
{
    private Joint[] _joints = Array.Empty<Joint>();
    private AnimationClip[] _animationClips = Array.Empty<AnimationClip>();

    public Joint[] Joints => _joints;
    public Matrix[] SkinMatrices;
    public AnimationClip[] AnimationClips => _animationClips;
    public Matrix[] InverseBindMatrix;

    public Skeleton() { }

    public Skeleton(Joint[] joints)
    {
        _joints = joints;
        SkinMatrices = new Matrix[joints.Length];
    }

    public void ComputeBindPose()
    {
        for (int jointIndex = 0;  jointIndex < _joints.Length; jointIndex++)
        {
           var joint = _joints[jointIndex];
           joint.BindPose = Matrix.Invert(InverseBindMatrix[jointIndex]);
           joint.InverseBindPose = InverseBindMatrix[jointIndex];
        }
    }

    public void AddAnimationClip(AnimationClip animationClip)
    {
        var list = _animationClips.ToList();
        list.Add(animationClip);
        _animationClips = list.ToArray();
    }


    private void SetString(Joint root, string prefix, ref string result)
    {
        foreach (Joint joint in root.Children)
        {
            result += $"{prefix}{joint.Name} {joint.WorldMatrix.Translation}\n";
            SetString(joint, prefix + "-", ref result);
        }
    }

    public Vector3 GetJointPosition(Joint joint, Matrix worldScale)
    {
        Vector3 pos = Vector3.Transform(Vector3.Zero, joint.WorldMatrix);
        return Vector3.Transform(pos, worldScale);
    }

    public override string ToString()
    {
        var roots = _joints.Where(x => !x.HasParent).ToList();

        if (roots.Count == 0)
            return "no joints";

        var root = roots[0];
        string result = $"{root.Name} {root.WorldMatrix.Translation}\n";
        SetString(root, "-", ref result);
        return result;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return ToString();
    }
}