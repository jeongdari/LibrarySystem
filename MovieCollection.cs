using System;
using ConsoleTables;
using System.Linq;

public class MovieCollection
{
    private const int MaxMoviePairs = 1000;
    private const int HashTableSize = 2000;
    private LinkedList<Movie>[] hashTable;

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
        if (hashTable[index] == null )
        {
            hashTable[index] = new LinkedList<Movie>();
        }
        return hashTable[index];
    }

    public void AddMovie(string title, Genre genre, Classification classification, int durationMinutes, int numCopies)
    {
        if (hashTable.Length >= MaxMoviePairs)
        {
            Console.WriteLine("Error: Maximum movie capacity reached. Unable to add new movie.");
            return;
        }

        int hash = CalculateHash(title);
        LinkedList<Movie> movieList = GetHashTable(hash);

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
            hashTable[hash].AddLast(newMovie);
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
        if (existingMovie != null)
        {
            bool removed = existingMovie.RemoveCopies(numCopiesToRemove);
            if (existingMovie.CopiesAvailable == 0)
            {
                movieList.Remove(existingMovie);
            }
            return removed;
        }
        else
        {
            Console.WriteLine($"Error: Movie '{title}' not found in the library.");
            return false;
        }
    }        

    public void DisplayAllMovies()
    {        
        Console.WriteLine("Movies currently available:");
        var table = new ConsoleTable("Title", "Genre", "Classification", "Copies Available");

        foreach (var movieList in hashTable.Where(list => list != null))
        {
            foreach (var movie in movieList)
            {
                table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.CopiesAvailable);
            }            
        }
        table.Write(Format.Default);
    }

    public void DisplayMovieInfo(string title)
    {
        int hash = CalculateHash(title);

        if (hashTable.ContainsKey(hash))
        {
            Movie? movie = hashTable[hash].FirstOrDefault(m => m.Title == title);
            if (movie != null)
            {
                var table = new ConsoleTable("Title", "Genre", "Classification", "Duration\n(minutes)", "Copies\nAvailable", "Times\nRented");
                table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.DurationMinutes, movie.CopiesAvailable, movie.TimesBorrowed);
                table.Write(Format.Default);
            }
            else
            {
                Console.WriteLine($"Movie '{title}' not found in the library.");
            }
        }
        else
        {
            Console.WriteLine($"Movie '{title}' not found in the library.");
        }
    }

    public Movie? BorrowMovie(Member member, string title)
    {
        while (true) // Loop until a valid movie is borrowed or user cancels
        {
            int hash = CalculateHash(title);

            // Check if the movie exists in the library
            if (!hashTable.ContainsKey(hash) || hashTable[hash].All(m => m.Title != title))
            {
                Console.WriteLine($"\nError: Movie '{title}' does not exist in the library.");
                Console.WriteLine("Please enter a valid movie title or type 'Q' to exit:");
                title = Console.ReadLine(); // Prompt user to enter a valid movie title or cancel
                if (title.ToUpper() == "Q")
                {
                    Console.WriteLine("Borrowing operation canceled.");
                    return null; 
                }
                continue;
            }

            // Find the movie to borrow
            Movie? movieToBorrow = hashTable[hash].FirstOrDefault(m => m.Title == title && m.CopiesAvailable > 0);

            if (movieToBorrow == null)
            {
                Console.WriteLine($"\nError: Movie '{title}' is not available for borrowing since all the copies are rented.");
                Console.WriteLine("Please enter a valid movie title or type 'Q' to exit:");
                title = Console.ReadLine(); // Prompt user to enter a different movie title or cancel
                if (title.ToUpper() == "Q")
                {
                    Console.WriteLine("Borrowing operation canceled.");
                    return null; 
                }
                continue;
            }

            // Check if the member has already borrowed this movie
            if (member.HasBorrowedMovie(movieToBorrow))
            {
                Console.WriteLine("\nError: You have already borrowed this movie.");
                Console.WriteLine("Please enter a valid movie title or type 'Q' to exit:");
                title = Console.ReadLine(); // Prompt user to enter a different movie title or cancel
                if (title.ToUpper() == "Q")
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

        if (!hashTable.ContainsKey(hash) || hashTable[hash].All(m => m.Title != movie.Title))
        {
            Console.WriteLine($"\nError: Movie '{movie.Title}' not found in the library.");
            return;
        }

        Movie? movieToReturn = hashTable[hash].FirstOrDefault(m => m.Title == movie.Title);
        if (movieToReturn != null)
        {
            movieToReturn.ReturnCopy();
        }
    }

    public LinkedList<Movie> GetBorrowingMovies(Member member)
    {
        var borrowingMovies = hashTable.SelectMany(pair => pair.Value.Where(m => m.CopiesAvailable < m.TotalCopies && member.HasBorrowedMovie(m)));
        return new LinkedList<Movie>(borrowingMovies);
    }

    public LinkedList<Movie> GetMostFrequentlyBorrowedMovies(int topCount)
    {
        var allMovies = hashTable.SelectMany(pair => pair.Value);
        var sortedMovies = allMovies.Where(m => m.TimesBorrowed > 0)
                                .OrderByDescending(m => m.TimesBorrowed)
                                .Take(topCount);
        return new LinkedList<Movie>(sortedMovies);
    }
    public Movie? FindMovieByTitle(string title)
    {
        foreach (var movies in hashTable.Values)
        {
            foreach (var movie in movies)
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
        // Check if the movie exists in the hash table of movies
        int hash = CalculateHash(movieTitle);
        return hashTable.ContainsKey(hash) && hashTable[hash].Any(m => m.Title == movieTitle);
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
                    Console.WriteLine($"Movie '{title}' found at node index {index} in the linked list at hash index {hash}.");
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