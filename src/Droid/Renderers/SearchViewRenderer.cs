using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using OfficeLocator.Droid.Renderers;

[assembly: ExportRenderer(typeof(SearchBar), typeof(SearchViewRenderer))]
namespace OfficeLocator.Droid.Renderers
{
    public class SearchViewRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                SearchBar element = (SearchBar)this.Element;
                global::Android.Widget.SearchView native = (global::Android.Widget.SearchView)Control;

                // do whatever you want to the controls here!
                //--------------------------------------------
                // element.BackgroundColor = Color.Transparent;
                // native.SetBackgroundColor(element.BackgroundColor.ToAndroid());
                // native.SetBackgroundColor(Color.White.ToAndroid());

                //The text color of the SearchBar / SearchView 
                //AutoCompleteTextView textField = (AutoCompleteTextView)
                //    (((Control.GetChildAt(0) as ViewGroup)
                //        .GetChildAt(2) as ViewGroup)
                //        .GetChildAt(1) as ViewGroup)
                //        .GetChildAt(0);

                //if (textField != null)
                    //textField.SetTextColor(Color.White.ToAndroid());
            }
        }
    }
}