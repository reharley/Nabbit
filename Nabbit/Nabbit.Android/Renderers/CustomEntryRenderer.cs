﻿using System;
using Android.Content;
using Nabbit.Controls;
using Nabbit.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(CustomEntryRenderer), new[] { typeof(CustomVisual) })]

namespace Nabbit.Droid.Renderers {
	public class CustomEntryRenderer : EntryRenderer {
		public CustomEntryRenderer (Context context) : base(context) {
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				Control.SetBackground(null);
			}
		}
	}
}