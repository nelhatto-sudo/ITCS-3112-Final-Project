namespace ITCS_3112_Final_Project;

public class User
{
    public int UserId { get; }
    public string Name { get; }
    
    public List<int> Ratings { get; private set; }

    public User(int UserId, string Name)
    {
        this.UserId = UserId;
        this.Name = Name;
        this.Ratings = new List<int>();
    }
}