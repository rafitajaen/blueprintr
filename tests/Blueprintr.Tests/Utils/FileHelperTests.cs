using Blueprintr.Utils;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="FileHelper"/> utility class that provides file and folder operations.
/// </summary>
[TestFixture]
public class FileHelperTests
{
    private string _testDirectory = null!;

    [SetUp]
    public void SetUp()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileHelperTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    #region TryDeleteFile Tests

    [Test]
    public void TryDeleteFile_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "test-file.txt");
        File.WriteAllText(filePath, "test content");
        Assert.That(File.Exists(filePath), Is.True, "Precondition: File should exist before test");

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.True, "TryDeleteFile should return true when file is successfully deleted");
        Assert.That(File.Exists(filePath), Is.False, "File should no longer exist after deletion");
    }

    [Test]
    public void TryDeleteFile_WithNonExistentFile_ReturnsFalse()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "non-existent-file.txt");
        Assert.That(File.Exists(filePath), Is.False, "Precondition: File should not exist");

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false for non-existent file");
    }

    [Test]
    public void TryDeleteFile_WithNull_ReturnsFalse()
    {
        // Arrange
        string? filePath = null;

        // Act
        var result = FileHelper.TryDeleteFile(filePath!);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false for null path");
    }

    [Test]
    public void TryDeleteFile_WithEmptyString_ReturnsFalse()
    {
        // Arrange
        var filePath = string.Empty;

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false for empty string path");
    }

    [Test]
    public void TryDeleteFile_WithWhitespace_ReturnsFalse()
    {
        // Arrange
        var filePath = "   ";

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false for whitespace-only path");
    }

    [Test]
    [Platform("Win")]
    public void TryDeleteFile_WithFileInUse_ReturnsFalse()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "locked-file.txt");
        File.WriteAllText(filePath, "test content");

        // Lock the file by opening it with FileShare.None (Windows-only behavior)
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false when file is in use");
        Assert.That(File.Exists(filePath), Is.True, "File should still exist when deletion fails due to lock");
    }

    [Test]
    public void TryDeleteFile_WithInvalidCharactersInPath_ReturnsFalse()
    {
        // Arrange
        // Use a path that exists check will fail on (not null/whitespace but invalid)
        var filePath = Path.Combine(_testDirectory, "non-existent-subdir", "file.txt");
        Assert.That(File.Exists(filePath), Is.False, "Precondition: File should not exist");

        // Act
        var result = FileHelper.TryDeleteFile(filePath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFile should return false for file in non-existent directory");
    }

    #endregion

    #region TryDeleteFolder Tests

    [Test]
    public void TryDeleteFolder_WithExistingEmptyFolder_ReturnsTrue()
    {
        // Arrange
        var folderPath = Path.Combine(_testDirectory, "empty-folder");
        Directory.CreateDirectory(folderPath);
        Assert.That(Directory.Exists(folderPath), Is.True, "Precondition: Folder should exist before test");

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath);

        // Assert
        Assert.That(result, Is.True, "TryDeleteFolder should return true when empty folder is successfully deleted");
        Assert.That(Directory.Exists(folderPath), Is.False, "Folder should no longer exist after deletion");
    }

    [Test]
    public void TryDeleteFolder_WithFolderContainingFiles_DeletesRecursively()
    {
        // Arrange
        var folderPath = Path.Combine(_testDirectory, "folder-with-contents");
        Directory.CreateDirectory(folderPath);

        // Create files and nested directories
        File.WriteAllText(Path.Combine(folderPath, "file1.txt"), "content 1");
        File.WriteAllText(Path.Combine(folderPath, "file2.txt"), "content 2");

        var nestedFolder = Path.Combine(folderPath, "nested");
        Directory.CreateDirectory(nestedFolder);
        File.WriteAllText(Path.Combine(nestedFolder, "nested-file.txt"), "nested content");

        Assert.That(Directory.Exists(folderPath), Is.True, "Precondition: Folder should exist");
        Assert.That(Directory.GetFiles(folderPath).Length, Is.EqualTo(2), "Precondition: Folder should contain 2 files");
        Assert.That(Directory.GetDirectories(folderPath).Length, Is.EqualTo(1), "Precondition: Folder should contain 1 subdirectory");

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath);

        // Assert
        Assert.That(result, Is.True, "TryDeleteFolder should return true when folder with contents is successfully deleted");
        Assert.That(Directory.Exists(folderPath), Is.False, "Folder and all contents should no longer exist after recursive deletion");
    }

    [Test]
    public void TryDeleteFolder_WithNonExistentFolder_ReturnsFalse()
    {
        // Arrange
        var folderPath = Path.Combine(_testDirectory, "non-existent-folder");
        Assert.That(Directory.Exists(folderPath), Is.False, "Precondition: Folder should not exist");

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFolder should return false for non-existent folder");
    }

    [Test]
    public void TryDeleteFolder_WithNull_ReturnsFalse()
    {
        // Arrange
        string? folderPath = null;

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath!);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFolder should return false for null path");
    }

    [Test]
    public void TryDeleteFolder_WithEmptyString_ReturnsFalse()
    {
        // Arrange
        var folderPath = string.Empty;

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFolder should return false for empty string path");
    }

    [Test]
    public void TryDeleteFolder_WithWhitespace_ReturnsFalse()
    {
        // Arrange
        var folderPath = "   ";

        // Act
        var result = FileHelper.TryDeleteFolder(folderPath);

        // Assert
        Assert.That(result, Is.False, "TryDeleteFolder should return false for whitespace-only path");
    }

    #endregion
}
