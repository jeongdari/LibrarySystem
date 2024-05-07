﻿using System;
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

        // Check if the movie already exists in the linked list
        Movie? existingMovie = hashTable[hash].FirstOrDefault(m => m.Title == title);
        if (existingMovie != null)
        {
            // Add copies to the existing movie
            existingMovie.AddCopies(numCopies);
            Console.WriteLine($"Added {numCopies} copies of '{title}' to the library.");
        }
        else
        {
            Console.WriteLine($"Movie '{title}' not found in the library.");
        }
    }



    public bool RemoveMovie(string title, int numCopiesToRemove)
    {
        int hash = CalculateHash(title);

        if (hashTable.ContainsKey(hash))
        {
            Movie? movieToRemove = hashTable[hash].FirstOrDefault(m => m.Title == title);
            if (movieToRemove != null)
            {
                bool removed = movieToRemove.RemoveCopies(numCopiesToRemove);
                if (removed && movieToRemove.CopiesAvailable == 0)
                {
                    hashTable[hash].Remove(movieToRemove);
                }
                return removed;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
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

                // Check if the member has reached the borrowing limit
                if (member.BorrowedMovies.Count >= 5)
                {
                    Console.WriteLine("Maximum borrowing limit (5 movies) reached.");
                    return null;
                }

                // Borrow the movie
                movieToBorrow.BorrowCopy();
                member.BorrowMovie(movieToBorrow); // Add the movie to the member's borrowed list
                movieToBorrow.Borrower = member; // Set the borrower to the member who borrowed the movie
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

    public LinkedList<Movie> GetBorrowingMovies(Member member)
{
    var borrowingMovies = hashTable.SelectMany(pair => pair.Value.Where(m => m.CopiesAvailable < m.TotalCopies && member.HasBorrowedMovie(m)));

    return new LinkedList<Movie>(borrowingMovies);
}


    public LinkedList<Movie> GetMostFrequentlyBorrowedMovies(int topCount)
    {
        var allMovies = hashTable.SelectMany(pair => pair.Value);
        var sortedMovies = allMovies.OrderByDescending(m => m.TimesBorrowed).Take(topCount);
        return new LinkedList<Movie>(sortedMovies);
    }
    public Movie FindMovieByTitle(string title)
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

        return null!; // Movie not found
    }
}
