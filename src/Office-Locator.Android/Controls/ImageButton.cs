using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;

[assembly: ExportRenderer (typeof (OfficeLocator.ImageButton), typeof (OfficeLocator.Droid.ImageButtonRenderer))]
namespace OfficeLocator.Droid
{
	public class ImageButtonRenderer : ButtonRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Button> e) {
			base.OnElementChanged (e);

			if (Control != null) {
				var button = (Android.Widget.Button)Control; // for example
				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "ionicons.ttf");
				button.Typeface = font;
			}
		}
	}
}

