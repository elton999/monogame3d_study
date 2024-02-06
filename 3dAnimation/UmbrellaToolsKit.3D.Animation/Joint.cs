using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Joint
    {
        public string Name;
        public Transform Transform;
        public List<Joint> Parent;

        public Joint(string name, Transform transform) 
        {
            Name = name;
            Transform = transform;
            Parent = new List<Joint>();
        }

        public Joint() 
        {
            Parent = new List<Joint>();
        }
    }
}
