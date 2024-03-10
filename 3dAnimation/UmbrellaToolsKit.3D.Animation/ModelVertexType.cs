using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace UmbrellaToolsKit.Animation3D
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ModelVertexType : IVertexType
    {
        public Vector3 Position;

        public Color Color;

        public Vector3 Normal;

        public Vector2 TextureCoordinate;

        public Vector4 Joints;

        public Vector4 Weights;

        public static readonly VertexDeclaration VertexDeclaration;

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public ModelVertexType(Vector3 position, Color color, Vector3 normal, Vector2 textureCoordinate, Vector4 joints, Vector4 weights)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Joints = joints;
            Weights= weights;
        }

        public override int GetHashCode()
        {
            return (((((Position.GetHashCode() * 397) ^ Color.GetHashCode()) * 397) ^ Normal.GetHashCode()) * 397) ^ TextureCoordinate.GetHashCode();
        }

        public override string ToString()
        {
            string[] obj = new string[13] { "{{Position:", null, null, null, null, null, null, null, null, null, null, null, null };
            Vector3 position = Position;
            obj[1] = position.ToString();
            obj[2] = " Color:";
            Color color = Color;
            obj[3] = color.ToString();
            obj[4] = " Normal:";
            position = Normal;
            obj[5] = position.ToString();
            obj[6] = " TextureCoordinate:";
            Vector2 textureCoordinate = TextureCoordinate;
            obj[7] = textureCoordinate.ToString();
            obj[8] = " Joints:";
            Vector4 joints = Joints;
            obj[9] = joints.ToString();
            obj[10] = " Weights:";
            Vector4 weights = Weights;
            obj[11] = weights.ToString();
            obj[12] = "}}";
            return string.Concat(obj);
        }

        public static bool operator ==(ModelVertexType left, ModelVertexType right)
        {
            if (left.Position == right.Position && left.Color == right.Color && left.Normal == right.Normal)
            {
                return left.TextureCoordinate == right.TextureCoordinate;
            }

            return false;
        }

        public static bool operator !=(ModelVertexType left, ModelVertexType right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return this == (ModelVertexType)obj;
        }

        static ModelVertexType()
        {
            VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
                new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), 
                new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
                new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.BlendIndices, 0),
                new VertexElement(52, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0)
            );
        }
    }
}
