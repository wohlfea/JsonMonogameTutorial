using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core s_instance;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => s_instance;

    // The scene that is currently active.
    private static Scene s_activeScene;

    // The next scene to switch to, if there is one.
    private static Scene s_nextScene;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the render target used for virtual resolution rendering.
    /// </summary>
    private static RenderTarget2D s_renderTarget;

    /// <summary>
    /// Gets the virtual resolution width.
    /// </summary>
    private static int s_virtualWidth;

    /// <summary>
    /// Gets the virtual resolution height.
    /// </summary>
    private static int s_virtualHeight;

    /// <summary>
    /// Gets the destination rectangle for drawing the render target to screen.
    /// </summary>
    private static Rectangle s_destinationRectangle;

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Gets a reference to to the input management system.
    /// </summary>
    public static InputManager Input { get; private set; }

    /// <summary>
    /// Gets or Sets a value that indicates if the game should exit when the esc key on the keyboard is pressed.
    /// </summary>
    public static bool ExitOnEscape { get; set; }

    /// <summary>
    /// Gets a reference to the audio control system.
    /// </summary>
    public static AudioController Audio { get; private set; }

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Ensure that multiple cores are not created.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        s_instance = this;

        // Store virtual resolution
        s_virtualWidth = width;
        s_virtualHeight = height;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the back buffer dimensions
        if (fullScreen)
        {
            // In fullscreen, use the native monitor resolution
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else
        {
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
        }
        Graphics.IsFullScreen = fullScreen;
        Graphics.ApplyChanges();

        // Set the window title
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;

        // Exit on escape is true by default
        ExitOnEscape = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the render target at virtual resolution
        s_renderTarget = new RenderTarget2D(GraphicsDevice, s_virtualWidth, s_virtualHeight);

        // Calculate destination rectangle for scaling with aspect ratio
        CalculateDestinationRectangle();

        // Subscribe to window size changes
        Window.ClientSizeChanged += OnClientSizeChanged;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a new input manager.
        Input = new InputManager();

        // Create a new audio controller.
        Audio = new AudioController();
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        CalculateDestinationRectangle();
    }

    private static void CalculateDestinationRectangle()
    {
        float targetAspectRatio = (float)s_virtualWidth / s_virtualHeight;
        int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int height = (int)(width / targetAspectRatio);

        if (height > GraphicsDevice.PresentationParameters.BackBufferHeight)
        {
            height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            width = (int)(height * targetAspectRatio);
        }

        int x = (GraphicsDevice.PresentationParameters.BackBufferWidth - width) / 2;
        int y = (GraphicsDevice.PresentationParameters.BackBufferHeight - height) / 2;

        s_destinationRectangle = new Rectangle(x, y, width, height);
    }

    protected override void UnloadContent()
    {
        // Dispose of the audio controller.
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Update the input manager.
        Input.Update(gameTime);

        // Update the audio controller.
        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Exit();
        }

        // if there is a next scene waiting to be switch to, then transition
        // to that scene.
        if (s_nextScene != null)
        {
            TransitionScene();
        }

        // If there is an active scene, update it.
        if (s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Set render target to draw to virtual resolution
        GraphicsDevice.SetRenderTarget(s_renderTarget);
        GraphicsDevice.Clear(Color.Black);

        // If there is an active scene, draw it to the render target
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        // Now draw the render target to the back buffer, scaled
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(s_renderTarget, s_destinationRectangle, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next)
    {
        // Only set the next scene value if it is not the same
        // instance as the currently active scene.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        // If there is an active scene, dispose of it.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Force the garbage collector to collect to ensure memory is cleared.
        GC.Collect();

        // Change the currently active scene to the new scene.
        s_activeScene = s_nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        s_nextScene = null;

        // If the active scene now is not null, initialize it.
        // Remember, just like with Game, the Initialize call also calls the
        // Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }
}
