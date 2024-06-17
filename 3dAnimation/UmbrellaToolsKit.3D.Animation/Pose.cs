using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UmbrellaToolsKit.Animation3D
{
    public class Pose
    {
        public Transform[] mJoints;
        public int[] mParents;

        public Pose() { }

        public Pose(int numJoint) => Resize(numJoint);

        public void Resize(int numJoint)
        {
            mJoints = new Transform[numJoint];
            mParents = new int[numJoint];
        }

        public int Size() => mJoints != null ? mJoints.Length : 0;

        public Transform GetLocalTransform(int index) => mJoints[index];

        public void SetLocalTransform(int index, Transform transform) => mJoints[index] = transform;

        public Transform GetGlobalTransform(int index)
        {
            if(index < 0)
                return new Transform();
            Transform result = mJoints[index];
            int parentIndex = GetParent(index);
            int i = 0;
            while (parentIndex != -1 && i < 100)
            {
                result = Transform.Combine(mJoints[parentIndex], result);
                parentIndex = GetParent(parentIndex);
                i++;
            }

            return result;
        }

        public Transform this[int index] => GetGlobalTransform(index);

        public void GetMatrixPalette(ref Matrix[] result, int[] rm)
        {
            int size = Size();

            if (result.Length != size)
	        {
		        result = new Matrix[size];
            }

            for (int i = 0; i < size; i++)
            {
                if(i < size)
                {
                    Transform t = GetGlobalTransform(i);
		            result[i] =  Transform.TransformToMatrix(t);
                }
            }

            Matrix[] rClone = (new List<Matrix>(result)).ToArray();
            for(int i =0; i < rm.Length; i++)
            {
                result[i] = rClone[rm[i]];
            }
        }

        public int GetParent(int index) 
        {
            if (index == mParents.Length - 1)
                return -1;
            if(index >= mParents.Length)
                return -1;
            return mParents[index];
        }

        public void SetParent(int index, int parent) => mParents[index] = parent;
    }
}
