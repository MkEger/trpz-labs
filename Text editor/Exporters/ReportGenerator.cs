using System;
using System.IO;
using System.Text;

namespace TextEditor.Exporters
{
    public abstract class ReportGenerator
    {
        public void ExportDocument(string filePath, string content)
        {
            var sb = new StringBuilder();

            sb.AppendLine(CreateHeader(filePath));

            string sysInfo = $"Generated: {DateTime.Now} | Size: {content.Length} chars";
            sb.AppendLine(FormatMetaData(sysInfo));

            sb.AppendLine(FormatBody(content));

            sb.AppendLine(CreateFooter());

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        protected abstract string CreateHeader(string title);
        protected abstract string FormatBody(string text);
        protected abstract string CreateFooter();

        protected virtual string FormatMetaData(string metaInfo)
        {
            return $"[SYSTEM INFO] {metaInfo}";
        }
    }
}
