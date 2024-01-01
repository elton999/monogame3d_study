namespace UmbrellaToolsKit.Animation3D
{
    public class Frame
    {
        public float[] mValue;
        public float[] mIn;
        public float[] mOut;
        public float mTime;
    }

    public class ScalarFrame : Frame
    {
        public ScalarFrame() 
        {
            mValue = new float[1];
            mIn = new float[1];
            mOut = new float[1];
        }
    }

    public class VectorFrame : Frame
    {
        public VectorFrame()
        {
            mValue = new float[3];
            mIn = new float[3];
            mOut = new float[3];
        }
    }

    public class QuaternionFrame : Frame
    {
        public QuaternionFrame()
        {
            mValue = new float[4];
            mIn = new float[4];
            mOut = new float[4];
        }
    }
}
