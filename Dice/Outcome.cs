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
    public class Outcome
    {
        private List<object> m_rgResult;

        public IReadOnlyList<object> ResultList { get { return m_rgResult; } }

        private Outcome()
        {
        }

        public static Outcome Make()
        {
            Outcome outcome = new Outcome();
            outcome.m_rgResult = new List<object>();
            return outcome;
        }

        public static Outcome Make(Outcome outcome)
        {
            Outcome outcomeNew = Make();
            outcomeNew.m_rgResult = new List<object>(outcome.m_rgResult);
            return outcomeNew;
        }

        public Outcome Add(int result)
        {
            m_rgResult.Add(result);
            return this;
        }

        public int Result()
        {
            int sum = 0;
            foreach (object o in m_rgResult)
            {
                sum += (int)o;
            }
            return sum;
        }

        public Outcome DropLowest()
        {
            Outcome outcome = Outcome.Make();

            if (m_rgResult.Count == 0)
                return outcome;

            int iLowest = 0;
            for (int i = 1; i < m_rgResult.Count; ++i )
            {
                if ((int)m_rgResult[i] < (int)m_rgResult[iLowest])
                    iLowest = i;
            }

            for (int i = 0; i < m_rgResult.Count; ++i)
            {
                if (i != iLowest)
                    outcome.Add((int)m_rgResult[i]);
            }

            return outcome;
        }
    }

    [TestFixture]
    public class OutcomeTest
    {
        [Test]
        public void NewOutcomeHasNoResultList()
        {
            Assert.AreEqual(0, Outcome.Make().ResultList.Count());
        }

        [Test]
        public void AddResultsAreAddedInOrder()
        {
            var outcome = Outcome.Make().Add(3).Add(1).Add(2);
            Assert.AreEqual(3, outcome.ResultList.Count, "Result lists expected count");
            Assert.AreEqual(3, outcome.ResultList[0], "Result lists 0th item expected value");
            Assert.AreEqual(1, outcome.ResultList[1], "Result lists 1st item expected value");
            Assert.AreEqual(2, outcome.ResultList[2], "Result lists 2nd item expected value");
        }

        [Test]
        public void GetResultsForEmptyOutcomeReturns0()
        {
            Assert.AreEqual(0, Outcome.Make().Result());
        }

        [Test]
        public void GetResultsForOutcomeOfIntsIsCorrect()
        {
            Assert.AreEqual(13, Outcome.Make().Add(3).Add(10).Result());
        }

        [Test]
        public void DropLowestOnEmptyReturnsNewEmptyOutcome()
        {
            Assert.AreEqual(0, Outcome.Make().DropLowest().ResultList.Count);
        }

        [Test]
        public void DropLowestOn1ItemReturnsNewEmptyOutcome()
        {
            Outcome outcome = Outcome.Make().Add(1);
            Outcome outcomeNew = outcome.DropLowest();

            Assert.AreEqual(0, outcomeNew.ResultList.Count);
            Assert.That(outcome != outcomeNew, "Expect a new object");
        }

        [Test]
        public void DropLowestOn2ItemsThatAreSameReturnsOutcomeWith1()
        {
            Outcome outcome = Outcome.Make().Add(10).Add(10);
            Outcome outcomeNew = outcome.DropLowest();

            Assert.AreEqual(1, outcomeNew.ResultList.Count);
            Assert.AreEqual(10, outcomeNew.ResultList[0]);
            Assert.That(outcome != outcomeNew, "Expect a new object");
        }

        [Test]
        public void DropLowestOn2ItemsWhereFirstIsLessReturnsOutcomeWithSecond()
        {
            Outcome outcome = Outcome.Make().Add(8).Add(10);
            Outcome outcomeNew = outcome.DropLowest();

            Assert.AreEqual(1, outcomeNew.ResultList.Count);
            Assert.AreEqual(10, outcomeNew.ResultList[0]);
            Assert.That(outcome != outcomeNew, "Expect a new object");
        }

        [Test]
        public void DropLowestOn2ItemsWhereFirstIsGreaterReturnsOutcomeWithFirst()
        {
            Outcome outcome = Outcome.Make().Add(10).Add(8);
            Outcome outcomeNew = outcome.DropLowest();

            Assert.AreEqual(1, outcomeNew.ResultList.Count);
            Assert.AreEqual(10, outcomeNew.ResultList[0]);
            Assert.That(outcome != outcomeNew, "Expect a new object");

        }
    }
}