namespace FinalKPLibrary.Models;

public class UserVisibilityArea
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int VisibilityAreaId { get; set; }
    public VisibilityArea VisibilityArea { get; set; }
}