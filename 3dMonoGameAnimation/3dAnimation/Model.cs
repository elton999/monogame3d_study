using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3dAnimation;
public class Model
{
    private BasicEffect _basicEffect;
    private VertexBuffer _vertexBuffer;
    private GraphicsDevice _graphicsDevice;
    private Mesh _mesh;

    //TODO: remove it soon as possible
    Matrix world = Matrix.CreateTranslation(0, 0, 0);
    Matrix view = Matrix.CreateLookAt(new Vector3(0, 4, 20), new Vector3(0, 3, 0), new Vector3(0, 1, 0));
    Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

    public Model(GraphicsDevice graphicsDevice, Mesh mesh)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = new BasicEffect(graphicsDevice);
        _mesh = mesh;

        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), _mesh.Vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_mesh.Vertices);
    }

    public void Draw()
    {
        _basicEffect.World = world;
        _basicEffect.View = view;
        _basicEffect.Projection = projection;

        _basicEffect.VertexColorEnabled = true;

        _graphicsDevice.SetVertexBuffer(_vertexBuffer);

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