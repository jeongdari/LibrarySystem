using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Member
{
    public string FirstName { get; }
    public string LastName { get; }
    public string ContactNumber { get; }
    public string Password { get; }
    public List<Movie> BorrowedMovies { get; } = new List<Movie>();

    public Member(string firstName, string lastName, string contactNumber, string? password)
{
    FirstName = firstName;
    LastName = lastName;
    ContactNumber = contactNumber;

    while (!IsValidPassword(password!))
    {
        Console.WriteLine("Password must be a four-digit number.");
        Console.Write("Please enter a valid password: ");
        password = Console.ReadLine()?.Trim();
    }

    Password = password!;
}

    private bool IsValidPassword(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Length == 4 && int.TryParse(password, out _);
    }

    public bool BorrowMovie(Movie movie)
    {
        if (BorrowedMovies.Count >= 5)
        {
            Console.WriteLine("Maximum borrowing limit (5 movies) reached.");
            return false;
        }

        if (BorrowedMovies.Contains(movie))
        {
            Console.WriteLine("You have already borrowed this movie.");
            return false;
        }

        BorrowedMovies.Add(movie);        
        return true;
    }

    public bool ReturnMovie(Movie movie)
    {
        if (BorrowedMovies.Contains(movie))
        {
            BorrowedMovies.Remove(movie);
            Console.WriteLine($"'{movie.Title}' returned successfully.");
            return true;
        }

        Console.WriteLine($"You have not borrowed '{movie.Title}'.");
        return false;
    }

    public bool HasBorrowedMovie(Movie movie)
    {
        return BorrowedMovies.Contains(movie);
    }

    public bool HasBorrowedMovieByTitle(string movieTitle)
    {
        return BorrowedMovies.Exists(m => m.Title == movieTitle);
    }

    public void DisplayBorrowedMovies()
    {
        Console.WriteLine($"Borrowed Movies for {FirstName} {LastName}:");
        foreach (var movie in BorrowedMovies)
        {
            Console.WriteLine($"- {movie.Title}");
        }
    }
    public Movie? FindBorrowedMovieByTitle(string title)
    {
        foreach (var movie in BorrowedMovies)
        {
            if (movie.Title == title)
            {
                return movie;
            }
        }

        return null; // Movie not found among borrowed movies
    }
}