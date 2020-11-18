using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.XmlDiffPatch;

namespace IBAR.Syncer.Application.Helpers
{
    public static class XmlHelper
    {
        public static bool CompareXml(XDocument xDocOrigin, XDocument xDocNew, out string resultXml)
        {
            var resultCompare = false;
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    var xmlDiff = new XmlDiff
                    {
                        Options = XmlDiffOptions.IgnoreWhitespace | XmlDiffOptions.IgnoreComments |
                                  XmlDiffOptions.IgnoreXmlDecl
                    };

                    try
                    {
                        resultCompare = xmlDiff.Compare(xDocOrigin.CreateReader(), xDocNew.CreateReader(), xmlTextWriter);
                    }
                    catch (Exception e)
                    {
                        resultXml = "";
                        return false;
                    }
                }
            }

            if (!resultCompare)
            {
                var xmlDiffView = new XmlDiffView();
                var diffGram = new XmlTextReader(new StringReader(result.ToString()));
                xmlDiffView.Load(xDocOrigin.CreateReader(), diffGram);

                // orig.Close();
                diffGram.Close();

                resultXml = result.Replace("xd:", "").ToString();

                return true;
            }

            resultXml = "";

            return false;
        }

        public static string GetValueBetweenAdded(IEnumerable<XElement> xElements, string name)
        {
            return xElements.FirstOrDefault(el => el.HasAttributes && el.Attribute("name")?.Value == name)?.Value;
        }
    }
}