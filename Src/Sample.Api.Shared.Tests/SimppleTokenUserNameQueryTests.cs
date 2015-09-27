using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Sample.Api.Shared.Tests
{
    public class SimppleTokenUserNameQueryTests
    {
        [Theory]
        [AutoData]
        public void execute_returns_correct_user_name_on_valid_token(string userName)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(
                                                    "Bearer", 
                                                    new SimpleToken(new Claim("userName", userName)).ToString());

            var actual = new SimppleTokenUserNameQuery().Execute(request);

            Assert.Equal(userName, actual);
        }

        [Fact]
        public void execute_returns_null_user_name_when_missing_token()
        {
            var request = new HttpRequestMessage();
            var actual = new SimppleTokenUserNameQuery().Execute(request);

            Assert.Null(actual);
        }

        [Theory]
        [AutoData]
        public void execute_returns_null_on_invalid_schema(string schema)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(
                                                    schema,
                                                    new SimpleToken(new Claim("userName", "bob")).ToString());

            var actual = new SimppleTokenUserNameQuery().Execute(request);

            Assert.Null(actual);
        }

        [Theory]
        [AutoData]
        public void execute_returns_null_on_invalid_claim(string claimName)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(
                                                    "Bearer",
                                                    new SimpleToken(new Claim(claimName, "bob")).ToString());

            var actual = new SimppleTokenUserNameQuery().Execute(request);

            Assert.Null(actual);
        }

        [Theory]
        [AutoData]
        public void execute_returns_null_on_invalid_header_value(string headerValue)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(
                                                    "Bearer",
                                                    headerValue);

            var actual = new SimppleTokenUserNameQuery().Execute(request);

            Assert.Null(actual);
        }
    }
}
