Noesis.Monogame - "Native" Noesis renderer in Monogame
=============

[![DevCI-WindowsDX](https://github.com/ManoxCZ/Noesis.Monogame/actions/workflows/Dev-CI.WindowsDX.yml/badge.svg)](https://github.com/ManoxCZ/Noesis.Monogame/actions/workflows/Dev-CI.WindowsDX.yml)[![DevCI-DesktopGL](https://github.com/ManoxCZ/Noesis.Monogame/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ManoxCZ/Noesis.Monogame/actions/workflows/dotnet.yml)

This library provides the solution for integration [NoesisGUI](http://noesisengine.com) with [MonoGame 3.8.1](http://monogame.net) library.
Example MonoGame project with integrated NoesisGUI is included.

Prerequisites

Installation
-----
1. Open `Noesis.MonoGame.sln` with Visual Studio.
2. Open the context menu on `Noesis.Monogame.TestApp` project and select `Set as StartUp Project`.
3. Press F5 to launch the example game project.

Please note that the game example project uses default NoesisGUI theme from https://github.com/Noesis/Managed/tree/master/Src/NoesisApp/Theme and samples from https://github.com/Noesis/Tutorials/tree/master/Samples/Gallery/C%23 NoesisGUI works with XAML files without any preprocessing/building step (it has an extremely fast XAML parser that is faster than compiled BAML from WPF/UWP). You could store XAML files in any folder you want and robocopy them this way. One of the useful approaches is to store them in a WPF class library project which could be opened with Visual Studio to edit XAML with full syntax support and autocomplete. It also could be done as a WPF application project which could be executed independently to verify your UI is working properly (but that will require writing more demonstration logic there). WPF XAML is almost 100% compatible with NoesisGUI (see [docs](http://noesisengine.com/docs)).

Implementation limitations
-----
* Some features are still missing
* Tested on WindowsDX and DesktopGL platforms

Contributing
-----
Pull requests are welcome.

License
-----
The code is provided under the MIT License. Please read [LICENSE.md](LICENSE.md) for details.
