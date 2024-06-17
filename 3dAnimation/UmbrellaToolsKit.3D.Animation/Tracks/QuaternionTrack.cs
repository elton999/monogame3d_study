using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D.Tracks
{
    public class QuaternionTrack : Track<Quaternion>
    {
        public override Frame this[int index]
        {
            get
            {
                if (mFrames[index] == null)
                    mFrames[index] = new QuaternionFrame();
                return mFrames[index];
            }
        }

        protected override Quaternion Cast(float[] value)
        {
            Quaternion q = new Quaternion(value[0], value[1], value[2], value[3]);
            return Quaternion.Normalize(q);
        }

        protected override Quaternion Hermite(float t, Quaternion p1, Quaternion s1, Quaternion _p2, Quaternion s2)
        {
            float tt = t * t;
            float ttt = tt * t;

            Quaternion p2 = _p2;
            TrackHelpers.Neighborhood(p1, ref p2);

            float h1 = 2.0f * ttt - 3.0f * tt + 1.0f;
            float h2 = -2.0f * ttt + 3.0f * tt;
            float h3 = ttt - 2.0f * tt + t;
            float h4 = ttt - tt;

            Quaternion result = p1 * h1 + p2 * h2 + s1 * h3 + s2 * h4;
            return TrackHelpers.AdjustHermiteResult(result);
        }

        protected override Quaternion SampleCubic(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return new Quaternion();

            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return new Quaternion();

            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            Quaternion point1 = Cast(mFrames[thisFrame].mValue);
            Quaternion slope1 = Cast(mFrames[thisFrame].mOut) * frameDelta;
            slope1 = slope1 * frameDelta;

            Quaternion point2 = Cast(mFrames[nextFrame].mValue);
            Quaternion slope2 = Cast(mFrames[nextFrame].mIn) * frameDelta;
            slope2 = slope2 * frameDelta;

            return Hermite(t, point1, slope1, point2, slope2);
        }

        protected override Quaternion SampleLinear(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return new Quaternion();

            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return new Quaternion();

            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            Quaternion start = Cast(mFrames[thisFrame].mValue);
            Quaternion end = Cast(mFrames[nextFrame].mValue);

            return TrackHelpers.Interpolate(start, end, t);
        }
    }
}
