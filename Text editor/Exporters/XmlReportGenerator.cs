namespace TextEditor.Exporters
{
    public class XmlReportGenerator : ReportGenerator
    {
        protected override string CreateHeader(string title)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Report>";
        }

        protected override string FormatMetaData(string metaInfo)
        {
            return $"  <Metadata>\n    <Info>{metaInfo}</Info>\n  </Metadata>";
        }

        protected override string FormatBody(string text)
        {
            string safeText = text.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
            return $"  <Body>\n    <Content>{safeText}</Content>\n  </Body>";
        }

        protected override string CreateFooter()
        {
            return "</Report>";
        }
    }
}
