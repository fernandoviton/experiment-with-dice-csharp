using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceCollection
{
    class Program
    {
        static void RollCharactersUntilTarget()
        {
            const int target = 16 + 13 + 16 + 17 + 16 + 15;

            Dice dice = Dice.Make(4, Die.Make(6));
            //Dice dice = Dice.Make(7, Dice.Make(4, Die.Make(6)));
            // dice.Roll().DropLowest().Result();

            int iLoop = 0;
            while (true)
            {
                ++iLoop;

                List<int> rg = new List<int>();

                for (int i = 0; i < 7; ++i)
                {
                    rg.Add(dice.Roll().DropLowest().Result());
                    Console.WriteLine(rg[i]);
                }

                rg.Sort();

                int sum = 0;
                for (int i = 1; i < 7; ++i)
                {
                    sum += rg[i];
                }

                Console.WriteLine("Total (dropped lowest): " + sum.ToString());

                if (sum >= target)
                    break;
            }

            Console.WriteLine("It took " + iLoop.ToString() + " tries to beat " + target.ToString());
        }

        static void HistogramOfDieRoll(Dice dice, int nTimes)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();

            for (int i = 0; i < nTimes; ++i)
            {
                int rollResult = dice.Roll().Result();
                if (d.ContainsKey(rollResult))
                    d[rollResult]++;
                else
                    d.Add(rollResult, 1);
            }

            for (int i = 0; i < 1000; ++i)
            {
                if (d.ContainsKey(i))
                    Console.WriteLine(i + " " + d[i]);
            }
        }

        static void HistogramOfBestDieRoll(Dice dice, int nTimes)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();

            for (int i = 0; i < nTimes; ++i)
            {
                int rollResult = dice.Roll().ResultList.Cast<int>().Max();

                if (d.ContainsKey(rollResult))
                    d[rollResult]++;
                else
                    d.Add(rollResult, 1);
            }

            var sum = 0;
            for (int i = 0; i < 1000; ++i)
            {
                if (d.ContainsKey(i))
                {
                    var percent = d[i] * 100.0 / nTimes;
                    Console.WriteLine(i + " " + d[i] + " " + percent);
                    sum += i * d[i];
                }
            }
            Console.WriteLine($"Average: {(double)sum/nTimes}");
        }

        static Die Battle(Die die1, Die die2)
        {
            var die1Roll = die1.Roll();
            var die2Roll = die2.Roll();

            var result = die1Roll - die2Roll;
            if (result > 0)
                return die1;
            else if (result < 0)
                return die2;
            return Battle(die1, die2);
        }

        static Die Battle(Dice dice)
        {
            Die currentWinner = null;
            foreach (var die in dice.DieList)
            {
                if (currentWinner == null)
                    currentWinner = die;
                else
                    currentWinner = Battle(currentWinner, die);
            }

            return currentWinner;
        }


        /// <summary>
        /// The first dice will roll against the second.  The higher wins and advances and faces the third, etc.
        /// Last die standing is the winner.
        /// </summary>
        static void HistogramOfDiceBattle(Dice dice, int iterations)
        {
            var allWinners = new Dictionary<int, int>();
            for (var i = 0; i < iterations; ++i)
            {
                var winner = Battle(dice);
                if (allWinners.ContainsKey(winner.Sides))
                    allWinners[winner.Sides]++;
                else
                    allWinners.Add(winner.Sides, 1);
            }

            foreach(var winner in allWinners.OrderBy(w => w.Key))
                Console.WriteLine($"{winner.Key} sided die won {winner.Value} times");
        }

        static void Main(string[] args)
        {
            // Setup global random
            Util.TheRandom.m_random = new Random();

            var battleDice = new Dice(new[]
            {
                new Die(20),
                new Die(12),
                new Die(8),
                new Die(6),
                new Die(4),
                new Die(2),
            });

            // HistogramOfDiceBattle(battleDice, 10000);
            HistogramOfBestDieRoll(Dice.Make(2, Die.Make(20)), 1000000);
            // HistogramOfDieRoll(Dice.Make(3, Die.Make(6)), 1000000);
            // HistogramOfDieRoll(Dice.Make(4, Die.Make(6)), 1000000);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
