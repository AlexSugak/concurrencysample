using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.UnitTests
{
    public class MoqAutoDataAttribute : AutoDataAttribute
    {
        public MoqAutoDataAttribute()
            : base(new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
