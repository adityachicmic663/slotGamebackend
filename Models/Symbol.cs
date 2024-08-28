using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class Symbol
    {
        [Key]
       public Guid symbolId {  get; set; }= Guid.NewGuid();

       public string symbolName { get; set; }

       public string imagePath {  get; set; }
     
    }
}
