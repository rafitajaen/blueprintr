using Blueprintr.Utils;
using NUnit.Framework;
using System.Reflection;

namespace Blueprintr.Tests.Utils;

[TestFixture]
public class AssemblyInformationTests
{
    private Assembly _testAssembly = null!;

    [SetUp]
    public void SetUp()
    {
        _testAssembly = typeof(AssemblyInformationTests).Assembly;
    }

    [Test]
    public void GetCompanyName_ShouldReturnCompanyOrUnknown()
    {
        var result = _testAssembly.GetCompanyName();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetConfiguration_ShouldReturnConfiguration()
    {
        var result = _testAssembly.GetConfiguration();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("Debug").Or.EqualTo("Release").Or.EqualTo("<unknown>"));
    }

    [Test]
    public void GetCopyright_ShouldReturnCopyrightOrUnknown()
    {
        var result = _testAssembly.GetCopyright();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetDescription_ShouldReturnDescriptionOrUnknown()
    {
        var result = _testAssembly.GetDescription();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetProduct_ShouldReturnProductOrUnknown()
    {
        var result = _testAssembly.GetProduct();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetTitle_ShouldReturnTitleOrUnknown()
    {
        var result = _testAssembly.GetTitle();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetTargetFramework_ShouldReturnFrameworkOrUnknown()
    {
        var result = _testAssembly.GetTargetFramework();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAppVersion_ShouldReturnVersionOrUnknown()
    {
        var result = _testAssembly.GetAppVersion();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetGitCommit_ShouldReturnCommitOrUnknown()
    {
        var result = _testAssembly.GetGitCommit();
        Assert.That(result, Is.Not.Null);
    }
}
