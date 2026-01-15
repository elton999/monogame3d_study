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
        public Matrix[] InverseBindMatrix;

        public string _animationName = "";
        public int _currentFrame = 0;

        public Skeleton() { }

        public Skeleton(Joint[] joints)
        {
            _joints = joints;
            SkinMatrices = new Matrix[joints.Length];
        }

        public void ComputeBindPose()
        {
            for (int jointIndex = 0;  jointIndex < _joints.Length; jointIndex++)
            {
               var joint = _joints[jointIndex];
               joint.BindPose = Matrix.Invert(InverseBindMatrix[jointIndex]);
               joint.InverseBindPose = InverseBindMatrix[jointIndex];
            }
        }

        public void PlayAnimation(string animationName)
        {
            _animationName = animationName;
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(_animationName))
            {
                for (int i = 0; i < SkinMatrices.Length; i++)
                    SkinMatrices[i] = Matrix.Identity;

                return;
            }

            var clip = _animationClips.FirstOrDefault(c => c.Name == _animationName);
            if (clip == null || clip.JoinByFrameTransform == null)
                return;

            if (_currentFrame >= clip.JoinByFrameTransform.Length)
                _currentFrame = 0;

            // 1. Sempre volte ao bind pose antes de aplicar animação
            foreach (var joint in _joints)
                joint.ResetToBindPose();

            // 2. Aplica animação LOCAL (sem world ainda)
            var frame = clip.JoinByFrameTransform[_currentFrame];

            if (frame != null)
            {
                for (int i = 0; i < _joints.Length; i++)
                {
                    if (frame.TryGetValue(i, out var animTransform))
                        _joints[i].ApplyTransformAnimation(animTransform);
                }
            }

            _currentFrame++;

            // 3. Atualiza world matrices (hierarquia correta)
            foreach (var root in _joints.Where(j => !j.HasParent))
                root.UpdateWorld(Matrix.Identity);

            // 4. Calcula skin matrices finais
            for (int i = 0; i < _joints.Length; i++)
            {
                SkinMatrices[i] = _joints[i].InverseBindPose * _joints[i].WorldMatrix;
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

        public Vector3 GetJointPosition(Joint joint, Matrix worldScale)
        {
            Vector3 pos = Vector3.Transform(Vector3.Zero, joint.WorldMatrix);
            return Vector3.Transform(pos, worldScale);
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