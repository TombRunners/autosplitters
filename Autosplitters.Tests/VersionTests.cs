using FluentAssertions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Autosplitters.Tests;

public class VersionTests
{
    [Theory]
    [InlineData("TR123", "TR123")]
    [InlineData("TR456", "TR456")]
    [InlineData("TombRaider1996", "TR1996")]
    [InlineData("TombRaiderII", "TR2")]
    [InlineData("TombRaiderIII", "TR3")]
    [InlineData("TombRaiderIV", "TR4")]
    [InlineData("TombRaiderV", "TR5")]
    public void Version_Strings_Should_Be_Equal(string project, string dll)
    {
        var currentDirectory = Directory.GetCurrentDirectory(); // autosplitters\Autosplitters.Tests\bin\[Debug/Release]

        var assemblyInfoPath = Path.Combine(currentDirectory, $"../../../{project}/Properties/AssemblyInfo.cs");
        var assemblyInfoContents = File.ReadAllText(assemblyInfoPath);
        var assemblyInfoVersionString = Regex.Matches(assemblyInfoContents, """\[assembly: AssemblyVersion\("(?<VersionString>.+)"\)]""")
                                             .Cast<Match>()
                                             .Last()
                                             .Groups["VersionString"]
                                             .Value;
        var assemblyInfoVersion = ParseVersion(assemblyInfoVersionString);

        var updateXmlPath = Path.Combine(currentDirectory, $"../../../{project}/Components/update.xml");
        var updateXmlContents = File.ReadAllText(updateXmlPath);
        var updateXmlVersionString = Regex.Match(updateXmlContents, """<update version="(?<VersionString>.+)">""")
                                          .Groups["VersionString"]
                                          .Value;
        var updateXmlVersion = ParseVersion(updateXmlVersionString);

        var dllPath = Path.Combine(currentDirectory, $"../../../{project}/Components/{dll}.dll");
        var dllVersionInfo = FileVersionInfo.GetVersionInfo(dllPath).FileVersion;
        var dllVersion = ParseVersion(dllVersionInfo);

        assemblyInfoVersion.Should().Be(dllVersion, $"Assembly Info version {assemblyInfoVersionString} must match DLL version {dllVersionInfo}");
        updateXmlVersion.Should().Be(dllVersion, $"Update XML version {updateXmlVersionString} must match DLL version {dllVersionInfo}");
    }

    private static Version ParseVersion(string versionString)
    {
        var version = new Version(versionString);

        return version.Revision == -1
            ? new Version(version.Major, version.Minor, version.Build, 0)
            : version;
    }
}
