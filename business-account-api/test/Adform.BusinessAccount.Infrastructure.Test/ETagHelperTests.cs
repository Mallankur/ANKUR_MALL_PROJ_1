using System;
using Xunit;

namespace Adform.BusinessAccount.Infrastructure.Test
{
    public class ETagHelperTests
    {
        [Fact]
        public void ReturnsZeroForNull()
        {
            var res = ETagHelper.FromWeakETagHeader(null);
            Assert.Equal(0, res);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"\"")]
        [InlineData("W/\"\"")]
        public void ReturnsZeroForEmpty(string ts)
        {
            var res = ETagHelper.FromWeakETagHeader(ts);
            Assert.Equal(0, res);
        }

        [Fact]
        public void CanParseLong()
        {
            var ts = DateTime.Now.Ticks;
            var res = ETagHelper.FromWeakETagHeader(ts.ToString());
            Assert.Equal(ts, res);
        }

        [Fact]
        public void CanParseQuotedLong()
        {
            var ts = DateTime.Now.Ticks;
            var res = ETagHelper.FromWeakETagHeader($"\"{ts}\"");
            Assert.Equal(ts, res);
        }

        [Fact]
        public void CanParseWeakETagFormat()
        {
            var ts = DateTime.Now.Ticks;
            var res = ETagHelper.FromWeakETagHeader($"W/\"{ts}\"");
            Assert.Equal(ts, res);
        }
    }

}