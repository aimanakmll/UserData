using System.Security.Cryptography;

namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class UserAdd : User
    {
        public int Age { get; set; }
        public string School { get; set; } = "";
        public string EduLevel { get; set; } = "";
    }
    public class UserEncrypt 
    {
        public string username { get; set; }
        public string Password { get; set; } //use aes or rsa
    }
}
