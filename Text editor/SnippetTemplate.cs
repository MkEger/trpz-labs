using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextEditor
{
    public class SnippetTemplate
    {
        public string Template { get; set; }
        public Dictionary<string, string> DefaultValues { get; set; }

        public SnippetTemplate(string template, Dictionary<string, string> defaultValues)
        {
            Template = template ?? string.Empty;
            DefaultValues = defaultValues ?? new Dictionary<string, string>();
        }

        public string Expand(Dictionary<string, string> customValues = null)
        {
            string result = Template;

            var allValues = new Dictionary<string, string>(DefaultValues);
            if (customValues != null)
            {
                foreach (var kvp in customValues)
                {
                    allValues[kvp.Key] = kvp.Value;
                }
            }

            foreach (var kvp in allValues)
            {
                result = result.Replace($"${{{kvp.Key}}}", kvp.Value);
            }

            return result;
        }
    }
}
