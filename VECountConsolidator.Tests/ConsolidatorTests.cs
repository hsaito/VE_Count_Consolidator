using System.Collections.Generic;
using System.Linq;
using VECountConsolidator;
using Xunit;

namespace VECountConsolidator.Tests
{
    public class ConsolidatorTests
    {
        [Fact]
        public void Process_WithARRL_ReturnsPersons()
        {
            // Mock provider returns valid config and table HTML
            string configHtml = "<option value='CA'>California</option>";
            string tableHtml = "<html><body><table><tr><td>W1AW (John Doe)</td><td>5</td></tr></table></body></html>";
            var mockProvider = new System.Func<string, string>(url => {
                if (url.Contains("ve-session-counts?state=")) return tableHtml;
                return configHtml;
            });
            var arrl = new ARRL(mockProvider);
            var result = Consolidator.Process(new List<Consolidator.ICountGetter> { arrl });
            Assert.NotNull(result);
            Assert.Equal(63, result.Count()); // Expect 63 states
            var caPerson = result.FirstOrDefault(p => p.State.StateCode == "CA");
            Assert.NotNull(caPerson);
            Assert.Equal("ARRL", caPerson.Vec);
            Assert.Equal("W1AW", caPerson.Call);
            Assert.Equal("John Doe", caPerson.Name);
            Assert.Equal(5, caPerson.Count);
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
