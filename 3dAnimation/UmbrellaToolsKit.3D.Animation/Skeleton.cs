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
            UpdateInverseBindPose();
        }

        public Pose GetBindPose() => mBindPose;

        public Pose GetRestPose() => mRestPose;

        public Matrix[] GetInvBindPose() => mInvBindPose;

        public List<string> GetJoinNames() => mJointNames;

        public string GetJoinName(int index) => mJointNames[index];

        public void UpdateInverseBindPose()
        {
            int size = mBindPose.Size();
            mInvBindPose = new Matrix[size];

            for (int i = 0; i < size; i++)
            {
                Transform world = mBindPose.GetGlobalTransform(i);
                mInvBindPose[i] = Matrix.Invert(Transform.TransformToMatrix(world));
            }
        }

        public static Matrix Inverse(Matrix m)
        {
            float n11 = m.M11, n12 = m.M12, n13 = m.M13, n14 = m.M14;
            float n21 = m.M12, n22 = m.M22, n23 = m.M23, n24 = m.M24;
            float n31 = m.M13, n32 = m.M32, n33 = m.M33, n34 = m.M34;
            float n41 = m.M14, n42 = m.M42, n43 = m.M43, n44 = m.M44;

            float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
            float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
            float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
            float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

            float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
            float idet = 1.0f / det;

            Matrix ret;

            ret.M11 = t11 * idet;
            ret.M21 = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
            ret.M31 = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
            ret.M41 = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

            ret.M12 = t12 * idet;
            ret.M22 = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
            ret.M32 = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
            ret.M42 = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

            ret.M13 = t13 * idet;
            ret.M23 = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
            ret.M33 = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
            ret.M43 = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

            ret.M14 = t14 * idet;
            ret.M24 = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
            ret.M34 = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
            ret.M44 = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

            return ret;
        }
    }
}
