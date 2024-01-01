using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Mesh
    {
        public  List<Vector3> Vertices;
        public List<int> Indices;
        
        public Mesh()
        {
            Vertices = new List<Vector3>();
            Indices = new List<int>();
        }
    }
}
