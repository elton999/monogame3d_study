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

        public Animation3D.Mesh LoadMesh(glTFLoader.Schema.Gltf gltf)
        {
            var meshR = new Animation3D.Mesh();
            var vertices = new List<Vector3>();
            var indices = new List<short>();
            
            for(int i = 0; i < gltf.Meshes.Length; i++)
            {
                var attributes = gltf.Meshes[i].Primitives;
                int accessorLenght = gltf.Accessors.Length;

                for (int j = 0; j < accessorLenght; j++)
                {
                    string meshPrimitive = "";
                    if (attributes.Length > j)
                        meshPrimitive = attributes[j].Mode.ToString();

                    var accessor = gltf.Accessors[j];
                    int bufferIndex = accessor.BufferView.Value;
                    var bufferView = gltf.BufferViews[bufferIndex];
                    byte[] uriBytes = Convert.FromBase64String(gltf.Buffers[bufferView.Buffer].Uri.Replace("data:application/octet-stream;base64,", ""));

                    // vertices
                    if (meshPrimitive == "TRIANGLES")
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
                    
                    // Indicies
                    if(accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
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
            meshR.Indices = indices.ToArray();
            return meshR;
        }

        public static void ValidateFile(glTFLoader.Schema.Gltf gltf)
        {
            if (gltf.Buffers.Length == 0)
                throw new InvalidContentException($"Could not load buffers");
        }
    }
}