using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D.Tracks
{
    public class VectorTrack : Track<Vector3>
    {
        public override Frame this[int index]
        {
            get
            {
                if (mFrames[index] == null)
                    mFrames[index] = new VectorFrame();
                return mFrames[index];
            }
        }

        protected override Vector3 Cast(float[] value) => new Vector3(value[0], value[1], value[2]);

        protected override Vector3 Hermite(float t, Vector3 p1, Vector3 s1, Vector3 _p2, Vector3 s2)
        {
            float tt = t * t;
            float ttt = tt * t;

            Vector3 p2 = _p2;
            TrackHelpers.Neighborhood(p1, p2);

            float h1 = 2.0f * ttt - 3.0f * tt + 1.0f;
            float h2 = -2.0f * ttt + 3.0f * tt;
            float h3 = ttt - 2.0f * tt + t;
            float h4 = ttt - tt;

            Vector3 result = p1 * h1 + p2 * h2 + s1 * h3 + s2 * h4;
            return TrackHelpers.AdjustHermiteResult(result);
        }

        protected override Vector3 SampleCubic(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return Vector3.Zero;
            
            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return Vector3.Zero;
            
            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            Vector3 point1 = Cast(mFrames[thisFrame].mValue);
            Vector3 slope1=  Cast(mFrames[thisFrame].mOut) * frameDelta;
            slope1 = slope1 * frameDelta;

            Vector3 point2 = Cast(mFrames[nextFrame].mValue);
            Vector3 slope2 = Cast(mFrames[nextFrame].mIn) * frameDelta;
            slope2 = slope2 * frameDelta;

            return Hermite(t, point1, slope1, point2, slope2);
        }

        protected override Vector3 SampleLinear(float time, bool looping)
        {
            int thisFrame = FrameIndex(time, looping);
            if (thisFrame < 0 || thisFrame >= (int)(mFrames.Length - 1))
                return Vector3.Zero;

            int nextFrame = thisFrame + 1;

            float trackTime = AdjustTimeToFitTrack(time, looping);
            float frameDelta = mFrames[nextFrame].mTime - mFrames[thisFrame].mTime;
            if (frameDelta <= 0.0f)
                return Vector3.Zero;

            float t = (trackTime - mFrames[thisFrame].mTime) / frameDelta;

            Vector3 start = Cast(mFrames[thisFrame].mValue);
            Vector3 end = Cast(mFrames[nextFrame].mValue);

            return TrackHelpers.Interpolate(start, end, t);
        }
    }
}
