using System;
using System.Collections.Generic;
using System.Threading;

namespace OOP8_3
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramCore core = new();
            core.Game();
            Console.ReadLine();
        }
    }

    class ProgramCore
    {
        private Warrior[] _warriors = { new Archer(), new Slinger(), new SwordsMan(), new SpearMan(), new Knight() };
        private UI _ui = new();
        private Configs _configs = new();
        private Fight _fight = new();
        private Warrior[] _gladiators;

        internal void Game()
        {
            _ui.ShowNewGame();
            CreateWariors();
            Console.ReadLine();
            _ui.ShowWarriors();
            ChooseGladiators();
            ShowGladiatorParameters();
            StartFight();
        }

        private void CreateWariors()
        {
            for (int i = 0; i < _warriors.Length; i++)
            {
                _warriors[i].CreateWariors(i);
            }
        }

        private void ChooseGladiators()
        {
            int firstGladiator = 1;
            int secondGladiator = 2;

            _ui.SelectGladiators(_warriors.Length, ref firstGladiator, ref secondGladiator);
            AddGladiators(firstGladiator, secondGladiator);
        }

        private void AddGladiators(int firstGladiator, int secondGladiator)
        {
            Warrior[] gladiatorsTemp = new Warrior[_configs.GladiatorsInArena];

            gladiatorsTemp[0] = _warriors[firstGladiator];
            gladiatorsTemp[1] = _warriors[secondGladiator];
            _gladiators = gladiatorsTemp;
        }

        private void ShowGladiatorParameters()
        {
            Console.Clear();

            for (int i = 0; i < _gladiators.Length; i++)
            {
                string classWarriors = _gladiators[i].ClassWarriors;
                int healt = _gladiators[i].Healt;
                int fury = _gladiators[i].Fury;
                int armor = _gladiators[i].Armor;

                _ui.ShowGladiatorParameters(classWarriors, healt, fury, armor);
            }
        }

        private void StartFight()
        {
            int firstFighter = _fight.FindWhoFirst(_gladiators.Length);
            bool continueFight = true;
            int move = firstFighter;

            while (continueFight)
            {
                _fight.Damage(_gladiators[move], _gladiators[MoveAbout(move)]);

                if (_gladiators[move].Fury == _configs.GetFuryWarrior(_gladiators[move].ClassWarriors))
                {
                    _gladiators[move].AttackFuriously(_gladiators[move], _gladiators[MoveAbout(move)]);
                    _gladiators[move].SetFury(0);
                }

                ShowGladiatorParameters();
                move = MoveAbout(move);
                Thread.Sleep(_configs.StopFight);

                if (_gladiators[move].Healt <= 0)
                {
                    continueFight = false;
                    Death(_gladiators[move], _gladiators[MoveAbout(move)]);
                    Console.ReadLine();
                }
            }
        }

        private void Death(Warrior deadWarrior, Warrior victoriousWarrior)
        {
            _ui.ShowBattleResult(deadWarrior, victoriousWarrior);
        }

        private int MoveAbout(int move)
        {
            int temp = 0;

            if (move == 0)
                temp = 1;

            return temp;
        }
    }

    class Fight
    {
        private Configs _configs = new();

        internal int FindWhoFirst(int howManyFighters)
        {
            Random random = new();
            int first = random.Next(0, howManyFighters - 1);
            return first;
        }

        internal void Damage(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int damageDealt = attackingWarrior.Damage - defendingWarrior.Armor;

            if (damageDealt <= 0)
            {
                damageDealt = 1;
            }

            defendingWarrior.SetHealt(
            defendingWarrior.Healt - damageDealt);

            if (defendingWarrior.Armor > 0)
            {
                defendingWarrior.SetArmor(defendingWarrior.Armor - (defendingWarrior.Armor / 10));
            }
            else if (defendingWarrior.Armor < 0)
            {
                defendingWarrior.SetArmor(0);
            }

            if (damageDealt > _configs.DamageWhichAddFury)
            {
                attackingWarrior.SetFury(attackingWarrior.Fury + _configs.FuryIncrement);

                if (attackingWarrior.Fury > _configs.GetFuryWarrior(attackingWarrior.ClassWarriors))
                {
                    attackingWarrior.SetFury(_configs.Fury[_configs.GetIndexClassWarior(attackingWarrior.ClassWarriors)]);
                }
            }
        }
    }

    class UI
    {
        Configs _configs = new();

        internal void ShowNewGame()
        {
            Console.Clear();
            Console.WriteLine("Выберите двух бойцов из предоставленных ниже пяти гладиаторов.");
        }

        internal void ShowBattleResult(Warrior deadWarrior, Warrior victoriousWarrior)
        {
            Console.WriteLine("Победил - " + victoriousWarrior.ClassWarriors);
        }

        internal void ShowGladiatorParameters(string classGladiator, int healt, int fury, int armor)
        {

            int index = _configs.GetIndexClassWarior(classGladiator);
            int maximumHealt = _configs.Healt[index];
            int maximumFury = _configs.Fury[index];
            int maximumArmor = _configs.Armor[index];

            Console.WriteLine(classGladiator);
            ShowParametr("Жизнь ", healt, maximumHealt, '#', ConsoleColor.Red);
            ShowParametr("Броня ", armor, maximumArmor, '#', ConsoleColor.Yellow);
            ShowParametr("Ярость", fury, maximumFury, '#', ConsoleColor.Green);
            Console.WriteLine();
        }

        internal void ShowWarriors()
        {
            Console.Clear();

            for (int i = 0; i < _configs.ClassWarriors.Length; i++)
            {
                string classWarrior = _configs.ClassWarriors[i];
                int healt = _configs.Healt[i];
                int fury = _configs.Fury[i];
                int armor = _configs.Armor[i];
                int damage = _configs.Damage[i];
                int id = i + 1;

                Console.WriteLine();
                Console.WriteLine("Класс воина №" + id + "  " + classWarrior);
                Console.WriteLine("Уровень жизни - " + healt + "  Урон - " + damage + "  Броня - " + armor + "  Ярость - " + fury);
            }
        }

        internal void SelectGladiators(int countGladiators, ref int firstGladiator, ref int secondGladiator)
        {
            Console.WriteLine();
            bool continueSelection = true;

            while (continueSelection)
            {
                Console.Write("Выберите номер первого гладиатора - ");
                string firstGladiatorString = Console.ReadLine();

                if (IsNumber(firstGladiatorString, ref firstGladiator) & firstGladiator <= countGladiators & firstGladiator > 0)
                {
                    continueSelection = false;
                }
                else
                {
                    firstGladiatorString = "";
                    Console.WriteLine("Вы ввели не корректные данные\nПопробуйте ещё.");
                }
            }
            continueSelection = true;

            while (continueSelection)
            {

                Console.WriteLine();
                Console.Write("Выберите номер второго гладиатора - ");
                string secondGladiatorString = Console.ReadLine();

                if (IsNumber(secondGladiatorString, ref secondGladiator) & secondGladiator <= countGladiators & secondGladiator > 0 & firstGladiator != secondGladiator)
                {
                    continueSelection = false;
                }
                else
                {
                    secondGladiatorString = "";
                    Console.WriteLine("Вы ввели не корректные данные\nПопробуйте ещё.");
                    continueSelection = true;
                }
            }
            firstGladiator--;
            secondGladiator--;
        }

        private void ShowParametr(string nameParametr, int parametr, int maximumParametr, char simvol, ConsoleColor color)
        {
            double procent100 = 100;
            double procent = procent100 / (double)maximumParametr * (double)parametr;
            double temp = _configs.MaximumLengthBar / procent100 * (double)procent;
            int lengthBar = (int)temp;
            Console.ForegroundColor = color;
            string bar = "";

            for (int i = 0; i < lengthBar; i++)
            {
                bar += simvol;
            }
            Console.WriteLine(nameParametr + " " + bar);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private bool IsNumber(string text, ref int number)
        {
            bool isNumber = int.TryParse(text, out number);
            return isNumber;
        }
    }

    class Warrior
    {
        private Configs _configs = new();

        internal string ClassWarriors { get; private set; }

        internal int Healt { get; private set; }

        internal int Fury { get; private set; }

        internal int Armor { get; private set; }

        internal int Damage { get; private set; }

        internal void SetDamage(int value)
        {
            Damage = value;
        }

        internal void SetArmor(int value)
        {
            Armor = value;
        }

        internal void SetFury(int value)
        {
            Fury = value;
        }

        internal void SetHealt(int value)
        {
            Healt = value;
        }

        internal void CreateWariors(int index)
        {
            ClassWarriors = _configs.ClassWarriors[index];
            SetHealt(_configs.Healt[index]);
            SetFury(0);
            SetArmor(_configs.Armor[index]);
            SetDamage(_configs.Damage[index]);
        }

        internal virtual void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
        }

    }

    internal class Archer : Warrior
    {
        internal override void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 2;
            int damage = attackingWarrior.Damage * multiplier;
            defendingWarrior.SetHealt(defendingWarrior.Healt - damage);
            attackingWarrior.SetFury(0);
        }
    }

    class Slinger : Warrior
    {
        internal override void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 5;
            int armorDamage = attackingWarrior.Damage / multiplier;
            defendingWarrior.SetArmor(defendingWarrior.Armor - armorDamage);
            defendingWarrior.SetHealt(defendingWarrior.Healt - attackingWarrior.Damage);
            defendingWarrior.SetFury(0);
        }
    }

    class SwordsMan : Warrior
    {
        internal override void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 3;
            int armorDamage = attackingWarrior.Damage / multiplier;
            defendingWarrior.SetArmor(defendingWarrior.Armor - armorDamage);
            defendingWarrior.SetHealt(defendingWarrior.Healt - attackingWarrior.Damage);
            defendingWarrior.SetFury(0);
        }
    }

    class SpearMan : Warrior
    {
        internal override void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int armorDamage = attackingWarrior.Damage;
            defendingWarrior.SetArmor(defendingWarrior.Armor - armorDamage);
            defendingWarrior.SetHealt(defendingWarrior.Healt - attackingWarrior.Damage);
            defendingWarrior.SetFury(0);
        }
    }

    class Knight : Warrior
    {
        internal override void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 2;
            int armorDamage = attackingWarrior.Damage / multiplier;
            defendingWarrior.SetArmor(defendingWarrior.Armor - armorDamage / multiplier);
            defendingWarrior.SetHealt(defendingWarrior.Healt - attackingWarrior.Damage);
            defendingWarrior.SetFury(0);
        }
    }

    class Configs
    {
        internal readonly string[] ClassWarriors = { "Лучник", "Пращник", "Мечник", "Копейщик", "Рыцарь" };

        private int[] _damage = { 550, 500, 800, 600, 600 };
        private int[] _healt = { 5000, 4500, 8000, 6000, 10000 };
        private int[] _fury = { 800, 700, 900, 1000, 2000 };
        private int[] _armor = { 300, 350, 800, 600, 1000 };
        private int _gladiatorsInArena = 2;
        private double maximumLengthBar = 20;
        private int _furyIncrement = 100;
        private int _damageWhichAddFury = 2;
        private int _stopFight = 200;

        internal double MaximumLengthBar { get => maximumLengthBar; }
        internal int FuryIncrement { get => _furyIncrement; }
        internal int DamageWhichAddFury { get => _damageWhichAddFury; }
        internal int GladiatorsInArena { get => _gladiatorsInArena; }
        internal int StopFight { get => _stopFight; }
        internal int[] Healt { get => _healt; }
        internal int[] Fury { get => _fury; }
        internal int[] Armor { get => _armor; }
        internal int[] Damage { get => _damage; }


        internal int GetIndexClassWarior(string classWarior)
        {
            int index = 0;

            for (int i = 0; i < ClassWarriors.Length; i++)
            {
                if (ClassWarriors[i] == classWarior)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        internal int GetFuryWarrior(string classWarrior)
        {
            int index = GetIndexClassWarior(classWarrior);
            int fury = _fury[index];
            return fury;
        }
    }
}
