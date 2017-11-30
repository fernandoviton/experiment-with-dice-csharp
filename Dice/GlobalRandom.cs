using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace Util
{
    static public class TheRandom
    {
        public static int Next(int min, int limit)
        {
            return m_random.Next(min, limit);
        }

        internal static Random m_random { set; get; }
    }

    [TestFixture]
    class TheRandomTest
    {
        [Test]
        public void TheRandomDoesntHaveARandomObjectInTests()
        {
            Assert.That(TheRandom.m_random == null);
        }
    }
}