namespace Nucleus.Misc.Utilities;

/// <summary>
///     Represents a command line console for interacting with the user.
/// </summary>
public sealed class NConsole
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NConsole" /> class.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    public NConsole(string appName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appName);
        _appName = appName;

        // process exit requests
        OnUserInput += (_, args) =>
        {
            if (args.UserInput?.ToLowerInvariant().Trim() is "exit" or "ex" or "exit()" or "bye")
                Exit();
        };

        Console.CancelKeyPress += (_, _) => Exit();
    }

    #region Private Helper Methods

    private void PrintBanner()
    {
        AnsiConsole.Write(Banner is null
            ? new FigletText(_appName)
            : new FigletText(Banner)
        );
    }

    private void Write(string? message, NLogLevel logLevel, bool writeLine = true)
    {
        if (LogLevel > logLevel) return;

        var color = logLevel switch
        {
            NLogLevel.Debug => ConsoleColor.Gray,
            NLogLevel.Info => ConsoleColor.Green,
            NLogLevel.Warn => ConsoleColor.DarkYellow,
            NLogLevel.Error => ConsoleColor.DarkRed,
            NLogLevel.Fatal => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };

        var markup = $"[{color}]{message.EscapeMarkup()}[/]";

        if (writeLine)
            AnsiConsole.MarkupLine(markup);
        else
            AnsiConsole.Write(markup);
    }

    #endregion

    #region Private Fields

    private readonly string _appName;

    private bool _isExiting;

    #endregion

    #region Public Properties

    /// <summary>
    ///     The log level of the shell.
    /// </summary>
    public NLogLevel LogLevel { get; set; } = NLogLevel.Debug;

    /// <summary>
    ///     Indicates whether the shell should echo the user input.
    /// </summary>
    public bool EchoInput { get; set; } = false;

    /// <summary>
    ///     The prompt string displayed to the user.
    /// </summary>
    public string Prompt { get; set; } = ">";

    /// <summary>
    ///     The color of the prompt text.
    /// </summary>
    public ConsoleColor PromptColor { get; set; } = ConsoleColor.Cyan;

    /// <summary>
    ///     The banner text displayed to the user.
    /// </summary>
    public string? Banner { get; set; }

    /// <summary>
    ///     Occurs when user input is received.
    /// </summary>
    public event EventHandler<NUserInputEventArgs>? OnUserInput;

    #endregion

    #region Public Methods

    /// <summary>
    ///     Writes a debug message to the console.
    /// </summary>
    /// <param name="message"></param>
    public void WriteDebug(string? message = null)
    {
        Write(message, NLogLevel.Debug, false);
    }

    /// <summary>
    ///     Writes a debug message to the console followed by a newline.
    /// </summary>
    /// <param name="message"></param>
    public void WriteDebugLine(string? message = null)
    {
        Write(message, NLogLevel.Debug);
    }

    /// <summary>
    ///     Writes an informational message to the console.
    /// </summary>
    /// <param name="message"></param>
    public void WriteInfo(string? message = null)
    {
        Write(message, NLogLevel.Info, false);
    }

    /// <summary>
    ///     Writes an informational message to the console followed by a newline.
    /// </summary>
    /// <param name="message"></param>
    public void WriteInfoLine(string? message = null)
    {
        Write(message, NLogLevel.Info);
    }

    /// <summary>
    ///     Writes a warning message to the console.
    /// </summary>
    /// <param name="message"></param>
    public void WriteWarn(string? message = null)
    {
        Write(message, NLogLevel.Warn, false);
    }

    /// <summary>
    ///     Writes a warning message to the console followed by a newline.
    /// </summary>
    /// <param name="message"></param>
    public void WriteWarnLine(string? message = null)
    {
        Write(message, NLogLevel.Warn);
    }

    /// <summary>
    ///     Writes an error message to the console.
    /// </summary>
    /// <param name="message"></param>
    public void WriteError(string? message = null)
    {
        Write(message, NLogLevel.Error, false);
    }

    /// <summary>
    ///     Writes an error message to the console followed by a newline.
    /// </summary>
    /// <param name="message"></param>
    public void WriteErrorLine(string? message = null)
    {
        Write(message, NLogLevel.Error);
    }

    /// <summary>
    ///     Writes a fatal message to the console.
    /// </summary>
    /// <param name="message"></param>
    public void WriteFatal(string? message = null)
    {
        Write(message, NLogLevel.Fatal, false);
    }

    /// <summary>
    ///     Writes a fatal message to the console followed by a newline.
    /// </summary>
    /// <param name="message"></param>
    public void WriteFatalLine(string? message = null)
    {
        Write(message, NLogLevel.Fatal);
    }

    /// <summary>
    ///     Executes the shell asynchronously, processing user input until exiting.
    /// </summary>
    public void Run()
    {
        PrintBanner();

        while (!_isExiting)
        {
            var userInput = AnsiConsole.Ask<string>($"[{PromptColor}]{Prompt}[/] ");

            if (EchoInput) Console.WriteLine(userInput);

            // notify subscribers
            OnUserInput?.Invoke(this, new NUserInputEventArgs { UserInput = userInput });
        }

        AnsiConsole.MarkupLine($"[{PromptColor}]Goodbye![/]");
    }

    /// <summary>
    ///     Hints the shell to exit.
    /// </summary>
    public void Exit()
    {
        _isExiting = true;
    }

    #endregion
}