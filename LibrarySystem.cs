using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LibrarySystem
{
    private MovieCollection movieCollection;
    private MemberCollection memberCollection;

    public LibrarySystem()
    {
        movieCollection = new MovieCollection();
        memberCollection = new MemberCollection();
    }

    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu");
            Console.WriteLine("1. Login as Staff");
            Console.WriteLine("2. Login as Member");
            Console.WriteLine("3. Exit");

            Console.Write("Enter option: ");
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

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
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void StaffLogin()
    {
        Console.Write("Enter staff username: ");
        string? username = Console.ReadLine();

        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return; // or handle the error appropriately
        }

        Console.Write("Enter staff password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return; // or handle the error appropriately
        }

        // Proceed with staff authentication using username and password
        if (username == "staff" && password == "today123")
        {
            Console.WriteLine("Staff login successful.");
            StaffMenu();
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }


    private void StaffMenu()
    {
        while (true)
        {
            Console.WriteLine("\nStaff Menu");
            Console.WriteLine("1. Add Movie DVDs");
            Console.WriteLine("2. Remove Movie DVDs");
            Console.WriteLine("3. Register New Member");
            Console.WriteLine("4. Remove Registered Member");
            Console.WriteLine("5. Find Member's Contact Number");
            Console.WriteLine("6. Display Members Renting a Movie");
            Console.WriteLine("7. Return to Main Menu");

            Console.Write("Enter option: ");
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (option)
            {
                case 1:
                    // Implement Add Movie DVDs functionality
                    Console.Write("Enter movie title: ");
                    string? title = Console.ReadLine();

                    if (string.IsNullOrEmpty(title)) // Check if title is null or empty
                    {
                        Console.WriteLine("Invalid title. Please enter a valid movie title.");
                        continue;
                    }

                    // Validate genre input
                    Genre genre;
                    Console.WriteLine("Enter genre (drama, adventure, family, action, sci-fi, comedy, animated, thriller, other): ");
                    if (!Enum.TryParse(Console.ReadLine(), true, out genre) || !Enum.IsDefined(typeof(Genre), genre))
                    {
                        Console.WriteLine("Invalid genre. Please enter a valid genre from the provided list.");
                        continue;
                    }

                    // Validate classification input
                    Classification classification;
                    Console.WriteLine("Enter classification (G, PG, M15Plus, MA15Plus): ");
                    if (!Enum.TryParse(Console.ReadLine(), true, out classification) || !Enum.IsDefined(typeof(Classification), classification))
                    {
                        Console.WriteLine("Invalid classification. Please enter a valid classification from the provided list.");
                        continue;
                    }

                    Console.Write("Enter duration (in minutes): ");
                    if (!int.TryParse(Console.ReadLine(), out int durationMinutes))
                    {
                        Console.WriteLine("Invalid duration. Please enter a valid number.");
                        continue;
                    }

                    Console.Write("Enter number of copies: ");
                    if (!int.TryParse(Console.ReadLine(), out int numCopies))
                    {
                        Console.WriteLine("Invalid number of copies. Please enter a valid number.");
                        continue;
                    }

                    // Call AddMovie method from MovieCollection
                    movieCollection.AddMovie(title, genre, classification, durationMinutes, numCopies);
                    break;
                case 2:
                    // Implement Remove Movie DVDs functionality
                    Console.Write("Enter movie title: ");
                    string? removeTitle = Console.ReadLine();

                    if (string.IsNullOrEmpty(removeTitle))
                    {
                        Console.WriteLine("Invalid movie title. Please enter a valid title.");
                        continue;
                    }

                    Console.Write("Enter number of copies to remove: ");
                    if (!int.TryParse(Console.ReadLine(), out int removeNumCopies))
                    {
                        Console.WriteLine("Invalid number of copies. Please enter a valid number.");
                        continue;
                    }

                    // Call RemoveMovie method from MovieCollection if removeTitle is not null
                    if (removeTitle != null)
                    {
                        movieCollection.RemoveMovie(removeTitle, removeNumCopies);
                    }
                    break;
                case 3:
                    // Implement Register New Member functionality
                    Console.Write("Enter first name: ");
                    string? firstName = Console.ReadLine();
                    Console.Write("Enter last name: ");
                    string? lastName = Console.ReadLine();
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    {
                        Console.WriteLine("Invalid input. Please enter valid values for first name and last name.");
                        break;
                    }
                    // Check if member already exists
                    if (memberCollection.FindMember(firstName, lastName) != null)
                    {
                        Console.WriteLine("Member already exists.");
                        break;
                    }
                    Console.Write("Enter contact phone number: ");
                    string? contactPhoneNumber = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string? password = Console.ReadLine();

                    // Check if any of the required inputs are null or empty
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                        string.IsNullOrEmpty(contactPhoneNumber) || string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("Invalid input. Please enter valid values for all fields.");
                        break;
                    }

                    // Call AddMember method from MemberCollection with non-null arguments
                    memberCollection.AddMember(firstName, lastName, contactPhoneNumber, password);
                    break;
                case 4:
                    // Implement Remove Registered Member functionality
                    Console.Write("Enter member's first name: ");
                    string? removeFirstName = Console.ReadLine();

                    Console.Write("Enter member's last name: ");
                    string? removeLastName = Console.ReadLine();

                    // Check if any of the inputs are null or empty
                    if (string.IsNullOrEmpty(removeFirstName) || string.IsNullOrEmpty(removeLastName))
                    {
                        Console.WriteLine("Invalid input. Please enter valid first name and last name.");
                        break;
                    }

                    // Call RemoveMember method from MemberCollection
                    memberCollection.RemoveMember(removeFirstName, removeLastName);
                    break;
                case 5:
                    // Implement Find Member's Contact Number functionality
                    Console.Write("Enter member's first name: ");
                    string? findFirstName = Console.ReadLine();

                    Console.Write("Enter member's last name: ");
                    string? findLastName = Console.ReadLine();

                    // Check if any of the inputs are null or empty
                    if (string.IsNullOrEmpty(findFirstName) || string.IsNullOrEmpty(findLastName))
                    {
                        Console.WriteLine("Invalid input. Please enter valid first name and last name.");
                        break;
                    }

                    // Call FindMemberContactNumber method from MemberCollection
                    string? contactNumber = memberCollection.FindMemberContactNumber(findFirstName, findLastName);

                    if (contactNumber != null)
                    {
                        Console.WriteLine($"Contact number for {findFirstName} {findLastName}: {contactNumber}");
                    }
                    else
                    {
                        Console.WriteLine($"Member {findFirstName} {findLastName} not found.");
                    }
                    break;
                case 6:
                    // Implement Display Members Renting a Movie functionality
                    Console.Write("Enter movie title to display renting members: ");
                    string? movieTitle = Console.ReadLine();

                    // Check if movie title is null or empty
                    if (string.IsNullOrEmpty(movieTitle))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid movie title.");
                        break;
                    }

                    memberCollection.DisplayMembersRentingMovie(movieTitle);
                    break;
                case 7:
                    return; // Return to main menu
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void MemberLogin()
    {
        Console.Write("Enter first name: ");
        string? firstName = Console.ReadLine();

        if (string.IsNullOrEmpty(firstName))
        {
            Console.WriteLine("First name cannot be empty.");
            return;
        }

        Console.Write("Enter last name: ");
        string? lastName = Console.ReadLine();

        if (string.IsNullOrEmpty(lastName))
        {
            Console.WriteLine("Last name cannot be empty.");
            return;
        }

        Console.Write("Enter password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        Member? member = memberCollection.FindMember(firstName, lastName);

        if (member != null && member.Password == password)
        {
            Console.WriteLine($"{firstName} {lastName} login successful.");
            MemberMenu(member); // Pass the found member to the MemberMenu method
        }
        else
        {
            Console.WriteLine("Invalid member credentials.");
        }
    }


    private void MemberMenu(Member member)
    {
        while (true)
        {
            Console.WriteLine("\nMember Menu");
            Console.WriteLine("1. Browse All Movies");
            Console.WriteLine("2. Display Movie Information");
            Console.WriteLine("3. Borrow a Movie DVD");
            Console.WriteLine("4. Return a Movie DVD");
            Console.WriteLine("5. List Current Borrowing Movies");
            Console.WriteLine("6. Display Top Three Most Borrowed Movies");
            Console.WriteLine("7. Return to Main Menu");

            Console.Write("Enter option: ");
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (option)
            {
                case 1:
                    // Implement Browse All Movies functionality
                    movieCollection.DisplayAllMovies();
                    break;
                case 2:
                    // Implement Display Movie Information functionality
                    Console.Write("Enter movie title: ");
                    string? movieTitle = Console.ReadLine();
                    if (string.IsNullOrEmpty(movieTitle))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid movie title.");
                        break;
                    }
                    movieCollection.DisplayMovieInfo(movieTitle);
                    break;
                case 3:
                    // Implement Borrow a Movie DVD functionality
                    Console.Write("Enter movie title to borrow: ");
                    string? movieTitleToBorrow = Console.ReadLine();

                    if (!string.IsNullOrEmpty(movieTitleToBorrow))
                    {
                        // Call BorrowMovie method from MovieCollection, passing the current member and movie title
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
                    else
                    {
                        Console.WriteLine("Invalid movie title. Please enter a valid movie title.");
                    }
                    break;
                case 4:
                    // Implement Return a Movie DVD functionality
                    Console.Write("Enter movie title to return: ");
                    string? movieTitleToReturn = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(movieTitleToReturn))
                    {
                        Console.WriteLine("Invalid movie title. Please enter a valid title.");
                        break;
                    }

                    // Retrieve the movie object based on the title
                    Movie? movieToReturn = member.FindBorrowedMovieByTitle(movieTitleToReturn);

                    if (movieToReturn != null)
                    {
                        // Call ReturnMovie method from MovieCollection
                        movieCollection.ReturnMovie(movieToReturn);
                        Console.WriteLine($"Successfully returned '{movieToReturn.Title}'. Thank you!");
                    }
                    else
                    {
                        Console.WriteLine($"You have not borrowed '{movieTitleToReturn}'.");
                    }
                    break;
                case 5:
                    // Implement List Current Borrowing Movies functionality
                    LinkedList<Movie> borrowingMovies = movieCollection.GetBorrowingMovies();

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
                    break;
                case 6:
                    // Implement Display Top Three Most Borrowed Movies functionality
                    Console.WriteLine("Top Three Most Frequently Borrowed Movies:");

                    LinkedList<Movie> topBorrowedMovies = movieCollection.GetMostFrequentlyBorrowedMovies(3); // Adjust the number of movies to display here

                    if (topBorrowedMovies.Count > 0)
                    {
                        int rank = 1;
                        foreach (var movie in topBorrowedMovies)
                        {
                            Console.WriteLine($"{rank}. Title: {movie.Title}, Genre: {movie.MovieGenre}, Classification: {movie.MovieClassification}, Times Borrowed: {movie.TimesBorrowed}");
                            rank++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No movies have been borrowed yet.");
                    }
                    break;
                case 7:
                    return; // Return to main menu
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
