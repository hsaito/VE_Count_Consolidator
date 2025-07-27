using VECountConsolidator;
using Xunit;

namespace VECountConsolidator.Tests
{
    public class ArrlTests
    {
        [Fact]
        public void ARRL_Extract_ReturnsPersons()
        {
            var arrl = new ARRL();
            var persons = arrl.Extract();
            Assert.NotNull(persons);
            Assert.All(persons, p => Assert.Equal("ARRL", p.Vec));
        }
    }
}
