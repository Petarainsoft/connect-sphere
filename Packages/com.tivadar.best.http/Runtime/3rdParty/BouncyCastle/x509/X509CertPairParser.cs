#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Collections.Generic;
using System.IO;

using Best.HTTP.SecureProtocol.Org.BouncyCastle.Asn1;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Security.Certificates;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Utilities.IO;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.X509
{
	public class X509CertPairParser
	{
		private Stream currentStream;

		private X509CertificatePair ReadDerCrossCertificatePair(
			Stream inStream)
		{
			Asn1InputStream dIn = new Asn1InputStream(inStream);//, ProviderUtil.getReadLimit(in));
			Asn1Sequence seq = (Asn1Sequence)dIn.ReadObject();
			CertificatePair pair = CertificatePair.GetInstance(seq);
			return new X509CertificatePair(pair);
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public X509CertificatePair ReadCertPair(byte[] input)
		{
			return ReadCertPair(new MemoryStream(input, false));
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public IList<X509CertificatePair> ReadCertPairs(byte[] input)
		{
			return ReadCertPairs(new MemoryStream(input, false));
		}

		public X509CertificatePair ReadCertPair(Stream inStream)
		{
			if (inStream == null)
				throw new ArgumentNullException("inStream");
			if (!inStream.CanRead)
				throw new ArgumentException("inStream must be read-able", "inStream");

			if (currentStream == null)
			{
				currentStream = inStream;
			}
			else if (currentStream != inStream) // reset if input stream has changed
			{
				currentStream = inStream;
			}

			try
			{
                int tag = inStream.ReadByte();
                if (tag < 0)
                    return null;

                if (inStream.CanSeek)
                {
                    inStream.Seek(-1L, SeekOrigin.Current);
                }
                else
                {
                    PushbackStream pis = new PushbackStream(inStream);
                    pis.Unread(tag);
                    inStream = pis;
                }

                return ReadDerCrossCertificatePair(inStream);
			}
			catch (Exception e)
			{
				throw new CertificateException(e.ToString());
			}
		}

		public IList<X509CertificatePair> ReadCertPairs(Stream inStream)
		{
			var certPairs = new List<X509CertificatePair>();

			X509CertificatePair certPair;
			while ((certPair = ReadCertPair(inStream)) != null)
			{
				certPairs.Add(certPair);
			}

			return certPairs;
		}
	}
}
#pragma warning restore
#endif
