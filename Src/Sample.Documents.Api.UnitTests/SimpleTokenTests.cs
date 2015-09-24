using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;
using System.Security.Claims;
using Ploeh.AutoFixture.Xunit;

namespace Sample.Documents.Api.UnitTests
{
    public class SimpleTokenTests
    {
        [Fact]
        public void simple_token_is_collection_of_claims()
        {
            var sut = new SimpleToken();

            Assert.IsAssignableFrom<IEnumerable<Claim>>(sut);
        }

        [Theory]
        [AutoData]
        public void simple_token_yields_passed_claims(List<Claim> claims)
        {
            var sut = new SimpleToken(claims.ToArray());

            sut.Should().Equal(claims);
        }

        [Theory]
        [InlineData(new string[0], "")]
        [InlineData(new [] { "foo|bar" }, "foo=bar")]
        [InlineData(new [] { "foo|bar", "f|b" }, "foo=bar&f=b")]
        [InlineData(new [] { "foo|bar", "d|b", "ddf|asb" }, "foo=bar&d=b&ddf=asb")]
        public void to_string_returns_serialized_claims(string[] input, string expected)
        {
            var claims = input
                            .Select(i => i.Split('|'))
                            .Select(s => new Claim(s[0], s[1]))
                            .ToArray();

            var actual = new SimpleToken(claims).ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new object[] { new string[0]})]
        [InlineData(new object[] { new[] { "foo|bar" }})]
        [InlineData(new object[] { new[] { "foo|bar", "f|b" }})]
        [InlineData(new object[] { new[] { "foo|bar", "d|b", "ddf|asb" }})]
        public void parse_returns_correct_claims_on_valid_string(string[] input)
        {
            var claims = input
                            .Select(i => i.Split('|'))
                            .Select(s => new Claim(s[0], s[1]))
                            .ToArray();

            var expected = new SimpleToken(claims);
            SimpleToken actual;
            var result = SimpleToken.TryParse(expected.ToString(), out actual);

            Assert.True(result);
            Assert.True(expected.SequenceEqual(actual, new ClaimComparer()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("foo=bar&baz")]
        public void parse_returns_false_on_invalid_string(string input)
        {
            SimpleToken actual;
            var result = SimpleToken.TryParse(input, out actual);

            Assert.False(result);
            Assert.Null(actual);
        }
    }
}
