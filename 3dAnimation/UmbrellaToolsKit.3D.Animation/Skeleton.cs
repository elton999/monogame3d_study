using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Skeleton
    {
        public Pose mRestPose;
        public Pose mBindPose;
        public List<Matrix> mInvBindPose;
        public List<string> mJointNames;

        public Skeleton() { }

        public Skeleton(Pose rest, Pose bind, List<string> names) => Set(rest, bind, names);

        public void Set(Pose rest, Pose bind, List<string> names)
        {
            mRestPose = rest;
            mBindPose = bind;
            mJointNames = names;
            UpdateInverseBindPose();
        }

        public Pose GetBindPose() => mBindPose;

        public Pose GetRestPose() => mRestPose;

        public List<Matrix> GetInvBindPose() => mInvBindPose;

        public List<string> GetJoinNames() => mJointNames;

        public string GetJoinName(int index) => mJointNames[index];

        protected void UpdateInverseBindPose()
        {
            int size = mBindPose.Size();
            mInvBindPose.Capacity = size;
            mInvBindPose.TrimExcess();

            for (int i = 0; i < size; ++i)
            {
                Transform world = mBindPose.GetGlobalTransform(i);
                mInvBindPose[i] = Matrix.Invert(Transform.TransformToMatrix(world));
            }
        }
    }
}
