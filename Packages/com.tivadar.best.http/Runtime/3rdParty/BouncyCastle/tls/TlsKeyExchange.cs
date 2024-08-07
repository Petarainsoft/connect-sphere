#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

using Best.HTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.Tls
{
    /// <summary>A generic interface for key exchange implementations in (D)TLS.</summary>
    public interface TlsKeyExchange
    {
        void Init(TlsContext context);

        /// <exception cref="IOException"/>
        void SkipServerCredentials();

        /// <exception cref="IOException"/>
        void ProcessServerCredentials(TlsCredentials serverCredentials);

        /// <exception cref="IOException"/>
        void ProcessServerCertificate(Certificate serverCertificate);

        bool RequiresServerKeyExchange { get; }

        /// <exception cref="IOException"/>
        byte[] GenerateServerKeyExchange();

        /// <exception cref="IOException"/>
        void SkipServerKeyExchange();

        /// <exception cref="IOException"/>
        void ProcessServerKeyExchange(Stream input);

        short[] GetClientCertificateTypes();

        /// <exception cref="IOException"/>
        void SkipClientCredentials();

        /// <exception cref="IOException"/>
        void ProcessClientCredentials(TlsCredentials clientCredentials);

        /// <exception cref="IOException"/>
        void ProcessClientCertificate(Certificate clientCertificate);

        /// <exception cref="IOException"/>
        void GenerateClientKeyExchange(Stream output);

        /// <exception cref="IOException"/>
        void ProcessClientKeyExchange(Stream input);

        bool RequiresCertificateVerify { get; }

        /// <exception cref="IOException"/>
        TlsSecret GeneratePreMasterSecret();
    }
}
#pragma warning restore
#endif
