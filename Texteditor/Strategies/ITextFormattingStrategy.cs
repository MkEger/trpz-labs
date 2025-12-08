namespace Texteditor.Strategies
{
    /// <summary>
    /// Strategy pattern interface for text formatting operations
    /// </summary>
    public interface ITextFormattingStrategy
    {
        string FormatText(string text);
        string Description { get; }
    }
}