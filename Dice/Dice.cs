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
    public class Dice
    {
        private List<Die> m_rgDie;

        public IReadOnlyList<Die> DieList { get { return m_rgDie; } }

        public Dice()
        {
        }

        // TODO: convert Dice itself to be IEnumerable
        public Dice(IEnumerable<Die> dieEnum)
        {
            m_rgDie = dieEnum.ToList();
        }

        public static Dice Make()
        {
            Dice dice = new Dice();
            dice.m_rgDie = new List<Die>();
            return dice;
        }

        public static Dice Make(Dice dice)
        {
            Dice diceNew = new Dice();
            diceNew.m_rgDie = new List<Die>(dice.m_rgDie);
            return diceNew;
        }

        public static Dice Make(int cDie, Die die)
        {
            Dice dice = Make();
            for (uint i = 0; i < cDie; ++i)
                dice.Add(die);
            return dice;
        }

        public Dice Add(Die die)
        {
            m_rgDie.Add(die);
            return this;
        }

        public Outcome Roll()
        {
            Outcome outcome = Outcome.Make();
            foreach (Die die in m_rgDie)
            {
                outcome.Add(die.Roll());
            }
            return outcome;
        }
    }

    [TestFixture]
    class DiceTest
    {
        [Test]
        public void MakeEmptyCreatesEmptyDiceList()
        {
            Assert.That(Dice.Make().DieList.Count == 0);
        }

        [Test]
        public void MakeSetOfDiceOfSameSidesWorks()
        {
            Dice dice = Dice.Make(2, Die.Make(3));
            Assert.AreEqual(2, dice.DieList.Count, "dice list has expected count");
            Assert.AreEqual(3, dice.DieList[0].Sides, "0th die has expected sides");
            Assert.AreEqual(3, dice.DieList[1].Sides, "1st die has expected sides");
        }

        [Test]
        public void MakeFromPreexistingDiceClones()
        {
            Dice diceOld = Dice.Make(1, Die.Make(3));
            diceOld.Add(Die.Make(1));

            Dice diceNew = Dice.Make(diceOld);

            Assert.AreEqual(2, diceNew.DieList.Count, "New dice has expected count");
            Assert.AreEqual(3, diceNew.DieList[0].Sides, "new dice's 0th item had expected sides");
            Assert.AreEqual(1, diceNew.DieList[1].Sides, "new dice's 1st item had expected sides");

            Assert.That(diceOld.DieList != diceNew.DieList, "DieLists are not the same object (ie it was cloned)");

            Assert.That(diceOld.DieList[0] == diceNew.DieList[0], "Expect actual Die objects not to be cloned (ie multi reference is ok)");
        }

        [Test]
        public void AddedDiceAreAccessibleInSameOrder()
        {
            var dice = Dice.Make().Add(Die.Make(1)).Add(Die.Make(3)).Add(Die.Make(2));

            Assert.AreEqual(3, dice.DieList.Count, "dice list has expected count");
            Assert.AreEqual(1, dice.DieList[0].Sides, "0th die has expected sides");
            Assert.AreEqual(3, dice.DieList[1].Sides, "1st die has expected sides");
            Assert.AreEqual(2, dice.DieList[2].Sides, "2nd die has expected sides");            
        }

        [Test]
        public void RollOfEmptyDiceReturnsEmptyResults()
        {
            var outcome = Dice.Make().Roll();
            Assert.AreEqual(0, outcome.ResultList.Count);
        }

        [Test]
        public void RollOfDiceReturnsRollsInOrder()
        {
            Die die1 = Substitute.For<Die>();
            die1.Roll().Returns(3);
            Die die2 = Substitute.For<Die>();
            die2.Roll().Returns(7);

            Dice dice = Dice.Make().Add(die1).Add(die2);
            Outcome outcome = dice.Roll();

            Assert.AreEqual(2, outcome.ResultList.Count, "Result lists expected count");
            Assert.AreEqual(3, outcome.ResultList[0], "Result lists 0th item expected value");
            Assert.AreEqual(7, outcome.ResultList[1], "Result lists 1st item expected value");
        }

        [Test]
        public void DiceAreOrdered()
        {
            Assert.AreEqual(
                Dice.Make().Add(Die.Make(2)).Add(Die.Make(3)).DieList.Select(die => die.Sides),
                new[] { 2, 3 });
            Assert.AreEqual(
                Dice.Make().Add(Die.Make(3)).Add(Die.Make(2)).DieList.Select(die => die.Sides),
                new[] { 3, 2 });
        }
    }
}