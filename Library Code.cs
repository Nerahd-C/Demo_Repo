using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;
using System.Globalization;

namespace Grocycle
{
    internal class Program
    {
        static string CurrentUser;
        static string UserFolder;
        static Queue<string> ExpiringItems =
            new Queue<string>();
        static Stack<string> WasteHistory =
            new Stack<string>();

        static void Main(string[] args)
        {
            Directory.CreateDirectory("Users");

            StartMenu();
        }

        static void StartMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("=======================================================================================================================");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"

                              ██████╗ ██████╗  ██████╗  ██████╗██╗   ██╗ ██████╗██╗     ███████╗
                             ██╔════╝ ██╔══██╗██╔═══██╗██╔════╝╚██╗ ██╔╝██╔════╝██║     ██╔════╝
                             ██║  ███╗██████╔╝██║   ██║██║      ╚████╔╝ ██║     ██║     █████╗
                             ██║   ██║██╔══██╗██║   ██║██║       ╚██╔╝  ██║     ██║     ██╔══╝
                             ╚██████╔╝██║  ██║╚██████╔╝╚██████╗   ██║   ╚██████╗███████╗███████╗
                              ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  ╚═════╝   ╚═╝    ╚═════╝╚══════╝╚══════╝

                                                 SMART GROCERY PLANNER 
                                      SDG 12: Responsible Consumption & Production

                                    ");
                Console.ResetColor();
                Console.WriteLine("=======================================================================================================================\n");

                Console.WriteLine("[1] Login");
                Console.WriteLine("[2] Sign Up");
                Console.WriteLine("[3] Exit");

                Console.Write("\nEnter Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login();
                        break;

                    case "2":
                        SignUp();
                        break;

                    case "3":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\nPlease input a valid option.");
                        Pause();
                        break;
                }
            }
        }

        static void SignUp()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("                SIGN UP");
                Console.WriteLine("=========================================\n");

                Console.Write("Create Username: ");
                string username = Console.ReadLine().Trim().ToLower();

                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a username.\n");
                    Pause();
                    Console.Clear();
                    continue;
                }

                string userFolder = Path.Combine("Users", username);

                if (Directory.Exists(userFolder))
                {
                    Console.WriteLine();
                    Console.WriteLine("Username already exists!\n");
                    Pause();
                    Console.Clear();
                    continue;
                }

                Console.Write("Password: ");
                string password = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a password.\n" +
                        "");
                    Pause();
                    Console.Clear();
                    continue;
                }

                Console.Write("Confirm Password: ");
                string repassword = Console.ReadLine();

                if (password != repassword)
                {
                    
                    Console.WriteLine("\nPasswords do not match.\n");
                    Pause();
                    Console.Clear();
                    continue;

                }

                

                Console.Write("\nMonthly Grocery Budget ($): ");
                string budget = Console.ReadLine();

                Console.Write("Supermarket Visits Per Month: ");
                string visits = Console.ReadLine();

                Console.Write("Number of Family Members: ");
                string members = Console.ReadLine();

                Directory.CreateDirectory(userFolder);

                File.WriteAllLines(
                    Path.Combine(userFolder, "Profile.txt"),
                    new string[]
                    {
                    $"Username|{username}",
                    $"Password|{password}",
                    $"Budget|{budget}",
                    $"Visits|{visits}",
                    $"Members|{members}"
                    });

                File.WriteAllLines(
                    Path.Combine(userFolder, "Points.txt"),
                    new string[]
                    {
                    "Points|0",
                    "Rank|Eco Beginner"
                    });

                File.Create(
                    Path.Combine(userFolder, "Inventory.txt"))
                    .Close();

                File.Create(
                    Path.Combine(userFolder, "Purchases.txt"))
                    .Close();

                File.Create(
                    Path.Combine(userFolder, "Waste.txt"))
                    .Close();

                File.Create(
                    Path.Combine(userFolder, "Reports.txt"))
                    .Close();

                Console.WriteLine("\nAccount Created Successfully!");
                Pause();
                break;
            }
        }

        static void Login()
        {

            Console.Clear();
            while (true)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("                 LOGIN");
                Console.WriteLine("=========================================\n");

                Console.Write("Username: ");
                string username = Console.ReadLine().Trim();

                string userFolder = Path.Combine("Users", username);

                if (!Directory.Exists(userFolder))
                {
                    Console.WriteLine("\nAccount not found!");
                    Pause();
                    Console.Clear();
                    continue;
                }

                Console.Write("Password: ");
                string password = Console.ReadLine().Trim();

                string profilePath =
                    Path.Combine(userFolder, "Profile.txt");

                string[] profile = File.ReadAllLines(profilePath);

                string savedPassword = "";

                foreach (string pass in profile)
                {
                    string[] data = pass.Split('|');

                    if (data[0] == "Password")
                    {
                        savedPassword = data[1];
                    }
                }

                if (password != savedPassword)
                {
                    Console.WriteLine("\nIncorrect Password!");
                    Pause();
                    Console.Clear();
                    continue;
                }

                CurrentUser = username;
                UserFolder = userFolder;

                Console.WriteLine("\nLogin Successful!");
                Pause();

                Dashboard();
                break;
            }
        }

        static void Dashboard()
        {
            while (true)
            {
                Console.Clear();

                string profilePath =
                    Path.Combine(UserFolder, "Profile.txt");

                string[] profile =
                    File.ReadAllLines(profilePath);

                string budget = "";
                string visits = "";
                string members = "";

                foreach (string line in profile)
                {
                    string[] data = line.Split('|');

                    switch (data[0])
                    {
                        case "Budget":
                            budget = data[1];
                            break;

                        case "Visits":
                            visits = data[1];
                            break;

                        case "Members":
                            members = data[1];
                            break;
                    }
                }

                Console.WriteLine("=========================================");
                Console.WriteLine("               GROCYCLE");
                Console.WriteLine("=========================================\n");

                Console.WriteLine($"Welcome, {CurrentUser}");
                Console.WriteLine();

                Console.WriteLine($"Monthly Budget : ${budget}");
                Console.WriteLine($"Store Visits   : {visits}");
                Console.WriteLine($"Family Members : {members}");

                Console.WriteLine("\n=========================================\n");

                Console.WriteLine("[1] Account Information");
                Console.WriteLine("[2] Inventory Management");
                Console.WriteLine("[3] Grocery Planner");
                Console.WriteLine("[4] Expiry Checker");
                Console.WriteLine("[5] Log out");

                Console.Write("\nEnter Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {

                    case "1":

                        AccountInformation();
                        break;

                    case "2":
                        InventoryMenu();
                        break;

                    case "3":
                        GroceryPlanner();
                        break;

                    case "4":
                        ExpiryMenu();
                        break;

                    case "5":
                        CurrentUser = "";
                        UserFolder = "";
                        return;

                    default:
                        Console.WriteLine("\nInvalid Choice!");
                        Pause();
                        break;
                }
            }
        }

        static void AccountInformation()
        {
            Console.Clear();

            string profilePath =
            Path.Combine(UserFolder, "Profile.txt");

            string[] profile =
                File.ReadAllLines(profilePath);

            Console.WriteLine("=================================");
            Console.WriteLine("      ACCOUNT INFORMATION");
            Console.WriteLine("=================================\n");

            foreach (string line in profile)
            {
                Console.WriteLine(
                    line.Replace("|", ": "));
            }

            Console.WriteLine();
            Console.Write("Update household information? (Y/N): ");

            string choice =
                Console.ReadLine().ToUpper();

            if (choice != "Y")
                return;

            


            Console.WriteLine("\n\n=================================");
            Console.WriteLine(" UPDATE HOUSEHOLD INFORMATION");
            Console.WriteLine("=================================\n");

            Console.Write("Monthly Grocery Budget: $");
            string budget =
                Console.ReadLine();

            Console.Write("Supermarket Visits Per Month: ");
            string visits =
                Console.ReadLine();

            Console.Write("Number of Family Members: ");
            string members =
                Console.ReadLine();

            string[] newProfile =
            {
                $"Username|{CurrentUser}",
                $"Budget|{budget}",
                $"Visits|{visits}",
                $"Members|{members}"
    };

            File.WriteAllLines(
                profilePath,
                newProfile);

            Console.WriteLine();
            Console.WriteLine(
                "Household information updated successfully!");

            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void InventoryMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("=========================================");
                Console.WriteLine("         INVENTORY MANAGEMENT");
                Console.WriteLine("=========================================");

                ViewInventory();


                Console.WriteLine("\n[1] Add Item");
                Console.WriteLine("[2] Update Quantity");
                Console.WriteLine("[3] Remove Item");
                Console.WriteLine("[4] Back");

                Console.Write("\nChoice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddInventory();
                        break;

                    case "2":
                        UpdateInventory();
                        break;

                    case "3":
                        RemoveInventory();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Please input a valid option...");
                        break;

                }
            }
        }

        static void AddInventory()
        {
            Console.Clear();

            Console.Write("Item Name: ");
            string name = Console.ReadLine();

            Console.Write("Quantity: ");
            string quantity = Console.ReadLine();

            Console.Write("Expiry Date (MM/DD/YYYY): ");
            string expiry = Console.ReadLine();

            string inventoryPath =
                Path.Combine(UserFolder, "Inventory.txt");

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(expiry))
            {
                Console.WriteLine();
                Console.WriteLine("No information entered. Returning to menu...");
                Console.ReadKey();
                return;
            }



                Console.WriteLine("\nItem Added!");

                File.AppendAllText(
                inventoryPath,
                $"{name}|{quantity}|{expiry}\n");
            
            

            Pause();
        }

        static void ViewInventory()
        {
            Console.Clear();

            string inventoryPath =
                Path.Combine(UserFolder, "Inventory.txt");

            string[] items =
                File.ReadAllLines(inventoryPath);

            Console.WriteLine("=========================================");
            Console.WriteLine("               INVENTORY");
            Console.WriteLine("=========================================\n");

            if (items.Length == 0)
            {
                Console.WriteLine("Inventory Empty.");
            }
            else
            {
                Console.WriteLine(
                    "Item\t\tQty\tExpiry");

                Console.WriteLine(
                    "-----------------------------------------");

                foreach (string item in items)
                {
                    string[] data = item.Split('|');

                    Console.WriteLine($"{data[0]}\t\t{data[1]}\t{data[2]}");
                }
            }

        }

        static void UpdateInventory()
        {
            Console.Clear();

            string inventoryPath =
                Path.Combine(UserFolder, "Inventory.txt");

            List<string> items =
                File.ReadAllLines(inventoryPath).ToList();

            Console.Write("Item Name: ");
            string itemName = Console.ReadLine();

            bool found = false;

            for (int i = 0; i < items.Count; i++)
            {
                string[] data = items[i].Split('|');

                if (data[0].ToLower() ==
                    itemName.ToLower())
                {
                    Console.Write("New Quantity: ");

                    string qty =
                        Console.ReadLine();

                    data[1] = qty;

                    items[i] =
                        string.Join("|", data);

                    found = true;
                    break;
                }
            }

            if (found)
            {
                File.WriteAllLines(
                    inventoryPath,
                    items);

                Console.WriteLine(
                    "\nQuantity Updated!");
            }
            else
            {
                Console.WriteLine(
                    "\nItem Not Found.");
            }

            Pause();
        }

        static void RemoveInventory()
        {
            Console.Clear();

            string inventoryPath =
                Path.Combine(UserFolder, "Inventory.txt");

            List<string> items =
                File.ReadAllLines(inventoryPath).ToList();

            Console.Write("Item Name: ");

            string name =
                Console.ReadLine();

            bool removed = false;

            for (int i = 0; i < items.Count; i++)
            {
                string[] data =
                    items[i].Split('|');

                if (data[0].ToLower() ==
                    name.ToLower())
                {
                    items.RemoveAt(i);

                    removed = true;
                    break;
                }
            }

            if (removed)
            {
                File.WriteAllLines(
                    inventoryPath,
                    items);

                Console.WriteLine(
                    "\nItem Removed!");
            }
            else
            {
                Console.WriteLine(
                    "\nItem Not Found.");
            }

            Pause();
        }


        static void GroceryPlanner()
        {
            Console.Clear();

            string inventoryPath =
                Path.Combine(UserFolder, "Inventory.txt");

            string[] items =
                File.ReadAllLines(inventoryPath);

            Console.WriteLine("=========================================");
            Console.WriteLine("            GROCERY PLANNER");
            Console.WriteLine("=========================================\n");

            bool foundLowStock = false;

            foreach (string item in items)
            {
                string[] data = item.Split('|');

                int quantity =
                    Convert.ToInt32(data[1]);

                if (quantity <= 2)
                {
                    foundLowStock = true;

                    Console.WriteLine(
                        $"Buy More: {data[0]}");
                }
            }

            if (!foundLowStock)
            {
                Console.WriteLine(
                    "No low-stock items detected.");
            }

            Console.WriteLine();
            Console.WriteLine(
                "Suggested purchases are based on low inventory.");

            Pause();
        }

        static void RecordPurchase()
        {
            Console.Clear();

            Console.Write("Item Name: ");
            string item = Console.ReadLine();

            Console.Write("Cost: ");
            string cost = Console.ReadLine();

            string purchasePath =
                Path.Combine(UserFolder,
                "Purchases.txt");

            File.AppendAllText(
                purchasePath,
                $"{item}|{cost}\n");

            Console.WriteLine(
                "\nPurchase Recorded!");

            Pause();
        }

        static void GenerateShoppingList()
        {
        }

        static void AnalyzeConsumption()
        {
        }

        static void ExpiryMenu()
        {
        }

        static void CheckExpiringItems()
        {
        }

        static void SuggestItemUsage()
        {
        }



        static void WasteMenu()
        {
        }

        static void RecordWaste()
        {
        }

        static void SuggestDisposal()
        {
        }


        static void AddPoints(int points)
        {
        }

        static void DeductPoints(int points)
        {
        }

        static void ViewPoints()
        {
        }



        static void BudgetAnalysis()
        {
        }

        static void CheckOverspending()
        {
        }

        static void DetectOverconsumption()
        {
        }



        static void GenerateMonthlyReport()
        {
        }

        static void ComparePreviousMonth()
        {
        }


        static void SaveUserData()
        {
        }

        static void LoadUserData()
        {
        }
    }
}
