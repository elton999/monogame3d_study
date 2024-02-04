using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Joint
    {
        public string Name;
        public Vector3 Position;
        public List<Joint> Children;

        public Joint(string name, Vector3 position) 
        {
            Name = name;
            Position = position;
            Children = new List<Joint>();
        }
    }
}
