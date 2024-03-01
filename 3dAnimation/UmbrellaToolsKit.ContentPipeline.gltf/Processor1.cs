using System;
using System.Collections.Generic;
using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using UmbrellaToolsKit.Animation3D;
using UmbrellaToolsKit.Animation3D.Tracks;
using TInput = glTFLoader.Schema.Gltf;
using TOutput = UmbrellaToolsKit.Animation3D.Mesh;

namespace UmbrellaToolsKit.ContentPipeline.gltf
{
    [ContentProcessor(DisplayName = "Processor1")]
    internal class Processor1 : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            ValidateFile(input);
            var mesh = LoadMesh(input);
            return mesh;
        }

        private static byte[][] uriBytesList;
        public TOutput LoadMesh(TInput gltf)
        {
            var meshR = new TOutput();
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var indices = new List<short>();
            var joints = new List<Joint>();

            uriBytesList = new byte[gltf.Buffers.Length][];
            for(int i = 0; i < uriBytesList.Length; i++)
                uriBytesList[i] = Convert.FromBase64String(gltf.Buffers[i].Uri.Replace("data:application/octet-stream;base64,", ""));

            for (int i = 0; i < gltf.Meshes.Length; i++)
            {
                var attributes = gltf.Meshes[i].Primitives;
                int accessorLenght = gltf.Accessors.Length;

                for (int j = 0; j < accessorLenght; j++)
                {
                    var accessor = gltf.Accessors[j];
                   
                    int bufferIndex = accessor.BufferView.Value;
                    var bufferView = gltf.BufferViews[bufferIndex];
                    byte[] uriBytes = uriBytesList[bufferView.Buffer];

                    // vertices
                    if (attributes[i].Attributes["POSITION"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC3)
                    {
                        float[] ScalingFactorForVariables = new float[3] { 1.0f, 1.0f, 1.0f };

                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[0];
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[1];
                            n += 4;
                            float z = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[2];

                            vertices.Add(new Vector3(x, y, z));
                        }
                    }

                    // Normals
                    if (attributes[i].Attributes["NORMAL"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC3)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float z = BitConverter.ToSingle(uriBytes, n);

                            normals.Add(new Vector3(x, y, z));
                        }
                    }

                    //Texture Coords
                    if (attributes[i].Attributes.ContainsKey("TEXCOORD_0") && attributes[i].Attributes["TEXCOORD_0"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC2)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n);

                            texCoords.Add(new Vector2(x, y));
                        }
                    }

                    // Indicies
                    if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 2)
                        {
                            UInt16 TriangleItem = BitConverter.ToUInt16(uriBytes, n);
                            indices.Add((short)TriangleItem);
                        }
                    }
                }
            }

            meshR.Vertices = vertices.ToArray();
            meshR.Normals = normals.ToArray();
            meshR.Indices = indices.ToArray();
            meshR.TexCoords = texCoords.ToArray();
            meshR.Clips = LoadAnimationClips(gltf);
            meshR.RestPose = new Pose[1] { LoadRestPose(gltf) };
            meshR.CurrentPose = meshR.RestPose;

            Console.WriteLine($"joins---->: {meshR.RestPose[0].Size()}");
            return meshR;
        }

        public static Pose LoadRestPose(glTFLoader.Schema.Gltf gltf)
        {
            int boneCount = gltf.Nodes.Length;
            Console.WriteLine(boneCount);
            Pose result = new Pose(boneCount);

            for(int i = 0; i < boneCount; i++)
            {
                var node = gltf.Nodes[i];

                Transform transform = GetLocalTransform(gltf, node);
                result.SetLocalTransform(i, transform);

                int parent = -1;
                for(int j = 0; j < gltf.Nodes.Length; j++)
                {
                    if(gltf.Nodes[j].Children != null)
                    {
                        for (int k = 0; k < gltf.Nodes[j].Children.Length; k++)
                            if (gltf.Nodes[j].Children[k] == i)
                                parent = j;
                        if(parent != -1)
                            result.SetParent(i, parent);
                    }
                }
            }

            return result;
        }

        public static Transform GetLocalTransform(glTFLoader.Schema.Gltf gltf, glTFLoader.Schema.Node node)
        {
            Transform result = new Transform();

            if (node.Translation != null)
                result.Position = new Vector3(node.Translation[0], node.Translation[1], node.Translation[2]);

            if (node.Rotation != null)
                result.Rotation = new Quaternion(node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]);

            if (node.Scale != null)
                result.Scale = new Vector3(node.Scale[0], node.Scale[1], node.Scale[2]);

            return result;
        }

        public static Clip[] LoadAnimationClips(glTFLoader.Schema.Gltf gltf)
        {
            Clip[] result;

            int numClips = gltf.Animations.Length;
            int numNodes = gltf.Nodes.Length;

            result = new Clip[numClips];

            for(int i = 0; i < numClips; ++i)
            {
                result[i] = new Clip();
                result[i].SetName(gltf.Animations[i].Name);
                var animation = gltf.Animations[i];
                int numChannels = gltf.Animations[i].Channels.Length;

                for (int j = 0; j < numChannels; ++j)
                {
                    var channel = gltf.Animations[i].Channels[j];
                    var target = channel.Target;
                    int nodeId = (int)target.Node;

                    if(channel.Target.Path == AnimationChannelTarget.PathEnum.translation)
                    {
                        VectorTrack track = result[i][nodeId].GetPosition();
                        TrackFromChannel(ref track, gltf, channel, animation, channel.Target.Path);
                    }
                    else if(channel.Target.Path == AnimationChannelTarget.PathEnum.scale)
                    {
                        VectorTrack track = result[i][nodeId].GetScale();
                        TrackFromChannel(ref track, gltf, channel, animation, channel.Target.Path);
                    }
                    else if(channel.Target.Path == AnimationChannelTarget.PathEnum.rotation)
                    {
                        QuaternionTrack track = result[i][nodeId].GetRotation();
                        //TrackFromChannel(ref track, gltf, channel, animation, channel.Target.Path);
                    }

                }

                result[i].RecalculateDuration();
            }

            return result;
        }

        public static void TrackFromChannel(ref VectorTrack inOutTrack, TInput gltf, AnimationChannel inChannel, glTFLoader.Schema.Animation animation, AnimationChannelTarget.PathEnum path)
        {
            Interpolation interpolation = Interpolation.Constant;
            var sampler = animation.Samplers[inChannel.Sampler];
            if (sampler.Interpolation == AnimationSampler.InterpolationEnum.LINEAR)
                interpolation = Interpolation.Linear;
            else if (sampler.Interpolation == AnimationSampler.InterpolationEnum.CUBICSPLINE)
                interpolation = Interpolation.Cubic;

            bool isSampleCubic = interpolation == Interpolation.Cubic;
            inOutTrack.SetInterpolation(interpolation);

            List<float> timelineFloats = new List<float>();
            List<float> valuesFloats = new List<float>();
            List<int> ids = new List<int>();

            inOutTrack.Resize(gltf.Accessors[sampler.Input].Count);
            Console.WriteLine(gltf.Accessors[sampler.Input].Count);

            for (int i = 0; i < animation.Samplers.Length; i++)
            {
                int input = animation.Samplers[i].Input;
                int output = animation.Samplers[i].Output;

                if (inChannel.Target.Path == path)
                {
                    for(int j = 0; j < gltf.Accessors.Length; j++)
                    {
                        var acessor = gltf.Accessors[j];
                        int bufferIndex = (int)acessor.BufferView;
                        if (bufferIndex == output && acessor.Type == Accessor.TypeEnum.VEC3 && !ids.Contains(j))
                        {;
                            var bufferView = gltf.BufferViews[bufferIndex];
                            byte[] uriBytes = uriBytesList[bufferView.Buffer];

                            int frameCount = 0;
                            int byteOffset = bufferView.ByteOffset;
                            int byteTotal = byteOffset + bufferView.ByteLength;
                            ids.Add(j);

                            for (int n = byteOffset; n < byteTotal; n += 4)
                            {
                                float x = BitConverter.ToSingle(uriBytes, n);
                                n += 4;
                                float y = BitConverter.ToSingle(uriBytes, n);
                                n += 4;
                                float z = BitConverter.ToSingle(uriBytes, n);

                                inOutTrack[frameCount].mValue = new float[3] {x,y,z};
                                valuesFloats.AddRange(inOutTrack[frameCount].mValue);
                                frameCount++;
                            }

                            j = gltf.Accessors.Length;
                        }
                    }
                }
                
            }
        }

        public static void TrackFromChannel(ref QuaternionTrack inOutTrack, TInput gltf, AnimationChannel inChannel, glTFLoader.Schema.Animation animation, AnimationChannelTarget.PathEnum path)
        {
            Interpolation interpolation = Interpolation.Constant;
            var sampler = animation.Samplers[inChannel.Sampler];
            if (sampler.Interpolation == AnimationSampler.InterpolationEnum.LINEAR)
                interpolation = Interpolation.Linear;
            else if (sampler.Interpolation == AnimationSampler.InterpolationEnum.CUBICSPLINE)
                interpolation = Interpolation.Cubic;

            bool isSampleCubic = interpolation == Interpolation.Cubic;
            inOutTrack.SetInterpolation(interpolation);

            float[] timelineFloats = new float[animation.Samplers.Length];

            for (int i = 0; i < animation.Samplers.Length; i++)
            {
                int input = animation.Samplers[i].Input;
                int output = animation.Samplers[i].Output;

                if (inChannel.Target.Path == path)
                {
                    for (int j = 0; j < gltf.Accessors.Length; j++)
                    {
                        if (gltf.Accessors[j].BufferView == output && gltf.Accessors[j].Type == Accessor.TypeEnum.VEC4)
                        {
                            int bufferIndex = (int)gltf.Accessors[j].BufferView;
                            var bufferView = gltf.BufferViews[bufferIndex];
                            byte[] uriBytes = Convert.FromBase64String(gltf.Buffers[bufferView.Buffer].Uri.Replace("data:application/octet-stream;base64,", ""));

                            int frameCount = 0;

                            for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                            {
                                float x = BitConverter.ToSingle(uriBytes, n);
                                n += 4;
                                float y = BitConverter.ToSingle(uriBytes, n);
                                n += 4;
                                float z = BitConverter.ToSingle(uriBytes, n);
                                n += 4;
                                float w = BitConverter.ToSingle(uriBytes, n);

                                inOutTrack[frameCount].mValue = new float[4] { x, y, z, w };
                                frameCount++;
                            }

                            j = gltf.Accessors.Length;
                        }
                    }
                }

            }

            float[] valuesFloats = new float[sampler.Input * 3];
        }

        public static void ValidateFile(glTFLoader.Schema.Gltf gltf)
        {
            if (gltf.Buffers.Length == 0)
                throw new InvalidContentException($"Could not load buffers");
        }
    }
}