using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Transactions;

namespace Ch08_EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("List of products that cost more than a given price");
            string input;
            decimal price;
            do
            {
                Write("Enter a product price: ");
                input = ReadLine();
            } while (!decimal.TryParse(input, out price));

            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(10)

            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {


                var db = new Northwind();
                //if you manually ope conn it will stop automatically opening and closing eepeatedly
                db.Database.Connection.Open();

                db.Database.Log = new Action<string>(message => { WriteLine(message); });
                IQueryable<Product> query = db.Products
                    .Where(product => product.UnitPrice > price)
                    .OrderByDescending(product => product.UnitPrice);
                //WriteLine(query.ToString());
                foreach (Product item in query)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} costs {item.UnitPrice:$#,##0.00}");
                }
                var newProduct = new Product
                {
                    ProductName = "Bob's Burger",
                    UnitPrice = 500M
                };
                db.Products.Add(newProduct);
                db.SaveChanges();
                foreach (Product item in query)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} costs {item.UnitPrice:$#,##0.00}");
                }

                Product updateProduct = db.Products.Find(78);
                updateProduct.UnitPrice += 20M;
                db.SaveChanges();
                foreach (Product item in query)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} costs {item.UnitPrice:$#,##0.00}");
                }

                db.Database.Connection.Close();
            }
        }
    }
}
