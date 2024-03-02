using UmbrellaToolsKit.Animation3D.Tracks;

namespace UmbrellaToolsKit.Animation3D
{
    public class TransformTrack
    {
        public int mId;
        public VectorTrack mPosition;
        public QuaternionTrack mRotation;
        public VectorTrack mScale;

        public TransformTrack()
        {
            mPosition = new VectorTrack();
            mRotation = new QuaternionTrack();
            mScale = new VectorTrack();
        }

        public int GetID() => mId;
        
        public  void SetID(int id) => mId= id;
        
        public VectorTrack GetPosition() => mPosition;
        
        public QuaternionTrack GetRotation() => mRotation;

        public VectorTrack GetScale() => mScale;
        
        public float GetStartTime() 
        {
            float result = 0.0f;
            bool isSet = false;

            if (mPosition.Size() > 1)
            {
                result = mPosition.GetStartTime();
                isSet = true;
            }

            if (mRotation.Size() > 1)
            {
                float rotationStart = mRotation.GetStartTime();
                if (rotationStart < result || !isSet)
                {
                    result = rotationStart;
                    isSet = true;
                }
            }

            if (mScale.Size() > 1)
            {
                float scaleStart = mScale.GetStartTime();
                if (scaleStart < result || !isSet)
                {
                    result = scaleStart;
                    isSet = true;
                }
            }

            return result;
        }

        public float GetEndTime() 
        {
            float result = 0.0f;
            bool isSet = false;

            if (mPosition.Size() > 1)
            {
                result = mPosition.GetEndTime();
                isSet = true;
            }

            if (mRotation.Size() > 1)
            {
                float rotationEnd = mRotation.GetEndTime();
                if (rotationEnd > result || !isSet)
                {
                    result = rotationEnd;
                    isSet = true;
                }
            }

            if (mScale.Size() > 1)
            {
                float scaleEnd = mScale.GetEndTime();
                if (scaleEnd > result || !isSet)
                {
                    result = scaleEnd;
                    isSet = true;
                }
            }

            return result;
        }

        public bool IsValid() => mPosition.Size() > 1 || mRotation.Size() > 1 || mScale.Size() > 1;
        
        public Transform Sample(Transform refi, float time, bool loop) 
        {
            Transform result = refi;
            if (mPosition.Size() > 1)
                result.Rotation = mRotation.Sample(time, loop);

            if (mScale.Size() > 1)
                result.Scale = mScale.Sample(time, loop);

            return result;
        }

    }
}
