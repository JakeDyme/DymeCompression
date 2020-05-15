# DymeCompression
Simple compression algorithm for C# with zero dependencies.

This "Sequence" algorithm is pretty simple, all it does is find long sequences of the same character, and then rolls them up into a token that specifies the character and how many repetitions there were.
It works well for uncompressed image data that has lots of alpha.
I wrote these functions for a particular use case where I was making a plugin for an application that didn't allow me to import any other libraries.
I had to save raw image data to disk, there was no built in compression methods on the available libs, and I was only allowed to save string data.
So this was a very basic solution that allowed me to compress and decompress the texture data as string data, to and from disk.
It's meant to be simple, have no dependencies (not even on Collections) and be as fast as possible.

I've provided two versions, the verbose version, and the drop-in version.
The drop-in version are just the two functions (CompressString and DecompressString) that can be dropped into your code as-is without any additional dependencies.

# Drop-ins
You can copy these functions into your code as-is with no additional dependencies. (Please leave a star if you find this useful).
~~~C#
public static string Compress(string inputString)
{
    byte flagByte = 1;
    const ushort COMPRESSION_THRESHOLD = 4;
    if (inputString.Length < 5) return inputString;
    var inputBuffer = new byte[inputString.Length];
    for (var i = 0; i < inputString.Length; i++)
        inputBuffer[i] = Convert.ToByte(inputString[i]);
    var inputBufferLength = inputString.Length;
    byte rolledUpByte = inputBuffer[0];
    var newBuffer = new byte[inputBufferLength];
    var newBufferLength = 0;
    ushort rollupCount = 1;
    for (var i = 1; i < inputBufferLength; i++)
    {
        if (inputBuffer[i] == inputBuffer[i - 1]) rollupCount++;
        else
        {
            rolledUpByte = inputBuffer[i - 1];
            if (rollupCount > COMPRESSION_THRESHOLD) {
                byte[] rollupCountBytes = BitConverter.GetBytes(rollupCount);
                newBuffer[newBufferLength++] = flagByte;
                newBuffer[newBufferLength++] = rolledUpByte;
                newBuffer[newBufferLength++] = rollupCountBytes[0];
                newBuffer[newBufferLength++] = rollupCountBytes[1];
            }
            else
                for (var x = i - rollupCount; x < i; x++) 
                    newBuffer[newBufferLength++] = inputBuffer[x];
            rollupCount = 1;
        }
    }
    rolledUpByte = inputBuffer[inputBufferLength - 1];
    if (rollupCount > COMPRESSION_THRESHOLD) {
        byte[] rollupCountBytes = BitConverter.GetBytes(rollupCount);
        newBuffer[newBufferLength++] = flagByte;
        newBuffer[newBufferLength++] = rolledUpByte;
        newBuffer[newBufferLength++] = rollupCountBytes[0];
        newBuffer[newBufferLength++] = rollupCountBytes[1];
    }
    else
        for (var x = inputBufferLength - rollupCount; x < inputBufferLength; x++)
            newBuffer[newBufferLength++] = inputBuffer[x];
    inputBuffer = newBuffer;
    char[] finalCharArray = new char[newBufferLength];
    for (int i = 0; i < newBufferLength; i++)
        finalCharArray[i] = Convert.ToChar(inputBuffer[i]);
    var newString = new string(finalCharArray);
    return newString;
}
~~~

~~~C#
public static string Decompress(string compressedString)
{
    byte flagByte = 1;
    if (compressedString.Length < 4) return compressedString;
    var inputBuffer = new byte[compressedString.Length];
    for (var i = 0; i < compressedString.Length; i++)
        inputBuffer[i] = Convert.ToByte(compressedString[i]);
    var inputBufferLength = compressedString.Length;
    int newBufferLength = 0;
    for (var i = 0; i < inputBufferLength; i++)
    {
        if (inputBuffer[i] == flagByte)
        {
            i++;
            byte[] repetitionLengthByteArray = new byte[] { inputBuffer[++i], inputBuffer[++i] };
            var repetitionLength = BitConverter.ToInt16(repetitionLengthByteArray, 0);
            newBufferLength += repetitionLength;
        }
        else newBufferLength += 1;
    }
    var newBuffer = new byte[newBufferLength];
    int newBufferCounter = 0;
    for (var i = 0; i < inputBufferLength; i++)
    {
        if (inputBuffer[i] == flagByte)
        {
            var originalCharacter = inputBuffer[++i];
            var repetitionLengthByteArray = new byte[] { inputBuffer[++i], inputBuffer[++i] };
            var repetitionLength = BitConverter.ToInt16(repetitionLengthByteArray, 0);
            for (var x = 0; x < repetitionLength; x++)
                newBuffer[newBufferCounter++] = originalCharacter;
        }
        else newBuffer[newBufferCounter++] = inputBuffer[i];
    }
    char[] finalCharArray = new char[newBufferCounter];
    for (int i = 0; i < newBufferCounter; i++)
        finalCharArray[i] = Convert.ToChar(newBuffer[i]);
    var newString = new string(finalCharArray);
    return newString;
}
~~~

# Limitations
- The compression token uses a 2 byte unsigned integer to record the repetitions. Which means that it cannot record sequences longer than 65,535 characters. I haven't built in any checks for this, but if its a real problem for you then let me know and I'll put some checks in.

- A "flag byte" is used to seperate compressed content from uncompressed content within a compressed file. I've just chosen an arbitrary byte '1' (Start-Of-Heading in ASCII) which I don't think ever really occurs in normal string content, but it is possible that your data may already have this byte in it. In which case, some unsettling things might happen, as the function will try to decompress things that it shouldn't. Anyway you can set the flagByte to whatever you want, as long as its not 0 and below 256 (not any byte thats going to occur in your string).