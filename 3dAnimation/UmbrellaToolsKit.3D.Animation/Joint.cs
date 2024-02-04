using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Joint
    {
        public string Name;
        public Transform Transform;
        public List<Joint> Parents;

        public Joint(string name, Transform transform) 
        {
            Name = name;
            Transform = transform;
            Parents = new List<Joint>();
        }

        public Joint() 
        {
            Parents = new List<Joint>();
        }
    }
}
