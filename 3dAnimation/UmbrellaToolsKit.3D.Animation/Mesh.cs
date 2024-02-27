using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D
{
    public class Mesh
    {
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] TexCoords;
        public short[] Indices;
        public Joint[] Joints;

        public Clip[] Clips;
        public Pose[] RestPose;
        public Pose[] CurrentPose;
    }
}
