using Boilerplatr.Utils;

namespace Boilerplatr.Exceptions;

/// <summary>
/// The exception that is thrown when one of the arguments provided to a method is not valid.
/// </summary>
public class InvalidArgumentException : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the System.ArgumentException class.
    /// </summary>
    public InvalidArgumentException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the System.ArgumentException class with a specified error message.
    /// </summary>
    /// 
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    public InvalidArgumentException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the System.ArgumentException class with a specified
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// 
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// 
    /// <param name="innerException">
    /// The exception that is the cause of the current exception. If the innerException
    /// parameter is not a null reference, the current exception is raised in a catch
    /// block that handles the inner exception.
    /// </param>
    public InvalidArgumentException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the System.ArgumentException class with a specified
    /// error message and the name of the parameter that causes this exception.
    /// </summary>
    /// 
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// 
    /// <param name="paramName">
    /// The name of the parameter that caused the current exception.
    /// </param>
    public InvalidArgumentException(string? message, string? paramName) : base(message, paramName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the System.ArgumentException class with a specified
    /// error message, the parameter name, and a reference to the inner exception that
    /// is the cause of this exception.
    /// </summary>
    /// 
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// 
    /// <param name="paramName">
    /// The name of the parameter that caused the current exception.
    /// </param>
    /// 
    /// <param name="innerException">
    /// The exception that is the cause of the current exception. If the innerException
    /// parameter is not a null reference, the current exception is raised in a catch
    /// block that handles the inner exception.
    /// </param>
    public InvalidArgumentException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException)
    {
    }

    /// <summary>
    /// Throws an exception if argument is null, empty, or consists only of white-space characters or contains non-alphanumeric characters.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException">
    /// argument is null.
    /// </exception>
    /// 
    /// <exception cref="ArgumentException">
    /// argument is empty or consists only of white-space characters.
    /// </exception>
    /// 
    /// <exception cref="InvalidArgumentException">
    /// argument contains non-alphanumeric characters
    /// </exception>
    public static void ThrowIfNullOrWhiteSpaceOrNonAlphanumeric(string value, string? paramName = null)
    {
        ThrowIfNullOrWhiteSpace(value, paramName);

        if (!StringUtilities.IsAlphabetic(value))
        {
            throw new InvalidArgumentException("String argument contains non-alphanumeric characters", paramName);
        }
    }
}