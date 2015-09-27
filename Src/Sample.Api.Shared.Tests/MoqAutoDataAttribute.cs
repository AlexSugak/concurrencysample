using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture.AutoMoq;

namespace Sample.Api.Shared.Tests
{
    public class MoqAutoDataAttribute : AutoDataAttribute
    {
        public MoqAutoDataAttribute() : this(new Fixture())
        {
        }

        public MoqAutoDataAttribute(IFixture fixture)
            : base(fixture.Customize(new AutoMoqCustomization()))
        {
        }
    }
}
