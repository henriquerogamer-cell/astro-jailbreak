using System.Text.Json.Nodes;
using LibProsperoPkg;
using LibProsperoPkg.PKG;

if (args.Length != 4)
{
    Console.Error.WriteLine("Usage: AstroPkgBuilder <launcher-eboot.bin> <output-dir> <deeplink-uri> <assets-dir>");
    return 2;
}

var ebootSource = Path.GetFullPath(args[0]);
var outputDir = Path.GetFullPath(args[1]);
var deeplinkUri = args[2];
var assetsDir = Path.GetFullPath(args[3]);
const string contentId = "IV9999-PPSX40001_00-ASTROJAILBREAK00";
const string titleId = "PPSX40001";

if (!File.Exists(ebootSource))
    throw new FileNotFoundException("Provide a PS5-compatible eboot.bin.", ebootSource);
Directory.CreateDirectory(outputDir);
var appRoot = Path.Combine(outputDir, "astro-app");
if (Directory.Exists(appRoot)) Directory.Delete(appRoot, recursive: true);
var sceSys = Path.Combine(appRoot, "sce_sys");
Directory.CreateDirectory(sceSys);
File.Copy(ebootSource, Path.Combine(appRoot, "eboot.bin"), overwrite: true);

// Copy the companion runtime for native apps built from the public PS5 boilerplate.
var runtimeSource = Path.Combine(Path.GetDirectoryName(ebootSource)!, "sce_module", "libc.prx");
if (File.Exists(runtimeSource))
{
    var sceModule = Path.Combine(appRoot, "sce_module");
    Directory.CreateDirectory(sceModule);
    File.Copy(runtimeSource, Path.Combine(sceModule, "libc.prx"), overwrite: true);
}

var param = new JsonObject
{
    ["applicationCategoryType"] = 0,
    ["attribute"] = 1,
    ["attribute2"] = 0,
    ["attribute3"] = 4,
    ["contentBadgeType"] = 1,
    ["contentId"] = contentId,
    ["contentVersion"] = "01.000.000",
    ["deeplinkUri"] = deeplinkUri,
    ["downloadDataSize"] = 0,
    ["localizedParameters"] = new JsonObject
    {
        ["defaultLanguage"] = "en-US",
        ["en-US"] = new JsonObject { ["titleName"] = "ASTRO Jailbreak" }
    },
    ["masterVersion"] = "01.00",
    ["originContentVersion"] = "01.000.000",
    ["pubtoolVer"] = "0x05500000",
    ["requiredSystemSoftwareVersion"] = "0x00000000",
    ["sdkVersion"] = "0x0000000000000000",
    ["titleId"] = titleId,
    ["userDefinedParam1"] = 1,
    ["userDefinedParam2"] = 0,
    ["userDefinedParam3"] = 0,
    ["userDefinedParam4"] = 0,
    ["versionFileUri"] = ""
};
File.WriteAllText(Path.Combine(sceSys, "param.json"), param.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

foreach (var name in new[] { "icon0.png", "pic0.png", "pic1.png" })
    File.Copy(Path.Combine(assetsDir, name), Path.Combine(sceSys, name), overwrite: true);
var extraArt = Path.Combine(sceSys, "astro-assets");
Directory.CreateDirectory(extraArt);
foreach (var name in new[] { "logo-astro.png", "background.png", "splash.png" })
    File.Copy(Path.Combine(assetsDir, name), Path.Combine(extraArt, name), overwrite: true);

var result = ProsperoHomebrewPackager.Package(new ProsperoHomebrewPackageOptions
{
    HomebrewFolder = appRoot,
    OutputFolder = outputDir,
    ContentId = contentId,
    Title = "ASTRO Jailbreak",
    Version = "01.00"
}, Console.WriteLine);
Console.WriteLine($"Launch readiness: {result.LaunchReadiness}");
foreach (var warning in result.Warnings) Console.WriteLine($"Warning: {warning}");

var finalPath = Path.Combine(outputDir, "ASTRO-Jailbreak.pkg");
File.Copy(result.OutputPath, finalPath, overwrite: true);
var built = ProsperoPackageExtractor.Inspect(finalPath);
if (built.ContentId != contentId) throw new InvalidDataException($"Unexpected output content id: {built.ContentId}");
if (built.IsRetail) throw new InvalidDataException("Output is not a debug package.");
Console.WriteLine($"Verified package: {finalPath} ({new FileInfo(finalPath).Length} bytes)");
return 0;
