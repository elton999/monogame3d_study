﻿using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D
{
    public class Mesh
    {
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] TexCoords;
        public short[] Indices;
        public Vector4[] Weights;
        public Vector4[] Joints;

        public Clip[] Clips;
        public Matrix[] InverseBindMatrix;
        public int[] JointsIndexs;
        public Skeleton Skeleton;

        public static int GetNodeIndex(int target, int[] allNodes, int numNodes)
        {
            for (int i = 0; i < numNodes; ++i)
            {
                if (target == allNodes[i])
                {
                    return (int)i;
                }
            }
            return -1;
        }
    }
}
