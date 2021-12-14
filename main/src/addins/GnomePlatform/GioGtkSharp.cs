// Gio.cs
//
// Authors:
// Mike Kestner  <mkestner@novell.com>
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
//
// Refactoring to GtkSharp:
// Lytico  (http://www.limada.org)
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

using System.Collections.Generic;
using System.Linq;
using GLib;
using MonoDevelop.Ide.Desktop;

namespace MonoDevelop.Platform
{

	internal static class Gio
	{

		static GnomeDesktopApplication AppFromAppInfoPtr (IAppInfo handle, DesktopApplication defaultApp)
		{
			string id = handle.Id;
			string name = handle.Name;
			string executable = handle.Executable;

			if (!string.IsNullOrEmpty (name) && !string.IsNullOrEmpty (executable) &&
			    !executable.Contains ("monodevelop "))
				return new GnomeDesktopApplication (executable, name, defaultApp != null && defaultApp.Id == id);
			return null;
		}

		static string ContentTypeFromMimeType (string mime_type)
		{
			var content_type = ContentType.FromMimeType (mime_type);
			return content_type;
		}

		public static DesktopApplication GetDefaultForType (string mime_type)
		{
			var content_type = ContentTypeFromMimeType (mime_type);
			var ret = AppInfoAdapter.GetDefaultForType (content_type, false);
			return ret == default ? null : AppFromAppInfoPtr (ret, null);
		}

		public static IList<DesktopApplication> GetAllForType (string mime_type)
		{
			var def = GetDefaultForType (mime_type);

			var content_type = ContentTypeFromMimeType (mime_type);
			var ret = AppInfoAdapter.GetAllForType (content_type);
			if (ret == null || ret.Length == 0)
				return new DesktopApplication[0];
			return ret.Select (i => AppFromAppInfoPtr (i, def)).ToArray ();
		}

		public static string GetGSettingsString (string schema, string key)
		{
			using var gsettings = new Settings (schema);
			var ret = gsettings.GetString (key);
			return ret;
		}

		public static string GetIconIdForFile (string filename)
		{
			if (string.IsNullOrEmpty (filename))
				return null;

			var gfile = FileFactory.NewForPath (filename);
			var native_attrs = "standard::icon";
			using var info = gfile.QueryInfo (native_attrs, FileQueryInfoFlags.None, null);

			var iconnative = info?.Icon?.ToString ();
			if (iconnative == null)
				return default;

			var iconid = iconnative.Split (' ');
			// g_icon_to_string should give us 4 fields, 2nd is GThemedIcon
			// if this isn't the case, we're into crazyland, and fall back
			if (iconid.Length == 4 && iconid[1].Trim () == "GThemedIcon")
				return iconid[2].Trim ();
			return null;
		}

		public static string GetMimeTypeDescription (string mime_type)
		{
			var content_type = ContentTypeFromMimeType (mime_type);
			var desc = ContentType.GetDescription (content_type);

			return desc;
		}

		public static string GetMimeTypeForUri (string uri)
		{
			if (string.IsNullOrEmpty (uri))
				return null;
			var gfile = FileFactory.NewForPath (uri);
			var native_attrs = "standard::content-type";

			using var info = gfile.QueryInfo (native_attrs, FileQueryInfoFlags.None, null);

			var content_type = info?.ContentType;
			if (content_type == default)
				return default;

			string mime_type = ContentType.GetMimeType (content_type);

			return mime_type;
		}
	}

}