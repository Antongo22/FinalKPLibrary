namespace FinalKPLibrary.Models;

public class Doc
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Topic { get; set; }
    public DateTime UploadDate { get; set; }
    public ICollection<DocVisibilityArea> DocVisibilityAreas { get; set; }
}
