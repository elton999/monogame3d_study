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
            var weights = new List<Vector4>();
            var joints =  new List<Vector4>();
            var indices = new List<short>();

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
                    if (attributes[i].Attributes.ContainsKey("NORMAL") && attributes[i].Attributes["NORMAL"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC3)
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
                    if (attributes[i].Attributes.ContainsKey("NORMAL") &&  attributes[i].Attributes.ContainsKey("TEXCOORD_0") && attributes[i].Attributes["TEXCOORD_0"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC2)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n);

                            texCoords.Add(new Vector2(x, y));
                        }
                    }

                    //Joints 
                    if (attributes[i].Attributes.ContainsKey("JOINTS_0") && attributes[i].Attributes["JOINTS_0"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                    {
                        var listJoints = gltf.Skins[0].Joints;
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n++)
                        {
                            float x = uriBytes[n];
                            n++;
                            float y = uriBytes[n];
                            n++;
                            float z = uriBytes[n];
                            n++;
                            float w = uriBytes[n];
                            joints.Add(new Vector4(x,y,z,w));
                        }
                    }

                    //Weights 
                    if (attributes[i].Attributes.ContainsKey("WEIGHTS_0") && attributes[i].Attributes["WEIGHTS_0"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                        {
                            
                            float x = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float z = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float w = BitConverter.ToSingle(uriBytes, n);
                            weights.Add(new Vector4(x, y, z, w));
                        }
                    }

                    // Indicies
                    if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                    {
                        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 2)
                        {
                            short TriangleItem = BitConverter.ToInt16(uriBytes, n);
                            indices.Add((short)TriangleItem);
                        }
                    }
                }
            }

            if (gltf.Skins.Length > 0 && gltf.Skins[0].ShouldSerializeInverseBindMatrices())
            {
                int inverseBindIndex = (int)gltf.Skins[0].InverseBindMatrices;
                var accessor = gltf.Accessors[inverseBindIndex];

                int bufferIndex = accessor.BufferView.Value;
                var bufferView = gltf.BufferViews[bufferIndex];
                byte[] uriBytes = uriBytesList[bufferView.Buffer];

                var InverseBindList = new List<Matrix>();

                for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                {
                    var matrix = new Matrix();
                    for (int i = 0; i < 16; i++)
                    {
                        matrix[i] = BitConverter.ToSingle(uriBytes, n);
                        if(i < 15) n += 4;
                    }

                    InverseBindList.Add(matrix);
                }


                meshR.InverseBindMatrix = InverseBindList.ToArray();
                meshR.JointsIndexs = gltf.Skins[0].Joints;
            }

            meshR.Vertices = vertices.ToArray();
            meshR.Normals = normals.ToArray();
            meshR.Indices = indices.ToArray();
            meshR.TexCoords = texCoords.ToArray();
            meshR.Joints = joints.ToArray();
            meshR.Weights = weights.ToArray();
            meshR.Clips = LoadAnimationClips(gltf);
            var restPose = LoadRestPose(gltf);
            var bindPose = LoadRestPose(gltf);
            meshR.Skeleton = new Skeleton(restPose, bindPose, LoadJoitNames(gltf));
            return meshR;
        }

        public static Pose LoadRestPose(glTFLoader.Schema.Gltf gltf)
        {
            int boneCount = gltf.Nodes.Length;
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

        public static List<string> LoadJoitNames(glTFLoader.Schema.Gltf gltf)
        {
            List<string> result = new List<string>();
            foreach (var node in gltf.Nodes)
                result.Add(node.Name);
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
                        TrackFromChannel(ref track, gltf, channel, animation, channel.Target.Path);
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

            bool isSamplerCubic = interpolation == Interpolation.Cubic;
            inOutTrack.SetInterpolation(interpolation);

            List<float> timelineFloats = new List<float>();
            List<float> valuesFloats = new List<float>();
            int numFrames = gltf.Accessors[sampler.Input].Count;

            inOutTrack.Resize(numFrames);

            int input = sampler.Input;
            int output = sampler.Output;

            if (inChannel.Target.Path == path)
            {
                for(int j = 0; j < gltf.Accessors.Length; j++)
                {
                    var acessor = gltf.Accessors[j];
                    int bufferIndex = (int)acessor.BufferView;

                    if (bufferIndex == output && acessor.Type == Accessor.TypeEnum.VEC3)
                    {
                        var bufferView = gltf.BufferViews[bufferIndex];
                        byte[] uriBytes = uriBytesList[bufferView.Buffer];

                        int frameCount = 0;
                        int byteOffset = bufferView.ByteOffset;
                        int byteTotal = byteOffset + bufferView.ByteLength;

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
                    } // floats output

                    if(bufferIndex == input && acessor.Type == Accessor.TypeEnum.SCALAR)
                    {
                        var bufferView = gltf.BufferViews[bufferIndex];
                        byte[] uriBytes = uriBytesList[bufferView.Buffer];

                        int frameCount = 0;
                        int byteOffset = bufferView.ByteOffset;
                        int byteTotal = byteOffset + bufferView.ByteLength;

                        for (int n = byteOffset; n < byteTotal; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);

                            timelineFloats.Add(x);
                            frameCount++;
                        }
                    } // time input

                }
            }

            int numberOfValuesPerFrame = valuesFloats.Count / timelineFloats.Count;
            for(int i = 0; i < numFrames; i++)
            {
                int baseIndex = i * numberOfValuesPerFrame;
                var frame = inOutTrack[i];
                int offset = 0;

                frame.mTime = timelineFloats[i];

                for (int component = 0; component < 3; ++component)
                {
                    frame.mIn[component] = isSamplerCubic ? valuesFloats[baseIndex + offset++] : 0.0f;
                }

                for (int component = 0; component < 3; ++component)
                {
                    frame.mValue[component] = valuesFloats[baseIndex + offset++];
                }

                for (int component = 0; component < 3; ++component)
                {
                    frame.mOut[component] = isSamplerCubic ? valuesFloats[baseIndex + offset++] : 0.0f;
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

            bool isSamplerCubic = interpolation == Interpolation.Cubic;
            inOutTrack.SetInterpolation(interpolation);

            List<float> timelineFloats = new List<float>();
            List<float> valuesFloats = new List<float>();
            int numFrames = gltf.Accessors[sampler.Input].Count;

            inOutTrack.Resize(numFrames);

            int input = sampler.Input;
            int output = sampler.Output;

            if (inChannel.Target.Path == path)
            {
                for (int j = 0; j < gltf.Accessors.Length; j++)
                {
                    var acessor = gltf.Accessors[j];
                    int bufferIndex = (int)acessor.BufferView;

                    if (bufferIndex == output && acessor.Type == Accessor.TypeEnum.VEC4)
                    {
                        var bufferView = gltf.BufferViews[bufferIndex];
                        byte[] uriBytes = uriBytesList[bufferView.Buffer];

                        int frameCount = 0;
                        int byteOffset = bufferView.ByteOffset;
                        int byteTotal = byteOffset + bufferView.ByteLength;

                        for (int n = byteOffset; n < byteTotal; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float y = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float z = BitConverter.ToSingle(uriBytes, n);
                            n += 4;
                            float w = BitConverter.ToSingle(uriBytes, n);

                            inOutTrack[frameCount].mValue = new float[4] { x, y, z, w };
                            valuesFloats.AddRange(inOutTrack[frameCount].mValue);
                            frameCount++;
                        }
                    } // floats output

                    if (bufferIndex == input && acessor.Type == Accessor.TypeEnum.SCALAR)
                    {
                        var bufferView = gltf.BufferViews[bufferIndex];
                        byte[] uriBytes = uriBytesList[bufferView.Buffer];

                        int frameCount = 0;
                        int byteOffset = bufferView.ByteOffset;
                        int byteTotal = byteOffset + bufferView.ByteLength;

                        for (int n = byteOffset; n < byteTotal; n += 4)
                        {
                            float x = BitConverter.ToSingle(uriBytes, n);

                            timelineFloats.Add(x);
                            frameCount++;
                        }
                    } // time input

                }
            }

            int numberOfValuesPerFrame = valuesFloats.Count / timelineFloats.Count;
            for (int i = 0; i < numFrames; i++)
            {
                int baseIndex = i * numberOfValuesPerFrame;
                var frame = inOutTrack[i];
                int offset = 0;

                frame.mTime = timelineFloats[i];

                for (int component = 0; component < 4; ++component)
                {
                    frame.mIn[component] = isSamplerCubic ? valuesFloats[baseIndex + offset++] : 0.0f;
                }

                for (int component = 0; component < 4; ++component)
                {
                    frame.mValue[component] = valuesFloats[baseIndex + offset++];
                }

                for (int component = 0; component < 4; ++component)
                {
                    frame.mOut[component] = isSamplerCubic ? valuesFloats[baseIndex + offset++] : 0.0f;
                }
            }
        }

        public static void ValidateFile(glTFLoader.Schema.Gltf gltf)
        {
            if (gltf.Buffers.Length == 0)
                throw new InvalidContentException($"Could not load buffers");
        }
    }
}