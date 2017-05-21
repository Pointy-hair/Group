using Microsoft.AspNetCore.Http;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Streams;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Traffk.Bal.Services
{
    public class BasicFormFile : BaseDisposable, IFormFile
    {
        private readonly StreamMuxer Muxer;

        public BasicFormFile(byte[] buf)
        : this(new MemoryStream(buf))
        { }

        public BasicFormFile(Stream st)
        {
            Muxer = new StreamMuxer(st, false);
            Length = st.Length;
        }

        protected override void OnDispose(bool disposing)
        {
            Muxer.Dispose();
            base.OnDispose(disposing);
        }

        public string ContentType { get; set; }

        public string ContentDisposition { get; set; }

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length { get; private set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public void CopyTo(Stream target)
            => Muxer.OpenRead().CopyTo(target);

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
            => Muxer.OpenRead().CopyToAsync(target, int.MaxValue, cancellationToken);

        public Stream OpenReadStream()
            => Muxer.OpenRead();
    }
}
