using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
            Transform result = mJoints[index];
            //for (int p = mParents[i]; p < mParents.Length; p++)
            //    result = Transform.Combine(mJoints[p], result);

            int parentIndex = GetParent(index);
            List<int> lastIndex = new List<int>();
            lastIndex.Add(-1);

            while (parentIndex != -1 && parentIndex != index)
            {
                System.Console.WriteLine(parentIndex);
                result = Transform.Combine(mJoints[parentIndex], result);
                lastIndex.Add(parentIndex);
                parentIndex = GetParent(parentIndex);
            }

            return result;
        }

        public Transform this[int index] => GetGlobalTransform(index);

        public void GetMatrixPalette(List<Matrix> r)
        {
            int size = Size();

            if (r.Count != size)
	        {
                r.Capacity = size;
                r.TrimExcess();
            }

            for (int i = 0; i < size; ++i)
            {
                Transform t = GetGlobalTransform(i);
		        r[i] = Transform.TransformToMatrix(t);
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
