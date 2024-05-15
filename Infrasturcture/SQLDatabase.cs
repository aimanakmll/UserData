using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Application;
using Domain;
using Dapper;
using System.Threading.Tasks.Sources;
using System.Threading;
using System.Linq;

namespace Infrastructure
{
    public class SQLDatabase : Database
    {
        private readonly string _connectionString;

        public SQLDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override List<T> Select<T>(string Users)
        {
            using(var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var sql = $"SELECT Id, Name, Age, School, EduLevel FROM dbo.Users";

                var allUsers = sqlConnection.Query<T>(sql).ToList();

                return (allUsers);
            }
        }

        public override void Insert<T>(T item)
        {
            using(var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                //Insert into database query
                var sql = "INSERT INTO dbo.Users (Name, Age, School, EduLevel) VALUES (@Name, @Age, @School, @EduLevel)";
             
                //Dapper Implementation
                sqlConnection.Execute(sql, item);
            }
            Console.WriteLine("User Successfully Added");
        }

        public override void Update<T>(T item)
        {
            using(var sqlConnection=new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                //Update user information query
                var sql = "UPDATE dbo.Users SET NAME = @Name, Age = @Age, School = @School, EduLevel = @EduLevel WHERE Id = @id";

                //Dapper implementation
                sqlConnection.Execute(sql, item) ;

                Console.WriteLine("User updated successfully");

            }
        }
    }
}
