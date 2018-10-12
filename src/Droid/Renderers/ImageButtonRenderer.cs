using System;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(OfficeLocator.Controls.ImageButton), typeof(OfficeLocator.Droid.Renderers.ImageButtonRenderer))]
namespace OfficeLocator.Droid.Renderers
{
    public class ImageButtonRenderer : ButtonRenderer
    {
        Context _context;

        public ImageButtonRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var button = (Android.Widget.Button)Control; // for example
                Typeface font = Typeface.CreateFromAsset(_context.Assets, "ionicons.ttf");
                button.Typeface = font;
            }
        }
    }
}