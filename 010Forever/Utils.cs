namespace Z10Forever;

internal static class Utils
{
    internal static bool ExecuteIfRequested(string message, Action callback)
    {
        Console.Write(message);

        // ORing the KeyChar with ' ' ensures that both 'y' and 'Y' are considered valid
        if ((Console.ReadKey(true).KeyChar | ' ') == 'y')
        {
            callback();
            return true;
        }

        return false;
    }

    internal static long ReplacePatternIfFound(Stream stream, Patterns pattern)
    {
        // reset stream position
        stream.Position = 0;
        var orgPattern = pattern.Original;
        //var mask = pattern.Mask;
        var patternLength = orgPattern.Length;
        long lastPossiblePosition = stream.Length - patternLength + 1;
        int j = 0;
        long currentPos = 0;
        while (stream.Position < lastPossiblePosition)
        {
            if (stream.ReadByte() != orgPattern[j] /*&& mask[j] == 'x'*/)
            {
                if (j > 0)
                {
                    j = 0;
                    stream.Position = currentPos;
                }
                
                continue;
            }

            if (j++ == 0)
                currentPos = stream.Position; // we're setting currentPos to 1 position after where we got the byte matching the first byte in the pattern

            if (j == patternLength)
            {
                currentPos = (stream.Position -= patternLength);
                stream.Write(pattern.Patch);
                return currentPos;
            }
        }

        return -1;
    }
}