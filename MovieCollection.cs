using System;
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
            Console.WriteLine("Maximum movie capacity reached.");
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
            existingMovie.AddCopies(numCopies);
            Console.WriteLine($"Added {numCopies} copies of '{title}' to the library.");
        }
        else
        {
            // Create a new movie object
            Movie newMovie = new Movie(title, genre, classification, durationMinutes, numCopies);
            hashTable[hash].AddLast(newMovie); // Add the new movie to the end of the linked list
            Console.WriteLine($"'{title}' added to the library with {numCopies} copies.");
        }
    }


    public void RemoveMovie(string title, int numCopiesToRemove)
    {
        int hash = CalculateHash(title);

        if (hashTable.ContainsKey(hash))
        {
            Movie? movieToRemove = hashTable[hash].FirstOrDefault(m => m.Title == title);
            if (movieToRemove != null)
            {
                movieToRemove.RemoveCopies(numCopiesToRemove);
                if (movieToRemove.CopiesAvailable == 0)
                {
                    hashTable[hash].Remove(movieToRemove);
                }
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

    public void DisplayAllMovies()
    {
        var allMovies = hashTable.SelectMany(pair => pair.Value);
        var sortedMovies = allMovies.OrderBy(m => m.Title);
        foreach (var movie in sortedMovies)
        {
            Console.WriteLine($"Title: {movie.Title}, Genre: {movie.MovieGenre}, Classification: {movie.MovieClassification}, Copies Available: {movie.CopiesAvailable}");
        }
    }

    public void DisplayMovieInfo(string title)
    {
        int hash = CalculateHash(title);

        if (hashTable.ContainsKey(hash))
        {
            Movie? movie = hashTable[hash].FirstOrDefault(m => m.Title == title);
            if (movie != null)
            {
                Console.WriteLine($"Title: {movie.Title}, Genre: {movie.MovieGenre}, Classification: {movie.MovieClassification}, Duration: {movie.DurationMinutes} minutes, Copies Available: {movie.CopiesAvailable}");
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

        if (hashTable.ContainsKey(hash))
        {
            Movie? movieToBorrow = hashTable[hash].FirstOrDefault(m => m.Title == title && m.CopiesAvailable > 0);
            if (movieToBorrow != null)
            {
                // Check if the member has already borrowed this movie
                if (member.HasBorrowedMovie(movieToBorrow))
                {
                    Console.WriteLine("You have already borrowed this movie.");
                    return null;
                }

                // Borrow the movie
                movieToBorrow.BorrowCopy();
                member.BorrowMovie(movieToBorrow); // Add the movie to the member's borrowed list
                return movieToBorrow;
            }
        }

        Console.WriteLine($"Movie '{title}' not available for borrowing.");
        return null;
    }

    public void ReturnMovie(Movie movie)
    {
        int hash = CalculateHash(movie.Title);

        if (hashTable.ContainsKey(hash))
        {
            Movie? movieToReturn = hashTable[hash].FirstOrDefault(m => m.Title == movie.Title);
            if (movieToReturn != null)
            {
                movieToReturn.ReturnCopy();
            }
        }
    }

    public LinkedList<Movie> GetBorrowingMovies()
    {
        var borrowingMovies = hashTable.SelectMany(pair => pair.Value.Where(m => m.CopiesAvailable < m.TotalCopies));
        return new LinkedList<Movie>(borrowingMovies);
    }

    public LinkedList<Movie> GetMostFrequentlyBorrowedMovies(int topCount)
    {
        var allMovies = hashTable.SelectMany(pair => pair.Value);
        var sortedMovies = allMovies.OrderByDescending(m => m.TimesBorrowed).Take(topCount);
        return new LinkedList<Movie>(sortedMovies);
    }
}
