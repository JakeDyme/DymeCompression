using System;

namespace Dyme.Compression
{
	public class DymeCompressionDropIns
	{
		/// <summary>
		/// Drop-in functions for your code.
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
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

	}
}
