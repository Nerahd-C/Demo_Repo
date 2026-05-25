using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Dynamic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ConsoleApp26
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] StudentsList = File.ReadAllLines("StudentList.txt");
            string[] LibrariansList = File.ReadAllLines("LibrarianList.txt");
            string[] BooksList = File.ReadAllLines("Books.txt");

            List<string[]> Students = new List<string[]>();
            List<string[]> Librarians = new List<string[]>();
            List<string[]> Books = new List<string[]>();


            foreach (var line in StudentsList)
            {
                Students.Add(line.Split(','));
            }


            foreach (var line in LibrariansList)
            {
                Librarians.Add(line.Split(','));
            }


            foreach (var line in BooksList)
            {
                var parts = line.Split(',');

                if (parts.Length < 2) continue;

                string title = parts[0];
                string author = parts[1];

                string borrowed = "";
                if (parts.Length > 2)
                {
                    borrowed = parts[2];
                }

                string queue = "";
                if (parts.Length > 3)
                {
                    queue = parts[3];
                }

                Books.Add(new string[] { title, author, borrowed, queue });
            }


            List<string[]> Data = new List<string[]>();
            Data.AddRange(Students);
            Data.AddRange(Librarians);

            string[] names = new string[Data.Count];
            string[] passwords = new string[Data.Count];
            string[] roles = new string[Data.Count];

            for (int i = 0; i < Data.Count; i++)
            {
                names[i] = Data[i][0];
                passwords[i] = Data[i][1];
                roles[i] = Data[i][2];
            }



            while (true)
            {
                Console.Clear();

                string choice = Front();

                if (choice == "2")
                {
                    SignIn(Students);
                }



                else if (choice == "1")
                {
                    string role;
                    string current;
                    role = FindUser(Students, Librarians, out current);

                    if (role == "Student")
                    {
                        StudentMenu(Books, current);
                    }

                    else if (role == "Librarian")
                    {
                        LibrarianControl(Books);
                    }

                    else
                    {
                        Console.WriteLine("Invalid Input!");
                        Console.ReadKey();
                    }
                    
                }

                else
                {
                    Console.WriteLine("Invalid Input!");
                    Console.ReadKey();
                }
            }
        }



        static string Front()
        {

            Console.WriteLine("=========================================");
            Console.WriteLine("         GARDEN OF WORDS LIBRARY         ");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("\t [1] Log In");
            Console.WriteLine("\t [2] Sign Up");
            Console.WriteLine("=========================================");
            Console.Write("Select an option: ");
            return Console.ReadLine();
        }


        static bool LibrarianControl(List<string[]> Books)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("                 DASHBOARD                ");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("[1] View All Books");
                Console.WriteLine("[2] Add Book");
                Console.WriteLine("[3] Review Pending");
                Console.WriteLine("[4] Log Out");
                Console.WriteLine("=========================================");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewBooks(Books, "Librarian");
                        Pause();
                        break;

                    case "2":
                        AddBooks(Books);
                        break;

                    case "3":
                        BorrowApproval(Books);
                        break;

                    case "4":
                        return false;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }


            }
        }


        static bool StudentMenu(List<string[]> Books, string currentuser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("             STUDENT DASHBOARD              ");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("[1] View Books");
                Console.WriteLine("[2] Borrow Book");
                Console.WriteLine("[3] Return Book");
                Console.WriteLine("[4] Log Out");
                Console.WriteLine("=========================================");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewBooks(Books, "Student");
                        Pause();
                        break;

                    case "2":
                        BorrowBook(Books, currentuser);
                        break;

                    case "3":
                        ReturnBookStudent(Books, currentuser);
                        break;

                    case "4":
                        return false;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }


            }
        }


        static void AddBooks(List<string[]> Books)
        {
            Console.Clear();
            Console.Write("Enter Title: ");
            string titleInput = Console.ReadLine();
            Console.Write("Enter Author: ");
            string authorInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(titleInput) || string.IsNullOrWhiteSpace(authorInput))
            {
                Console.WriteLine();
                Console.WriteLine("No information entered. Returning to menu...");
                Console.ReadKey();
                return;

            }

            foreach (var book in Books)
            {
                if (book[0].Equals(titleInput, StringComparison.OrdinalIgnoreCase) &&
                    book[1].Equals(authorInput, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"\n\"{titleInput}\" by {authorInput} already exists.");
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine();
            Console.Write($"Are you sure you want to add \"{titleInput}\" by {authorInput}? (yes/no): ");
            string confirm = Console.ReadLine();

            if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {

                Books.Add(new string[] { titleInput, authorInput });
                Console.WriteLine($"\n\"{titleInput}\" added to collection!");
                File.AppendAllText("Books.txt", $"{titleInput},{authorInput}\n");
            }
            else
            {
                Console.WriteLine("\nReturning to menu...");
            }
        }


        static void ViewBooks(List<string[]> Books, string role)
        {

            Console.Clear();

            Console.WriteLine("===========================================================================");
            Console.WriteLine("                      BOOK COLLECTION                         ");
            Console.WriteLine("===========================================================================");

            if (role == "Librarian")
            {
                Console.WriteLine("{0,-5} {1,-25} {2,-20} {3,-20}", "#", "Title", "Author", "Details");
            }
            else if (role == "Student")
            {
                Console.WriteLine("{0,-5} {1,-25} {2,-20} {3,-20}", "#", "Title", "Author", "Status");
            }

            Console.WriteLine(new string('-', 75));

            for (int i = 0; i < Books.Count; i++)
            {
                string borrowed = (Books[i].Length > 2) ? Books[i][2] : "";

                string queue = (Books[i].Length > 3) ? Books[i][3] : "";

                string display;

                if (role == "Librarian")
                {
                    string queueDisplay = string.IsNullOrEmpty(queue) ? "None" : queue;
                    display = $"Borrowed: {borrowed} | Queue: {queueDisplay}";
                }
                else
                {
                    int queueCount = string.IsNullOrEmpty(queue) ? 0 : queue.Split('|').Length;

                    if (string.IsNullOrEmpty(borrowed))
                        display = "Available";
                    else
                        display = $"Unavailable | Queue: {queueCount}";
                }

                Console.WriteLine("{0,-5} {1,-25} {2,-20} {3,-20}", i + 1, Books[i][0], Books[i][1], display);

            }

        }


        static void SignIn(List<string[]> Students)
        {
            Console.Clear();
            Console.WriteLine("----- SIGN IN -----");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();


            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine();
                Console.WriteLine("No information entered. Returning to menu...");
                Console.ReadKey();
                return;

            }

            else
            {
                foreach (var user in Students)
                {
                    if (user[0] == username)
                    {
                        Console.WriteLine("Username already exists!");
                        Console.ReadKey();
                        return;
                    }
                }

                string[] newUser = { username, password, "Student" };
                Students.Add(newUser);
                File.AppendAllText("StudentList.txt", $"{username},{password},Student\n");


                Console.WriteLine("Account created successfully!");
                Console.ReadKey();
            }
           
        }


        static string FindUser(List<string[]> Students, List<string[]> Librarians, out string current)
        {
            Console.Write("Username: ");
            current = Console.ReadLine();

            Console.Write("Password: ");
            string pass = Console.ReadLine();



            foreach (var s in Students)
                if (s[0] == current && s[1] == pass)
                    return s[2];

            foreach (var l in Librarians)
                if (l[0] == current && l[1] == pass)
                    return l[2];

            return null;
        }


        static void BorrowBook(List<string[]> Books, string currentUser)
        {
            List<string> borrowedThisSession = new List<string>();

            while (true)
            {
                Console.Clear();
                ViewBooks(Books, "Student");

                Console.Write("\nBorrow a book? (yes/no): ");
                string choice = Console.ReadLine().ToLower();

                if (choice == "no") break;
                if (choice != "yes") continue;

                Console.Write("Enter book title: ");
                string input = Console.ReadLine();

                int index = FindBookIndex(Books, input);

                if (index == -1)
                {
                    Console.WriteLine("\nBook not found!");
                    Pause();
                    continue;
                }

                string[] book = Books[index];

                string borrowedBy = GetField(book, 2);
                string queue = GetField(book, 3);


                if (borrowedBy == currentUser)
                {
                    Console.WriteLine("\nYou already borrowed this book.");
                    Pause();
                    continue;
                }


                if (string.IsNullOrEmpty(borrowedBy))
                {
                    Books[index] = new string[] { book[0], book[1], currentUser, "" };
                    borrowedThisSession.Add(book[0]);

                    Console.WriteLine($"\nYou borrowed \"{book[0]}\".");
                }
                else
                {
                    Queuing(Books, index, queue, currentUser);
                }

                SaveBooks(Books);
                Pause();
            }

            DisplaySummary(borrowedThisSession);
        }


        static void Pause()
        {
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }


        static int FindBookIndex(List<string[]> Books, string title)
        {
            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i][0].Equals(title, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }


        static string GetField(string[] arr, int index)
        {
            if (arr.Length > index)
                return arr[index];

            return "";
        }


        static void Queuing(List<string[]> Books, int index, string queue, string user)
        {
            List<string> queueList;

            if (string.IsNullOrEmpty(queue))
                queueList = new List<string>();
            else
                queueList = queue.Split('|').ToList();

            if (queueList.Contains(user))
            {
                Console.WriteLine("\nYou are already in the queue.");
                return;
            }

            queueList.Add(user);

            string[] book = Books[index];
            Books[index] = new string[]
            {
                 book[0],
                 book[1],
                GetField(book, 2),
                string.Join("|", queueList)};


            Console.WriteLine("\nAdded to queue.");
        }


        static void SaveBooks(List<string[]> Books)
        {
            File.WriteAllLines("Books.txt",
                Books.Select(b => string.Join(",", b)));
        }


        static void DisplaySummary(List<string> list)
        {
            Console.Clear();
            Console.WriteLine("----- Borrow Summary -----");

            if (list.Count == 0)
            {
                Console.WriteLine("No books borrowed.");
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {list[i]}");
                }
            }

            Console.ReadKey();
        }


        static void BorrowApproval(List<string[]> Books)
        {
            Console.Clear();


            ViewBooks(Books, "Librarian");
            Console.WriteLine("\n ----- Pending approval requests -----\n");

            Console.Write("Enter a number for approval: ");
            Console.WriteLine();

            if (!int.TryParse(Console.ReadLine(), out int id))
                return;

            id -= 1;
            if (id < 0 || id >= Books.Count)
                return;

            string[] book = Books[id];
            string borrowedby = GetField(book, 2);
            string queue = GetField(book, 3);

            if (!string.IsNullOrEmpty(borrowedby))
            {
                Console.WriteLine("\nBook has not been returned yet.");
                Pause();
                return;
            }
            if (string.IsNullOrEmpty(queue))
            {
                Console.WriteLine("\nThere are no pending requests");
                Pause();
                return;
            }

            List<string> queueList = queue.Split('|').ToList();
            string next = queueList[0];

            Console.WriteLine($"Pending user: {next}");
            Console.Write("[A] Approve | [D] Decline : ");
            string choice = Console.ReadLine().ToUpper();

            if (choice == "A")
            {
                queueList.RemoveAt(0);

                Books[id] = new string[]
                {
                    book[0],
                    book[1],
                    next,
                    string.Join("|", queueList)
                };



                Console.WriteLine($"\nApproved. Book assigned to {next}.");

            }

            else if (choice == "D")
            {
                queueList.RemoveAt(0);

                Books[id] = new string[]
                {
                    book[0],
                    book[1],
                    "",
                    string.Join("|", queueList)
                };

                Console.WriteLine($"\nRequest user {next} has been removed from queue...");
            }

            else
            {
                Console.WriteLine("\nInvalid choice.");
                Pause();
                return;
            }

            SaveBooks(Books);
            Pause();

        }


        static void ReturnBookStudent(List<string[]> Books, string currentUser)
        {
            Console.Clear();
            Console.WriteLine("----- Your Borrowed Books -----");


            List<int> userBooks = new List<int>();

            for (int i = 0; i < Books.Count; i++)
            {
                string borrowedBy = GetField(Books[i], 2);

                if (borrowedBy == currentUser)
                {
                    userBooks.Add(i);
                    Console.WriteLine($"{userBooks.Count}. {Books[i][0]} by {Books[i][1]}");
                }
            }

            if (userBooks.Count == 0)
            {
                Console.WriteLine("\nYou have no borrowed books.");
                Pause();
                return;
            }

            Console.Write("\nEnter number to return: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
                return;

            choice -= 1;

            if (choice < 0 || choice >= userBooks.Count)
                return;

            int index = userBooks[choice];
            string[] book = Books[index];

            string queue = GetField(book, 3);


            if (string.IsNullOrWhiteSpace(queue))
            {
                Books[index] = new string[]
                {
                    book[0],
                    book[1],
                    "",
                    ""
                };

                Console.WriteLine("\nBook returned. It is now available.");
            }
            else
            {
                Books[index] = new string[]
                {
                    book[0],
                    book[1],
                     "",
                    queue
                };

                Console.WriteLine("\nBook returned.");
                Console.WriteLine("Waiting for librarian approval.");
            }

            SaveBooks(Books);
            Pause();
        }
    }
}




