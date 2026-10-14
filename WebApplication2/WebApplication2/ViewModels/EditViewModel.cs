using WebApplication2.ViewModels;

namespace WebApplication2.ViewModels
{
    public class EditViewModel : CreateViewModel
    {
        public new int ProductId { get; set; }
        public string? ExistingImagePath { get; set; }
    }
}
