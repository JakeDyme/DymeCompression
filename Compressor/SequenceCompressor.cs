using System;

namespace DymeCompression
{
	public class SequenceCompressor
	{
		byte _flagByte;

		public SequenceCompressor(byte flagByte)
		{
			_flagByte = flagByte;
		}

		public SequenceCompressor(char flagChar)
		{
			_flagByte = Convert.ToByte(flagChar);
		}

		public SequenceCompressor()
		{
			_flagByte = 1;
		}

		public string CompressString(string inputString)
		{
			// If its 4 bytes or less then theres no point in compressing 
			// since a compression token is 4 bytes long.
			if (inputString.Length < 5) return inputString;

			// Parse into byte array...
			byte[] inputBuffer = BufferFromString(inputString);

			// Compress...
			int newBufferLength = SequenceCompressAndReturnNewBufferLength(ref inputBuffer, inputBuffer.Length);

			// Parse buffer into string...
			string newString = StringFromBuffer(inputBuffer, newBufferLength);

			return newString;
		}

		public string DecompressString(string inputString)
		{
			// If the string is less than 4 bytes then there cannot be any compression applied
			// since a compression token is at least 4 bytes long.
			if (inputString.Length < 4) return inputString;

			// Parse string into buffer...
			byte[] inputBuffer = BufferFromString(inputString);

			// Decompress...
			byte[] newBuffer = SequenceDecompressBytes(inputBuffer, inputBuffer.Length);

			// Parse buffer into string...
			string newString = StringFromBuffer(newBuffer, newBuffer.Length);

			return newString;
		}

		public byte[] SequenceDecompressBytes(byte[] inputBuffer, int inputBufferLength)
		{
			int newBufferLength = 0;
			for (var i = 0; i < inputBufferLength; i++)
			{
				if (inputBuffer[i] == _flagByte)
				{
					i++; //...skip original character.
					byte[] repetitionLengthByteArray = new byte[]{ inputBuffer[++i], inputBuffer[++i] };
					var repetitionLength = BitConverter.ToInt16(repetitionLengthByteArray);
					newBufferLength += repetitionLength;
				}
				else
				{
					newBufferLength += 1;
				}
			}

			var newBuffer = new byte[newBufferLength];
			int newBufferCounter = 0;

			for (var i = 0; i < inputBufferLength; i++)
			{
				if (inputBuffer[i] == _flagByte)
				{
					// If this one is a flag byte, 
					//   - then the next byte is the repeated character
					//   - and the next 2 bytes(UInt16) are the length of the repetition
					var originalCharacter = inputBuffer[++i];
					var repetitionLengthByteArray = new byte[] { inputBuffer[++i], inputBuffer[++i] };
					var repetitionLength = BitConverter.ToInt16(repetitionLengthByteArray);
					for (var x = 0; x < repetitionLength; x++)
					{
						newBuffer[newBufferCounter++] = originalCharacter;
					}
				}
				else
				{
					newBuffer[newBufferCounter++] = inputBuffer[i];
				}
			}
			return newBuffer;
		}

		public int SequenceCompressAndReturnNewBufferLength(ref byte[] inputBuffer, int inputBufferLength, byte recursionLevel = 0)
		{

			byte rolledUpByte = inputBuffer[0];
			var newBuffer = new byte[inputBufferLength];
			var newBufferLength = 0;
			ushort rollupCount = 1;
			for (var i = 1; i < inputBufferLength; i++)
			{
				if (inputBuffer[i] == inputBuffer[i - 1])
				{
					rollupCount++;
				}
				else
				{
					rolledUpByte = inputBuffer[i - 1];
					ProcessSequence(inputBuffer, i, rolledUpByte, ref newBuffer, ref newBufferLength, ref rollupCount);
					rollupCount = 1;
				}
			}

			// Perform the final iteration here rather than checking in the main loop.
			// This avoids repeating a check that'll only happen once and saves us O^1 unneccesary checks
			rolledUpByte = inputBuffer[inputBufferLength - 1];
			ProcessSequence(inputBuffer, inputBufferLength, rolledUpByte, ref newBuffer, ref newBufferLength, ref rollupCount);

			inputBuffer = newBuffer;
			return newBufferLength;
		}

		private void ProcessSequence(byte[] inputBuffer, int inputBufferCursor, byte rolledUpByte, ref byte[] newBuffer, ref int newBufferLength, ref ushort rollupCount)
		{
			const ushort COMPRESSION_THRESHOLD = 4;
			// Add the set of collected bytes that are over the compression threshold...
			if (rollupCount > COMPRESSION_THRESHOLD)
			{
				RollUp(_flagByte, rolledUpByte, rollupCount, ref newBuffer, ref newBufferLength);
			}
			else // ...not long enough to be compressed
			{
				// Add any bytes that were collected but were under the compression threshold...
				for (var x = inputBufferCursor - rollupCount; x < inputBufferCursor; x++)
				{
					newBuffer[newBufferLength++] = inputBuffer[x];
				}
			}
			// place original byte
		}

		private static int RollUp(byte flagByte, byte rolledUpByte, ushort rollupCount, ref byte[] newBuffer, ref int newBufferLength)
		{
			byte[] rollupCountBytes = BitConverter.GetBytes(rollupCount);
			newBuffer[newBufferLength++] = flagByte;
			newBuffer[newBufferLength++] = rolledUpByte;
			newBuffer[newBufferLength++] = rollupCountBytes[0];
			newBuffer[newBufferLength++] = rollupCountBytes[1];
			return newBufferLength;
		}

		private static byte[] BufferFromString(string inputString)
		{
			var inputBuffer = new byte[inputString.Length];
			for (var i = 0; i < inputString.Length; i++)
			{
				inputBuffer[i] = Convert.ToByte(inputString[i]);
			}

			return inputBuffer;
		}

		private static string StringFromBuffer(byte[] inputBuffer, int newBufferLength)
		{
			// Parse into character array...
			char[] finalCharArray = new char[newBufferLength];
			for (int i = 0; i < newBufferLength; i++)
			{
				finalCharArray[i] = Convert.ToChar(inputBuffer[i]);
			}
			var newString = new string(finalCharArray);
			return newString;
		}

	}
}
