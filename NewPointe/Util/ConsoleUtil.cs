using System;
using System.Security;
using System.Text;

namespace NewPointe.Util
{
    public static class ConsoleUtil
    {

        /// Reads a password from the console
        public static string ReadPassword()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.Write("\n");
                        return input.ToString();

                    case ConsoleKey.Backspace:
                        if(input.Length > 0) input.Remove(input.Length - 2, 1);
                        Console.Write("\b \b");
                        break;

                    default:
                        input.Append(key.KeyChar);
                        Console.Write("*");
                        break;
                }

            }
        }
    }
}