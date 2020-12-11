using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    // Categories
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("---Menu---");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("1) Display all categories and their descriptions");
                    Console.WriteLine("2) Add category");
                    Console.WriteLine("3) Display category and related products"); // only active products
                    Console.WriteLine("4) Display all categories and their related products"); // only active products
                    Console.WriteLine("5) Edit category");
                    //Products
                    Console.WriteLine("6) Add product");
                    Console.WriteLine("7) Edit specific product");
                    Console.WriteLine("8) Display all products"); // name only
                    Console.WriteLine("9) Display specific product"); // all product fields displayed
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    
                    
                    if (choice == "1") 
                    {
                        // Display all categories in the Categories table (CategoryName and Description)

                        var db = new NorthwindConsole_32_PAKContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        logger.Info("All categories displayed (category names and descriptions).");
                    }
                    else if (choice == "2") 
                    {
                        // Add new records to Categories table

                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindConsole_32_PAKContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: save category to db
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3") 
                    {
                        // Display a specific Category and its related active (not discontinued)
                        // product data (CategoryName, ProductName)

                        var db = new NorthwindConsole_32_PAKContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4") 
                    {
                        // Display all Categories and there related **active** (not discontinued)
                        // product data (CategoryName, ProductName)


                        var db = new NorthwindConsole_32_PAKContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5") 
                    { 
                        // edit specified record from the categories table
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("\nEdit Category - Feature Under Construction");
                        Console.ForegroundColor = ConsoleColor.White;
                        // var db = new NorthwindConsole_32_PAKContext();
                        // var query = db.Categories.  ;
                    }
                     else if (choice == "6") 
                    {
                        // add new records to the product table

                        var db = new NorthwindConsole_32_PAKContext();
                        
                        // enter name of Product
                        string name;
                        do {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("Enter a name for a new Product: ");
                            Console.ForegroundColor = ConsoleColor.White;
                            name = Console.ReadLine();

                            // error: blank name
                            if (name == "") {
                                Console.WriteLine("The product name cannot be blank.\n");
                            }
                        } while (name == "");

                        // enter discontinued (true/false) for Product
                        string discontinued; // convert string to boolean
                        do {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Is {name} discontinued (true/false)?");
                            Console.ForegroundColor = ConsoleColor.White;
                            discontinued = Console.ReadLine();

                            if (discontinued != "true" && discontinued != "false") {
                                Console.WriteLine("You must enter 'true' or 'false'.");
                            }
                        } while (discontinued != "true" && discontinued != "false");

                        
                        // Enter Category ID
                        var query = db.Categories.OrderBy(c => c.CategoryId);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nList of Categories: ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        int counter = 0;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
                            counter++;
                        }

                        int catID;
                        do {
                           Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Assign the Product to a Category with the Category Id (1 - {counter})");
                            Console.ForegroundColor = ConsoleColor.White;
                            string temp = Console.ReadLine();
                            catID = int.Parse(temp);

                            Console.ForegroundColor = ConsoleColor.White;
                            if (catID < 1 || catID > counter) {
                                Console.WriteLine($"Use a number between 1 and {counter}\n");
                            }
                        } while (catID < 1 || catID > counter);
                        Console.ForegroundColor = ConsoleColor.White;
                        
                        /*
                        Console.WriteLine("TESTING - Enter control -c to exit before writing new product to database.");
                        var temporary = Console.ReadLine();
                        */

                        var product = new Product { ProductName = name };
                        product.Discontinued = bool.Parse(discontinued);
                        product.CategoryId = catID;
                        
                        db.AddProduct(product);
                        logger.Info($"Product and properties added - {name}");
                    }

                    else if (choice == "7") 
                    {
                        // edit specified record from the Products table

                        // List products
                        var db = new NorthwindConsole_32_PAKContext();
                        var query = db.Products.OrderBy(p => p.ProductName); 

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Edit a product from list");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductName}");
                        } 

                        // Choose product to edit
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nType a product name to edit its information."); 
                        Console.ForegroundColor = ConsoleColor.White;
                        string productChoice = Console.ReadLine();


                        // query the product to edited
                        var query2 = db.Products.Where(p => (p.ProductName == productChoice)); // query the Product
                        
                        int counter = 0;
                        foreach (var item in query2)
                        {
                            // if counter remains zero, productChoice does not correspond to any product names in database
                            counter++;
                            
                            // used to log initial name before displaying changes
                            string initialName = item.ProductName;

                            // edit name --------------------
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Enter the new product name: ");
                            Console.ForegroundColor = ConsoleColor.White;
                            string name = Console.ReadLine();
                            //item.ProductName = name;

                            // edit category -----------------
                            var db1 = new NorthwindConsole_32_PAKContext();
                            // list categories before choosing category to change
                            var queryCategory = db1.Categories.OrderBy(c => c.CategoryId);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nList of Categories to choose from: ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            int counterCategory = 0;
                            foreach (var categoryItem in queryCategory)
                            {
                                Console.WriteLine($"{categoryItem.CategoryId} - {categoryItem.CategoryName}");
                                counterCategory++;
                            }

                            // choose category to edit
                            int catID;
                            do {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Enter a new category number: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                string tempNumber = Console.ReadLine();
                                catID = int.Parse(tempNumber);

                                if (catID < 1 || catID > counterCategory) {
                                    Console.WriteLine($"Use a number between 1 and {counterCategory}\n");
                                }
                            } while (catID < 1 || catID > counterCategory);
                            // item.CategoryId = catID;
                            
                            // change discontinued ------------------------
                            string tempDiscontinued;
                            do {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Enter whether product is discontinued (true/false): ");
                                Console.ForegroundColor = ConsoleColor.White;
                                tempDiscontinued = Console.ReadLine();
                                
                                if (tempDiscontinued != "true" && tempDiscontinued != "false") {
                                   Console.WriteLine("You must enter 'true' or 'false'."); 
                                }
                            } while (tempDiscontinued != "true" && tempDiscontinued != "false");
                            
                            // assign new values to Product
                            item.ProductName = name;
                            item.CategoryId = catID;
                            item.Discontinued = bool.Parse(tempDiscontinued);

                            var db2 = new NorthwindConsole_32_PAKContext();
                            db2.EditProduct(item);
                            
                            // track with NLog -----------------------------
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"Item properites modified for: {initialName}.");
                            logger.Info("New values");
                            logger.Info($"Product ID: {item.ProductId}");
                            logger.Info($"Product name: {item.ProductName}");
                            logger.Info($"Category ID: {item.CategoryId}");
                            logger.Info($"Discontinued: {item.Discontinued}");
                        }

                         if (counter == 0) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"'{productChoice}' is not a product in the database.");
                            logger.Info("Product edit was attempted, but input was invalid.");
                        }
                        Console.ForegroundColor = ConsoleColor.White; 
                    }
                    else if (choice == "8") 
                    {
                        // Display all records in the Products table (ProductName only) - user decides
                        // if they want to see all products, discontinued products, or active (not
                        // discontinued) products. Discontinued products should be distinguished from active products.

                        Console.WriteLine("\n1) Display all products");
                        Console.WriteLine("2) Display only discontinued products");
                        Console.WriteLine("3) Display only active products");
                        choice = Console.ReadLine();
                        
                        var db = new NorthwindConsole_32_PAKContext();

                        if (choice == "1") { // all products
                           
                            var query = db.Products.OrderBy(p => p.ProductName); 

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"All records displayed - {query.Count()} records returned");
                        }

                        else if (choice == "2") { // discontinued products
                            var query = db.Products.OrderBy(p => p.ProductName).Where(p => (p.Discontinued == true)); // *** discontinued only

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"Discontinued products displayed - {query.Count()} records returned");
                        }

                        else if (choice == "3") { // active products
                            var query = db.Products.OrderBy(p => p.ProductName).Where(p => (p.Discontinued == false)); // *** active only 
                            
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"Discontinued products displayed - {query.Count()} records returned");
                        }

                        else { // invalid choice
                            Console.WriteLine("Invalid choice. Please choose a number: 1, 2, or 3.");
                            logger.Info("Invalid choice. No products were displayed.");
                        }
                        
                    }
                    else if (choice == "9") // display specific product
                    {
                        // display a specific product (all product fields should be displayed)
                        
                        var db1 = new NorthwindConsole_32_PAKContext();
                        var query = db1.Products.OrderBy(p => p.ProductId);

                        // List Products by ID number
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nChoose a product to display its information.");
                        Console.WriteLine($"There are {query.Count()} products to choose from.\n");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        logger.Info($"Products listed by ID.");

                        // Choose product to display information
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nType a product name to display its information."); 
                        Console.ForegroundColor = ConsoleColor.White;
                        string productChoice = Console.ReadLine();

                        // List product ID, product name, and active/discontinued
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        var query2 = db1.Products.Where(p => (p.ProductName == productChoice)); // query the Product
                        
                        int counter = 0;
                        foreach (var item in query2)
                        {
                            counter++;
                            Console.WriteLine($"Product ID: {item.ProductId}");
                            Console.WriteLine($"Product name: {item.ProductName}");
                            Console.WriteLine($"Supplier ID: {item.SupplierId}");
                            Console.WriteLine($"Category ID: {item.CategoryId}");
                            Console.WriteLine($"Quantity per unit: {item.QuantityPerUnit}");
                            Console.WriteLine($"Unit price: {item.UnitPrice}");
                            Console.WriteLine($"Units in stock: {item.UnitsInStock}");
                            Console.WriteLine($"Units on order {item.UnitsOnOrder}");
                            Console.WriteLine($"Reorder level: {item.ReorderLevel}");
                            Console.WriteLine($"Discontinued: {item.Discontinued}");

                            // track with NLog
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"Item properites display for: {item.ProductName}.");
                        }

                        
                        if (counter == 0) {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"'{productChoice}' is not a product in the database.");
                        }
                        Console.ForegroundColor = ConsoleColor.White;

                        
                    }

                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}
