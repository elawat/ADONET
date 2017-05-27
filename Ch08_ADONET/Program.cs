using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data.SqlClient;
using System.Configuration;

namespace Ch08_ADONET
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Data Source=(localdb)\mssqllocaldb; Initial Catalog=Northwind;Integrated Security=true;");
            connection.Open();
            WriteLine($"The connection is {connection.State}");
            WriteLine("Orginal list of categories:");
            ListCategories(connection);

            Write("Enter a new category name: ");
            string name = ReadLine();
            if (name.Length > 15) name = name.Substring(0, 15);
            var insertCategory = new SqlCommand($"Insert into Categories(CategoryName) values (@NewCategoryName)", connection);
            insertCategory.Parameters.AddWithValue("@NewCategoryName", name);
            int rowsAffected = insertCategory.ExecuteNonQuery();
            WriteLine($"{rowsAffected} rows(s) were inserted.");

            WriteLine("List of categories after inserting:");
            ListCategories(connection);

            var deleteCategory = new SqlCommand($"Delete Categories where CategoryName = @DeleteCategoryName", connection);
            deleteCategory.Parameters.AddWithValue("@DeleteCategoryName", name);
            rowsAffected = deleteCategory.ExecuteNonQuery();
            WriteLine($"{rowsAffected} rows(s) were deleted.");

            WriteLine("List of categories after deleting:");
            ListCategories(connection);




            connection.Close();
            WriteLine($"The connection is {connection.State}");
        }

        private static void ListCategories(SqlConnection connection)
        {
            var getCategories = new SqlCommand("Select CategoryID, CategoryName From Categories", connection);
            SqlDataReader reader = getCategories.ExecuteReader();

            //find out the index positions of the column that you want to read
            int indexofID = reader.GetOrdinal("CategoryID");
            int indexofName = reader.GetOrdinal("CategoryName");

            while (reader.Read())
            {
                //use the typed getxxxx methods to efficiently read the column values
                WriteLine($"{reader.GetInt32(indexofID)}:{reader.GetString(indexofName)}");
            }
            reader.Close();
        }
    }
}
