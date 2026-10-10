using System.Text.Json;
using System.Text.Json.Nodes;
using LibProsperoPkg;
using LibProsperoPkg.PKG;

if (args.Length != 4)
{
    Console.Error.WriteLine("Usage: AstroPkgBuilder <source.pkg> <output-dir> <deeplink-uri> <assets-dir>");
    return 2;
}

var sourcePkg = Path.GetFullPath(args[0]);
var outputDir = Path.GetFullPath(args[1]);
var deeplinkUri = args[2];
var assetsDir = Path.GetFullPath(args[3]);
const string contentId = "IV9999-PPSX40001_00-ASTROJAILBREAK00";
const string titleId = "PPSX40001";

Directory.CreateDirectory(outputDir);
var inspection = ProsperoPackageExtractor.Inspect(sourcePkg);
Console.WriteLine($"Source package: {inspection.PackageType}; content id: {inspection.ContentId}; retail: {inspection.IsRetail}");
if (inspection.RequiresSuppliedKey)
    throw new InvalidOperationException("The source package requires a console supplied key and cannot be used by this build.");

var extracted = Path.Combine(outputDir, "extracted");
if (Directory.Exists(extracted)) Directory.Delete(extracted, recursive: true);
ProsperoPackageExtractor.Extract(sourcePkg, extracted, new string('0', 32), Console.WriteLine);

var eboot = Directory.GetFiles(extracted, "eboot.bin", SearchOption.AllDirectories).FirstOrDefault()
    ?? throw new FileNotFoundException("The reference package did not contain eboot.bin.");
var appRoot = Path.GetDirectoryName(eboot)!;
var sceSys = Path.Combine(appRoot, "sce_sys");
Directory.CreateDirectory(sceSys);
var paramPath = Path.Combine(sceSys, "param.json");
if (!File.Exists(paramPath))
    throw new FileNotFoundException("The reference package did not contain sce_sys/param.json.");

var param = JsonNode.Parse(File.ReadAllText(paramPath)) as JsonObject
    ?? throw new InvalidDataException("sce_sys/param.json is not a JSON object.");
param["contentId"] = contentId;
param["titleId"] = titleId;
param["masterVersion"] = "01.00";
param["contentVersion"] = "01.00";
param["deeplinkUri"] = deeplinkUri;
var localized = param["localizedParameters"] as JsonArray;
if (localized is null)
{
    localized = new JsonArray();
    param["localizedParameters"] = localized;
}
if (localized.Count == 0)
    localized.Add(new JsonObject { ["defaultLanguage"] = "en-US", ["titleName"] = "ASTRO Jailbreak" });
foreach (var item in localized)
    if (item is JsonObject entry) entry["titleName"] = "ASTRO Jailbreak";
File.WriteAllText(paramPath, param.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

foreach (var name in new[] { "icon0.png", "pic0.png", "pic1.png" })
{
    var from = Path.Combine(assetsDir, name);
    if (!File.Exists(from)) throw new FileNotFoundException("Missing artwork", from);
    File.Copy(from, Path.Combine(sceSys, name), overwrite: true);
}
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
Console.WriteLine($"Build result: {result.LaunchReadiness}");
foreach (var warning in result.Warnings) Console.WriteLine($"Warning: {warning}");

var finalPath = Path.Combine(outputDir, "ASTRO-Jailbreak.pkg");
File.Copy(result.OutputPath, finalPath, overwrite: true);
var built = ProsperoPackageExtractor.Inspect(finalPath);
if (built.ContentId != contentId) throw new InvalidDataException($"Unexpected output content id: {built.ContentId}");
if (built.IsRetail) throw new InvalidDataException("Output is not a debug package.");
Console.WriteLine($"Verified package: {finalPath} ({new FileInfo(finalPath).Length} bytes)");
return 0;
