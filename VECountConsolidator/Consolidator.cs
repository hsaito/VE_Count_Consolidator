using System.Collections.Generic;

namespace VECountConsolidator
{
    public class Consolidator
    {
        /// <summary>
        ///     Process extractions.
        /// </summary>
        public static IEnumerable<Person> Process()
        {
            var countGetterList = new List<ICountGetter> {new ARRL()};
            var persons = ProcessList(countGetterList);
            return persons;
        }

        /// <summary>
        ///     Extract from each of VEC
        /// </summary>
        /// <param name="list">List of class for getting counts</param>
        /// <returns>List of person</returns>
        private static IEnumerable<Person> ProcessList(IEnumerable<ICountGetter> list)
        {
            var plist = new List<Person>();
            foreach (var item in list) plist.AddRange(item.Extract());

            return plist;
        }


        internal interface ICountGetter
        {
            string Vec { get; }
            IEnumerable<Person> Extract();
        }

        public class Person
        {
            public string Call;
            public int Count;
            public string Name;
            public State State;
            public string Vec;
        }

        public class State
        {
            public string StateCode;
            public string StateName;
        }
    }
}