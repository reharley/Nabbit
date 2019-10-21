using System;
using Nabbit.Controls;
using Nabbit.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer), new[] { typeof(CustomVisual) })]

namespace Nabbit.iOS.Renderers {
	public class CustomEntryRenderer : EntryRenderer {
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				Control.BorderStyle = UIKit.UITextBorderStyle.None;
			}
		}
	}
}