using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

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

        public TOutput LoadMesh(TInput gltf)
        {
            var meshR = new TOutput();
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var indices = new List<short>();
            
            for(int i = 0; i < gltf.Meshes.Length; i++)
            {
                var attributes = gltf.Meshes[i].Primitives;
                int accessorLenght = gltf.Accessors.Length;

                for (int j = 0; j < accessorLenght; j++)
                {
                    var accessor = gltf.Accessors[j];
                   
                    int bufferIndex = accessor.BufferView.Value;
                    var bufferView = gltf.BufferViews[bufferIndex];
                    byte[] uriBytes = Convert.FromBase64String(gltf.Buffers[bufferView.Buffer].Uri.Replace("data:application/octet-stream;base64,", ""));

                    // vertices
                    if (attributes[i].Attributes["POSITION"] == j && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC3)
                    {
                        float[] ScalingFactorForVariables = new float[3];
                        ScalingFactorForVariables = new float[3] { 1.0f, 1.0f, 1.0f };

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
            return meshR;
        }

        public static void ValidateFile(glTFLoader.Schema.Gltf gltf)
        {
            if (gltf.Buffers.Length == 0)
                throw new InvalidContentException($"Could not load buffers");
        }
    }
}