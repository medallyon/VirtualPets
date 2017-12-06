using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Globalization;

namespace VirtualPets
{
    public enum Type { Cat, Dog, Rabbit, Turtle, Parrot, Horse };

    class Pet
    {
        public static readonly string[] asciiArt = new string[6]
        {
            @"                    _,'|             _.-''``-...___..--';)
                   /_ \'.      __..-' ,      ,--...--'''
                  <\    .`--'''       `     /'
                   `-';'               ;   ; ;
             __...--''     ___...--_..'  .;.'
            (,__....----'''       (,..--''",
            @"                                    __
             ,                    ,´ e`--o
            ((                   (  | __,'
             \\~----------------' \_;/
             (                      /
             /) ._______________.  )
            (( (               (( (
             ``-'               ``-'",
            @"                       (`.         ,-,
                       `\ `.    ,;' /
                        \`. \ ,'/ .'
                  __     `.\ Y /.'
               .-'  ''--.._` ` (
             .'            /   `
            ,           ` '   Q '
            ,         ,   `._    \
            |         '     `-.;_'
            `  ;    `  ` --,.._;
            `    ,   )   .'
             `._ ,  '   /_
                ; ,''-,;' ``-
                 ``-..__\``--`",
            @"                                                                 ,'
                                                          ,;
                                                        .'/
                   `-_                                .'.'
                     `;-_                           .' /
                       `.-.        ,_.-'`'--'`'-._.` .'
                         `.`-.    /    .'´'.   _.'  /
                           `. '-.'_.._/0 _ 0\/`    {\
                             `.      |'-^Y^- |     //
                              (`\     \_.'._/\...-;..-.
                              `._'._,'` ```    _.:---''`
                                 ;-....----'''`
                                /   (
                           sk   |  (`
                                `.^'",
            @"        ______ __
       {-_-_= '. `'.
        {=_=_-  \   \
         {_-_   |   /
          '-.   |  /    .===,
       .--.__\  |_(_,==`  ( o)'-.
      `---.=_ `     ;      `/    \
          `,-_       ;    .'--') /
            {=_       ;=~`    `*`
             `//__,-=~`
             <<__ \\__
       jgs   /`)))/`)))",
            @"                                ;;
                              ,;;'\ 
                   __       ,;;' ' \
                 /'  '\'~~'~' \ /'\.)
              ,;(      )    /  | 
             ,;' \    /-.,,(   )    
                  ) /       ) /      
                  ||        ||   
                  (_\       (_\"
        };

        // Timers are a built-in Class that execute specified functions after an interval
        // https://msdn.microsoft.com/en-us/library/system.timers.timer(v=vs.110).aspx
        private static readonly Timer Tasks = new Timer();
        public static readonly int UpdateInterval = 7500;
        private static readonly Random RNG = new Random();
        
        private string name;
        public string Name
        {
            get
            {
                // The 'CultureInfo' Class enables me to convert strings to Title Case
                // https://msdn.microsoft.com/en-us/library/system.globalization.textinfo.totitlecase(v=vs.110).aspx
                TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
                return textInfo.ToTitleCase(name);
            }
            set
            {
                name = value;
            }
        }

        // The Pet Type declares the race of the animal, ultimately deciding how the animal acts
        public Type Type;
        
        private int hunger = 0;
        // The getters/setters for Hunger & Boredom will be publicly available, unlike the private 'hunger' & 'boredom' properties
        public int Hunger
        {
            get
            {
                return hunger;
            }
            set
            {
                hunger = value;
                if (hunger < 0) hunger = 0;
            }
        }
        
        private int boredom = 0;
        public int Boredom
        {
            get
            {
                return boredom;
            }
            set
            {
                boredom = value;
                if (boredom < 0) boredom = 0;
            }
        }

        // This integer intends to ensure that it's unlikey that a Pet can die of Boredom
        public double Mood => (Hunger + (Boredom * 0.2));

        public string MoodString
        {
            get
            {
                if (Mood < 5) return "Having the time of its life";
                else if (Mood < 25) return "Happy";
                else if (Mood < 50) return "Unhappy";
                else if (Mood < 75) return "Mad";
                else if (Mood < 95) return "Raging Mad";
                else return "Passed Out";
            }
        }
        
        public Pet(string name, int type)
        {
            Name = name;
            Type = (Type)type;

            InitTimers();
        }

        // An overload constructor with initial values for hunger and boredom
        public Pet(string name, int type, int h, int b)
        {
            Name = name;
            Type = (Type)type;

            Hunger = h;
            Boredom = b;

            InitTimers();
        }

        // This initiates timers to keep track of hunger & boredom values and makes sure the Pet expires
        private void InitTimers()
        {
            Tasks.Interval = UpdateInterval;

            // Declare which methods should be called every time the timer lapses
            Tasks.Elapsed += new ElapsedEventHandler(IncreaseHunger);
            Tasks.Elapsed += new ElapsedEventHandler(IncreaseBoredom);
            Tasks.Elapsed += new ElapsedEventHandler(CheckDeathStatus);
            
            Tasks.Enabled = true;
        }

        // This method checks that the program takes the appropriate actions when the Pet expires
        private void CheckDeathStatus(object source, ElapsedEventArgs e)
        {
            // The Pet is officially 'passed out' beyond 100 "Mood" points
            if (Mood >= 100)
            {
                Program.CurrentlyInMenu = false;
                Program.GameOver = true;
                Tasks.Enabled = false;
            }
        }

        // Nicely formatted string that displays necessary data about this pet
        public string GetStatusWindow()
        {
            return $"Name: {Name}\nType: {Type}\nHunger: {Hunger}\nBoredom: {Boredom}\nMood: {MoodString}";
        }

        // The method to talk
        // Outputs a string based on the type of pet
        public void Talk()
        {
            switch (Type)
            {
                case (Type)0:
                    Console.WriteLine("Meow");
                    break;
                case (Type)1:
                    Console.WriteLine("Woof");
                    break;
                case (Type)2:
                    Console.WriteLine("[Rabbit Noises]");
                    break;
                case (Type)3:
                    Console.WriteLine("[Turtle Noises]");
                    break;
                case (Type)4:
                    Console.WriteLine("Ca-caw! You're funny!");
                    break;
                case (Type)5:
                    Console.WriteLine("Weugh");
                    break;
            }
        }

        // A method that is called every x seconds that increases the pet's hunger level
        internal void IncreaseHunger(object source, ElapsedEventArgs e)
        {
            Hunger += RNG.Next(2, 8);
        }

        // Default method for feeding the pet with a default feeding level
        // Returns the updated 'Hunger' value
        public int Eat()
        {
            Hunger -= 5;
            return Hunger;
        }

        // Overload method for feeding the pet with a random value
        // Returns the updated 'Hunger' value
        public int Eat(bool random)
        {
            if (!random) return this.Eat();
            else Hunger -= RNG.Next(3, 8);
            return Hunger;
        }

        // Overload method for feeding the pet with a custom feeding level
        // Returns the updated 'Hunger' value
        public int Eat(int amount)
        {
            Hunger -= amount;
            return Hunger;
        }

        // A method that is called every x seconds that increases the pet's boredom level
        internal void IncreaseBoredom(object source, ElapsedEventArgs e)
        {
            Boredom += RNG.Next(2, 8);
        }

        // Default method for decreasing the pet's boredom level with a default value
        // Returns the updated 'Boredom' value
        public int Play()
        {
            Boredom -= 5;
            return Boredom;
        }
        
        // Overload method for decreasing the pet's boredom level with a random value
        // Returns the updated 'Boredom' value
        public int Play(bool random)
        {
            if (!random) return this.Play();
            else Boredom -= RNG.Next(3, 8);
            return Boredom;
        }

        // Overload method for decreasing the pet's boredom level with a custom value
        // Returns the updated 'Boredom' value
        public int Play(int amount)
        {
            Boredom -= amount;
            return Boredom;
        }
    }
}
