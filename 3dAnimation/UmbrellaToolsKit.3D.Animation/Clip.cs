using System.Collections.Generic;
using System.Linq;

namespace UmbrellaToolsKit.Animation3D
{
    public class Clip
    {
        public List<TransformTrack> mTracks;
        public string mName;
        public float mStartTime;
        public float mEndTime;
        public bool mLooping = true;

        public Clip() 
        {
            mName = "No name given";
            mStartTime = 0.0f;
            mEndTime = 0.0f;
            mLooping = true;
            mTracks = new List<TransformTrack>();
        }

        public float AdjustTimeToFitRange(float inTime)
        {
            if (mLooping)
            {
                float duration = mEndTime - mStartTime;
                if (duration <= 0)
                {
                    return 0.0f;
                }
                inTime = (inTime - mStartTime) % (mEndTime - mStartTime);
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

        public float Sample(Pose pose, float time)
        {
            if (GetDuration() == 0.0f)
                return 0.0f;

            time = AdjustTimeToFitRange(time);
            int size = mTracks.Count();
            for (int i = 0; i < size; ++i)
            {
                int j = mTracks[i].GetID();
                Transform local = pose.GetLocalTransform(j);
                Transform animated = mTracks[i].Sample(local, time, mLooping);
                pose.SetLocalTransform(j, animated);
            }

            return time;
        }

        public void RecalculateDuration()
        {
            mStartTime = 0.0f;
            mEndTime = 0.0f;
            bool startSet = false;
            bool endSet = false;
            int tracksSize = mTracks.Count();
            for (int i = 0; i < tracksSize; ++i)
            {
                if (mTracks[i].IsValid())
                {
                    float startTime = mTracks[i].GetStartTime();
                    float endTime = mTracks[i].GetEndTime();

                    if (startTime < mStartTime || !startSet)
                    {
                        mStartTime = startTime;
                        startSet = true;
                    }

                    if (endTime > mEndTime || !endSet)
                    {
                        mEndTime = endTime;
                        endSet = true;
                    }
                }
            }
        }

        public TransformTrack this[int joint]
        {
            get
            {
                for (int i = 0, s = mTracks.Count(); i < s; ++i)
                {
                    if (mTracks[i].GetID() == joint)
                    {
                        return mTracks[i];
                    }
                }

                mTracks.Add(new TransformTrack());
                mTracks[mTracks.Count() - 1].SetID(joint);

                return mTracks[mTracks.Count() - 1];
            }
        }

        public string GetName() =>  mName;

        public int GetIdAtIndex(int index) =>  mTracks[index].GetID();

        public int Size() => (int)mTracks.Count();

        public float GetDuration() =>  mEndTime - mStartTime;

        public float GetStartTime() => mStartTime;

        public float GetEndTime() =>  mEndTime;

        public bool GetLooping() =>  mLooping;

        public void SetName(string inNewName) =>  mName = inNewName;

        public void SetIdAtIndex(int index, int id) =>  mTracks[index].SetID(id);

        public void SetLooping(bool inLooping) => mLooping = inLooping;

    }
}
