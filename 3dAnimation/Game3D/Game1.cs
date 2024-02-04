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

        Matrix world = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(Vector3.One * 0.2f);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), SCREENWIDTH / SCREENHEIGHT, 0.01f, 100f);

        Vector3 lightPosition = new Vector3(2, 2, 2);

        UmbrellaToolsKit.Animation3D.Mesh mesh;
        UmbrellaToolsKit.Animation3D.Model model;

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
            
            model = new UmbrellaToolsKit.Animation3D.Model(mesh, GraphicsDevice);
            model.SetTexture(Content.Load<Texture2D>("WomanTex"));
            model.SetLightPosition(lightPosition);
            model.SetEffect(Content.Load<Effect>("DiffuseLighting"));
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            model.SetWorld(world);
            model.Draw(GraphicsDevice, projection, view);

            Line line = new Line(Vector3.Zero, Vector3.UnitX * 1f, GraphicsDevice);
            line.Draw(GraphicsDevice, projection, view);

            base.Draw(gameTime);
        }
    }
}