using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbrellaToolsKit.Animation3D
{
    public class Clip
    {
        protected List<TransformTrack> mTracks;
        protected string mName;
        protected float mStartTime;
        protected float mEndTime;
        protected bool mLooping;

        public Clip() 
        {
            mName = "No name given";
            mStartTime = 0.0f;
            mEndTime = 0.0f;
            mLooping = true;
        }

        protected float AdjustTimeToFitRange(float inTime)
        {
            if (mLooping)
            {
                float duration = mEndTime - mStartTime;
                inTime = inTime - mStartTime % mEndTime - mStartTime;

                if (inTime < 0.0f)
                {
                    inTime += mEndTime - mStartTime;
                }

                inTime = inTime + mStartTime;
            }
            else
            {
                if (inTime < mStartTime)
                {
                    inTime = mStartTime;
                }
                if (inTime > mEndTime)
                {
                    inTime = mEndTime;
                }
            }

            return inTime;
        }

    }
}
