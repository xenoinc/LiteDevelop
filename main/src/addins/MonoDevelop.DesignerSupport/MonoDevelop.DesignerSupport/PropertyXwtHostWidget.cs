// Authors:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//	 Lytico (http://www.limada.org)
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

	class PropertyXwtHostWidget : IPropertyGrid
	{
		public event EventHandler PropertyGridChanged;

		Components.PropertyGrid.PropertyGrid view;

		public string Name { get; set; }

		public bool ShowHelp {
			get => view.ShowHelp;
			set => view.ShowHelp = value;
		}

		public ShadowType ShadowType {
			get => view.ShadowType;
			set => view.ShadowType = value;
		}

		public Widget Widget => view;

		public bool IsGridEditing => view.IsEditing;

		public bool ShowToolbar {
			get => view.ShowToolbar;
			set => view.ShowToolbar = value;
		}

		public bool Sensitive {
			get => view.Sensitive;
			set => view.Sensitive = value;
		}

		public object CurrentObject {
			get => view.CurrentObject;
			set { view.SetCurrentObject (value, new object[] {value}); }
		}

		public PropertyXwtHostWidget ()
		{
			view = new Components.PropertyGrid.PropertyGrid ();
			view.Changed += View_PropertyGridChanged;
		}

		void View_PropertyGridChanged (object sender, EventArgs e)
			=> PropertyGridChanged?.Invoke (this, e);

		public void SetCurrentObject (object obj, object[] propertyProviders)
			=> view.SetCurrentObject (obj, propertyProviders);

		public void BlankPad () => view.BlankPad ();
		public void Hide () => view.Hide ();
		public void Show () => view.Show ();

		public void OnPadContentShown () => view.OnPadContentShown ();

		public void PopulateGrid (bool saveEditSession) => view.Populate (saveEditSession);

		public void SetToolbarProvider (object toolbarProvider)
		{
			if (toolbarProvider is MonoDevelop.Components.PropertyGrid.PropertyGrid.IToolbarProvider tbP)
				view.SetToolbarProvider (tbP);
		}

		public void CommitPendingChanges () => view.CommitPendingChanges ();

		public void Dispose ()
		{
			if (view == null) return;
			view.Changed -= View_PropertyGridChanged;
			view.Dispose ();
			view = null;
		}
	}

}