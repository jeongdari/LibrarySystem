using System;
using ConsoleTables;
using System.Collections.Generic;
using System.Linq;

public class MovieCollection
{
    private const int MaxMoviePairs = 1000;
    private const int HashTableSize = 2000;
    private Dictionary<int, LinkedList<Movie>> hashTable;

    public MovieCollection()
    {
        hashTable = new Dictionary<int, LinkedList<Movie>>();
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

    public void AddMovie(string title, Genre genre, Classification classification, int durationMinutes, int numCopies)
    {
        if (hashTable.Count >= MaxMoviePairs)
        {
            Console.WriteLine("Error: Maximum movie capacity reached. Unable to add new movie.");
            return;
        }

        int hash = CalculateHash(title);

        if (!hashTable.ContainsKey(hash))
        {
            hashTable[hash] = new LinkedList<Movie>();
        }

        // Check if the movie already exists in the linked list
        Movie? existingMovie = hashTable[hash].FirstOrDefault(m => m.Title == title);
        if (existingMovie != null)
        {
            // This method is responsible for adding copies to an existing movie
            existingMovie.AddCopies(numCopies);
            Console.WriteLine($"Added {numCopies} copies of '{title}' to the library.");
        }
        else
        {
            // Create a new movie object and add it to the linked list
            Movie newMovie = new Movie(title, genre, classification, durationMinutes, numCopies);
            hashTable[hash].AddLast(newMovie);
            Console.WriteLine($"'{title}' added to the library with {numCopies} copies.");
        }
    }

    public void AddMovieCopies(string title, int numCopies)
    {
        int hash = CalculateHash(title);

        if (!hashTable.ContainsKey(hash) || hashTable[hash].All(m => m.Title != title))
        {
            Console.WriteLine($"Error: Movie '{title}' not found in the library.");
            return;
        }

        // Add copies to the existing movie
        Movie existingMovie = hashTable[hash].First(m => m.Title == title);
        existingMovie.AddCopies(numCopies);
        Console.WriteLine($"Added {numCopies} copies of '{title}' to the library.");
    }

    public bool RemoveMovie(string title, int numCopiesToRemove)
    {
        int hash = CalculateHash(title);

        if (!hashTable.ContainsKey(hash) || hashTable[hash].All(m => m.Title != title))
        {
            Console.WriteLine($"Error: Movie '{title}' not found in the library.");
            return false;
        }

        // Remove copies from the existing movie
        Movie movieToRemove = hashTable[hash].First(m => m.Title == title);
        bool removed = movieToRemove.RemoveCopies(numCopiesToRemove);
        if (removed && movieToRemove.CopiesAvailable == 0)
        {
            hashTable[hash].Remove(movieToRemove);
        }

        return removed;
    }

    public void DisplayAllMovies()
    {
        var allMovies = hashTable.SelectMany(pair => pair.Value);
        var sortedMovies = allMovies.OrderBy(m => m.Title);
        Console.WriteLine("Movies currently available:");
        var table = new ConsoleTable("Title", "Genre", "Classification", "Copies Available");

        foreach (var movie in sortedMovies)
        {
            table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.CopiesAvailable);
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
                var table = new ConsoleTable("Title", "Genre", "Classification", "Duration (minutes)", "Copies Available");
                table.AddRow(movie.Title, movie.MovieGenre, movie.MovieClassification, movie.DurationMinutes, movie.CopiesAvailable);
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
    int hash = CalculateHash(title);

    // Check if the movie exists in the library
    if (!hashTable.ContainsKey(hash) || hashTable[hash].All(m => m.Title != title))
    {
        Console.WriteLine($"\nError: Movie '{title}' does not exist in the library.");
        return null;
    }

    while (true) // Loop until a valid movie is borrowed or user cancels
    {
        // Find the movie to borrow
        Movie? movieToBorrow = hashTable[hash].FirstOrDefault(m => m.Title == title && m.CopiesAvailable > 0);

        if (movieToBorrow == null)
        {
            Console.WriteLine($"\nError: Movie '{title}' is not available for borrowing.");
            return null; // No valid movie to borrow, exit method
        }

        // Check if the member has already borrowed this movie
        if (member.HasBorrowedMovie(movieToBorrow))
        {
            Console.WriteLine("\nError: You have already borrowed this movie.");
            return null; // Member already borrowed this movie, exit method
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
}