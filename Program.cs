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


                        // var db = new NorthwindConsole_32_PAKContext();
                        // var query = db.Categories.  ;
                    }
                     else if (choice == "6") 
                    {
                        // add new records to the product table


                        // var db = new NorthwindConsole_32_PAKContext();
                        // var query = db.Categories.  ;
                    }
                     else if (choice == "7") 
                    {
                        // edit specified record from the Products table

                        // var db = new NorthwindConsole_32_PAKContext();
                        // var query = db.Categories.  ;

                        
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

                        }
                        
                    }
                    else if (choice == "9") // display specific product
                    {
                        // display a specific product (all product fields should be displayed)
                        
                        // var db = new NorthwindConsole_32_PAKContext();
                        // var query = db.Categories.  ;
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
