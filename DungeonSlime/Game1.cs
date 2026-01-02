using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1 : Core
{
    private Sprite _slime;
    private Sprite _bat;

    public Game1() : base("DungeonSlime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas_definition.json");
        // retrieve the slime region from the atlas.
        _slime = atlas.CreateSprite("slime");
        _slime.Scale = Vector2.One * 4.0f;

        // retrieve the bat region from the atlas.
        _bat = atlas.CreateSprite("bat");
        _bat.Scale = Vector2.One * 4.0f;
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
        // Clear the back buffer.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime texture region at a scale of 4.0
        _slime.Draw(SpriteBatch, Vector2.Zero);

        // Draw the bat texture region 10px to the right of the slime at a scale of 4.0
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width, 0));

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
