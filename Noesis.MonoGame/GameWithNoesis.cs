using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Noesis.MonoGame;

public class GameWithNoesis : Microsoft.Xna.Framework.Game
{
    public string? NoesisLicenceName; 
    public string? NoesisLicenceKey;
    public string? NoesisThemeFilename;
    public string? NoesisMainViewFilename;
    public uint NoesisAntiAlliasingOffscreenSampleCount = 0;

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _log;
    private readonly Microsoft.Xna.Framework.GraphicsDeviceManager _graphicsDeviceManager;
    private Microsoft.Xna.Framework.Input.MouseState _previousState;
    private NoesisViewWrapper MainView = null!;

    public GameWithNoesis(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _log = _loggerFactory.CreateLogger<GameWithNoesis>();

        _graphicsDeviceManager = new(this)
        {
            GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.HiDef
        };        
    }
    
    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (sender, e) => { MainView.SetSize(Window.ClientBounds.Width, Window.ClientBounds.Height); };
        
        _graphicsDeviceManager.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24Stencil8;
        _graphicsDeviceManager.ApplyChanges();        

        InitializeNoesis();

        // TODO: Add your initialization logic here

        base.Initialize();
    }

    public void InitializeNoesis()
    {
        // ensure the Noesis.App assembly is loaded otherwise NoesisGUI will be unable to located "Window" type
        System.Reflection.Assembly.Load("Noesis.App");

        // setup Noesis Debug callbacks
        Log.SetLogCallback(NoesisLogCallbackHandler);
        Error.SetUnhandledCallback(NoesisUnhandledExceptionHandler);

        GUI.SetLicense(NoesisLicenceName, NoesisLicenceKey);
        GUI.Init();

        GUI.SetFontProvider(new FolderFontProvider());
        GUI.SetTextureProvider(new FolderTextureProvider(GraphicsDevice));
        GUI.SetXamlProvider(new FolderXamlProvider());
        
        if (NoesisThemeFilename is not null)
        {
            GUI.LoadApplicationResources(NoesisThemeFilename.Replace('/', '\\'));
        }

        if (NoesisMainViewFilename is not null)
        {
            FrameworkElement controlTreeRoot = (FrameworkElement)GUI.LoadXaml(NoesisMainViewFilename);

            NoesisRenderDevice renderDevice = new(GraphicsDevice, Content, _loggerFactory);

            // TODO: increased to deal with the glyph cache crash - refactor to move to NoesisConfig
            renderDevice.GlyphCacheWidth = renderDevice.GlyphCacheHeight = 2048;
            renderDevice.OffscreenSampleCount = NoesisAntiAlliasingOffscreenSampleCount;

            MainView = new NoesisViewWrapper(controlTreeRoot, renderDevice)
            {
                RenderFlags = RenderFlags.LCD | RenderFlags.PPAA
            };

            MainView.SetSize(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        Window.KeyDown += (sender, e) => { MainView.View.KeyDown(KeyConverter.Convert(e.Key)); };
        Window.KeyUp += (sender, e) => { MainView.View.KeyUp(KeyConverter.Convert(e.Key)); };
        Window.TextInput += (sender, e) => { MainView.View.Char(e.Character); };
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
        GUI.UnregisterNativeTypes();

        // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        if (Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
            Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
        {
            Exit();

            return;
        }        
        
        UpdateInput(gameTime, IsActive);

        // TODO: Add your game logic update code here

        base.Update(gameTime);

        // update NoesisGUI after updating game logic (it will perform layout and other operations)
        MainView.Update(gameTime);
    }

    /// <summary>
    /// Updates NoesisGUI input.
    /// </summary>
    /// <param name="gameTime">Current game time.</param>
    /// <param name="isWindowActive">Is game focused?</param>
    public void UpdateInput(Microsoft.Xna.Framework.GameTime gameTime, bool isWindowActive)
    {
        UpdateMouse(gameTime, isWindowActive);
    }

    public void UpdateMouse(Microsoft.Xna.Framework.GameTime gameTime, bool isWindowActive)
    {
        Microsoft.Xna.Framework.Input.MouseState state = Microsoft.Xna.Framework.Input.Mouse.GetState();

        if (isWindowActive)
        {
            if (_previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
                state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                MainView.View.MouseButtonDown(state.X, state.Y, MouseButton.Left);

            if (_previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                MainView.View.MouseButtonUp(state.X, state.Y, MouseButton.Left);

            if (_previousState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
                state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                MainView.View.MouseButtonDown(state.X, state.Y, MouseButton.Right);

            if (_previousState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                MainView.View.MouseButtonUp(state.X, state.Y, MouseButton.Right);

            if (state.ScrollWheelValue != _previousState.ScrollWheelValue)
                MainView.View.MouseWheel(state.X, state.Y, state.ScrollWheelValue - _previousState.ScrollWheelValue);

            if (state.HorizontalScrollWheelValue != _previousState.HorizontalScrollWheelValue)
                MainView.View.MouseHWheel(state.X, state.Y, state.HorizontalScrollWheelValue - _previousState.HorizontalScrollWheelValue);

            if (state.X != _previousState.X ||
                state.Y != _previousState.Y)
                MainView.View.MouseMove(state.X, state.Y);
        }

        _previousState = state;
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        MainView.PreRender();

        GraphicsDevice.Clear(
            Microsoft.Xna.Framework.Graphics.ClearOptions.Target |
            Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer |
            Microsoft.Xna.Framework.Graphics.ClearOptions.Stencil,
            Microsoft.Xna.Framework.Color.LightBlue,
            depth: 1,
            stencil: 0);

        // TODO: Add your drawing code here

        MainView.Render();

        base.Draw(gameTime);
    }          

    private void NoesisLogCallbackHandler(LogLevel level, string channel, string message)
    {
        switch (level)
        {
            case LogLevel.Trace:
                _log.LogTrace(new EventId((int)LogLevel.Trace, channel), message);
                break;
            case LogLevel.Debug:
                _log.LogDebug(new EventId((int)LogLevel.Debug, channel), message);
                break;
            case LogLevel.Info:
                _log.LogInformation(new EventId((int)LogLevel.Info, channel), message);
                break;
            case LogLevel.Warning:
                _log.LogWarning(new EventId((int)LogLevel.Warning, channel), message);
                break;
            case LogLevel.Error:
                _log.LogError(new EventId((int)LogLevel.Error, channel), message);
                break;
        }
    }

    private void NoesisUnhandledExceptionHandler(Exception exception)
    {
        _log.LogError(exception, exception.Message);
    }
}