namespace FinalKPLibrary.Models;

public class VisibilityArea
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserVisibilityArea> UserVisibilityAreas { get; set; }
    public ICollection<DocVisibilityArea> DocVisibilityAreas { get; set; }
}
