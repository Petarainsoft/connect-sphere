#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

using Best.HTTP.SecureProtocol.Org.BouncyCastle.Crypto.IO;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.Crypto.Operators
{
    public class DefaultVerifierCalculator
        : IStreamCalculator<IVerifier>
    {
        private readonly SignerSink mSignerSink;

        public DefaultVerifierCalculator(ISigner signer)
        {
            this.mSignerSink = new SignerSink(signer);
        }

        public Stream Stream
        {
            get { return mSignerSink; }
        }

        public IVerifier GetResult()
        {
            return new DefaultVerifierResult(mSignerSink.Signer);
        }
    }
}
#pragma warning restore
#endif
