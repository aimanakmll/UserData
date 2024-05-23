using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Application;
using Domain;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Reflection;

namespace Infrastructure
{
    public class MYSQLDatabase : Database
    {
        private readonly string _connectionString;

        public MYSQLDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
         
        public override List<T> Select<T>(string User)
        {
            using(var mySqlConnection = new MySqlConnection(_connectionString))
            {
                mySqlConnection.Open();

                var mysql = "SELECT Id, Name, Age, School, EduLevel FROM Users";

                var allUser = mySqlConnection.Query<T>(mysql).ToList();

                return allUser;
            }
        }

        //uses reflection to map the properties of 'item'
        public override void Insert<T>(T item)
        {
            using(var mySqlConnection = new MySqlConnection(_connectionString))
            {
                mySqlConnection.Open();

                var mysql = "INSERT INTO Users (Name, Age, School, EduLevel) VALUES (@Name, @Age, @School, @EduLevel)";

                mySqlConnection.Execute(mysql, item);
            }
            Console.WriteLine("User Added Successfully into MYSQL.");
        }

        public override void Update<T>(T item)
        {
            using(var mySqlConnection = new MySqlConnection(_connectionString))
            {
                mySqlConnection.Open();

                var mysql = "UPDATE Users SET Name=@Name, Age=@Age, School=@School, EduLevel=@EduLevel WHERE Id=@id";

                mySqlConnection.Execute(mysql, item);

                Console.WriteLine("User Updated via MYSQL Successfully.");
            }
        }
    }
}
