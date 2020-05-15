# DymeCompression
Simple compression algorithm for C# with zero dependencies.

This "Sequence" algorithm is pretty simple, all it does is find long sequences of the same character, and then rolls them up into a token that specifies the character and how many repetitions there were.
It works well for uncompressed image data that has lots of alpha.
I wrote these functions for a particular use case where I was making a plugin for an application that didn't allow me to import any other libraries.
I had to save raw image data to disk, there was no built in compression methods on the available libs, and I was only allowed to save string data.
So this was a very basic solution that allowed me to compress and decompress the texture data as string data, to and from disk.
Its meant to be simple, have no dependencies (not even on Collections) and be as fast as possible.
I've provided two versions, the verbose version, and the drop-in version.
The drop-in version are just the two functions (CompressString and DecompressString) that can be dropped into your code as-is without any additional dependencies.

