using System.Collections.Generic;
using System.Linq;

namespace VECountConsolidator
{
    public class Consolidator
    {
        public enum VEC
        {
            ARRL
        }

        /// <summary>
        ///     Process extractions.
        /// </summary>
        /// <param name="vec">Name of VEC</param>
        /// <returns>List of VEs</returns>
        public static IEnumerable<Person> Process(VEC vec)
        {
            switch (vec)
            {
                case VEC.ARRL:
                {
                    var countGetterList = new List<ICountGetter> {new ARRL()};
                    var persons = ProcessList(countGetterList);
                    return persons;
                }

                default:
                {
                    throw new VECountConsolidatorException();
                }
            }
        }

        /// <summary>
        ///     Process extractions (for multiple VECs)
        /// </summary>
        /// <param name="vecs">List of VECs</param>
        /// <returns>List of VEs</returns>
        public static IEnumerable<Person> Process(IEnumerable<VEC> vecs)
        {
            var list = new List<Person>();
            foreach (var vecItem in vecs)
            {
                var resultList = Process(vecItem);
                list.AddRange(resultList.ToList());
            }

            return list;
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