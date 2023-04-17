using System;

namespace Ion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Ion: Habbo Hotel server emulation environment";
            Console.Beep();

            IonEnvironment.Initialize();

            // Input loop
            while (true)
            {
                Console.ReadKey(true);

                IonEnvironment.GetLog().WriteInformation("Shutting down...");
                IonEnvironment.GetLog().WriteInformation("Press a key to exit.");
                Console.ReadKey(true);
                IonEnvironment.Destroy();
            }
        }
    }
}
