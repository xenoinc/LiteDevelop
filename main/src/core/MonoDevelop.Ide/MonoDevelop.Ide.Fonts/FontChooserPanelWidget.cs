﻿//
// FontChooserPanelWidget.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Components.AtkCocoaHelper;
using Gtk;
using System.Diagnostics;
using MonoDevelop.Components;
using Xwt.Drawing;

#if MAC
//using AppKit; oe REMOVED...
//using Foundation; oe REMOVED...
#endif

namespace MonoDevelop.Ide.Fonts
{
	public partial class FontChooserPanelWidget : Gtk.Bin
	{
		Dictionary<string, string> selectedFonts = new Dictionary<string, string> ();
		Dictionary<string, string> customFonts = new Dictionary<string, string> ();


		public void SetFont (string fontName, string fontDescription)
		{
			customFonts [fontName] = fontDescription;
			IdeServices.FontService.SetFont (fontName, fontDescription);
		}


		public string GetFont (string fontName)
		{
			if (customFonts.ContainsKey (fontName))
				return customFonts [fontName];

			return IdeServices.FontService.GetUnderlyingFontName (fontName);
		}

		public void Store ()
		{
			foreach (var val in customFonts) {
				selectedFonts[val.Key] = val.Value;
			}
		}

		protected override void OnDestroyed ()
		{
			foreach (var val in selectedFonts) {
				IdeServices.FontService.SetFont (val.Key, val.Value);
			}
			base.OnDestroyed ();
		}

		public FontChooserPanelWidget ()
		{
			this.Build ();

			foreach (var desc in IdeServices.FontService.FontDescriptions) {
				selectedFonts [desc.Name] = IdeServices.FontService.GetUnderlyingFontName (desc.Name);
				var fontNameLabel = new Label (desc.DisplayName);
				fontNameLabel.Justify = Justification.Left;
				fontNameLabel.Xalign = 0;
				mainBox.PackStart (fontNameLabel, false, false, 0);
				var hBox = new HBox ();
				var setFontButton = new Button ();
				setFontButton.Label = GetDisplayDescription (IdeServices.FontService.GetFont (desc.Name));

				var descStr = GettextCatalog.GetString ("Set the font options for {0}", desc.DisplayName);
				setFontButton.Accessible.Description = descStr;
				setFontButton.Clicked += delegate {

					// var xwtParent = Xwt.Toolkit.CurrentEngine.WrapWindow (this.Toplevel);
					Xwt.Toolkit.NativeEngine.Invoke (() => {
						var selectionDialog = new Xwt.SelectFontDialog (GettextCatalog.GetString ("Select Font"));
						string fontValue = IdeServices.FontService.FilterFontName (GetFont (desc.Name));
						selectionDialog.SelectedFont = Xwt.Drawing.Font.FromName (fontValue);
						if (!selectionDialog.Run ()) {
							return;
						}
						fontValue = selectionDialog.SelectedFont.ToString ();
						if (fontValue == IdeServices.FontService.FilterFontName (IdeServices.FontService.GetFontDescriptionCodon (desc.Name).FontDescription))
							fontValue = IdeServices.FontService.GetFontDescriptionCodon (desc.Name).FontDescription;
						SetFont (desc.Name, fontValue);
						setFontButton.Label = GetDisplayDescription (selectionDialog.SelectedFont);
					});
				};
				hBox.PackStart (setFontButton, true, true, 0);

				var setDefaultFontButton = new Button ();
				setDefaultFontButton.Label = GettextCatalog.GetString ("Set To Default");
				setDefaultFontButton.Clicked += delegate {
					SetFont (desc.Name, IdeServices.FontService.GetFontDescriptionCodon (desc.Name).FontDescription);
					setFontButton.Label = GetDisplayDescription (IdeServices.FontService.GetFont (desc.Name));
				};
				hBox.PackStart (setDefaultFontButton, false, false, 0);
				mainBox.PackStart (hBox, false, false, 0);
			}
			mainBox.ShowAll ();
		}

		private string GetDisplayDescription (Font selectedFont)
		{
#if MAC
			var nsFont = selectedFont.ToNSFont ();
			return nsFont.DisplayName + " " + nsFont.PointSize;
#else
			return selectedFont.ToString ();
#endif
		}
	}
}

