using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Pose
    {
        protected Transform[] mJoints;
        protected int[] mParents;

        public Pose(int numJoint) => Resize(numJoint);

        public void Resize(int numJoint)
        {
            mJoints = new Transform[numJoint];
            mParents = new int[numJoint];
        }

        public int Size() => mJoints.Length;

        public Transform GetLocalTransform(int index) => mJoints[index];

        public void SetLocalTransform(int index, Transform transform) => mJoints[index] = transform;

        public Transform GetGlobalTransform(int i)
        {
            Transform result = mJoints[i];
            for (int p = mParents[i]; p >= 0; p = mParents[p])
                result = Transform.Combine(mJoints[p], result);

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

        public int GetParent(int index) => mParents[index];

        public void SetParent(int index, int parent) => mParents[index] = parent;
    }
}
