using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinningInformation.Animation;
using System;

namespace basic3DMonogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(2, 45, -110), new Vector3(0, 45, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 800f / 480f, 1f, 300f);

        Model model;
        AnimationPlayer animationPlayer;
        Texture2D texture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("teste");
            SkinningData skinningData = model.Tag as SkinningData;
            if (skinningData == null)
                throw new InvalidOperationException("This model does not contain a skinningData tag.");
            animationPlayer = new AnimationPlayer(skinningData);
            animationPlayer.StartClip(skinningData.AnimationClips["basic character|walk"]);
            texture = Content.Load<Texture2D>("Woman");
        }

        float frameRate = 0;
        Vector3 position = new Vector3(0, 0, 0);
        float angle = 0;
        protected override void Update(GameTime gameTime)
        {
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            base.Update(gameTime);

            angle += gameTime.ElapsedGameTime.Milliseconds * 0.02f;
            if (angle > 360.0f)
                angle -= 360.0f;
        }

        float timer = 0;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix[] bones = animationPlayer.GetSkinTransforms();
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = view;
                    effect.World = Matrix.CreateScale(0.06f) * Matrix.CreateRotationY(frameRate) * Matrix.CreateRotationY(MathHelper.ToRadians(angle));
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    //effect.SpecularColor = Vector3.Zero;
                }
                mesh.Draw();
            }

            frameRate = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Draw(gameTime);
        }
    }
}