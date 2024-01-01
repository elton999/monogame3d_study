using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinningInformation.Animation
{
    public class AnimationPlayer
    {
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyFrame;
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;
        SkinningData skinningDatavalue;

        public AnimationPlayer(SkinningData skinningdata)
        {
            if (skinningdata == null)
                throw new ArgumentNullException("skinningData");

            skinningDatavalue = skinningdata;
            boneTransforms = new Matrix[skinningdata.BindPose.Count];
            worldTransforms = new Matrix[skinningdata.BindPose.Count];
            skinTransforms = new Matrix[skinningdata.BindPose.Count];
        }

        public void StartClip(AnimationClip clip)
        {
            if (clip == null)
                throw new ArgumentException("clip");

            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyFrame = 0;

            skinningDatavalue.BindPose.CopyTo(boneTransforms, 0);
        }

        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }

        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if (currentClipValue == null)
                throw new ArgumentNullException("AnimationPlayer.Update was called before StartClip");

            if (relativeToCurrentTime)
            {
                time += currentTimeValue;
                while (time >= currentClipValue.Duration)
                    time -= currentClipValue.Duration;
            }

            if ((time < TimeSpan.Zero) || (time >= currentClipValue.Duration))
                throw new ArgumentOutOfRangeException("time");

            if(time < currentTimeValue)
            {
                currentKeyFrame = 0;
                skinningDatavalue.BindPose.CopyTo(boneTransforms, 0);
            }
            currentTimeValue = time;

            IList<Keyframe> keyframes = currentClipValue.Keyframes;
            while(currentKeyFrame < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyFrame];
                if (keyframe.Time > currentTimeValue)
                    break;

                boneTransforms[keyframe.Bone] = keyframe.Transform;
                currentKeyFrame++;
            }
        }

        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            worldTransforms[0] = boneTransforms[0] * rootTransform;
            for(int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDatavalue.SkeletonHierarchy[bone];
                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }

        public void UpdateSkinTransforms()
        {
            for(int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDatavalue.InverseBindPose[bone] * worldTransforms[bone];
            }
        }

        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }

        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }

        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }

        public AnimationClip CurrentClip => currentClipValue;

        public TimeSpan CurrentTime => currentTimeValue;
    }
}
