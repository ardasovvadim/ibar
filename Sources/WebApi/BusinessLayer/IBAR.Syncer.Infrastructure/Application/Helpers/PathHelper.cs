using System.Configuration;
using System.IO;
using System.Linq;

namespace IBAR.Syncer.Infrastructure.Application.Helpers
{
    public static class PathHelper
    {
        private static readonly string _histDir = ConfigurationManager.AppSettings["historyFolder"];

        public static string GetPathToFileInHistoryDir(string filePath)
        {
            var fileDir = Path.GetFileNameWithoutExtension(filePath);
            fileDir = new DirectoryInfo(_histDir).GetFileSystemInfos($"{fileDir}*").FirstOrDefault()?.FullName;
            // do
            // {
            //     switch (Path.GetExtension(originFileName))
            //     {
            //         case ".xml":
            //             canContinue = false;
            //             break;
            //
            //         case ".asc":
            //         case ".gpt":
            //             originFileName = Path.GetFileNameWithoutExtension(originFileName);
            //             break;
            //         case ".gz":
            //             originFileName = Path.GetFileNameWithoutExtension(originFileName);
            //             break;
            //         default:
            //             canContinue = false;
            //             break;
            //     }
            // }
            // while (canContinue && ++i < 3);
            return Path.Combine(_histDir, fileDir, filePath);
        }
    }
}