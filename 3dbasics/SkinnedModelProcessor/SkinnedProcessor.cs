using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.Generic;
using SkinningInformation.Animation;
using System;

namespace SkinnedModelProcessor
{
    [ContentProcessor(DisplayName = "UmbrellaToolsKit - SkinnedModelProcessor")]
    public class SkinnedProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ValidateMesh(input, context, null);

            BoneContent skeleton = MeshHelper.FindSkeleton(input);
            if (skeleton == null)
                throw new InvalidContentException("Input Skeleton not found.");

            FlattenTransforms(input, skeleton);
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);
            if(bones.Count > SkinnedEffect.MaxBones)
            {
                throw new InvalidContentException(string.Format("skeleton has {0} bones, but the maximum supported is {1}.", bones.Count, SkinnedEffect.MaxBones));
            }
            List<Matrix> bindPose = new List<Matrix>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();
            foreach(BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }

            Dictionary<string, AnimationClip> animationClips = ProcessAnimations(skeleton.Animations, bones);

            ModelContent model = base.Process(input, context);
            model.Tag = new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy);
            return model;
        }

        static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach(NodeContent child in node.Children)
            {
                if (child == skeleton)
                    continue;
                MeshHelper.TransformScene(child, child.Transform);
                child.Transform = Matrix.Identity;
                FlattenTransforms(child, skeleton);
            }
        }

        static Dictionary<string, AnimationClip> ProcessAnimations(AnimationContentDictionary animations, IList<BoneContent> bones)
        {
            Dictionary<string, int> boneMap = new Dictionary<string, int>();
            for(int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;
                if(!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();
            foreach(KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);
                animationClips.Add(animation.Key, processed);
            }

            if(animationClips.Count == 0)
                throw new InvalidContentException("Input file does not contain any animations");
            
            return animationClips;
        }

        static AnimationClip ProcessAnimation(AnimationContent animation, Dictionary<string, int> boneMap)
        {
            List<Keyframe> keyframes = new List<Keyframe>();
            foreach(KeyValuePair<string, AnimationChannel> channel in animation.Channels)
            {
                int boneIndex;
                if(!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    throw new InvalidContentException(string.Format("Found animation for bone '{0}', which is not part of the skeleton.", channel.Key));
                }

                foreach(AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex, keyframe.Time, keyframe.Transform));
                }
            }

            keyframes.Sort(CompareKeyFrameTimes);
            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has no keyframes.");

            return new AnimationClip(animation.Duration, keyframes);
        }

        static int CompareKeyFrameTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }

        static void ValidateMesh(NodeContent node, ContentProcessorContext context, string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;
            if (mesh != null)
            {
                if(parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null, "Mesh {0] is a child of bone {1}. SkinnedModelProcessor does't correctly handle messhes that are children of bones", mesh.Name, parentBoneName);
                }

                if(!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null, "Mesh {0] has no skinning info, so it has been deleted.", mesh.Name);
                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
                else if(node is BoneContent)
                {
                    parentBoneName = node.Name;
                }

                foreach(NodeContent child in new List<NodeContent>(node.Children))
                    ValidateMesh(child, context, parentBoneName);
            }
        }

        static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach(GeometryContent geometry in mesh.Geometry)
            {
                if(!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }
            return true;
        }
    }
}