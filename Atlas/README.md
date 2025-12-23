# 🗺️ Atlas - Private Media Vault

<div align="center">

![Atlas Logo](https://img.shields.io/badge/Atlas-Private%20Media%20Vault-007ACC?style=for-the-badge&logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**A modern Windows desktop application for securing and managing your private photos and videos with PIN protection and hidden storage.**

[Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Architecture](#-architecture) • [Contributing](#-contributing)

</div>

---

## 📖 Overview

Atlas is a privacy-focused media vault application built with .NET 8.0 and WPF. It provides a secure, user-friendly way to hide and protect your sensitive photos and videos from unauthorized access. With PIN-protected authentication and hidden folder storage, your media files remain invisible to file explorers while being easily accessible through Atlas's built-in viewers.

### Why Atlas?

- **🔐 Privacy First**: PIN-protected access ensures only you can view your media
- **👁️ Hidden Storage**: Files stored in hidden folders, invisible to file explorers
- **🎬 Built-in Players**: No need for external apps - view images and play videos directly
- **🚀 Simple & Fast**: Intuitive interface with drag-and-drop import
- **🎨 Modern UI**: Clean, dark-themed interface designed for Windows 10/11

---

## ✨ Features

### Security
- **4-8 Digit PIN Protection**: Numeric PIN authentication for quick access
- **SHA-256 Hashing**: Secure PIN storage using industry-standard hashing
- **Hidden Storage**: Automatic folder hiding with Windows file attributes
- **Reset Mechanism**: Complete app reset option if PIN is forgotten

### Media Management
- **Image Support**: JPG, JPEG, PNG, GIF, BMP, WebP
- **Video Support**: MP4, AVI, MKV, MOV, WMV, FLV
- **Drag & Drop Import**: Simply drag files into the app
- **File Browser Import**: Traditional file picker for adding media
- **Grid Gallery View**: Thumbnail-based gallery with file information

### Viewers
- **Image Viewer**: Full-screen image preview with zoom support
- **Video Player**: Built-in player with playback controls (play, pause, stop, seek)
- **Smooth Playback**: Hardware-accelerated video rendering

### User Experience
- **First-Time Setup**: Guided onboarding with storage location selection
- **Auto-Save Settings**: Persistent configuration across sessions
- **Status Indicators**: Real-time feedback on operations
- **Scrollable Interfaces**: All windows support scrolling for various screen sizes

---

## 🚀 Installation

### Prerequisites
- **Windows 10/11** (64-bit)
- **.NET 8.0 Runtime** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** (for building from source)

### Building from Source

1. **Clone the repository**
```bash
git clone https://github.com/rillToMe/atlas.git
cd atlas
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the project**
```bash
dotnet build --configuration Release
```

4. **Run the application**
```bash
dotnet run
```

### Creating an Executable

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `bin/Release/net8.0/win-x64/publish/`

---

## 📱 Usage

### First Time Setup

1. **Welcome Screen**
   - Read about Atlas's features
   - Click "Let's Go!" to proceed

2. **Storage Location Setup**
   - Choose where to store your hidden files
   - Options: Default location (AppData) or custom folder
   - Click "Next: Set Your PIN"

3. **PIN Creation**
   - Enter a 4-8 digit numeric PIN
   - Confirm your PIN
   - Click "Create and Enter Atlas"

### Daily Use

1. **Login**
   - Enter your PIN
   - Click "Unlock" or press Enter

2. **Import Media**
   - **Method 1**: Click "📁 Import Files" button
   - **Method 2**: Drag and drop files into the window
   - Supported formats will be automatically copied to hidden storage

3. **View Media**
   - Click any thumbnail to open the viewer
   - Images: Full-screen preview with zoom
   - Videos: Built-in player with standard controls

4. **Forgot PIN?**
   - Click "Forgot PIN?" on login screen
   - Confirm reset (this clears app data, not your files)
   - Go through setup process again

---

## 🏗️ Architecture

### Project Structure

```
Atlas/
├── Models/
│   └── AppConfig.cs          # Configuration & Media models
├── Services/
│   ├── ConfigService.cs      # JSON config management
│   ├── SecurityService.cs    # PIN hashing & validation
│   └── StorageService.cs     # File operations & hidden folders
├── Views/
│   ├── WelcomeWindow.xaml    # First-time welcome screen
│   ├── SetupWindow.xaml      # Storage location setup
│   ├── PinSetupWindow.xaml   # PIN creation
│   ├── PinEntryWindow.xaml   # Login screen
│   ├── MainWindow.xaml       # Main gallery interface
│   ├── ImageViewerWindow.xaml # Image viewer
│   ├── VideoPlayerWindow.xaml # Video player
│   └── SettingsWindow.xaml   # Settings & About
├── App.xaml                  # Application entry & styles
└── App.xaml.cs               # Startup logic
```

### UI/UX Flow Diagram

```
┌─────────────────┐
│   App Startup   │
└────────┬────────┘
         │
    ┌────▼────┐
    │Is First │
    │  Run?   │
    └─┬────┬──┘
      │    │
   Yes│    │No
      │    │
      ▼    ▼
┌──────────┐  ┌──────────────┐
│ Welcome  │  │  PIN Entry   │
│  Screen  │  │   (Login)    │
└────┬─────┘  └──┬────────┬──┘
     │           │        │
     │       Correct   Forgot?
     │         PIN       │
     ▼           │       ▼
┌──────────┐     │  ┌─────────┐
│  Setup   │     │  │  Reset  │
│ Storage  │     │  │   App   │
└────┬─────┘     │  └────┬────┘
     │           │       │
     ▼           │       │
┌──────────┐     │       │
│  Create  │     │       │
│   PIN    │     │       │
└────┬─────┘     │       │
     │           │       │
     └───────┬───┴───────┘
             │
             ▼
      ┌─────────────┐
      │    Main     │
      │   Gallery   │
      └──┬──────┬───┘
         │      │
    Import│      │Click Media
         │      │
         ▼      ▼
    ┌────────┐ ┌────────────┐
    │ Files  │ │   Viewer   │
    │ Added  │ │  (Image/   │
    └────────┘ │   Video)   │
               └────────────┘
```

### Data Flow

```
┌──────────────┐
│    User      │
│   Actions    │
└──────┬───────┘
       │
       ▼
┌──────────────────┐      ┌──────────────┐
│     Views        │◄────►│   Services   │
│  (XAML/C#)       │      │              │
└──────────────────┘      └──────┬───────┘
                                 │
                   ┌─────────────┼─────────────┐
                   │             │             │
                   ▼             ▼             ▼
           ┌──────────┐  ┌──────────┐  ┌──────────┐
           │  Config  │  │ Storage  │  │ Security │
           │ Service  │  │ Service  │  │ Service  │
           └────┬─────┘  └────┬─────┘  └────┬─────┘
                │             │              │
                ▼             ▼              ▼
           ┌──────────┐  ┌──────────┐  ┌──────────┐
           │ config   │  │  Hidden  │  │   PIN    │
           │  .json   │  │  Folder  │  │  Hashing │
           └──────────┘  └──────────┘  └──────────┘
```

### Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 8.0 |
| UI Framework | WPF (Windows Presentation Foundation) |
| Language | C# 12.0 |
| Config Storage | System.Text.Json |
| Security | SHA-256 Hashing |
| Video Playback | MediaElement (WPF) |
| Styling | XAML Resources & Styles |

---

## 🔒 Security Features

### PIN Storage
- PINs are **never stored in plain text**
- SHA-256 hashing algorithm used for secure storage
- Hash comparison for authentication

### Hidden Storage
- Media files stored in folders with Windows `Hidden` attribute
- Folders invisible in standard file explorer views
- Files remain encrypted by Windows filesystem if drive encryption is enabled

### Important Security Notes

⚠️ **Current Version (v1.0):**
- Uses basic hidden folder storage (not encrypted)
- PIN hashing prevents plain-text storage but isn't bullet-proof security
- Suitable for privacy from casual browsing, not forensic-level security

🔐 **Future Enhancements:**
- AES-256 file encryption
- Biometric authentication (Windows Hello)
- Secure key management
- Multiple user profiles

---

## 🎨 UI/UX Design

### Design Principles
- **Dark Theme First**: Optimized for low-light use and privacy
- **Minimal Clutter**: Focus on content, hide complexity
- **Immediate Feedback**: Visual states for all interactions
- **Accessibility**: Keyboard shortcuts and clear visual hierarchy

### Color Palette

| Purpose | Color | Hex |
|---------|-------|-----|
| Background | Dark Gray | `#1E1E1E` |
| Surface | Darker Gray | `#2D2D30` |
| Primary | Blue | `#007ACC` |
| Primary Hover | Light Blue | `#1C97EA` |
| Text | White | `#FFFFFF` |
| Text Secondary | Light Gray | `#CCCCCC` |
| Border | Gray | `#3F3F46` |

---

## 📊 File Support

### Images
| Format | Extension | Notes |
|--------|-----------|-------|
| JPEG | `.jpg`, `.jpeg` | Most common format |
| PNG | `.png` | Transparency support |
| GIF | `.gif` | Animation support |
| BMP | `.bmp` | Uncompressed |
| WebP | `.webp` | Modern format |

### Videos
| Format | Extension | Notes |
|--------|-----------|-------|
| MP4 | `.mp4` | Best compatibility |
| AVI | `.avi` | Legacy format |
| MKV | `.mkv` | High quality |
| MOV | `.mov` | QuickTime format |
| WMV | `.wmv` | Windows Media |
| FLV | `.flv` | Flash video |

**Note**: Video playback depends on codecs installed on Windows system.

---

## 🛠️ Configuration

### Config File Location
```
%AppData%\Atlas\config.json
```

### Config Structure
```json
{
  "isFirstRun": false,
  "pinHash": "hashed_pin_string",
  "storagePath": "C:\\path\\to\\storage",
  "theme": "dark"
}
```

### Default Storage Location
```
%AppData%\Atlas\Storage\
```

---

## 🐛 Troubleshooting

### Common Issues

**Q: Video won't play**
- A: Install Windows Media Feature Pack or VLC codec pack
- Some video formats require additional codecs

**Q: Forgot PIN, can't reset**
- A: Click "Forgot PIN?" on login screen
- Note: This resets app settings, not your files

**Q: Files not showing in gallery**
- A: Check file format is supported
- Verify files were successfully imported

**Q: Hidden folder visible in Explorer**
- A: Ensure "Show hidden files" is disabled in Explorer
- File → Options → View → Hidden files and folders

**Q: Import fails**
- A: Check disk space
- Verify file isn't locked by another program
- Run app as administrator if needed

---

## 🚧 Roadmap

### Version 1.x (Current)
- [x] PIN authentication
- [x] Hidden storage
- [x] Image viewer
- [x] Video player
- [x] Drag & drop import
- [x] Settings window

### Version 2.0 (Planned)
- [ ] AES-256 file encryption
- [ ] Thumbnail generation for videos
- [ ] Search & filter functionality
- [ ] Categories/Albums
- [ ] Batch operations (delete, move)
- [ ] Export functionality
- [ ] Dark/Light theme toggle

### Version 3.0 (Future)
- [ ] Windows Hello integration
- [ ] Cloud backup option
- [ ] Multi-user support
- [ ] Mobile companion app
- [ ] Advanced security features
- [ ] Slideshow mode

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to the branch** (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

### Development Guidelines
- Follow existing code style and naming conventions
- Add XML documentation for public methods
- Test on Windows 10 and Windows 11
- Update README if adding new features

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**DitDev**
- Portfolio: [https://ditdev.vercel.app](https://ditdev.vercel.app)
- GitHub: [@rillToMe](https://github.com/rillToMe)

---

## 🙏 Acknowledgments

- Built for privacy-conscious users
- Inspired by the need for simple, effective media protection
- Thanks to the .NET and WPF community for excellent documentation

---

## 📞 Support

If you encounter any issues or have questions:
- Open an issue on GitHub
- Check the [Troubleshooting](#-troubleshooting) section
- Contact via portfolio website

---

<div align="center">

**⭐ Star this repo if you find it helpful!**

Made by [DitDev](https://ditdev.vercel.app)

</div>