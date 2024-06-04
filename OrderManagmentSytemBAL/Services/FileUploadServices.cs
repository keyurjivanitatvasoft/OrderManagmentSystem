using Microsoft.AspNetCore.Http;

namespace OrderManagmentSytemBAL.Services
{
    public class FileUploadServices
    {
       
        public static string GetActualFileName(string filename)
        {
            if (filename.Length >= 36)
            {
                return filename.Substring(0, filename.Length - 36);
            }
            else
            {
                return filename;
            }
        }
        public static string GetFilenameWithoutExtension(string fileName)
        {
            int lastDotIndex = fileName.LastIndexOf('.');
            if (lastDotIndex == -1) // If there's no dot in the filename
            {
                return fileName;
            }
            else
            {
                return fileName.Substring(0, lastDotIndex);
            }
        }
    }
}
