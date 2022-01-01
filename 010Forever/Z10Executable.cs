using System.Diagnostics;

namespace Z10Forever;

internal class Z10Executable
{
    internal const string ExeName = "010Editor.exe";

    private readonly string _filePath;

    private readonly string _exeVersion;

    private readonly List<Patterns> _patches;

    internal Z10Executable(string path)
    {
        _filePath = Path.Combine(path, ExeName);
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException();
        }

        _exeVersion = FileVersionInfo.GetVersionInfo(_filePath).FileVersion;
        _patches = new()
        {
            new()  // $getLicenseStatus (we overwrite the function to return '113' which indicates that the evalutation period is still valid)
            {
                Original = new byte[]{
                                             0x48,  // the start of the function
                                             0x89,
                                             0x5C,
                                             0x24,
                                             0x08,
                                             0x57,
                                             0x48,
                                             0x83,
                                             0xEC,
                                             0x20,
                                             0x83,
                                             0x79,
                                             0x3C,
                                             0x00
                                         },
                Patch = new byte[]{
                                          0xB8,
                                          0x71,
                                          0x00,
                                          0x00,
                                          0x00,
                                          0xC3
                                      }

            },
            new()   // $getRemainingDays (purely for visual purposes: overwrites the value with a const)
            {
                Original = new byte[] {
                                             0x8B,
                                             0x50,
                                             0x28,
                                             0x2B,
                                             0x50,
                                             0x24
                                         },
                Patch = new byte[]{
                                          0xBA,
                                          0xC9,
                                          0x7D,
                                          0x27,
                                          0x04,
                                          0x90
                                      }
            },
            new() // // $getRemainingDays1 (purely for visual purposes: overwrites the value with a const)
            {
                Original = new byte[] { 0x44, 0x8B, 0x70, 0x28 },
                Patch = new byte[] {
                                           0x41,
                                           0xBE,
                                           0xC9,
                                           0x7D,
                                           0x27,
                                           0x04,
                                           0x90,
                                           0x90
                                       }
            }
        };
    }

    internal void CreateBackup()
        => File.Copy(_filePath, $"{_filePath}.bak", true);

    internal void RunPatch()
    {
        using BufferedStream stream = new(File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None), 4096);
        Console.WriteLine($"\n[!] Opened '{ExeName} v.{_exeVersion}' for reading and writing");
        long pos;
        string pattern;
        for (int i = 0; i < _patches.Count; i++)
        {
            pattern = $"$pattern{i + 1}";
            Console.WriteLine($"[!] Searching for {pattern}");
            if ((pos = Utils.ReplacePatternIfFound(stream, _patches[i])) != -1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"[Replaced {pattern} at 0x{pos:X8}]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[X] Could not find {pattern}");
                if (i == 0) // we only care about the actual patch not the visual effects
                {
                    Console.WriteLine($"[X] Could not patch the executable. Please make sure that the executable file is authentic and untouched. If the error persists, please contact me on github.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"[Succesfully patched the executable. Enjoy 010Editor f o r e v e r :D]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n\nNB: If you need to reverse the changes made to the file, simply navigate to your installation folder, delete '{ExeName}' and rename '{ExeName}.bak' to '{ExeName}'.");
    }
}