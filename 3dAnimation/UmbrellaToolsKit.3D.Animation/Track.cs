using System;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public abstract class Track<T>
    {
        protected List<Frame> mFrames;
        protected Interpolation mInterpolation;

        public List<Frame> Frame => mFrames;

        public Track() => mInterpolation = Interpolation.Linear;

        public Frame this[int index] => mFrames[index];

        public void Resize(int size)
        {
            mFrames.Capacity = size;
            mFrames.TrimExcess();
        }

        public int Size() => mFrames.Count;

        public Interpolation GetInterpolation() =>  mInterpolation;
        
        public void SetInterpolation(Interpolation interpolation) => mInterpolation = interpolation;
        
        public float GetStartTime() => mFrames[0].mTime;
        
        public float GetEndTime() => mFrames[mFrames.Count - 1].mTime;
        
        public T Sample(float time, bool looping)
        {
            if (mInterpolation == Interpolation.Constant)
                return SampleConstant(time, looping);
            else if (mInterpolation == Interpolation.Linear)
                return SampleLinear(time, looping);
            return SampleCubic(time, looping);
        }

        protected T SampleConstant(float time, bool looping)
        {
            int frame = FrameIndex(time, looping);
            if (frame < 0 || frame >= (int)mFrames.Count)
                return (T)Convert.ChangeType(null, typeof(T));

            return Cast(mFrames[frame].mValue);
        }

        protected float AdjustTimeToFitTrack(float time, bool looping)
        {
            int size = (int)mFrames.Count;
            if (size <= 1) { return 0.0f; }

            float startTime = mFrames[0].mTime;
            float endTime = mFrames[size - 1].mTime;
            float duration = endTime - startTime;
            if (duration <= 0.0f) { return 0.0f; }
            if (looping)
            {
                time = time - startTime % endTime - startTime;
                if (time < 0.0f)
                {
                    time += endTime - startTime;
                }
                time = time + startTime;
            }
            else
            {
                if (time <= mFrames[0].mTime) { time = startTime; }
                if (time >= mFrames[size - 1].mTime) { time = endTime; }
            }

            return time;
        }

        protected int FrameIndex(float time, bool looping)
        {
            int size = mFrames.Count;
            if (size <= 1) return -1;

            if (looping)
            {
                float startTime = mFrames[0].mTime;
                float endTime = mFrames[size - 1].mTime;
                float duration = endTime - startTime;

                time = time - startTime % endTime - startTime;
                if (time < 0.0f)
                {
                    time += endTime - startTime;
                }
                time = time + startTime;
            }
            else
            {
                if (time <= mFrames[0].mTime)
                    return 0;

                if (time >= mFrames[size - 2].mTime)
                    return (int)size - 2;
            }

            for (int i = (int)size - 1; i >= 0; --i)
                if (time >= mFrames[i].mTime)
                    return i;
            // Invalid code, we should not reach here!
            return -1;
        }

        protected abstract T SampleLinear(float time, bool looping);
        protected abstract T SampleCubic(float time, bool looping);
        protected abstract T Hermite(float time, T p1, T s1, T p2, T s2);
        protected abstract T Cast(float[] value);
    }
}
