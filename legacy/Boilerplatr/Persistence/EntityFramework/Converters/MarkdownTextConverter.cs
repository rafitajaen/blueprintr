using Boilerplatr.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class MarkdownTextConverter : ValueConverter<MarkdownText, string>
{
    public MarkdownTextConverter() : base
    (
        normalized => normalized.Value,
        s => new MarkdownText(s)
    ) { }
}
