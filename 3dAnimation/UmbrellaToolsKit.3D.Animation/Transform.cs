using Microsoft.Xna.Framework;
using System;

namespace UmbrellaToolsKit.Animation3D
{
    public class Transform
    {
        public Vector3 Position;
        public Quaternion Rotation; 
        public Vector3 Scale;

        public Transform() { }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public static Transform Combine(Transform a, Transform b)
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
            float scalar = q.W;

            return qVector * 2.0f * Vector3.Dot(qVector, v) +
            v * (MathF.Pow(scalar, 2) - Vector3.Dot(qVector, qVector)) +
            Vector3.Cross(qVector, v) * 2.0f * scalar;
        }

        public static Matrix TransformToMatrix(Transform t)
        {
            Vector3 x = QuatMultVector(t.Rotation, Vector3.UnitX);
            Vector3 y = QuatMultVector(t.Rotation, Vector3.UnitY);
            Vector3 z = QuatMultVector(t.Rotation, Vector3.UnitZ);

            x = x * t.Scale.X;
            y = y * t.Scale.Y;
            z = z * t.Scale.Z;

            Vector3 p = t.Position;

            return new Matrix(
                x.X, x.Y, x.Z, 0, // x basis (& Scale)
                y.X, y.Y, y.Z, 0, // Y basis (& Scale)
                z.X, z.Y, z.Z, 0, // Z basis (& Scale)
                p.X, p.Y, p.Z, 1 // Position
            );
        }
    }
}
