using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D
{
    public class Transform
    {
        public Vector3 Position;
        public Quaternion Rotation; 
        public Vector3 Scale;

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Transform() { }

        public static Transform Conbine(Transform a, Transform b)
        {
            Transform result = new Transform();

            result.Scale = a.Scale * b.Scale;
            result.Rotation = b.Rotation * a.Rotation;

            result.Position = QuatMultVector(a.Rotation , (a.Scale * b.Position));
            result.Position = a.Position + result.Position;

            return result;
        }

        public static Vector3 QuatMultVector(Quaternion q, Vector3 v)
        {
            Vector3 qVector = new Vector3(q.X, q.Y, q.Z);

            return qVector * 2.0f * Vector3.Dot(qVector, v) +
            v * (q.W * q.W - Vector3.Dot(qVector, qVector)) +
            Vector3.Cross(qVector, v) * 2.0f * q.W;
        }
    }
}
