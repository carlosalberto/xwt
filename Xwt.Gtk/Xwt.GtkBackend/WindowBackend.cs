// 
// WindowBackend.cs
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
using Xwt.Backends;

namespace Xwt.GtkBackend
{
	public class WindowBackend: WindowFrameBackend, IWindowBackend
	{
		Gtk.Alignment alignment;
		Gtk.MenuBar mainMenu;
		Gtk.VBox mainBox;
		
		public WindowBackend ()
		{
			Window = new Gtk.Window ("");
			mainBox = new Gtk.VBox ();
			Window.Add (mainBox);
			mainBox.Show ();
			alignment = new Gtk.Alignment (0, 0, 1, 1);
			mainBox.PackStart (alignment, true, true, 0);
			alignment.Show ();
		}
		
		public Gtk.VBox MainBox {
			get { return mainBox; }
		}
		
		public void SetChild (IWidgetBackend child)
		{
			var w = (IGtkWidgetBackend) child;
			alignment.Child = w.Widget;
		}
		
		public void SetMainMenu (IMenuBackend menu)
		{
			if (mainMenu != null)
				mainBox.Remove (mainMenu);
			
			if (menu != null) {
				MenuBackend m = (MenuBackend) menu;
				mainMenu = m.MenuBar;
				mainBox.PackStart (mainMenu, false, false, 0);
				((Gtk.Box.BoxChild)mainBox[mainMenu]).Position = 0;
			} else
				mainMenu = null;
		}

		#region IWindowBackend implementation
		
		
		public void UpdateLayout ()
		{
			Window win = (Window) Frontend;
			alignment.LeftPadding = (uint) win.Padding.Left;
			alignment.RightPadding = (uint) win.Padding.Right;
			alignment.TopPadding = (uint) win.Padding.Top;
			alignment.BottomPadding = (uint) win.Padding.Bottom;
		}
		
		#endregion
	}
}

