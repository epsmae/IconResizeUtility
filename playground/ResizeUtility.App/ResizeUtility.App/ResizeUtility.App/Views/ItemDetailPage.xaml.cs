using ResizeUtility.App.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ResizeUtility.App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}