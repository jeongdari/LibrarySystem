using System;
using ConsoleTables;
using System.Linq;

public class MovieCollection
{
    private const int MaxMoviePairs = 1000;
    private const int HashTableSize = 2000;
    private LinkedList<Movie>[] hashTable;
    private List<Member> members = new List<Member>();

    public MovieCollection()
    {
        hashTable = new LinkedList<Movie>[HashTableSize];
    }

    private static int CalculateHash(string key)
    {
        int primeNumber = 67;
        int hash = 0;
        foreach (char c in key)
        {
            hash = (hash * primeNumber + (int)c) % HashTableSize;
        }
        return hash;
    }

    private LinkedList<Movie> GetHashTable(int index)
    {
        if (hashTable[index] == null)
        {
            hashTable[index] = new LinkedList<Movie>();
        }
        return hashTable[index];
    }

    public void AddMovie(string title, Genre genre, Classification classification, int durationMinutes, int numCopies)
    {
        int hash = CalculateHash(title);
        LinkedList<Movie> movieList = GetHashTable(hash);
        // Check the current number of movies at the hash index
        int currentMovieCount = movieList.Count;

        if (currentMovieCount >= MaxMoviePairs)
        {
            Console.WriteLine("Error: Maximum movie capacity reached. Unable to add new movie.");
            return;
        }
        Movie? existingMovie = movieList.FirstOrDefault(m => m.Title == title);
        if (existingMovie != null)
        {
            // This method is responsible for adding copies to an existing movie
            existingMovie.AddCopies(numCopies);
            Console.WriteLine($"\nAdded {numCopies} copies of '{title}' to the library.");
        }
        else
        {
            // Create a new movie object and add it to the linked list
            Movie newMovie = new Movie(title, genre, classification, durationMinutes, numCopies);
            movieList.AddLast(newMovie); // Add the new movie to the linked list at the hash index
            Console.WriteLine($"\n'{title}' added to the library with {numCopies} copies.");
        }
    }


    public void AddMovieCopies(string title, int numCopies)
    {
        int hash = CalculateHash(title);
        LinkedList<Movie> movieList = GetHashTable(hash);

        Movie? existingMovie = movieList.FirstOrDefault(m => m.Title == title);

        if (existingMovie != null)
        {
            existingMovie.AddCopies(numCopies);
            Console.WriteLine($"Added {numCopies} copies of '{title}' to the library.");
        }
        else
        {
            Console.WriteLine($"Error: Movie '{title}' not found in the library.");
        }
    }

    public bool RemoveMovie(string title, int numCopiesToRemove)
{
    int hash = CalculateHash(title);
    LinkedList<Movie> movieList = GetHashTable(hash);

    // Find the existing movie in the linked list
    Movie? existingMovie = movieList.FirstOrDefault(m => m.Title == title);
    if (existingMovie == null)
    {
        Console.WriteLine($"Error: Movie '{title}' not found in the library.");
        return false;
    }

    // Check if there are enough available copies to remove
    if (existingMovie.CopiesAvailable < numCopiesToRemove)
    {
        Console.WriteLine($"Error: You can only remove up to {existingMovie.CopiesAvailable} copies for '{title}'.");
        return false;
    }

    // Attempt to remove the specified number of copies
    bool removed = existingMovie.RemoveCopies(numCopiesToRemove);

    // Check if all copies (including borrowed) have been removed
    if (existingMovie.CopiesAvailable == 0)
    {
        // Check for active borrowing records
        bool hasActiveBorrowing = CheckActiveBorrowingRecords(existingMovie);
        if (hasActiveBorrowing)
        {
            Console.WriteLine($"There are still borrowed copies of '{title}' in the system. Movie information will be retained.");
        }
        else
        {
            // No active borrowing records, remove the movie from the list
            movieList.Remove(existingMovie);
            Console.WriteLine($"'{title}' has been completely removed from the library.");
        }
    }

    return removed;
}
    private bool CheckActiveBorrowingRecords(Movie movie)
    {
        // Retrieve all members who have borrowed this movie
        foreach (Member member in members)
        {
            if (member.HasBorrowedMovie(movie))
            {
                return true; // There is an active borrowing record
            }
        }

        return false; // No active borrowing records found
    }

    public void DisplayAllMovies()
    {
        Console.WriteLine("Movies currently available:");

        var allMovies = hashTable.SelectMany(list => list ?? Enumerable.Empty<Movie>())
                                 .OrderBy(movie => movie.Title);

        if (!allMovies.Any())
        {
            Console.WriteLine("No movies found in the library.");
            return;
        }

        var table = new ConsoleTable("Title", "Genre", "Classification", "Copies Available");
        foreach (var movie in allMovies)
        {
            table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.CopiesAvailable);
        }
        table.Write(Format.Default);
    }


    public void DisplayMovieInfo(string title)
    {
        int hash = CalculateHash(title);

        LinkedList<Movie> movieList = GetHashTable(hash);

        // Find the movie with the specified title in the linked list
        Movie? movie = movieList.FirstOrDefault(m => m.Title == title);
        if (movie != null)
        {
            // Display movie information in a formatted table
            var table = new ConsoleTable("Title", "Genre", "Classification", "Duration (minutes)", "Copies Available", "Times Borrowed");
            table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.DurationMinutes, movie.CopiesAvailable, movie.TimesBorrowed);
            table.Write(Format.Default);
        }
        else
        {
            // Movie with the specified title not found in the library
            Console.WriteLine($"Movie '{title}' not found in the library.");
        }
    }

    public Movie? BorrowMovie(Member member, string? title)
    {
        while (true) // Loop until a valid movie is borrowed or user cancels
        {
            if (title == null)
            {
                Console.WriteLine("Invalid movie title provided.");
                return null;
            }

            int hash = CalculateHash(title);
            LinkedList<Movie> movieList = GetHashTable(hash);

            // Find the movie to borrow
            Movie? movieToBorrow = movieList.FirstOrDefault(m => m.Title == title);

            // Check if the member has already borrowed this movie
            if (movieToBorrow != null && member.HasBorrowedMovie(movieToBorrow))
            {
                Console.WriteLine("\nError: You have already borrowed this movie.");
                Console.WriteLine("Please enter a different movie title or type 'Q' to exit:");
                title = Console.ReadLine(); // Prompt user to enter a different movie title or cancel
                if (title?.ToUpper() == "Q")
                {
                    Console.WriteLine("Borrowing operation canceled.");
                    return null;
                }
                continue;
            }

            // Check if the movie exists in the library and has available copies
            if (movieToBorrow == null || movieToBorrow.CopiesAvailable == 0)
            {
                Console.WriteLine("\nSorry, all copies of the movie you are requesting have been borrowed or it is not available. Please try another movie.");
                Console.WriteLine("Please enter a valid movie title or type 'Q' to exit:");
                title = Console.ReadLine(); // Prompt user to enter a different movie title or cancel
                if (title?.ToUpper() == "Q")
                {
                    Console.WriteLine("Borrowing operation canceled.");
                    return null;
                }
                continue;
            }

            // Check if the member has reached the borrowing limit
            if (member.BorrowedMovies.Count >= 5)
            {
                Console.WriteLine("\nError: Maximum borrowing limit (5 movies) reached. \nIf you wish to borrow a new movie, please return one of the movies you have borrowed.");
                return null; // Member reached borrowing limit, exit method
            }

            // Attempt to borrow the movie
            movieToBorrow.BorrowCopy();
            member.BorrowMovie(movieToBorrow); // Add the movie to the member's borrowed list
            movieToBorrow.Borrower = member; // Set the borrower to the member who borrowed the movie
            Console.WriteLine($"Successfully borrowed '{movieToBorrow.Title}'. Enjoy watching!");
            return movieToBorrow; // Return the borrowed movie
        }
    }
    public void ReturnMovie(Movie movie)
    {
        int hash = CalculateHash(movie.Title);

        LinkedList<Movie> movieList = GetHashTable(hash);

        if (movieList == null || movieList.All(m => m.Title != movie.Title))
        {
            Console.WriteLine($"\nError: Movie '{movie.Title}' not found in the library.");
            return;
        }

        Movie? movieToReturn = movieList.FirstOrDefault(m => m.Title == movie.Title);
        if (movieToReturn != null)
        {
            movieToReturn.ReturnCopy();
        }
    }
    public LinkedList<Movie> GetBorrowingMovies(Member member)
    {
        List<Movie> borrowingMovies = new List<Movie>();

        foreach (LinkedList<Movie> movieList in hashTable)
        {
            if (movieList != null)
            {
                foreach (Movie movie in movieList)
                {
                    if (movie.CopiesAvailable < movie.TotalCopies && member.HasBorrowedMovie(movie))
                    {
                        borrowingMovies.Add(movie);
                    }
                }
            }
        }

        return new LinkedList<Movie>(borrowingMovies);
    }
    public LinkedList<Movie> GetMostFrequentlyBorrowedMovies(int topCount)
    {
        List<Movie> allMovies = new List<Movie>();

        foreach (LinkedList<Movie> movieList in hashTable)
        {
            if (movieList != null)
            {
                allMovies.AddRange(movieList);
            }
        }

        var sortedMovies = allMovies.Where(m => m.TimesBorrowed > 0)
                                    .OrderByDescending(m => m.TimesBorrowed)
                                    .Take(topCount);

        return new LinkedList<Movie>(sortedMovies);
    }

    public Movie? FindMovieByTitle(string title)
    {
        int hash = CalculateHash(title);

        if (hashTable[hash] != null)
        {
            foreach (Movie movie in hashTable[hash])
            {
                if (movie.Title == title)
                {
                    return movie;
                }
            }
        }

        return null;
    }
    public bool ContainsMovie(string movieTitle)
    {
        int hash = CalculateHash(movieTitle);

        return hashTable[hash] != null && hashTable[hash].Any(m => m.Title == movieTitle);
    }
    public void DisplayKeyLocation(string title) // Use Star Wars and foaa for collision
    {
        int hash = CalculateHash(title);
        Console.WriteLine("-----------------------------------------------------------------------------");
        Console.WriteLine($"Hash index for '{title}': {hash}"); // Print the hash index  

        LinkedList<Movie> movieList = hashTable[hash];
        if (movieList != null)
        {
            int index = 0;
            bool found = false;

            foreach (var movie in movieList)
            {
                if (movie.Title == title)
                {
                    Console.WriteLine($"Movie '{title}' stored at node index {index} in the linked list at hash index {hash}.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    found = true;
                }
                index++;
            }
            if (!found)
            {
                Console.WriteLine($"Movie '{title}' not found in the linked list at hash index {hash}.");
            }
        }
        else
        {
            Console.WriteLine($"No movies found in the library at hash index {hash}.");
        }
    }

}