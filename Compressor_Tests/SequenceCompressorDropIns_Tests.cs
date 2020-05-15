using DymeCompression;
using NUnit.Framework;
using System.IO;

namespace Compressor_Tests
{
	class SequenceCompressorDropIns_Tests
	{
		[Test]
		public void DoesNotCompress0()
		{
			// Arrange...
			
			var inputString = "";
			var expectedCompressedLength = 0;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress3()
		{
			// Arrange...
			
			var inputString = "aaa";
			var expectedCompressedLength = 3;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotCompress4()
		{
			// Arrange...
			
			var inputString = "aaaa";
			var expectedCompressedLength = 4;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses5()
		{
			// Arrange...
			
			var inputString = "aaaaa";
			var expectedCompressedLength = 4;
			
			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesMany()
		{
			// Arrange...
			
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			var expectedCompressedLength = 4;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesEndCapped()
		{
			// Arrange...
			
			var inputString = "aaaaab";
			var expectedCompressedLength = 5;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartCapped()
		{
			// Arrange...
			
			var inputString = "baaaaa";
			var expectedCompressedLength = 5;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void CompressesStartsAndEndsCapped()
		{
			// Arrange...
			
			var inputString = "baaaaab";
			var expectedCompressedLength = 6;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2Kinds()
		{
			// Arrange...
			
			var inputString = "aaaaabbbbb";
			var expectedCompressedLength = 8;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			
			var inputString = "caaaaabbbbbc";
			var expectedCompressedLength = 10;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void Compresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			
			var inputString = "caaaaacbbbbbc";
			var expectedCompressedLength = 11;

			// Act...
			var result = SequenceCompressorDropIns.CompressString(inputString);

			// Assert...
			Assert.AreEqual(expectedCompressedLength, result.Length);
		}

		[Test]
		public void DoesNotDecompress0()
		{
			// Arrange...
			
			var inputString = "";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress3()
		{
			// Arrange...
			
			var inputString = "aaa";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DoesNotDecompress4()
		{
			// Arrange...
			
			var inputString = "aaaa";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses5()
		{
			// Arrange...
			
			var inputString = "aaaaa";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesMany()
		{
			// Arrange...
			
			var inputString = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesEndCapped()
		{
			// Arrange...
			
			var inputString = "aaaaab";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartCapped()
		{
			// Arrange...
			
			var inputString = "baaaaa";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void DecompressesStartsAndEndsCapped()
		{
			// Arrange...
			
			var inputString = "baaaaab";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2Kinds()
		{
			// Arrange...
			
			var inputString = "aaaaabbbbb";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSide()
		{
			// Arrange...
			
			var inputString = "caaaaabbbbbc";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}

		[Test]
		public void Decompresses2KindsCappedOnEitherSideAndMiddle()
		{
			// Arrange...
			
			var inputString = "caaaaacbbbbbc";

			// Act...
			var compressedString = SequenceCompressorDropIns.CompressString(inputString);
			var result = SequenceCompressorDropIns.DecompressString(compressedString);

			// Assert...
			Assert.AreEqual(inputString, result);
		}



	}
}
