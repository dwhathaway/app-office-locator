using Android.Content;
using Android.Graphics;

using Plugin.CurrentActivity;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OfficeLocator.ImageButton), typeof(OfficeLocator.Droid.ImageButtonRenderer))]
namespace OfficeLocator.Droid
{
    public class ImageButtonRenderer : ButtonRenderer
    {
        public ImageButtonRenderer(Context context) : base(context)
        {

        }

        Context CurrentContext
        {
            get { return CrossCurrentActivity.Current.Activity; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Typeface font = Typeface.CreateFromAsset(CurrentContext.Assets, "ionicons.ttf");
                Control.Typeface = font;
            }
        }
    }
}

