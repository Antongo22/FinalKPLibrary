namespace FinalKPLibrary.Models;

public class DocVisibilityArea
{
    public int Id { get; set; }
    public int DocId { get; set; }
    public Doc Doc { get; set; }
    public int VisibilityAreaId { get; set; }
    public VisibilityArea VisibilityArea { get; set; }
}