using System;
using DungeonSlime.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Gum.Forms.Controls;
using MonoGameLibrary;
using MonoGameLibrary.Input;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes;

public class TitleScene : Scene
{
    private const string DUNGEON_TEXT = "Dungeon";
    private const string SLIME_TEXT = "Slime";

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _font5x;

    // The position to draw the dungeon text at.
    private Vector2 _dungeonTextPos;

    // The origin to set for the dungeon text.
    private Vector2 _dungeonTextOrigin;

    // The position to draw the slime text at.
    private Vector2 _slimeTextPos;

    // The origin to set for the slime text.
    private Vector2 _slimeTextOrigin;

    // The texture used for the background pattern.
    private Texture2D _backgroundPattern;

    // The destination rectangle for the background pattern to fill.
    private Rectangle _backgroundDestination;

    private SoundEffect _uiSoundEffect;
    private Panel _titleScreenButtonsPanel;
    private Panel _optionsPanel;

    // The offset to apply when drawing the background pattern so it appears to
    // be scrolling.
    private Vector2 _backgroundOffsetOne;
    private Vector2 _backgroundOffsetTwo;

    // The speed that the background pattern scrolls.
    private float _scrollSpeed = 50.0f;

    // The options button used to open the options menu.
    private AnimatedButton _optionsButton;

    // The back button used to exit the options menu back to the title menu.
    private AnimatedButton _optionsBackButton;

    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        // Set the position and origin for the Slime text.
        size = _font5x.MeasureString(SLIME_TEXT);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        // Initialize the offset of the background pattern at zero.
        _backgroundOffsetOne = Vector2.Zero;
        _backgroundOffsetTwo = new Vector2(_backgroundPattern.Width * 0.75f, _backgroundPattern.Height * 0.75f);

        // Set the background pattern destination rectangle to fill the entire
        // virtual resolution (1280x720).
        _backgroundDestination = new Rectangle(0, 0, 1280, 720);

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the font for the title text.
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        // Load the background pattern.
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");

        _uiSoundEffect = Content.Load<SoundEffect>("audio/ui");

        // Load the texture atlas from the xml configuration file.
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas_definition.json");
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        CreateOptionsPanel();
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        AnimatedButton startButton = new AnimatedButton(_atlas);
        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.X = 50;
        startButton.Y = -12;
        startButton.Width = 70;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new AnimatedButton(_atlas);
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.X = -50;
        _optionsButton.Y = -12;
        _optionsButton.Width = 70;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }

    private void HandleStartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Change to the game scene to start the game.
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Remove the title panel from the root to completely take it out of
        // focus navigation and input handling.
        _titleScreenButtonsPanel.RemoveFromRoot();

        // Add the options panel to the root and make it visible.
        _optionsPanel.AddToRoot();
        _optionsPanel.IsVisible = true;

        _titleScreenButtonsPanel.IsVisible = false;

        // Give the back button on the options panel focus.
        _optionsBackButton.IsFocused = true;
    }


    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        // Don't add to root yet - we'll add it when the options menu is opened

        TextRuntime optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        optionsText.UseCustomFont = true;
        optionsText.FontScale = 0.5f;
        optionsText.CustomFontFile = @"fonts/04b_30.fnt";
        _optionsPanel.AddChild(optionsText);

        OptionsSlider musicSlider = new OptionsSlider(_atlas);
        musicSlider.Name = "MusicSlider";
        musicSlider.Text = "MUSIC";
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        OptionsSlider sfxSlider = new OptionsSlider(_atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new AnimatedButton(_atlas);
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // Play the UI Sound effect so the player can hear the difference in audio.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Remove the options panel from the root to completely take it out of
        // focus navigation and input handling.
        _optionsPanel.RemoveFromRoot();

        // Add the title panel back to the root and make it visible.
        _titleScreenButtonsPanel.AddToRoot();
        _titleScreenButtonsPanel.IsVisible = true;

        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _optionsButton.IsFocused = true;
    }

    public override void Update(GameTime gameTime)
    {
        // Update the offsets for the background pattern wrapping so that it
        // scrolls up and to the right.
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffsetOne.X -= offset;
        _backgroundOffsetOne.Y += offset;

        _backgroundOffsetTwo.X += offset;
        _backgroundOffsetTwo.Y += offset;

        // Ensure that the offsets do not go beyond the texture bounds so it is
        // a seamless wrap.
        _backgroundOffsetOne.X %= _backgroundPattern.Width;
        _backgroundOffsetOne.Y %= _backgroundPattern.Height;
        _backgroundOffsetTwo.X %= _backgroundPattern.Width;
        _backgroundOffsetTwo.Y %= _backgroundPattern.Height;

        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffsetOne.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffsetTwo.ToPoint(), _backgroundDestination.Size), Color.White * 0.2f);
        Core.SpriteBatch.End();

        if (_titleScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Dungeon text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Dungeon text on top of that at its original position
            Core.SpriteBatch.DrawString(_font5x, DUNGEON_TEXT, _dungeonTextPos, Color.White, 0.0f, _dungeonTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Slime text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Slime text on top of that at its original position
            Core.SpriteBatch.DrawString(_font5x, SLIME_TEXT, _slimeTextPos, Color.White, 0.0f, _slimeTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();
        }

        GumService.Default.Draw();
    }
}

