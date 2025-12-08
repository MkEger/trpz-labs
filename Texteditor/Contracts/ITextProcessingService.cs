using System;

namespace Texteditor.Contracts
{
    /// <summary>
    /// Request model for text processing operations (SOA)
    /// </summary>
    [Serializable]
    public class TextProcessingRequest
    {
        public string Text { get; set; }
        public string Operation { get; set; }
        public string UserId { get; set; }
    }

    /// <summary>
    /// Response model for text processing operations (SOA)
    /// </summary>
    [Serializable]
    public class TextProcessingResponse
    {
        public string ProcessedText { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ProcessingTime { get; set; }
    }

    /// <summary>
    /// Service contract for text processing operations (SOA pattern)
    /// </summary>
    public interface ITextProcessingService
    {
        TextProcessingResponse ProcessText(TextProcessingRequest request);
        TextProcessingResponse FormatText(string text, string formatType);
        TextProcessingResponse CheckSpelling(string text);
        string[] GetAvailableFormats();
    }
}