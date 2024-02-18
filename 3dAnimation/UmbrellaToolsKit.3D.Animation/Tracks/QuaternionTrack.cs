using System;
using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D.Tracks
{
    internal class QuaternionTrack : Track<Quaternion>
    {
        public override void Resize(int size)
        {
            throw new NotImplementedException();
        }

        public override Quaternion Sample(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float AdjustTimeToFitTrack(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Quaternion Cast(float value)
        {
            throw new NotImplementedException();
        }

        protected override int FrameIndex(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Quaternion Hermite(float time, Quaternion p1, Quaternion s1, Quaternion p2, Quaternion s2)
        {
            throw new NotImplementedException();
        }

        protected override Quaternion SampleConstant(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Quaternion SampleCubic(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Quaternion SampleLinear(float time, bool looping)
        {
            throw new NotImplementedException();
        }
    }
}
