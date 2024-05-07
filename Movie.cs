using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Genre
{
    Drama, Adventure, Family, Action, SciFi, Comedy, Animated, Thriller, Other
}

public enum Classification
{
    G, PG, M15Plus, MA15Plus
}

public class Movie
{
    public string Title { get; }
    public Genre MovieGenre { get; }
    public Classification MovieClassification { get; }
    public int DurationMinutes { get; }
    public Member? Borrower { get; set; }
    public int TotalCopies { get; private set; }
    public int CopiesAvailable { get; private set; }
    public int TimesBorrowed { get; private set; }

    public Movie(string title, Genre genre, Classification classification, int durationMinutes, int totalCopies)
    {
        Title = title;
        MovieGenre = genre;
        MovieClassification = classification;
        DurationMinutes = durationMinutes;
        TotalCopies = totalCopies;
        CopiesAvailable = totalCopies;
        TimesBorrowed = 0;
        Borrower = null;
    }

    public void AddCopies(int numCopiesToAdd)
    {
        TotalCopies += numCopiesToAdd;
        CopiesAvailable += numCopiesToAdd;
    }

    public bool RemoveCopies(int numCopiesToRemove)
    {
        if (numCopiesToRemove <= CopiesAvailable)
        {
            CopiesAvailable -= numCopiesToRemove;
            return true; // Successfully removed copies
        }
        else
        {
            return false; // Not enough copies available to remove
        }
    }

    public void BorrowCopy()
    {
        if (CopiesAvailable > 0)
        {
            CopiesAvailable--;
            TimesBorrowed++;
        }
        else
        {
            Console.WriteLine($"No copies of '{Title}' available to borrow.");
        }
    }

    public void ReturnCopy()
    {
        if (CopiesAvailable < TotalCopies)
        {
            CopiesAvailable++;
        }
        else
        {
            Console.WriteLine($"All copies of '{Title}' have been returned.");
        }
    }
}
