using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D
{
    public class Model
    {
        private Mesh _mesh;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        
        private Matrix _modelWorld;
        private Texture _texture;

        private Effect basicEffect;
        private Vector3 _lightPosition;
        private Color _lightColor = Color.White;
        private float _lightIntensity = 1.0f;

        Matrix[] _restPose;

        private bool _debugMode = false;
        private int _currentBone = 0;

        public Skeleton Skeleton => _mesh.Skeleton;

        public Model(Mesh mesh, GraphicsDevice graphicsDevice)
        {
            _mesh = mesh;

            LoadModel(graphicsDevice);
        }

        public void SetLightPosition(Vector3 lightPosition) => _lightPosition = lightPosition;
        public void SetWorld(Matrix world) => _modelWorld = world;
        public void SetEffect(Effect effect) => basicEffect = effect;
        public void SetTexture(Texture texture) => _texture = texture;

        public void DebugMode(bool status, int bone)
        {
            _debugMode= status;
            _currentBone = bone;
        }
        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
            _restPose =  new Matrix[1]; 
            Skeleton.GetRestPose().GetMatrixPalette(ref _restPose, _mesh.JointsIndexs);

            basicEffect.Parameters["World"].SetValue(_modelWorld);
            basicEffect.Parameters["View"].SetValue(view);
            basicEffect.Parameters["Projection"].SetValue(projection);
            basicEffect.Parameters["lightPosition"].SetValue(_lightPosition);
            basicEffect.Parameters["lightColor"].SetValue(_lightColor.ToVector4());
            basicEffect.Parameters["lightIntensity"].SetValue(_lightIntensity);
            basicEffect.Parameters["SpriteTexture"].SetValue(_texture);
            basicEffect.Parameters["debugMode"].SetValue(_debugMode);
            basicEffect.Parameters["currentBone"].SetValue(_currentBone);
            basicEffect.Parameters["Bones"].SetValue(_mesh.InverseBindMatrix);
            basicEffect.Parameters["RestPose"].SetValue(_restPose);

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;
            
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if(_debugMode)
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _mesh.Vertices.Length, 0, _mesh.Indices.Length / 3);
                else
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _mesh.Vertices.Length, 0, _mesh.Indices.Length / 3);
            }
        }

        private void LoadModel(GraphicsDevice graphicsDevice)
        {
            ModelVertexType[] vertices = new ModelVertexType[_mesh.Vertices.Length];
            for (int i = 0; i < _mesh.Vertices.Length; i++)
                vertices[i] = new ModelVertexType(_mesh.Vertices[i], Color.White, _mesh.Normals[i], _mesh.TexCoords[i], _mesh.Joints[i], _mesh.Weights[i]);

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(ModelVertexType), _mesh.Vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<ModelVertexType>(vertices);

            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), _mesh.Indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_mesh.Indices);

            _restPose = new Matrix[1];
            Skeleton.GetRestPose().GetMatrixPalette(ref _restPose, _mesh.JointsIndexs);
        }
    }
}
