using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D;

public class Joint
{
    private Joint _parent;
    private string _name;
    private Transform _transform = new Transform();
    private Transform _bindTransform = new Transform();
        
    public bool HasParent => _parent != null;
    public Joint Parent => _parent;
    public string Name => _name;
    public Transform JointTransform => _transform;
    public Transform BindTransform => _bindTransform;
    public Matrix InverseBindPose;

    public List<Joint> Children = new();

    public Matrix LocalMatrix;
    public Matrix WorldMatrix;
    public Matrix BindPose;

    public Joint() { }

    public Joint(string name)
    { 
        _name = name;
    }

    public void UpdateWorld(Matrix parentWorld)
    {
        LocalMatrix = JointTransform.GetLocalMatrix();
        // Apply local transform THEN parent transform
        WorldMatrix = LocalMatrix * parentWorld;

        foreach (var child in Children)
            child.UpdateWorld(WorldMatrix);
    }

    public void ComputeBindPose()
    {
        BindPose = WorldMatrix;
        InverseBindPose = Matrix.Invert(BindPose);
    }

    public void SetTransform(Transform transform)
    {
        _transform = transform;
        _bindTransform = transform.GetCopy();
        _bindTransform.HasTranslation = true;
        _bindTransform.HasRotation = true;
        _bindTransform.HasScalar = true;
    }

    public void ApplyTransformAnimation(Transform anim)
    {
        if (anim.HasTranslation)
        {
            var t = anim.Translation.GetVector3();
            _transform.Translation.mValue = new float[] { t.X, t.Y, t.Z };
            _transform.HasTranslation = true;
        }

        if (anim.HasRotation)
        {
            var q = Quaternion.Normalize(anim.Rotation.GetQuaternion());
            _transform.Rotation.mValue = new float[] { q.X, q.Y, q.Z, q.W };
            _transform.HasRotation = true;
        }

        if (anim.HasScalar)
        {
            var s = anim.Scalar.GetVector3();
            _transform.Scalar.mValue = new float[] { s.X, s.Y, s.Z };
            _transform.HasScalar = true;
        }
    }

    public void ResetToBindPose()
    {
        _transform = _bindTransform.GetCopy();

        _transform.HasTranslation = true;
        _transform.HasRotation = true;
        _transform.HasScalar = true;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetParent(Joint parent)
    {
        parent.Children.Add(this);
        _parent = parent;
    }

}
