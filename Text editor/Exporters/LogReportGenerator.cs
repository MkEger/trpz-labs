using System;

namespace TextEditor.Exporters
{
    public class LogReportGenerator : ReportGenerator
    {
        protected override string CreateHeader(string title)
        {
            string fileName = System.IO.Path.GetFileName(title);
            return "========================================\n" +
                   $"LOG REPORT: {fileName.ToUpper()}\n" +
                   "========================================";
        }

        protected override string FormatBody(string text)
        {
            return $"\n>>> BEGIN CONTENT >>>\n{text}\n<<< END CONTENT <<<";
        }

        protected override string CreateFooter()
        {
            return "\n----------------------------------------\n" +
                   $"End of Report. Checksum: {Guid.NewGuid().ToString().Substring(0, 8)}\n" +
                   "----------------------------------------";
        }
    }
}
