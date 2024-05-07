using System;
using System.Threading;

public class LibrarySystem
{
    private readonly MovieCollection movieCollection;
    private readonly MemberCollection memberCollection;

    public LibrarySystem()
    {
        movieCollection = new MovieCollection();
        memberCollection = new MemberCollection();
    }

    public void Start()
    {
        while (true)
        {
            DisplayMainMenu();
            int option = InputHelper.GetIntInput("Enter your choice: ", 1, 3);

            switch (option)
            {
                case 1:
                    StaffLogin();
                    break;
                case 2:
                    MemberLogin();
                    break;
                case 3:
                    Console.WriteLine("Exiting...");
                    return;
            }
        }
    }

    private void DisplayMainMenu()
    {
        Console.WriteLine("\n********************************************************");
        Console.WriteLine("     COMMUNITY LIBRARY MOVIE DVD MANAGEMENT SYSTEM");
        Console.WriteLine("********************************************************");
        Console.WriteLine("\n[Main Menu]");
        Console.WriteLine("");
        Console.WriteLine("1. Login as Staff");
        Console.WriteLine("2. Login as Member");
        Console.WriteLine("3. Exit");
        Console.WriteLine("");
    }

    private void StaffLogin()
    {
        string username = InputHelper.GetNonEmptyInput("Enter staff username: ");
        string password = InputHelper.GetNonEmptyInput("Enter staff password: ");

        if (username == "staff" && password == "today123")
        {
            Console.WriteLine("\nStaff login successful!");
            Thread.Sleep(2000);
            Console.Clear();
            StaffMenu();
        }
        else
        {
            Console.WriteLine("\nInvalid username or password.");
            Thread.Sleep(2000);
            Console.Clear();
        }
    }

    private void StaffMenu()
    {
        while (true)
        {
            DisplayStaffMenu();
            int option = InputHelper.GetIntInput("Enter your choice: ", 1, 7);

            switch (option)
            {
                case 1:
                    AddMovieDVD();
                    break;
                case 2:
                    RemoveMovieDVD();
                    break;
                case 3:
                    RegisterNewMember();
                    break;
                case 4:
                    RemoveRegisteredMember();
                    break;
                case 5:
                    FindMembersContactNumber();
                    break;
                case 6:
                    DisplayMembersRentingMovie();
                    break;
                case 7:
                    return; // Return to main menu
            }
        }
    }

    private void DisplayStaffMenu()
    {
        Console.WriteLine("\n********************************************************");
        Console.WriteLine("     COMMUNITY LIBRARY MOVIE DVD MANAGEMENT SYSTEM");
        Console.WriteLine("********************************************************");
        Console.WriteLine("\n[Staff Menu]");
        Console.WriteLine("");
        Console.WriteLine("1. Add Movie DVDs");
        Console.WriteLine("2. Remove Movie DVDs");
        Console.WriteLine("3. Register New Member");
        Console.WriteLine("4. Remove Registered Member");
        Console.WriteLine("5. Find Member's Contact Number");
        Console.WriteLine("6. Display Members Renting a Movie");
        Console.WriteLine("7. Return to Main Menu");
        Console.WriteLine("");
    }

    private void AddMovieDVD()
    {
        Console.WriteLine("\n<Add Movie DVDs>");

        string title = InputHelper.GetNonEmptyInput("Enter movie title: ");

        // Check if the movie already exists in the library
        Movie? existingMovie = movieCollection.FindMovieByTitle(title);
        if (existingMovie != null)
        {
            Console.WriteLine ("The movie you are trying to add already exists, so you only add the number of new copies.");
            int numCopies = InputHelper.GetPositiveIntInput("How many copies do you want to add? ");

            // Update copies of the existing movie
            movieCollection.AddMovieCopies(title, numCopies);
        }
        else
        {
            // Prompt for genre, classification, duration, and number of copies to add a new movie
            AddNewMovie(title);
        }
    }
    private void AddNewMovie(string title)
    {
        Genre genre = InputHelper.GetGenreInput();
        Classification classification = InputHelper.GetClassificationInput();
        int durationMinutes = InputHelper.GetPositiveIntInput("Enter duration (in minutes): ");
        int numCopies = InputHelper.GetPositiveIntInput("Enter number of copies: ");

        movieCollection.AddMovie(title, genre, classification, durationMinutes, numCopies);
    }
    private void RemoveMovieDVD()
{
    Console.WriteLine("\n<Remove Movie DVDs>");

    string removeTitle = InputHelper.GetNonEmptyInput("Enter movie title: ");

    // Check if the movie exists in the library
    Movie? movieToRemove = movieCollection.FindMovieByTitle(removeTitle);
    if (movieToRemove == null)
    {
        Console.WriteLine($"Movie '{removeTitle}' not found in the library.");
        return;
    }

    int availableCopies = movieToRemove.CopiesAvailable;
while (true) // Loop until a valid number of copies to remove is entered
    {
        // Prompt user to input number of copies to remove
        int removeNumCopies = InputHelper.GetPositiveIntInput($"Enter number of copies to remove (Available Copies: {availableCopies}): ");

        // Validate the number of copies to remove
        if (removeNumCopies > availableCopies)
        {
            Console.WriteLine($"Error: You can only remove up to {availableCopies} copies for '{removeTitle}'.");
        }
        else
        {
            // Attempt to remove the specified number of copies
            bool success = movieCollection.RemoveMovie(removeTitle, removeNumCopies);

            if (success)
            {
                Console.WriteLine($"Successfully removed {removeNumCopies} copies of '{removeTitle}'.");
            }
            else
            {
                Console.WriteLine($"Failed to remove {removeNumCopies} copies of '{removeTitle}'. Movie not found.");
            }

            break; // Exit the loop since a valid operation was performed
        }
    }
}

    private void RegisterNewMember()
    {
        Console.WriteLine("\n<Register New Member>");

        string firstName = InputHelper.GetNonEmptyInput("Enter first name: ");
        string lastName = InputHelper.GetNonEmptyInput("Enter last name: ");

        if (memberCollection.FindMember(firstName, lastName) != null)
        {
            Console.WriteLine("Member already exists.");
            return;
        }

        string contactNumber = InputHelper.GetNonEmptyInput("Enter contact phone number: ");
        string password = InputHelper.GetNonEmptyInput("Enter password: ");

        memberCollection.AddMember(firstName, lastName, contactNumber, password);
    }
    private void RemoveRegisteredMember()
    {
        Console.WriteLine("\n<Remove Registered Member>");

        string removeFirstName = InputHelper.GetNonEmptyInput("Enter member's first name: ");
        string removeLastName = InputHelper.GetNonEmptyInput("Enter member's last name: ");

        memberCollection.RemoveMember(removeFirstName, removeLastName);
    }
    private void FindMembersContactNumber()
    {
        Console.WriteLine("\n<Find Member's Contact Number>");

        string findFirstName = InputHelper.GetNonEmptyInput("Enter member's first name: ");
        string findLastName = InputHelper.GetNonEmptyInput("Enter member's last name: ");

        string? contactNumber = memberCollection.FindMemberContactNumber(findFirstName, findLastName);

        if (contactNumber != null)
        {
            Console.WriteLine($"Contact number for {findFirstName} {findLastName}: {contactNumber}");
        }
        else
        {
            Console.WriteLine($"Member {findFirstName} {findLastName} not found.");
        }
    }
    private void DisplayMembersRentingMovie()
    {
        Console.WriteLine("\n<Display Members Renting a Movie>");

        string movieTitle = InputHelper.GetNonEmptyInput("Enter movie title to display renting members: ");

        bool movieFound = memberCollection.DisplayMembersRentingMovie(movieTitle);

        if (!movieFound)
        {
            Console.WriteLine($"The movie '{movieTitle}' is not found in the library.");
        }
    }
    private void MemberLogin()
    {
        string firstName = InputHelper.GetNonEmptyInput("Enter first name: ");
        string lastName = InputHelper.GetNonEmptyInput("Enter last name: ");
        string password = InputHelper.GetNonEmptyInput("Enter password: ");

        Member? member = memberCollection.FindMember(firstName, lastName);

        if (member != null && member.Password == password)
        {
            Console.WriteLine($"\n{firstName} {lastName} login successful.");
            MemberMenu(member); // Pass the found member to the MemberMenu method
        }
        else
        {
            Console.WriteLine("\nInvalid member credentials.");
        }
    }

    private void MemberMenu(Member member)
    {
        while (true)
        {
            DisplayMemberMenu(member.FirstName, member.LastName);
            int option = InputHelper.GetIntInput("Enter your choice: ", 1, 7);

            switch (option)
            {
                case 1:
                    Console.WriteLine("\n<Browse All Movies>");
                    movieCollection.DisplayAllMovies();
                    break;
                case 2:
                    Console.WriteLine("\n<Display Movie Information>");
                    string movieTitle = InputHelper.GetNonEmptyInput("Enter movie title: ");
                    movieCollection.DisplayMovieInfo(movieTitle);
                    break;
                case 3:
                    BorrowMovie(member);
                    break;
                case 4:
                    ReturnMovie(member);
                    break;
                case 5:
                    ListCurrentBorrowingMovies(member);
                    break;
                case 6:
                    DisplayTopThreeMostBorrowedMovies();
                    break;
                case 7:
                    return; // Return to main menu
            }
        }
    }

    private void DisplayMemberMenu(string firstName, string lastName)
    {
        string memberFullName = $"{firstName} {lastName}";
        Console.WriteLine("\n********************************************************");
        Console.WriteLine("     COMMUNITY LIBRARY MOVIE DVD MANAGEMENT SYSTEM");
        Console.WriteLine("********************************************************");
        Console.WriteLine($"\n[Member Menu for {memberFullName}]");
        Console.WriteLine("");
        Console.WriteLine("1. Browse All Movies");
        Console.WriteLine("2. Display Movie Information");
        Console.WriteLine("3. Borrow a Movie DVD");
        Console.WriteLine("4. Return a Movie DVD");
        Console.WriteLine("5. List Current Borrowing Movies");
        Console.WriteLine("6. Display Top Three Most Borrowed Movies");
        Console.WriteLine("7. Return to Main Menu");
        Console.WriteLine("");
    }
    private void BorrowMovie(Member member)
    {
        Console.WriteLine("\n<Borrow a Movie DVD>");
        string movieTitleToBorrow = InputHelper.GetNonEmptyInput("Enter movie title to borrow: ");
        Movie? borrowedMovie = movieCollection.BorrowMovie(member, movieTitleToBorrow);

        if (borrowedMovie != null)
        {
            Console.WriteLine($"Successfully borrowed '{borrowedMovie.Title}'. Enjoy watching!");
        }
        else
        {
            Console.WriteLine($"Sorry, '{movieTitleToBorrow}' is not available for borrowing.");
        }
    }

    private void ReturnMovie(Member member)
    {
        Console.WriteLine("\n<Return a Movie DVD>");
        string movieTitleToReturn = InputHelper.GetNonEmptyInput("Enter movie title to return: ");
        Movie? movieToReturn = member.FindBorrowedMovieByTitle(movieTitleToReturn);

        if (movieToReturn != null)
        {
            movieCollection.ReturnMovie(movieToReturn);
            Console.WriteLine($"Successfully returned '{movieToReturn.Title}'. Thank you!");
        }
        else
        {
            Console.WriteLine($"You have not borrowed '{movieTitleToReturn}'.");
        }
    }

    private void ListCurrentBorrowingMovies(Member currentMember)
{
    Console.WriteLine("\n<List Current Borrowing Movies>");
    var borrowingMovies = movieCollection.GetBorrowingMovies(currentMember);

    if (borrowingMovies.Count > 0)
    {
        Console.WriteLine("Movies currently being borrowed:");
        foreach (var movie in borrowingMovies)
        {
            Console.WriteLine($"Title: {movie.Title}, Genre: {movie.MovieGenre}, Classification: {movie.MovieClassification}");
        }
    }
    else
    {
        Console.WriteLine("You are currently not borrowing any movies.");
    }
}

    private void DisplayTopThreeMostBorrowedMovies()
    {
        Console.WriteLine("\n<Display Top Three Most Borrowed Movies>");
        Console.WriteLine("Top Three Most Frequently Borrowed Movies:");

        LinkedList<Movie> topBorrowedMovies = movieCollection.GetMostFrequentlyBorrowedMovies(3);

        if (topBorrowedMovies.Count > 0)
        {
            int rank = 1;
            foreach (var movie in topBorrowedMovies)
            {
                Console.WriteLine($"Rank.{rank}: Title: {movie.Title}, Genre: {movie.MovieGenre}, Classification: {movie.MovieClassification}, Times Borrowed: {movie.TimesBorrowed}");
                rank++;
            }
        }
        else
        {
            Console.WriteLine("No movies have been borrowed yet.");
        }
    }
}
