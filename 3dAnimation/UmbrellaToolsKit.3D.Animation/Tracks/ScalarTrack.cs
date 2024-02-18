using System;

namespace UmbrellaToolsKit.Animation3D.Tracks
{
    public class ScalarTrack : Track<float>
    {
        public override void Resize(int size)
        {
            throw new NotImplementedException();
        }

        public override float Sample(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float AdjustTimeToFitTrack(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float Cast(float value)
        {
            throw new NotImplementedException();
        }

        protected override int FrameIndex(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float Hermite(float time, float p1, float s1, float p2, float s2)
        {
            throw new NotImplementedException();
        }

        protected override float SampleConstant(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float SampleCubic(float time, bool looping)
        {
            throw new NotImplementedException();
        }

        protected override float SampleLinear(float time, bool looping)
        {
            throw new NotImplementedException();
        }
    }
}
