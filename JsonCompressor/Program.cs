// See https://aka.ms/new-console-template for more information
using System.IO.Compression;
using System.Text;

Console.WriteLine($"Starting Resources Files Compression...");

var root = args[0];

//var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
//    .Where(f => f.EndsWith(".json") || f.EndsWith(".csv"));

var jsonFiles = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories);
var csvFiles = Directory.GetFiles(root, "*.csv", SearchOption.AllDirectories);

var files = jsonFiles.Concat(csvFiles);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

foreach (var file in files)
{
    //Since we are changing the way of loading the resources file, it is necessary to have them in utf-8 format instead of shift-jis
    //Convert the file if it is not utf-8 during the compression phase
    if (!IsUtf8(file))
    {
        var text = File.ReadAllText(file, Encoding.GetEncoding("shift_jis"));
        File.WriteAllText(file, text, Encoding.UTF8);
    }

    var output = file + ".gz";

    if (File.Exists(output))
    {
        var srcTime = File.GetLastWriteTimeUtc(file);
        var outTime = File.GetLastWriteTimeUtc(output);

        //we dont want to recompress the file if it is up to date
        if (outTime >= srcTime)
            continue; 
    }

    Console.WriteLine($"Compressing: {file}");

    using var input = File.OpenRead(file);
    using var outputStream = File.Create(output);
    using var gzip = new GZipStream(outputStream, CompressionLevel.Optimal);

    input.CopyTo(gzip);
}

Console.WriteLine($"Resources Files Compression Done!");


bool IsUtf8(string filePath)
{
    try
    {
        var bytes = File.ReadAllBytes(filePath);

        // Throw if invalid UTF-8
        var utf8 = new UTF8Encoding(false, true);
        utf8.GetString(bytes);

        return true;
    }
    catch
    {
        return false;
    }
}