using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public abstract class Track<T>
    {
        protected List<Frame> mFrames;
        protected Interpolation mInterpolation;

        public List<Frame> Frame => mFrames;


        public abstract void Resize(int size);
        public int Size() => mFrames.Count;
        public Interpolation GetInterpolation() =>  mInterpolation;
        public void SetInterpolation(Interpolation interpolation) => mInterpolation = interpolation;
        public float GetStartTime() => mFrames.Count;
        public float GetEndTime() =>  mFrames.Count;
        public abstract T Sample(float time, bool looping);

        protected abstract T SampleConstant(float time, bool looping);
        protected abstract T SampleLinear(float time, bool looping);
        protected abstract T SampleCubic(float time, bool looping);
        protected abstract T Hermite(float time, T p1, T s1, T p2, T s2);
        protected abstract int FrameIndex(float time, bool looping);
        protected abstract float AdjustTimeToFitTrack(float time, bool looping);
        protected abstract T Cast(float value);
    }
}
