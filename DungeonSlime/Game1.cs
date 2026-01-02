using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1 : Core
{
    private Texture2D _logo;

    public Game1() : base("DungeonSlime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {

        _logo = Content.Load<Texture2D>("images/logo");
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSeaGreen);

        SpriteBatch.Begin();
        SpriteBatch.Draw(_logo, Vector2.Zero, Color.White);
        SpriteBatch.Draw(_logo, new Vector2(
                    ((Window.ClientBounds.Width * 0.5f) - (_logo.Bounds.Width * 0.5f)),
                    ((Window.ClientBounds.Height * 0.5f) - (_logo.Bounds.Height * 0.5f))),
                Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
