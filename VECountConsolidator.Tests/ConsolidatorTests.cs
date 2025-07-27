using System.Collections.Generic;
using VECountConsolidator;
using Xunit;

namespace VECountConsolidator.Tests
{
    public class ConsolidatorTests
    {
        [Fact]
        public void Process_WithARRL_ReturnsPersons()
        {
            var result = Consolidator.Process(Consolidator.VEC.ARRL);
            Assert.NotNull(result);
            Assert.All(result, p => Assert.Equal("ARRL", p.Vec));
        }

        [Fact]
        public void Process_WithInvalidVEC_ThrowsException()
        {
            // Create a dummy VEC value not in enum
            var invalidVec = (Consolidator.VEC)999;
            Assert.Throws<VECountConsolidatorException>(() => Consolidator.Process(invalidVec));
        }
    }
}

