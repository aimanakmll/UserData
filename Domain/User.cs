namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class UserAdd : User
    {
        public string School {  get; set; }
        public string EduLevel { get; set; }
    }
}