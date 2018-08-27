using CsvHelper.Configuration;

// ReSharper disable ClassNeverInstantiated.Global

namespace VE_Count_Consolidator
{
    public class VeCountEntry
    {
        public string Call;
        public int Count;
        public string Name;
        public string State;
        public string Vec;
    }

    public sealed class VeCountMapping : ClassMap<VeCountEntry>
    {
        public VeCountMapping()
        {
            Map(m => m.Call).Name("Callsign").NameIndex(0);
            Map(m => m.Name).Name("Name").NameIndex(1);
            Map(m => m.State).Name("State").NameIndex(2);
            Map(m => m.Count).Name("Count").NameIndex(3);
            Map(m => m.Vec).Name("VEC").NameIndex(4);
        }
    }
}