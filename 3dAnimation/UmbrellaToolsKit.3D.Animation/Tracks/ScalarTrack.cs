namespace UmbrellaToolsKit.Animation3D.Tracks
{
    public class ScalarTrack : Track<float>
    {
        public override Frame this[int index]
        {
            get
            {
                if (mFrames[index] == null)
                    mFrames[index] = new ScalarFrame();
                return mFrames[index];
            }
        }

        protected override float Cast(float[] value) => value[0];

        protected override float Hermite(float t, float p1, float s1, float _p2, float s2)
        {
            float tt = t * t;
            float ttt = tt * t;

            float p2 = _p2;
            TrackHelpers.Neighborhood(p1, p2);

            float h1 = 2.0f * ttt - 3.0f * tt + 1.0f;
            float h2 = -2.0f * ttt + 3.0f * tt;
            float h3 = ttt - 2.0f * tt + t;
            float h4 = ttt - tt;

            float result = p1 * h1 + p2 * h2 + s1 * h3 + s2 * h4;
            return TrackHelpers.AdjustHermiteResult(result);
        }

        protected override float SampleCubic(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return 0.0f;

            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return 0.0f;

            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            float point1 = Cast(mFrames[thisFrame].mValue);
            float slope1 = mFrames[thisFrame].mOut[0] * frameDelta;
            slope1 = slope1 * frameDelta;

            float point2 = Cast(mFrames[nextFrame].mValue);
            float slope2 = mFrames[nextFrame].mIn[0] * frameDelta;
            slope2 = slope2 * frameDelta;

            return Hermite(t, point1, slope1, point2, slope2);
        }

        protected override float SampleLinear(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return 0.0f;

            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return 0.0f;

            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            float start = Cast(mFrames[thisFrame].mValue);
            float end = Cast(mFrames[nextFrame].mValue);

            return TrackHelpers.Interpolate(start, end, t);
        }
    }
}
