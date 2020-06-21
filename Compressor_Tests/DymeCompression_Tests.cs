using Dyme.Compression;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Compressor_Tests
{
	class DymeCompression_Tests
	{
		[Test]
		public void CompressBytes()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte[] testBuffer = new byte[] { 40, 40, 40, 40, 40, 40 };
			int testBufferLength = testBuffer.Length;
			var originalLength = 6;
			var expectedLength = 6;
			var expectedSaving = 0;

			// Act...
			var result = DymeCompression.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

			// Assert...
			Assert.AreEqual(originalLength, testBufferLength);
			Assert.AreEqual(expectedLength, result);
			Assert.AreEqual(expectedSaving, testBufferLength - result);
		}

		[Test]
		public void UsesDefaultFlagByte()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte expectedFlagByte = 1;
			byte[] testBuffer = new byte[] { 40, 40, 40, 40, 40, 40 };
			int testBufferLength = testBuffer.Length;
			var compressedBytesLength = DymeCompression.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

			// Act...
			var result = testBuffer[compressedBytesLength - 2];

			// Assert...
			Assert.AreEqual(expectedFlagByte, result);
		}

		[Test]
		public void DefaultFlagByteIsInUse_ExpectUseNextAvailableByte()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte expectedFlagByte = 2;
			byte[] testBuffer = new byte[] { 1, 40, 40, 40, 40, 40, 40 };
			int testBufferLength = testBuffer.Length;
			var compressedBytesLength = DymeCompression.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

			// Act...
			var result = testBuffer[compressedBytesLength - 2];

			// Assert...
			Assert.AreEqual(expectedFlagByte, result);
		}

		[Test]
		public void CompressBytes2()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte[] testBuffer = new byte[] { 40, 40, 40, 40, 40, 40, 9 };

			byte[] expectedBuffer = new byte[] { 1, 40, 6, 0, 9, 1, 48, 0,0 };

			int testBufferLength = testBuffer.Length;
			var originalLength = 7;
			var expectedLength = 7;
			var expectedSaving = 0;

			// Act...
			var result = DymeCompression.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

			// Assert...
			Assert.AreEqual(originalLength, testBufferLength);
			Assert.AreEqual(expectedLength, result);
			Assert.AreEqual(expectedSaving, testBufferLength - result);
			Assert.AreEqual(testBuffer, expectedBuffer);
		}

		[Test]
		public void DecompressBytes()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte[] buffer = new byte[] { 40, 40, 40, 40, 40, 40 };
			byte[] expectedCompressedBuffer = new byte[] { 40, 40, 40, 40, 40, 40 };
			byte[] expectedDecompressedBuffer = new byte[] { 40, 40, 40, 40, 40, 40 };
			var compressedBufferLength = DymeCompression.SequenceCompressAndReturnNewBufferLength(ref buffer, buffer.Length);

			// Act...
			var result = DymeCompression.SequenceDecompressBytes(buffer, compressedBufferLength);

			// Assert...
			Assert.AreEqual(result, expectedDecompressedBuffer);
		}

		[Test]
		public void DoesNotCompress0()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "";
			var expectedCompressedLength = 0;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress3()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaa";
			var expectedCompressedLength = 3;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress4()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaa";
			var expectedCompressedLength = 4;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses5()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaa";
			var expectedCompressedLength = 5;
			
			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressTextSafe()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaaaaaa";
			var expectedCompressedString = ">a19>1";
			// Act...
			var result = DymeCompression.Compress(inputString, true, '>');

			// Assert...
			Assert.AreEqual(expectedCompressedString, result);
		}

		[Test]
		public void CompressAndDecompressTextSafe()
		{
			// Arrange...
			var inputString = "aaaaaaaaa";
			// Act...
			var compressedString = DymeCompression.Compress(inputString, true, '>');
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void AllFlagBytesAreUsed_ExpectNoCompression()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "";
			for (var i = 1; i < 256; i++)
			{
				inputString += Convert.ToChar(i);
			}
			inputString += "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

			var flagByteAsChar = Convert.ToChar(0);
			var isTextSafe = '0';
			var expectedOutputString = inputString + flagByteAsChar + isTextSafe;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedOutputString, result);
		}

		[Test]
		public void AllFlagBytesAreUsed_DecompressStringThatHasNoCompression()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "";
			for (var i = 1; i < 256; i++)
			{
				inputString += Convert.ToChar(i);
			}
			inputString += "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			var compressedString = DymeCompression.Compress(inputString);

			// Act...
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void CompressesMany()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			var expectedCompressedLength = 6;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesEndCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaab";
			var expectedCompressedLength = 7;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "baaaaa";
			var expectedCompressedLength = 7;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartsAndEndsCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "baaaaab";
			var expectedCompressedLength = 8;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2Kinds()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaabbbbb";
			var expectedCompressedLength = 10;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "caaaaabbbbbc";
			var expectedCompressedLength = 12;

			// Act...
			var result = DymeCompression.Compress(inputString);
			var result2 = DymeCompression.Decompress(result);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
			Assert.AreEqual(result2, inputString);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "caaaaacbbbbbc";
			var expectedCompressedLength = 13;

			// Act...
			var result = DymeCompression.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotDecompress0()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress3()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaa";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress4()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaa";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses5()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaaa";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesMany()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesEndCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaab";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "baaaaa";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartsAndEndsCapped()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "baaaaab";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2Kinds()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaabbbbb";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "caaaaabbbbbc";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "caaaaacbbbbbc";

			// Act...
			var compressedString = DymeCompression.Compress(inputString);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}


		[Test]
		public void FileTest_LessCompressionDueToImageDensity()
		{
			// Arrange...
			var filePath = $@"{Directory.GetCurrentDirectory()}\TestFiles\UncompressedImages\1_Chair_Filled.1K_1K_24RGB.bytes";
			var fileData = File.ReadAllText(filePath);

			// Act...
			var compressedString = DymeCompression.Compress(fileData);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			// Make sure compression is lossless...
			Assert.AreEqual(fileData, result);
			// Assert compression percentage...
			var compressionPercentage = fileData.Length / compressedString.Length * 100;
			Assert.AreEqual("100%", compressionPercentage + "%");
			// Assert saved bytes...
			var originalStringLength = fileData.Length;
			var compressedStringLength = compressedString.Length;
			var savedBytes = originalStringLength - compressedStringLength;
			float savedMegaBytes = savedBytes / 1000000f;
			Assert.Greater(savedMegaBytes, 0.23);
		}


		[Test]
		public void FileTest_MoreCompressionDueToImageSparcity()
		{
			// Arrange...
			var filePath = $@"{Directory.GetCurrentDirectory()}\TestFiles\UncompressedImages\2_Chair_NotFilled.1K_1K_24RGB.bytes";
			var fileData = File.ReadAllText(filePath);

			// Act...
			var compressedString = DymeCompression.Compress(fileData);
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			// Make sure compression is lossless...
			Assert.AreEqual(fileData, result);
			// Assert compression percentage...
			var compressionPercentage = fileData.Length / compressedString.Length * 100;
			Assert.AreEqual("500%", compressionPercentage + "%");
			// Assert saved bytes...
			var originalStringLength = fileData.Length;
			var compressedStringLength = compressedString.Length;
			var savedBytes = originalStringLength - compressedStringLength;
			int savedMegaBytes = savedBytes / 1000000;
			Assert.AreEqual(2, savedMegaBytes);

		}


		[Test]
		public void CustomFlagByte()
		{
			// Arrange...
			var sut = new DymeCompression();
			var inputString = "aaaaaaaaa";
			var someNonB64CharButNotSpecial = '>';
			var flagAsByte = Convert.ToByte(someNonB64CharButNotSpecial);
			var rolledUpCharacterAsByte = Convert.ToByte('a');
			ushort rolledUpCount = 9;
			byte[] rolledUpCountAsBytes = BitConverter.GetBytes(rolledUpCount);
			byte isTextSafeAsByte = Convert.ToByte('0');
			//..............................................[ Seq Start ] [--- Seq character ---]  [-------------Seq Length-----------------------] [ Seq Flag]  [Is Text Safe flag] 
			byte[] expectedCompressedByteArray = new byte[] { flagAsByte, rolledUpCharacterAsByte, rolledUpCountAsBytes[0], rolledUpCountAsBytes[1], flagAsByte, isTextSafeAsByte };
			string expectedCompressedString = new string(expectedCompressedByteArray.Select(b => Convert.ToChar(b)).ToArray());
			// Act...
			string result = DymeCompression.Compress(inputString, false, Convert.ToByte(someNonB64CharButNotSpecial));

			// Assert...
			Assert.AreEqual(expectedCompressedString, result);
		}


		[Test]
		public void FileTest_MoreCompression_TextFriendly()
		{
			// Arrange...
			var filePath = $@"{Directory.GetCurrentDirectory()}\TestFiles\UncompressedImages\2_Chair_NotFilled.1K_1K_24RGB.bytes";
			byte[] fileData = File.ReadAllBytes(filePath);

			var b64 = Convert.ToBase64String(fileData);
			// Act...
			var compressedString = DymeCompression.Compress(b64, true, '|');
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			// Make sure compression is lossless...
			Assert.AreEqual(b64, result);
			// Assert compression percentage...
			var compressionPercentage = b64.Length / compressedString.Length * 100;
			Assert.AreEqual("300%", compressionPercentage + "%");
			// Assert saved bytes...
			var originalStringLength = b64.Length;
			var compressedStringLength = compressedString.Length;
			var savedBytes = originalStringLength - compressedStringLength;
			int savedMegaBytes = savedBytes / 1000000;
			Assert.AreEqual(2, savedMegaBytes);

		}


		[Test]
		public void FileTest_NoCompression_TextFriendly()
		{
			// Arrange...
			var filePath = $@"{Directory.GetCurrentDirectory()}\TestFiles\UncompressedImages\1_Chair_Filled.1K_1K_24RGB.bytes";
			var fileData = File.ReadAllBytes(filePath);
			var b64 = Convert.ToBase64String(fileData);

			// Act...
			var compressedString = DymeCompression.Compress(b64, true, '|');
			var result = DymeCompression.Decompress(compressedString);

			// Assert...
			// Make sure compression was lossless...
			Assert.AreEqual(b64, result);
			// Assert compression percentage...
			var compressionPercentage = b64.Length / compressedString.Length * 100;
			Assert.AreEqual("0%", compressionPercentage + "%");
			// Assert saved bytes...
			var originalStringLength = b64.Length;
			var compressedStringLength = compressedString.Length;
			var savedBytes = originalStringLength - compressedStringLength;
			float savedMegaBytes = savedBytes / 1000000f;
			Assert.AreEqual(-2, savedBytes);
		}

	}
}
