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
        Effect basicEffect;

        Matrix world = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(Vector3.One * 0.2f);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), SCREENWIDTH / SCREENHEIGHT, 0.01f, 100f);

        Vector3 lightPosition = new Vector3(2, 2, 2);
        Vector3 lightRealPosition = new Vector3(0, 0, 0);

        UmbrellaToolsKit.Animation3D.Mesh mesh;
        Texture2D modelTex2D;

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
            modelTex2D = Content.Load<Texture2D>("WomanTex");
            basicEffect = Content.Load<Effect>("DiffuseLighting");

            VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[mesh.Vertices.Length];
            for(int i = 0; i < mesh.Vertices.Length; i++)
            {
                vertices[i] = new VertexPositionColorNormalTexture(mesh.Vertices[i], Color.White, mesh.Normals[i], mesh.TexCoords[i]);
            }
            System.Console.WriteLine(mesh.TexCoords.Length);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormalTexture), mesh.Vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorNormalTexture>(vertices);

            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), mesh.Indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(mesh.Indices);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            angle += 0.5f;
            if (angle > 360.0f)
                angle -= 360.0f;

            lightPosition = lightRealPosition + (Vector3.UnitZ) * MathF.Cos((float)gameTime.TotalGameTime.TotalMilliseconds * 0.0005f) * 20f;

            basicEffect.Parameters["World"].SetValue(world * Matrix.CreateRotationY(MathHelper.ToRadians(angle)));
            basicEffect.Parameters["View"].SetValue(view);
            basicEffect.Parameters["Projection"].SetValue(projection);
            basicEffect.Parameters["lightPosition"].SetValue(lightPosition);
            basicEffect.Parameters["SpriteTexture"].SetValue(modelTex2D);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Vertices.Length, 0, mesh.Indices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}