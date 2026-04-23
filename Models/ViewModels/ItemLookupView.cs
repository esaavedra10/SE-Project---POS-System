using MongoExample.Models;

namespace MongoExample.Models.ViewModels;

public class ItemLookupView
{
    public string? SearchTerm { get; set; }
    public Products? SelectedProduct { get; set; }
}