using Z10Forever;

Console.ForegroundColor = ConsoleColor.DarkRed;
Console.WriteLine(@"-----------------------------------------------------------------------
_______  ___________  ___________                                     
\   _  \/_   \   _  \ \_   _____/__________   _______  __ ___________ 
/  /_\  \|   /  /_\  \ |    __)/  _ \_  __ \_/ __ \  \/ // __ \_  __ \
\  \_/   \   \  \_/   \|     \(  <_> )  | \/\  ___/\   /\  ___/|  | \/
 \_____  /___|\_____  /\___  / \____/|__|    \___  >\_/  \___  >__|   
       \/           \/     \/                    \/          \/       
------------------------by PsychoPast----------------------------------");

Console.ForegroundColor = ConsoleColor.White;
Console.Write("\n>> Enter the path to the 010Editor installation folder: ");
string folder = Console.ReadLine().Replace("\"",string.Empty);
if (!Directory.Exists(folder))
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("[X] Provided directory doesn't exist.");
    return 1;
}

Z10Executable exe;
try
{
    exe = new(folder);
}
catch (FileNotFoundException)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine($"[X] {Z10Executable.ExeName} doesn't exist in provided folder. It may have been moved, deleted or renamed.");
    return 2;
}

if (Utils.ExecuteIfRequested("[?] Do you want to create a backup of the current executable? [Y/Any Key] ", exe.CreateBackup))
{
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine("[Created a backup of the executable file.]");
}
else
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("[X] Backup file was not created.");
}

Console.ForegroundColor = ConsoleColor.White;
if (!Utils.ExecuteIfRequested("[?] Do you want to patch the executable? [Y/Any Key] ", exe.RunPatch))
{
    return 3;
}

return 0;