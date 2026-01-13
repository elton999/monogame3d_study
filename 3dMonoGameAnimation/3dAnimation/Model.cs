using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _3dAnimation;
public class Model
{
    private VertexBuffer _vertexBuffer;
    private GraphicsDevice _graphicsDevice;
    private Mesh _mesh;
    private IndexBuffer _indexBuffer;
    private Texture _texture;

    private Effect _basicEffect;
    private Vector3 _lightPosition;
    private Color _lightColor = Color.White;
    private float _lightIntensity = 1.0f;

    //TODO: remove it soon as possible
    Matrix _world = Matrix.CreateTranslation(0, 0, 0);
    Matrix _view = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(0, 3, 0), new Vector3(0, 1, 0));
    Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

    public Model(GraphicsDevice graphicsDevice, Mesh mesh, Effect effect)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = effect;
        _mesh = mesh;

        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(ModelVertexType), _mesh.Vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_mesh.Vertices);
    }

    public void SetLightPosition(Vector3 lightPosition) => _lightPosition = lightPosition;
    public void SetTexture(Texture texture) => _texture = texture;

    public void Draw()
    {
        for (int i = 0; i < _mesh.Skeleton.SkinMatrices.Length; i++)
            _mesh.Skeleton.SkinMatrices[i] = Matrix.Identity;

        _world *= Matrix.CreateRotationY(MathHelper.ToRadians(0.5f));

        _basicEffect.Parameters["World"].SetValue(_world);
        _basicEffect.Parameters["View"].SetValue(_view);
        _basicEffect.Parameters["Projection"].SetValue(_projection);
        _basicEffect.Parameters["lightPosition"].SetValue(_lightPosition);
        _basicEffect.Parameters["lightColor"].SetValue(_lightColor.ToVector4());
        _basicEffect.Parameters["lightIntensity"].SetValue(_lightIntensity);
        _basicEffect.Parameters["SpriteTexture"].SetValue(_texture);
        _basicEffect.Parameters["debugMode"].SetValue(false);
        _basicEffect.Parameters["currentBone"].SetValue(1);
        _basicEffect.Parameters["Bones"].SetValue(_mesh.Skeleton.SkinMatrices);

        _graphicsDevice.BlendState = BlendState.Opaque;
        _graphicsDevice.DepthStencilState = DepthStencilState.Default;
        _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        _graphicsDevice.RasterizerState = RasterizerState.CullNone;

        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        RasterizerState rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        _graphicsDevice.RasterizerState = rasterizerState;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _mesh.Vertices.Length/3);
        }
    }
    
}