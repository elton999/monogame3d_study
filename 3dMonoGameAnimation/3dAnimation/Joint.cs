using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _3dAnimation;
public class Joint
{
    private Joint _parent;
    private string _name;
    private Transform _transform = new Transform();
        
    public bool HasParent => _parent != null;
    public Joint Parent => _parent;
    public string Name => _name;
    public Transform JointTransform => _transform;
    public Matrix InversePose;

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
        WorldMatrix = LocalMatrix * parentWorld;

        foreach (var child in Children)
            child.UpdateWorld(WorldMatrix);
    }

    public void ComputeBindPose()
    {
        BindPose = WorldMatrix;
        InversePose = Matrix.Invert(BindPose);
    }

    public void SetTransform(Transform transform)
    {
        _transform = transform;
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
