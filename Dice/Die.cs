using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Util;

namespace DiceCollection
{
    public class Die
    {
        private readonly int m_sides;
        public int Sides { get { return m_sides; } }

        public static Die Make(int sides)
        {
            return new Die(sides);
        }

        public Die()
        {
            m_sides = 0;
        }

        public Die(int sides)
        {
            m_sides = sides;
        }

        virtual public int Roll()
        {
            return TheRandom.Next(1, m_sides+1);
        }
    }

    [TestFixture]
    class DieTest
    {
        [Test]
        public void DieOfAnySidesReturns1ForLowest(
             [Range(1, 3)] int sides)
        {
            TheRandom.m_random = Substitute.For<Random>();
            TheRandom.Next(1, sides+1).Returns(1);

            Assert.That(Die.Make(sides).Roll() == 1);

            TheRandom.m_random = null;
        }

        [Test]
        public void DiceOfAnySidesReturnsItsSidesCountAsHighest(
            [Range(1, 3)] int sides)
        {
            TheRandom.m_random = Substitute.For<Random>();
            TheRandom.Next(1, sides+1).Returns(sides);

            Assert.That(Die.Make(sides).Roll() == sides);

            TheRandom.m_random = null;
        }
    }
}