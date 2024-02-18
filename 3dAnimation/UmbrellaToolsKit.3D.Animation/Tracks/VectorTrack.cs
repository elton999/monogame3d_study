using System;
using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D.Tracks
{
    public class VectorTrack : Track<Vector3>
    {
        public override void Resize(int size)
        {
            throw new NotImplementedException();
        }

        public override Vector3 Sample(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float AdjustTimeToFitTrack(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 Cast(float value)
        {
            throw new NotImplementedException();
        }

        protected override int FrameIndex(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 Hermite(float time, Vector3 p1, Vector3 s1, Vector3 p2, Vector3 s2)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 SampleConstant(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 SampleCubic(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override Vector3 SampleLinear(float time, bool looping)
        {
            throw new NotImplementedException();
        }
    }
}
