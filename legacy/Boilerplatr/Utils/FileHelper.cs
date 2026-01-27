namespace Boilerplatr.Utils;

public static class FileHelper
{
    public static bool TryDeleteFile(string filePath)
    {
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch { }
        }

        return false;
    }

    public static bool TryDeleteFolder(string folderPath)
    {
        if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
        {
            try
            {
                Directory.Delete(folderPath, recursive: true);
                return true;
            }
            catch { }
        }

        return false;
    }
}
