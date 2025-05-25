# Webber

**Webber** is a cross-platform command-line tool for capturing full-page screenshots of websites as PNG images. It uses [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) to render web pages in a headless Chromium browser, allowing you to automate website previews and documentation.

---

## Features

- Capture screenshots of any website (HTTP/HTTPS)
- Specify viewport width and height, or capture the full page
- Save screenshots with a custom filename or use the domain name by default
- Optionally open the screenshot after saving
- Progress and status messages for each step

---

## Usage

```sh
webber <url> [width] [height] [filename] [open]
```

### Arguments

| Argument   | Required | Description                                                                                  |
|------------|----------|----------------------------------------------------------------------------------------------|
| url        | Yes      | The URL to capture (must start with `http://` or `https://`)                                 |
| width      | No       | Width of the viewport in pixels (default: 1080, use `0` for default)                         |
| height     | No       | Height of the viewport in pixels (default: full page, use `0` for full page)                 |
| filename   | No       | Output PNG file name (default: `<domain>.png`)                                               |
| open       | No       | Set to `1` to open the image after saving (default: `0`)                                     |

### Examples

```sh
webber https://example.com
webber https://example.com 1280 0
webber https://example.com 1024 2000 screenshot.png 1
webber https://example.com 0 0 "" 1
```

---

## Installation

### Build a Self-Contained Executable

1. **Publish for your platform:**

   ```sh
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
   ```

   Replace `linux-x64` with `win-x64` or `osx-x64` for Windows or macOS.

2. **Make the binary executable (Linux/macOS):**

   ```sh
   chmod +x bin/Release/net*/linux-x64/publish/Webber
   ```

3. **(Optional) Move to a directory in your `$PATH`:**

   ```sh
   sudo mv bin/Release/net*/linux-x64/publish/Webber /usr/local/bin/webber
   ```

---

## Platform Support

- **Linux:** Uses `xdg-open` to open images.
- **macOS:** Uses `open` to open images.
- **Windows:** Uses `start` command to open images.

---

## License

MIT License

---

## Credits

- [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp)