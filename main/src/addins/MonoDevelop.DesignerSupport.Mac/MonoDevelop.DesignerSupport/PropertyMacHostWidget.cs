//
// PropertyPad.cs: The pad that holds the MD property grid. Can also 
//     hold custom grid widgets.
//
// Authors:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (C) 2006 Michael Hutchinson
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using Gtk;

namespace MonoDevelop.DesignerSupport
{
	class PropertyMacHostWidget : IPropertyGrid
	{
		public event EventHandler PropertyGridChanged;

		readonly GtkNSViewHost host;

		MacPropertyGrid view;

		public string Name { get; set; }
		public bool ShowHelp { get; set; } //not implemented

		public ShadowType ShadowType { get; set; } //not implemented
		public Widget Widget => host;

		public bool IsGridEditing => view.IsEditing;

		public bool ShowToolbar {
			get => view.ToolbarVisible;
			set => view.ToolbarVisible = value;
		}

		public bool Sensitive {
			get => view.Sensitive;
			set => view.Sensitive = value;
		}

		public object CurrentObject {
			get => view.CurrentObject;
			set {
				view.SetCurrentObject (value, new object [] { value });
			}
		}

		public PropertyMacHostWidget ()
		{
			view = new MacPropertyGrid ();
			host = new GtkNSViewHost (view);

			view.PropertyGridChanged += View_PropertyGridChanged;
		}

		void View_PropertyGridChanged (object sender, EventArgs e)
			=> PropertyGridChanged?.Invoke (this, e);

		public void SetCurrentObject (object obj, object [] propertyProviders)
			=> view.SetCurrentObject (obj, propertyProviders);

		public void BlankPad () => view.BlankPad ();
		public void Hide () => view.Hidden = true;
		public void Show () => view.Hidden = false;

		public void OnPadContentShown ()
		{
			//not implemented;
		}

		public void PopulateGrid (bool saveEditSession)
		{
			//view.SetCurrentObject (obj, propertyProviders);
		}

		public void SetToolbarProvider (object toolbarProvider)
		{
			//not implemented;
		}

		public void CommitPendingChanges ()
		{
			//not implemented;
		}

		public void Dispose ()
		{
			if (view != null) {
				view.PropertyGridChanged -= View_PropertyGridChanged;
				view.Dispose ();
				view = null;
			}
		}
	}

}
