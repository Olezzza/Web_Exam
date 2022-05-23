using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Exam
{
    public static class DatabaseWork
    {
        static MySqlConnection conn;
        static string sql = "select * from article";
        static string insertRow = "insert into article (Title, Author, Date, Rate, CountOfViews, Description)" + 
            " values (@title, @author, @date, @rate, @count, @descr)";
        static string deleteRows = "delete from article";
        static string findDuplicate = "SELECT count(*) FROM article WHERE Title = @title";
        static string findArticle = "SELECT * FROM article WHERE Title = @title";

        public static void setConnection()
        {
            Console.WriteLine("Getting Connection ...");
            conn = DBUtils.GetDBConnection();

            try {
                Console.WriteLine("Openning Connection ...");
                conn.Open();
                Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public static void readRows()
        {
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string title = Convert.ToString(reader.GetValue(0));
                        string author = Convert.ToString(reader.GetValue(1));
                        string date = Convert.ToString(reader.GetValue(2));
                        string rate = Convert.ToString(reader.GetValue(3));
                        string countOfViews = Convert.ToString(reader.GetValue(4));
                        string description = Convert.ToString(reader.GetValue(5));

                        Console.WriteLine(author + "\t" + date);
                        Console.WriteLine(title);
                        Console.WriteLine(description);
                        Console.WriteLine(rate + "\t" + countOfViews);

                        Console.WriteLine("\n\n");
                    }
                }
            }
        }

        public static void InsertArticleInDB(Article article) {
            try
            {
                MySqlCommand findDup = new MySqlCommand(findDuplicate, conn);
                findDup.Parameters.Add("@title", MySqlDbType.VarChar).Value = article.title;


                if (int.Parse(findDup.ExecuteScalar().ToString()) == 0) {
                    MySqlCommand cmd = new MySqlCommand(insertRow, conn);

                    cmd.Parameters.Add("@title", MySqlDbType.VarChar).Value = article.title;
                    cmd.Parameters.Add("@author", MySqlDbType.VarChar).Value = article.author;
                    cmd.Parameters.Add("@date", MySqlDbType.VarChar).Value = article.date;
                    cmd.Parameters.Add("@rate", MySqlDbType.VarChar).Value = article.rate;
                    cmd.Parameters.Add("@count", MySqlDbType.VarChar).Value = article.countOfViews;
                    cmd.Parameters.Add("@descr", MySqlDbType.VarChar).Value = article.description;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e);
                //Console.WriteLine(e.StackTrace);
            }
        }

        public static Article findArticleInDB(string title)
        {
            MySqlCommand cmd = new MySqlCommand(findArticle, conn);
            cmd.Parameters.Add("@title", MySqlDbType.VarChar).Value = title;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return new Article(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                            reader.GetValue(3).ToString(), reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                    }
                }

                return null;
            }
        }

        public static void deleteAllRows()
        {
            MySqlCommand cmd = new MySqlCommand(deleteRows, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
