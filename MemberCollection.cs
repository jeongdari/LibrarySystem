using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MemberCollection
{
    private List<Member> members;

    public MemberCollection()
    {
        members = new List<Member>();
    }

    public void AddMember(string firstName, string lastName, string contactNumber, string password)
    {
        // Create and add new member
        Member newMember = new Member(firstName, lastName, contactNumber, password);
        members.Add(newMember);

        Console.WriteLine($"Member {firstName} {lastName} added successfully.");
    }

    public Member? FindMember(string firstName, string lastName)
    {
        return members.FirstOrDefault(m => m.FirstName == firstName && m.LastName == lastName);
    }

    public void RemoveMember(string firstName, string lastName)
    {
        Member? memberToRemove = FindMember(firstName, lastName);

        if (memberToRemove == null)
        {
            Console.WriteLine($"Member {firstName} {lastName} not found.");
            return;
        }

        // Check if member has borrowed movies
        if (memberToRemove.BorrowedMovies.Count > 0)
        {
            Console.WriteLine($"Member {firstName} {lastName} cannot be removed. Please return all borrowed movies first.");
            return;
        }

        // Remove the member
        members.Remove(memberToRemove);
        Console.WriteLine($"Member {firstName} {lastName} removed successfully.");
    }

    public string? FindMemberContactNumber(string firstName, string lastName)
    {
        Member? member = FindMember(firstName, lastName);
        return member?.ContactNumber;
    }

    public bool DisplayMembersRentingMovie(string movieTitle)
{
    bool movieExists = false;
    bool anyRenters = false;

    // Check if the movie exists in the library
    foreach (var member in members)
    {
        if (member.HasBorrowedMovieByTitle(movieTitle))
        {
            // Movie exists if at least one member has borrowed it
            movieExists = true;
            Console.WriteLine($"Members renting '{movieTitle}': {member.FirstName} {member.LastName}");
            anyRenters = true;
        }
    }

    // If the movie doesn't exist, output a message
    if (!movieExists)
    {
        Console.WriteLine($"The movie '{movieTitle}' is not found in the library.");
    }
    else if (!anyRenters)
    {
        // If movie exists but no one has borrowed it, output a message
        Console.WriteLine($"No one has borrowed the movie '{movieTitle}'.");
    }

    return anyRenters; // Return whether there are any renters for the movie
}


}
