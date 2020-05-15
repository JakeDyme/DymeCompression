using Dyme.Compression;
using NUnit.Framework;
using System.IO;

namespace Compressor_Tests
{
	class DymeCompression_Tests
	{
		[Test]
		public void CompressBytes()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte[] testBuffer = new byte[] { 40, 40, 40, 40, 40 };
			int testBufferLength = testBuffer.Length;
			var originalLength = 5;
			var expectedLength = 4;
			var expectedSaving = 1;

			// Act...
			var result = sut.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

			// Assert...
			Assert.AreEqual(originalLength, testBufferLength);
			Assert.AreEqual(expectedLength, result);
			Assert.AreEqual(expectedSaving, testBufferLength - result);
		}

		[Test]
		public void CompressBytes2()
		{
			// Arrange...
			var sut = new DymeCompression();

			byte[] testBuffer = new byte[] { 40, 40, 40, 40, 40, 9 };

			byte[] expectedBuffer = new byte[] { 1, 40, 5, 0, 9, 0 };

			int testBufferLength = testBuffer.Length;
			var originalLength = 6;
			var expectedLength = 5;
			var expectedSaving = 1;

			// Act...
			var result = sut.SequenceCompressAndReturnNewBufferLength(ref testBuffer, testBufferLength);

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

			byte[] buffer = new byte[] { 40, 40, 40, 40, 40 };
			byte[] expectedCompressedBuffer = new byte[] { 40, 40, 40, 40, 40 };
			byte[] expectedDecompressedBuffer = new byte[] { 40, 40, 40, 40, 40 };
			var compressedBufferLength = sut.SequenceCompressAndReturnNewBufferLength(ref buffer, buffer.Length);

			// Act...
			var result = sut.SequenceDecompressBytes(buffer, compressedBufferLength);

			// Assert...
			Assert.AreEqual(result, expectedDecompressedBuffer);
		}

		[Test]
		public void DoesNotCompress0()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "";
			var expectedCompressedLength = 0;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress3()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaa";
			var expectedCompressedLength = 3;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress4()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaa";
			var expectedCompressedLength = 4;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses5()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaa";
			var expectedCompressedLength = 4;
			
			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesMany()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			var expectedCompressedLength = 4;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesEndCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaab";
			var expectedCompressedLength = 5;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "baaaaa";
			var expectedCompressedLength = 5;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartsAndEndsCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "baaaaab";
			var expectedCompressedLength = 6;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2Kinds()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaabbbbb";
			var expectedCompressedLength = 8;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "caaaaabbbbbc";
			var expectedCompressedLength = 10;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "caaaaacbbbbbc";
			var expectedCompressedLength = 11;

			// Act...
			var result = sut.Compress(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotDecompress0()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress3()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaa";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress4()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaa";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses5()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaa";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesMany()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesEndCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaab";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "baaaaa";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartsAndEndsCapped()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "baaaaab";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2Kinds()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "aaaaabbbbb";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "caaaaabbbbbc";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			var sut = new DymeCompression('X');
			var inputString = "caaaaacbbbbbc";

			// Act...
			var compressedString = sut.Compress(inputString);
			var result = sut.Decompress(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}



	}
}
