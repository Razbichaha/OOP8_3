using System;
using System.Collections.Generic;
using System.Threading;

namespace OOP8_3
{
    //    Создать 5 бойцов, пользователь выбирает 2 бойцов и они сражаются друг с другом до смерти.
    //    У каждого бойца могут быть свои статы.
    //Каждый игрок должен иметь особую способность для атаки, которая свойственна только его классу!
    //Если можно выбрать одинаковых бойцов, то это не должна быть одна и та же ссылка на одного бойца,
    //чтобы он не атаковал сам себя.
    //Пример, что может быть уникальное у бойцов. Кто-то каждый 3 удар наносит удвоенный урон,
    //другой имеет 30% увернуться от полученного урона, кто-то при получении урона немного себя лечит.
    //Будут новые поля у наследников.
    //У кого-то может быть мана и это только его особенность.
    class Program
    {
        static void Main(string[] args)
        {
            Core core = new();
            core.Game();
            Console.ReadLine();
        }
    }

    class Core
    {
        private Warrior[] _warriors = { new Archer(), new Slinger(), new SwordsMan(), new SpearMan(), new Knight() };
        private UI _ui = new();
        private Configs _configs = new();
        private Fight _fight = new();
        private Warrior[] _gladiators=new Warrior[2];
        //private object _firstGladiator;
        //private object _secondGladiator;

        // Type _type = typeof(Warrior);


        internal void Game()
        {
            _ui.ShowNewGame();
            CreateWariors();
            Console.ReadLine();
            _ui.ShowWarriors();
            ChooseGladiators();
            //Console.ReadLine();
            ShowGladiatorParameters();
            //Console.ReadLine();
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

            _ui.ChooseGladiators(_warriors.Length, ref firstGladiator, ref secondGladiator);
            AddGladiators(firstGladiator, secondGladiator);
        }

        private void AddGladiators(int firstGladiator, int secondGladiator)
        {
            for (int i = 0; i < _gladiators.Length; i++)
            {
                int number = i;
                int classWarior = firstGladiator;

                if (i == 0)
                    classWarior = firstGladiator;
                else if (i == 1)
                    classWarior = secondGladiator;

                //  _gladiators[0] = _warriors[firstGladiator];
                //  _gladiators[1] = _warriors[secondGladiator];

                switch (_warriors[classWarior])
                {
                    case Archer archer:
                        // _gladiators[number] = _warriors[classWarior];

                        _gladiators[number] = archer;
                        break;
                    case Slinger slinger:
                        _gladiators[number] = slinger;
                        break;
                    case SwordsMan swordsMan:
                        _gladiators[number] = swordsMan;
                        break;
                    case SpearMan spearMan:
                        _gladiators[number] = spearMan;
                        break;
                    case Knight knight:
                        _gladiators[number] = knight;
                        break;
                }
            }
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
            int firstFighter = _fight.WhoFirst(_gladiators.Length);

            bool continueFight = true;
            int move = firstFighter;

            while (continueFight)
            {
                _fight.Damage(_gladiators[move], _gladiators[MoveAbout(move)]);

                if (_gladiators[move].Fury == _configs.GetFuryWarrior(_gladiators[move].ClassWarriors))
                {
                    //  _gladiators[move].AttackFuriously(_gladiators[move], _gladiators[MoveAbout(move)]);
                   _gladiators[move]. DamagAbility(_gladiators[move], _gladiators[MoveAbout(move)]);

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
            {
                temp = 1;
            }
            return temp;
        }
    }

    class Fight
    {
        private Configs _configs = new();

        internal int WhoFirst(int howManyFighters)
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

            defendingWarrior.Healt -= damageDealt;

            if (defendingWarrior.Armor > 0)
            {
                defendingWarrior.Armor = defendingWarrior.Armor - (defendingWarrior.Armor / 10);
            }
            else if (defendingWarrior.Armor < 0)
            {
                defendingWarrior.Armor = 0;
            }

            if (damageDealt > _configs.DamageWhichAddFury)
            {
                attackingWarrior.Fury += _configs.FuryIncrement;

                if (attackingWarrior.Fury > _configs.GetFuryWarrior(attackingWarrior.ClassWarriors))
                {
                    attackingWarrior.Fury = _configs.Fury[_configs.GetIndexClassWarior(attackingWarrior.ClassWarriors)];
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
            ;

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

        internal void ChooseGladiators(int countGladiators, ref int firstGladiator, ref int secondGladiator)
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
                    Console.WriteLine("Вы ввели не корректные данные\nПопробуйте ещё.");
                    continueSelection = true;
                }

                Console.WriteLine();
                Console.Write("Выберите номер второго гладиатора - ");
                string secondGladiatorString = Console.ReadLine();

                if (IsNumber(secondGladiatorString, ref secondGladiator) & secondGladiator <= countGladiators & secondGladiator > 0)
                {
                    continueSelection = false;
                }
                else
                {
                    Console.WriteLine("Вы ввели не корректные данные\nПопробуйте ещё.");
                    continueSelection = true;
                }
                firstGladiator--;
                secondGladiator--;
            }
        }

        private void ShowParametr(string nameParametr, int parametr, int maximumParametr, char simvol, ConsoleColor color)
        {
            double Procent100 = 100;
            double procent = Procent100 / (double)maximumParametr * (double)parametr;
            double temp = _configs.MaximumLengthBar / Procent100 * (double)procent;
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

    /*abstract*/
    class Warrior : IAbility
    {

        private Configs _configs = new();

        private string _classWarriors;
        private int _healt;
        private int _fury;
        private int _armor;
        private int _damage;

        public string ClassWarriors { get => _classWarriors; set => _classWarriors = value; }
        public int Healt { get => _healt; set => _healt = value; }
        public int Fury { get => _fury; set => _fury = value; }
        public int Armor { get => _armor; set => _armor = value; }
        public int Damage { get => _damage; set => _damage = value; }

        internal void CreateWariors(int index)
        {
            ClassWarriors = _configs.ClassWarriors[index];
            Healt = _configs.Healt[index];
            Fury = 0;
            Armor = _configs.Armor[index];
            Damage = _configs.Damage[index];
        }

        internal void DamagAbility(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            

            for (int i = 0; i < 2; i++)
            {
                int number = i;
                Warrior classWarior = attackingWarrior;

                if (i == 0)
                    classWarior = attackingWarrior;
                else if (i == 1)
                    classWarior = defendingWarrior;

                switch (classWarior)
                {
                    case Archer archer:
                        // _gladiators[number] = _warriors[classWarior];

                       classWarior = archer;
                        break;
                    case Slinger slinger:
                        //Slinger slinger1 = new Slinger();
                        //slinger
                        ;
                        o = slinger;
                       // type=slinger

                        classWarior = slinger;
                        //slinger1 = classWarior;
                        ;
                       //slinger.
                      

                        break;
                    case SwordsMan swordsMan:
                        classWarior = swordsMan;
                        break;
                    case SpearMan spearMan:
                        classWarior = spearMan;
                        break;
                    case Knight knight:
                        classWarior = knight;
                        break;
                }

                
            }

        }

    }

    class Archer : Warrior, IAbility// лучник
    {
        void  IAbility.AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 2;
            int damag = attackingWarrior.Damage * multiplier;
            defendingWarrior.Healt -= damag;
            attackingWarrior.Fury = 0;
        }

    }

    class Slinger : Warrior//пращник
    {
        internal void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 5;

            int damagArmor = attackingWarrior.Damage / multiplier;
            defendingWarrior.Armor -= damagArmor;
            defendingWarrior.Healt -= attackingWarrior.Damage;
            defendingWarrior.Fury = 0;
        }
    }

    class SwordsMan : Warrior//мечник
    {
        internal void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 3;
            int damagArmor = attackingWarrior.Damage / multiplier;
            defendingWarrior.Armor -= damagArmor;
            defendingWarrior.Healt -= attackingWarrior.Damage;
            defendingWarrior.Fury = 0;
        }
    }

    class SpearMan : Warrior//копейщик
    {
        internal void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {

            int damagArmor = attackingWarrior.Damage;
            defendingWarrior.Armor -= damagArmor;
            defendingWarrior.Healt -= attackingWarrior.Damage;
            defendingWarrior.Fury = 0;
        }
    }

    class Knight : Warrior //рыцарь
    {
        internal  void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 2;
            int damagArmor = attackingWarrior.Damage / multiplier;
            defendingWarrior.Armor -= damagArmor / multiplier;
            defendingWarrior.Healt -= attackingWarrior.Damage;
            defendingWarrior.Fury = 0;
        }
    }


    class Configs
    {
        private string[] _listParametrs = { "damage", "healt", "fury", "armor", "evasion" };
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
        internal int[] Healt { get => _healt; }
        internal int[] Fury { get => _fury; }
        internal int[] Armor { get => _armor; }
        internal int[] Damage { get => _damage; }
        public int StopFight { get => _stopFight; }

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

    interface IAbility
    {


        internal void AttackFuriously(Warrior attackingWarrior, Warrior defendingWarrior)
        {
            int multiplier = 5;
            int damagArmor = attackingWarrior.Damage / multiplier;
            defendingWarrior.Armor -= damagArmor;
            defendingWarrior.Healt -= attackingWarrior.Damage;
            defendingWarrior.Fury = 0;
        }


    }
}
