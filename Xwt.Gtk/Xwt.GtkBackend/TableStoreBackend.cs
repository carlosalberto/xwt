// 
// TableStoreBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
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
using Xwt.Drawing;
using Xwt.Engine;

namespace Xwt.GtkBackend
{
	public abstract class TableStoreBackend
	{
		Gtk.TreeModel store;
		
		public Gtk.TreeModel Store {
			get {
				return store;
			}
		}

		public void Initialize (Type[] columnTypes)
		{
			Type[] types = new Type[columnTypes.Length];
			for (int n=0; n<types.Length; n++) {
				if (columnTypes [n] == typeof(Image))
					types [n] = typeof(Gdk.Pixbuf);
				else
					types [n] = columnTypes [n];
			}
			store = InitializeModel (types);
		}
		
		public abstract Gtk.TreeModel InitializeModel (Type[] columnTypes);
		
		public void Initialize (object frontend)
		{
		}

		public void SetValue (Gtk.TreeIter it, int column, object value)
		{
			if (value is string)
				store.SetValue (it, column, (string)value);
			else if (value is Image)
				store.SetValue (it, column, (Gdk.Pixbuf)WidgetRegistry.GetBackend (value));
			else
				store.SetValue (it, column, value ?? DBNull.Value);
		}

		public object GetValue (Gtk.TreeIter it, int column)
		{
			object val = store.GetValue (it, column);
			if (val is Gdk.Pixbuf)
				return WidgetRegistry.CreateFrontend<Image> (val);
			else if (val is DBNull)
				return null;
			else
				return val;
		}
	}
}

