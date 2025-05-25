using System;
using System.Threading.Tasks;
using PuppeteerSharp;

class Program
{
    static async Task Main(string[] args)
    {
        string url;
        int width = 1080; // default width
        int? height = null; // null means "full page"

        // Parse arguments
        if (args.Length < 1)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run <url> [width] [height] [filename] [open]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  url       (required)  The URL to capture (must start with http:// or https://)");
            Console.WriteLine("  width     (optional)  Width of the viewport in pixels (default: 1080, 0 for default)");
            Console.WriteLine("  height    (optional)  Height of the viewport in pixels (default: full page, 0 for full page)");
            Console.WriteLine("  filename  (optional)  Output PNG file name (default: <domain>.png)");
            Console.WriteLine("  open      (optional)  Set to 1 to open the image after saving (default: 0)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run https://example.com");
            Console.WriteLine("  dotnet run https://example.com 1280 0");
            Console.WriteLine("  dotnet run https://example.com 1024 2000 screenshot.png 1");
            return;
        }
        url = args[0];
        if (args.Length > 1 && int.TryParse(args[1], out int w)) width = w == 0 ? 1080 : w;
        if (args.Length > 2 && int.TryParse(args[2], out int h)) height = h == 0 ? null : h;

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? outUri) || outUri is null ||
            (outUri.Scheme != Uri.UriSchemeHttp && outUri.Scheme != Uri.UriSchemeHttps))
        {
            Console.WriteLine("Invalid URL.");
            return;
        }

        // Optional output file name as 4th argument, open flag as 5th
        string fileName;
        bool openImage = false;
        if (args.Length > 4)
        {
            // 5 arguments: url width height filename openflag
            fileName = !string.IsNullOrWhiteSpace(args[3])
                ? (args[3].EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? args[3] : args[3] + ".png")
                : $"{outUri.Host}.png";
            openImage = args[4] == "1";
        }
        else if (args.Length > 3)
        {
            // 4 arguments: url width height openflag OR filename
            if (args[3] == "1" || args[3] == "0")
            {
                fileName = $"{outUri.Host}.png";
                openImage = args[3] == "1";
            }
            else
            {
                fileName = !string.IsNullOrWhiteSpace(args[3])
                    ? (args[3].EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? args[3] : args[3] + ".png")
                    : $"{outUri.Host}.png";
            }
        }
        else
        {
            fileName = $"{outUri.Host}.png";
        }

        try
        {
            // Download Chromium if not already present
            await new BrowserFetcher().DownloadAsync();
            Console.WriteLine("Downloading Chromium...");

            // Launch headless browser
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            using var page = await browser.NewPageAsync();

            // Go to the URL
            Console.WriteLine("Navigating to the URL...");
            await page.GoToAsync(url);

            int viewportHeight;
            if (height.HasValue)
            {
                viewportHeight = height.Value;
            }
            else
            {
                // Get full page height
                viewportHeight = await page.EvaluateExpressionAsync<int>("document.body.scrollHeight");
            }

            Console.WriteLine("Setting viewport...");
            await page.SetViewportAsync(new ViewPortOptions { Width = width, Height = viewportHeight });

            // Screenshot as PNG
            Console.WriteLine("Taking screenshot...");
            await page.ScreenshotAsync(fileName);

            Console.WriteLine($"Web page rendered and saved as {fileName}");

            // Open image if requested
            if (openImage)
            {
                var psi = new System.Diagnostics.ProcessStartInfo();
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    psi.FileName = "cmd";
                    psi.ArgumentList.Add("/c");
                    psi.ArgumentList.Add("start");
                    psi.ArgumentList.Add("");
                    psi.ArgumentList.Add(fileName);
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                {
                    psi.FileName = "open";
                    psi.ArgumentList.Add(fileName);
                }
                else
                {
                    psi.FileName = "xdg-open";
                    psi.ArgumentList.Add(fileName);
                }
                psi.UseShellExecute = false;
                System.Diagnostics.Process.Start(psi);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching or rendering data: {ex.Message}");
        }
    }
}