using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace VirtualPets
{
    class Program
    {
        // These static variables will be used for checks throughout the game
        public static Pet[] Pets;
        internal static bool CurrentlyInMenu;
        internal static bool GameOver;
        internal static int CurrentPet;

        public static void Main(string[] args)
        {
            // This clears the console in case the game is restarted, removing previous strings
            ClearConsole();

            // Set the values of these static variables every time the 'Main()' method is run to allow replayability
            Pets = new Pet[2];
            CurrentlyInMenu = false;
            GameOver = false;
            CurrentPet = -1;

            // Iteratively collect info from player about their pets
            for (int i = 0; i < Pets.Length; i++)
            {
                Console.WriteLine("Welcome to VirtualPets, the best virtual Pet-Keeping Simulator in .NET!\n");

                // Query about player's choice for a pet; The following line contains an in-line conditional operator:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator
                string PetTypesString = $"What is your {((i == 0) ? "first" : "second")} pet going to be? Choose from the following:\n";

                // This creates an Array from the Enum that stores the different Types of pets
                // https://msdn.microsoft.com/en-us/library/system.enum.getvalues.aspx
                var PetChoices = Enum.GetValues(typeof(Type));

                // Add to the original string using a 'for' loop to list all Pet Types
                for (int j = 0; j < PetChoices.Length; j++) PetTypesString += $"\n  {j + 1}. {PetChoices.GetValue(j)}";
                PetTypesString += "\n\n > ";

                // This custom function collects the player's answer
                // See the function declaration for an elaborate explanation
                int petChoice = CollectMenuAnswer(PetTypesString, 1, PetChoices.Length, () =>
                {
                    // If the answer is incorrect, clear the board until a valid answer is given
                    ClearConsole();
                    
                    return 0;
                });
                
                Console.Write($"\nYou chose a {(Type)petChoice} to be your {((i == 0) ? "first" : "second")} pet. What are you going to name it?\n > ");

                // Instantiate a new pet and add it to the static 'Pets' Array
                Pets[i] = new Pet(Console.ReadLine(), petChoice);
                
                ClearConsole();
            }

            // The player should now choose one pet to act on, since they have two pets
            CurrentPet = CollectMenuAnswer($"\nYou now have two pets; {Pets[0].Name} and {Pets[1].Name}. Select one to act on.\n\n  1. {Pets[0].Name} ({Pets[0].Type})\n  2. {Pets[1].Name} ({Pets[1].Type})\n\n > ", 1, 2, () =>
            {
                ClearConsole();
                return 0;
            });
            Console.WriteLine($"You have selected your {Pets[CurrentPet].Type}, {Pets[CurrentPet].Name}. To get started, press [ RETURN ]");

            // The following lines implement a Timer from the 'System.Timers' namespace
            System.Timers.Timer StatusTimer = new System.Timers.Timer();
            StatusTimer.Interval = Pet.UpdateInterval;
            StatusTimer.Elapsed += new ElapsedEventHandler(DisplayStatus);
            StatusTimer.Enabled = true;
            
            // Initiate the true game loop
            while (true)
            {
                if (GameOver) EndGame();

                // This indicates that the main menu is being shown
                // This is to make sure that the Pet's info is updated regularly. See the 'DisplayStatus' function below.
                CurrentlyInMenu = true;

                // Update the current Pet's info (Hunger, Boredom, Mood, etc.)
                ClearConsole();
                Console.WriteLine(Pet.asciiArt[(int)Pets[CurrentPet].Type] + "\n\n" + Pets[CurrentPet].GetStatusWindow());

                // The other pet will always be synonymous with the equation (CurrentPet - 1) * (-1)
                // i.e. if 'CurrentPet' == 0, then 'otherPet' will be -1 * -1, which is 1
                // i.e. if 'CurrentPet' == 1, then 'otherPet' will be 0 * -1, which is 0
                Pet otherPet = Pets[(CurrentPet - 1) * (-1)];
                if (otherPet.Hunger > 20) Console.WriteLine($"\n > Don't forget to feed {otherPet.Name} as well!");

                // Collect the Player's input for their next action
                int menuChoice = CollectMenuAnswer("\nWhat are you going to do next? Choose from the following:\n\n  1. Feed\n  2. Play\n  3. Speak with pet\n  4. Choose Pet\n  5. Exit\n\n > ", 1, 5, () =>
                {
                    ClearConsole();
                    Console.WriteLine(Pet.asciiArt[(int)Pets[CurrentPet].Type] + "\n\n" + Pets[CurrentPet].GetStatusWindow());
                    return 0;
                });

                // The Console Window should not be refreshed from now on
                CurrentlyInMenu = false;

                // This var is used to break out of the infinite 'While' loop if the Player wants to exit the game.
                bool endGame = false;
                
                switch (menuChoice)
                {
                    // If the Player selected 'Feed'
                    case 0:
                        int previousHunger = Pets[CurrentPet].Hunger;
                        // 'Pet.Eat(true)' is to indicate that a random value is to be used for this call
                        int amountFed = (Pets[CurrentPet].Eat(true) - previousHunger) * (-1);

                        // Output something depending on if the Pet was hungry in the first place
                        if (amountFed > 0) Console.WriteLine($"\nYou fed {Pets[CurrentPet].Name} {amountFed} unit{((amountFed != 1) ? "s" : "")} of food.");
                        else Console.WriteLine($"\n{Pets[CurrentPet].Name} isn't hungry right now. Try again when its hunger goes up!");

                        Console.Write("\nTo continue, press [ RETURN ]");
                        Console.ReadLine();

                        break;

                    // If the Player selected 'Play'
                    case 1:
                        int previousBoredom = Pets[CurrentPet].Boredom;
                        // 'Pet.Play(true)' is to indicate that a random value is to be used for this call
                        int amountPlayed = (Pets[CurrentPet].Play(true) - previousBoredom) * (-1);

                        // Output something depending on if the Pet was bored in the first place
                        if (amountPlayed > 0) Console.WriteLine($"\nYou played with {Pets[CurrentPet].Name} for {amountPlayed} minute{((amountPlayed != 1) ? "s" : "")}.");
                        else Console.WriteLine($"\n{Pets[CurrentPet].Name} doesn't need any attention right now. Try again when its boredom goes up!");

                        Console.Write("\nTo continue, press [ RETURN ]");
                        Console.ReadLine();

                        break;

                    // If the Player selected 'Talk'
                    case 2:
                        Console.WriteLine($"\n{Pets[CurrentPet].Name} says:");
                        Pets[CurrentPet].Talk();

                        Console.Write("\nTo continue, press [ RETURN ]");
                        Console.ReadLine();

                        break;
                            
                    // If the Player selected 'Switch Pets'
                    case 3:
                        string petSelectString = $"\nSelect a pet to pet:\n\n";
                        for (int i = 0; i < Pets.Length; i++) petSelectString += $"  {i + 1}. {Pets[i].Name}\n";
                        petSelectString += " > ";

                        // Collect the Player's input for their Pet selection
                        CurrentPet = CollectMenuAnswer(petSelectString, 1, Pets.Length, () =>
                        {
                            ClearConsole();
                            return 0;
                        });

                        Console.WriteLine($"\nYou have selected {Pets[CurrentPet].Name} the {Pets[CurrentPet].Type}.");

                        break;

                    // If the Player selected 'Exit Game'
                    case 4:
                        endGame = true;
                        break;
                }
                
                // If the Player pressed exit, the true game loop will be broken out of, moving onto the next chunk of code
                if (endGame) break;
            }

            Console.WriteLine("\nThanks for playing!");
            ExitProgram();
        }

        // Create a util function that completely clears the console
        public static void ClearConsole()
        {
            /*
             * This sets the Console's Cursor to the beginning of the Console
             * Anything outputted from here on out will overwrite the previous outputs
             * https://msdn.microsoft.com/en-us/library/system.console.setcursorposition(v=vs.110).aspx
             */
            Console.SetCursorPosition(0, 0);
            // This basically prints 100 lines of empty lines, which will be overwritten by new outputs to the Console
            // 'Console.WindowWidth' on its own does not seem to erase any lines at all, hence the '- 1'
            for (int i = 0; i < 100; ++i) Console.WriteLine(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, 0);
        }

        // This util function outputs the currently selected Pet's info (Hunger, Boredom, Mood, etc.)
        public static void DisplayStatus(object source, ElapsedEventArgs e)
        {
            // Only update the status if the Player is currently in the menu and the game is not over
            if (CurrentlyInMenu && !GameOver)
            {
                // Output the current Pet's Status Window to the Console
                ClearConsole();
                Console.WriteLine(Pet.asciiArt[(int)Pets[CurrentPet].Type] + "\n\n" + Pets[CurrentPet].GetStatusWindow());

                // The other pet will always be synonymous with the equation (CurrentPet - 1) * (-1)
                // i.e. if 'CurrentPet' == 0, then 'otherPet' will be -1 * -1, which is 1
                // i.e. if 'CurrentPet' == 1, then 'otherPet' will be 0 * -1, which is 0
                Pet otherPet = Pets[(CurrentPet - 1) * (-1)];
                // If the other pet's Hunger + Boredom is more than 20, tell the Player
                if (otherPet.Mood > 25) Console.WriteLine($"\n > Don't forget to tend to {otherPet.Name} as well!");

                // Repeat the Menu selection string since the original string was overwritten
                Console.Write("\nWhat are you going to do next? Choose from the following:\n\n  1. Feed\n  2. Play\n  3. Speak with pet\n  4. Choose Pet\n  5. Exit\n\n > ");
            }
        }

        /*
         * This is a custom function that returns an input from the player in response to a menu selection screen
         * I made this function because I found myself using the same block of code over and over again just to collect the player's input,
         * so for re-usability's and readability's sake, I made a function that does this for me
         * This function uses a 'Func<> Delegate': https://msdn.microsoft.com/en-us/library/bb549151(v=vs.110).aspx
         */
        public static int CollectMenuAnswer(string ChoiceString, int minInput, int maxInput, Func<int> WrongAnswerFunction)
        {
            Console.Write(ChoiceString);
            bool inputValid = int.TryParse(Console.ReadLine(), out int input);

            // While the player's input is invalid OR out of bounds, do the following
            while (!inputValid || (input < minInput || input > maxInput))
            {
                // Call the Delegate function
                // I'm not sure why, but this delegate function requires that a Type is returned.
                // This is why one will notice that every time this function is called, a 'return 0' is included as a work-around
                WrongAnswerFunction();
                
                Console.Write(ChoiceString);
                // Try to parse player's input again
                inputValid = int.TryParse(Console.ReadLine(), out input);
            }
            // Detract 1 from the user's input to make it program-compatible
            input--;
            
            return input;
        }

        public static StringBuilder CalculateTimeSpan(DateTime Start, DateTime End)
        {
            // Subtract the time of birth with the time of death to get the TimeSpan of life for this Pet
            TimeSpan Length = Start.Subtract(End);

            // The following chunk is just appending the appropriate units of time
            // Say, the pet lived for 5 minutes, it shouldn't display '0 Hours and 5 Minutes', but only '5 Minutes'
            StringBuilder LengthString = new StringBuilder();
            if (Length.TotalHours >= 1) LengthString.Append($"{Math.Ceiling(Length.TotalHours)} Hour{(Length.TotalHours == 1 ? "" : "s")} ");
            if (Length.TotalMinutes >= 1 && Length.TotalHours > 0) LengthString.Append($"{Length.Minutes} Minute{(Length.Minutes == 1 ? "" : "s")} ");
            else if (Length.TotalMinutes >= 1) LengthString.Append($"{Math.Ceiling(Length.TotalMinutes)} Minute{(Length.TotalMinutes == 1 ? "" : "s")}");
            if (Length.TotalSeconds >= 1 && Length.TotalMinutes > 0) LengthString.Append($"and {Length.Seconds} Second{(Length.Seconds == 1 ? "" : "s")}");
            else if (Length.TotalSeconds >= 1) LengthString.Append($"and {Math.Ceiling(Length.TotalSeconds)} Second{(Length.TotalSeconds == 1 ? "" : "s")}");

            return LengthString;
        }

        public static void EndGame()
        {
            ClearConsole();

            // Filter out all Pets that are "Passed Out" and select the first entry
            // https://msdn.microsoft.com/en-us/library/bb534803(v=vs.110).aspx
            Pet passedOutPet = Pets.Where((p) => p.Mood >= 100).ToArray()[0];

            StringBuilder LifeSpan = CalculateTimeSpan(passedOutPet.Born, passedOutPet.Died);

            Console.Write($"It seems that your {passedOutPet.Type} {passedOutPet.Name} has passed out! It lived for {LifeSpan.ToString()}.\n\nThe game is over. Do you want to restart? (y/n)\n > ");
            if (Console.ReadLine().ToLower().StartsWith("y")) Main(new String[0]);
            else Environment.Exit(0);
        }

        public static void ExitProgram()
        {
            // Initiate the timer for exiting the process
            int exitCount = 4;

            System.Timers.Timer exitTimer = new System.Timers.Timer();
            exitTimer.Interval = 1000;

            // The following code includes a lambda expression, (used multiple times throughout):
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions
            exitTimer.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs e) =>
            {
                // This function will be executed every time the Timer lapses (1 second)

                exitCount--;
                Console.Write("\x000DExiting in " + exitCount);
                if (exitCount == 0) ExitProgram();
            });

            exitTimer.Enabled = true;

            // Keep the process alive
            while (true) Thread.Sleep(6000);
        }
    }
}
