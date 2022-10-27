using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bracabot.UnitTests.Support
{
    public class FakeList<T> : IEnumerable<T>
    {
        private int callTimes = 0;
        public List<T> List { get; set; } = new List<T>();

        public FakeList(IEnumerable<T> elems)
        {
            List.AddRange(elems);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (callTimes == 0)
            {
                callTimes++;

                var l = new List<T>
                {
                    List.First()
                };
                return l.GetEnumerator();
            }

            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (callTimes == 0)
            {
                callTimes++;
                return new List<T>().GetEnumerator();
            }

            return List.GetEnumerator();
        }
    }
}
