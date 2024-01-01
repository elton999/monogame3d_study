using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using UmbrellaToolsKit;

namespace Game3D
{
    public class Game1 : Game
    {
        const int SCREENWIDTH = 1024, SCREENHEIGHT = 768;
        GraphicsDeviceManager graphics;
        GraphicsDevice gpu;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static public int screenW, screenH;
        Camera cam;

        Rectangle desktopRect;
        Rectangle screenRect;
        
        RenderTarget2D MainTarget;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        BasicEffect basicEffect;

        Matrix world = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(Vector3.One * 0.2f);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), SCREENWIDTH / SCREENHEIGHT, 0.01f, 100f);

        UmbrellaToolsKit.Animation3D.Mesh mesh;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            int desktop_width = SCREENWIDTH;
            int desktop_height = SCREENHEIGHT;

            graphics.PreferredBackBufferWidth = desktop_width;
            graphics.PreferredBackBufferHeight = desktop_height;
            graphics.IsFullScreen = false;
            graphics.PreferredDepthStencilFormat = DepthFormat.None;
            graphics.ApplyChanges();
            Window.Position = new Point(0, 30);
            gpu = GraphicsDevice;

            PresentationParameters pp = gpu.PresentationParameters;
            spriteBatch = new SpriteBatch(gpu);
            MainTarget = new RenderTarget2D(gpu, SCREENWIDTH, SCREENHEIGHT, false, pp.BackBufferFormat, DepthFormat.Depth24);
            screenH = MainTarget.Height;
            screenW = MainTarget.Width;
            desktopRect = new Rectangle(0,0, pp.BackBufferWidth, pp.BackBufferHeight);
            screenRect = new Rectangle(0, 0, screenW, screenH);

            cam = new Camera(gpu, Vector3.Up);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("BasicFont");
            mesh = Content.Load<UmbrellaToolsKit.Animation3D.Mesh>("Woman");

            basicEffect = new BasicEffect(GraphicsDevice);

            VertexPositionColor[] vertices = new VertexPositionColor[mesh.Vertices.Count];
            for(int i = 0; i < mesh.Vertices.Count; i++)
            {
                vertices[i] = new VertexPositionColor(mesh.Vertices[i], Color.Gray);
            }
            System.Console.WriteLine(mesh.Vertices.Count);

            short[] indices = new short[mesh.Indices.Count];
            for(int i = 0; i < indices.Length; i++)
            {
                indices[i] = (short)mesh.Indices[i];
            }

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), mesh.Vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        bool init = true;
        protected override void Update(GameTime gameTime)
        {
            if(init)
            {

            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        RasterizerState rs_ccW = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.CullClockwiseFace };
        void Set3DStates()
        {
            gpu.BlendState = BlendState.NonPremultiplied;
            gpu.DepthStencilState = DepthStencilState.Default;
            if (gpu.RasterizerState.CullMode == CullMode.None)
                gpu.RasterizerState = rs_ccW;
        }

        float angle;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            angle += 0.5f;
            if (angle > 360.0f)
                angle -= 360.0f;
            basicEffect.World = world * Matrix.CreateRotationY(MathHelper.ToRadians(angle));
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;
            basicEffect.EnableDefaultLighting();

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Vertices.Count, 0, mesh.Indices.Count / 3);
            }

            base.Draw(gameTime);
        }
    }
}