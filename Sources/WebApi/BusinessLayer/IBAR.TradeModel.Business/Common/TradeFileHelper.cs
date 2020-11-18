namespace IBAR.TradeModel.Business.Common
{
    public static class TradeFileHelper
    {
        public static string GetCorrectFileName(string fileName)
        {
            var pattern = ".xml";
            var i = fileName?.IndexOf(pattern);

            return (i.HasValue && i.Value != -1) ? fileName.Substring(0, i.Value + pattern.Length) : fileName;
        }
    }
}
