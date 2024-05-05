public static class InputHelper
{
    public static string GetNonEmptyInput(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();

            // Check if input is null or empty
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        } while (string.IsNullOrEmpty(input));  // Loop while input is null or empty

        return input.Trim();  // Safe to call Trim() after ensuring input is not null
    }
    public static int GetPositiveIntInput(string prompt)
    {
        int value;
        do
        {
            value = GetIntInput(prompt);
            if (value <= 0)
            {
                Console.WriteLine("Input must be a positive integer.");
            }
        } while (value <= 0);
        return value;
    }

    public static int GetIntInput(string prompt)
    {
        int value;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out value))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
        return value;
    }

    public static int GetIntInput(string prompt, int min, int max)
    {
        int value;
        do
        {
            value = InputHelper.GetIntInput(prompt);
            if (value < min || value > max)
            {
                Console.WriteLine($"Input must be between {min} and {max}.");
            }
        } while (value < min || value > max);
        return value;
    }

    public static Genre GetGenreInput()
    {
        Genre genre;
        Console.WriteLine("Enter genre (drama, adventure, family, action, sci-fi, comedy, animated, thriller, other): ");
        while (true)
        {
            string? userInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Input cannot be empty. Please enter a valid genre.");
                continue; // Restart the loop to prompt for input again
            }
            // Try to parse the input into the Genre enum
            if (!Enum.TryParse(userInput, true, out genre))
            {
                Console.WriteLine($"'{userInput}' is invalid genre. Please enter a valid genre.");
                continue; // Restart the loop to prompt for input again
            }
            // Check if the parsed genre is defined in the Genre enum
            if (!Enum.IsDefined(typeof(Genre), genre))
            {
                // Genre is not defined in the enum
                Console.WriteLine($"'{userInput}' is not a valid genre. Please enter a valid genre.");
                continue; // Restart the loop to prompt for input again
            }
            break;
        }
        return genre;
    }
    public static Classification GetClassificationInput()
    {
        Classification classification;
        while (true)
        {
            Console.WriteLine("Enter classification (G, PG, M15Plus, MA15Plus): ");
            string? userInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue; // Restart the loop to prompt for input again
            }

            // Try to parse the input into the Classification enum
            if (!Enum.TryParse(userInput, true, out classification))
            {
                Console.WriteLine($"'{userInput}' is an invalid classification. Please enter a valid classification.");
                continue; // Restart the loop to prompt for input again
            }

            // Check if the parsed classification is one of the defined values
            if (!Enum.IsDefined(typeof(Classification), classification))
            {
                Console.WriteLine($"'{userInput}' is not a valid classification. Please enter a valid classification.");
                continue; // Restart the loop to prompt for input again
            }

            // Validation succeeded, return the valid classification
            break;
        }

        return classification;
    }
}
