using System;
using System.Linq;
using VECountConsolidator;
using Xunit;

namespace VECountConsolidator.Tests
{
    public class ArrlTests
    {
        [Fact]
        public void ARRL_Extract_ReturnsPersons()
        {
            // Mock provider returns valid config and table HTML
            string configHtml = "<option value='CA'>California</option>";
            string tableHtml = "<html><body><table><tr><td>W1AW (John Doe)</td><td>5</td></tr></table></body></html>";
            var mockProvider = new Func<string, string>(url => {
                if (url.Contains("ve-session-counts?state=")) return tableHtml;
                return configHtml;
            });
            var arrl = new ARRL(mockProvider);
            var persons = arrl.Extract();
            Assert.NotNull(persons);
            Assert.Equal(63, persons.Count()); // Expect 63 states
            var caPerson = persons.FirstOrDefault(p => p.State.StateCode == "CA");
            Assert.NotNull(caPerson);
            Assert.Equal("ARRL", caPerson.Vec);
            Assert.Equal("W1AW", caPerson.Call);
            Assert.Equal("John Doe", caPerson.Name);
            Assert.Equal(5, caPerson.Count);
        }
    }
}
