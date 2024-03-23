using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Skeleton
    {
        public Pose mRestPose;
        public Pose mBindPose;
        public Matrix[] mInvBindPose;
        public List<string> mJointNames;

        public Skeleton() { }

        public Skeleton(Pose rest, Pose bind, List<string> joitNames) => Set(rest, bind, joitNames);

        public void Set(Pose rest, Pose bind, List<string> joitNames)
        {
            mRestPose = rest;
            mBindPose = bind;
            mJointNames= joitNames;
            //UpdateInverseBindPose();
        }

        public Pose GetBindPose() => mBindPose;

        public Pose GetRestPose() => mRestPose;

        public Matrix[] GetInvBindPose() => mInvBindPose;

        public List<string> GetJoinNames() => mJointNames;

        public string GetJoinName(int index) => mJointNames[index];

        public void UpdateInverseBindPose(int[] rm)
        {
            int size = mBindPose.Size();
            mInvBindPose = new Matrix[size];

            for (int i = 0; i < size; i++)
            {
                Transform world = mBindPose.GetGlobalTransform(i);
                mInvBindPose[i] =  Transform.TransformToMatrix(Transform.Inverse(world));
            }
        }
    }
}
