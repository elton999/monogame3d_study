using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace _3dAnimation
{
    public class Skeleton : IFormattable
    {
        private Joint[] _joints = Array.Empty<Joint>();
        private AnimationClip[] _animationClips = Array.Empty<AnimationClip>();

        public Joint[] Joints => _joints;
        public Matrix[] SkinMatrices;
        public AnimationClip[] AnimationClips => _animationClips;

        public string _currentAnimation = "Idle";
        public int currentFrame = 0;

        public Skeleton() { }

        public Skeleton(Joint[] joints)
        {
            _joints = joints;
            SkinMatrices = new Matrix[joints.Length];
        }

        public void ComputeBindPose()
        {
            foreach (var joint in _joints)
                joint.ResetToBindPose();

            foreach (var root in _joints.Where(j => !j.HasParent))
            {
                root.UpdateWorld(Matrix.Identity);
                ComputeBindPoseRecursive(root);
            }
        }

        private void ComputeBindPoseRecursive(Joint joint)
        {
            joint.ComputeBindPose();

            foreach (var child in joint.Children)
                ComputeBindPoseRecursive(child);
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(_currentAnimation))
            {
                for (int i = 0; i < SkinMatrices.Length; i++)
                    SkinMatrices[i] = Matrix.Identity;

                return;
            }

            var clip = AnimationClips.First(c => c.Name == _currentAnimation);

            if (currentFrame >= clip.JoinByFrameTransform.Length)
                currentFrame = 0;

            foreach (var joint in _joints)
                joint.ResetToBindPose();

            var frame = clip.JoinByFrameTransform[currentFrame];

            for (int i = 0; i < _joints.Length; i++)
            {
                if (frame != null && frame.TryGetValue(i, out var anim))
                {
                    _joints[i].ApplyTransformAnimation(anim);
                }
            }
            currentFrame++;

            foreach (var joint in _joints)
            {
                if (!joint.HasParent)
                    joint.UpdateWorld(Matrix.Identity);
            }

            for (int i = 0; i < _joints.Length; i++)
            {
                SkinMatrices[i] = _joints[i].WorldMatrix * _joints[i].InversePose;
            }
        }

        public void AddAnimationClip(AnimationClip animationClip)
        {
            var list = _animationClips.ToList();
            list.Add(animationClip);
            _animationClips = list.ToArray();
        }


        private void SetString(Joint root, string prefix, ref string result)
        {
            foreach (Joint joint in root.Children)
            {
                result += $"{prefix}{joint.Name} {joint.WorldMatrix.Translation}\n";
                SetString(joint, prefix + "-", ref result);
            }
        }

        public override string ToString()
        {
            var roots = _joints.Where(x => !x.HasParent).ToList();

            if (roots.Count == 0)
                return "no joints";

            var root = roots[0];
            string result = $"{root.Name} {root.WorldMatrix.Translation}\n";
            SetString(root, "-", ref result);
            return result;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }
}