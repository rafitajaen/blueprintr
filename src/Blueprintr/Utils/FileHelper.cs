namespace Blueprintr.Utils;

/// <summary>
/// Provides utility methods for file and folder operations.
/// </summary>
/// <remarks>
/// This class contains helper methods for safely deleting files and folders,
/// handling exceptions gracefully and returning success/failure status.
/// Added in version 1.0.0.
/// </remarks>
public static class FileHelper
{
    /// <summary>
    /// Attempts to delete a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file to delete.</param>
    /// <returns>
    /// <c>true</c> if the file was successfully deleted; otherwise, <c>false</c>.
    /// Returns <c>false</c> if the path is null/empty, the file doesn't exist, or an exception occurs.
    /// </returns>
    /// <remarks>
    /// This method silently catches and ignores any exceptions that occur during deletion.
    /// It performs validation to ensure the path is not null or whitespace and that the file exists
    /// before attempting deletion.
    /// Added in version 1.0.0.
    /// </remarks>
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

    /// <summary>
    /// Attempts to delete a folder and all its contents at the specified path.
    /// </summary>
    /// <param name="folderPath">The path to the folder to delete.</param>
    /// <returns>
    /// <c>true</c> if the folder was successfully deleted; otherwise, <c>false</c>.
    /// Returns <c>false</c> if the path is null/empty, the folder doesn't exist, or an exception occurs.
    /// </returns>
    /// <remarks>
    /// This method recursively deletes the folder and all its contents.
    /// It silently catches and ignores any exceptions that occur during deletion.
    /// It performs validation to ensure the path is not null or whitespace and that the folder exists
    /// before attempting deletion.
    /// Added in version 1.0.0.
    /// </remarks>
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
