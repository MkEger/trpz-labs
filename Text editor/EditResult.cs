namespace TextEditor
{
    public class EditResult
    {
        public string ModifiedText { get; set; }
        public int NewSelectionStart { get; set; }
        public int NewSelectionLength { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public EditResult(string modifiedText, int newSelectionStart, int newSelectionLength = 0, bool success = true, string message = "")
        {
            ModifiedText = modifiedText;
            NewSelectionStart = newSelectionStart;
            NewSelectionLength = newSelectionLength;
            Success = success;
            Message = message;
        }
    }
}
