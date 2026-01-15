using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace _3dAnimation;
public class Model
{
    public enum Debug
    {
        None = 0,
        RenderJoint = 1,
        RenderMeshLines = 2,
    }

    private VertexBuffer _vertexBuffer;
    private GraphicsDevice _graphicsDevice;
    private Mesh _mesh;
    private IndexBuffer _indexBuffer;
    private Texture _texture;
    private Debug _debugState;

    private RenderLine _line;

    private Effect _basicEffect;
    private Vector3 _lightPosition;
    private Color _lightColor = Color.White;
    private float _lightIntensity = 1.0f;

    //TODO: remove it soon as possible
    Matrix _world = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(0.01f);
    Matrix _view = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(0, 3, 0), new Vector3(0, 1, 0));
    Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

    public Model(GraphicsDevice graphicsDevice, Mesh mesh, Effect effect)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = effect;
        _mesh = mesh;

        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(ModelVertexType), _mesh.Vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_mesh.Vertices);

        _line = new RenderLine(graphicsDevice);
    }

    public void SetLightPosition(Vector3 lightPosition) => _lightPosition = lightPosition;
    public void SetTexture(Texture texture) => _texture = texture;
    public void SetDebugState(Debug debug) => _debugState = debug;

    public void Draw()
    {
        _basicEffect.Parameters["World"].SetValue(_world);
        _basicEffect.Parameters["View"].SetValue(_view);
        _basicEffect.Parameters["Projection"].SetValue(_projection);
        _basicEffect.Parameters["SpriteTexture"].SetValue(_texture);
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
            if (_debugState is Debug.None)
            {
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _mesh.Vertices.Length / 3);
            }

            if (_debugState is Debug.RenderMeshLines)
            {
                _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, _mesh.Vertices.Length);
            }
        }

        if (_debugState is Debug.RenderJoint)
        {
            RenderJoints();
        }
    }

    private void RenderJoints()
    {
        var worldScale = Matrix.CreateScale(1f);
        worldScale *= _world;

        int count = 0;
        foreach (var joint in _mesh.Skeleton.Joints)
        {
            var p = Vector3.Transform(Vector3.Zero, joint.WorldMatrix);
            count++;

            if (!joint.HasParent)
                continue;

            Vector3 a = _mesh.Skeleton.GetJointPosition(joint, worldScale);
            Vector3 b = _mesh.Skeleton.GetJointPosition(joint.Parent, worldScale);

            _line.DrawLine(_graphicsDevice, a, b, _view, _projection, Color.Green);
        }
    }
}