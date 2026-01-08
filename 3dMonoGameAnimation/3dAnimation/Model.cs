using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3dAnimation;
public class Model
{
    private BasicEffect _basicEffect;
    private VertexPositionColor[] _vertices;
    private VertexBuffer _vertexBuffer;
    private GraphicsDevice _graphicsDevice;

    //TODO: remove it soon as possible
    Matrix world = Matrix.CreateTranslation(0, 0, 0);
    Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
    Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

    public Model(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = new BasicEffect(graphicsDevice);
        _vertices = new VertexPositionColor[3];
        _vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
        _vertices[1] = new VertexPositionColor(new Vector3(+0.5f, 0, 0), Color.Green);
        _vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 0, 0), Color.Blue);

        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_vertices);
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
            _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
        }
    }
    
}