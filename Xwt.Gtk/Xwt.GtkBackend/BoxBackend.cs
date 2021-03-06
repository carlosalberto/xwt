// 
// BoxBackend.cs
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
using System.Linq;
using Xwt.Backends;
using Xwt;
using System.Collections.Generic;

namespace Xwt.GtkBackend
{
	class BoxBackend: WidgetBackend, IBoxBackend
	{
		public BoxBackend ()
		{
			Widget = new CustomContainer ();
			Widget.Show ();
		}
		
		new CustomContainer Widget {
			get { return (CustomContainer)base.Widget; }
			set { base.Widget = value; }
		}
		
		public override void Initialize ()
		{
			Widget.Frontend = Frontend;
		}

		public void Add (IWidgetBackend widget)
		{
			Widget.Add (GetWidget (widget));
		}

		public void Remove (IWidgetBackend widget)
		{
			Widget.Remove (GetWidget (widget));
		}
		
		public void SetAllocation (IWidgetBackend widget, Rectangle rect)
		{
			var w = GetWidget (widget);
			Widget.SetAllocation (w, rect);
		}
	}
	
	class CustomContainer: Gtk.Container
	{
		public Widget Frontend;
		Dictionary<Gtk.Widget, Rectangle> children = new Dictionary<Gtk.Widget, Rectangle> ();
		
		public CustomContainer (IntPtr p): base (p)
		{
		}
		
		public CustomContainer ()
		{
			WidgetFlags |= Gtk.WidgetFlags.NoWindow;
		}
		
		
		public void SetAllocation (Gtk.Widget w, Rectangle rect)
		{
			children [w] = rect;
			QueueResize ();
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			children.Add (widget, new Rectangle (0,0,0,0));
			widget.Parent = this;
			QueueResize ();
		}
		
		protected override void OnRemoved (Gtk.Widget widget)
		{
			children.Remove (widget);
			widget.Unparent ();
		}
		
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			IWidgetSurface ws = Frontend;
			int w = (int)ws.GetPreferredWidth ().MinSize;
			int h = (int)ws.GetPreferredHeight ().MinSize;
			if (requisition.Width < w)
				requisition.Width = w;
			if (requisition.Height < h)
				requisition.Height = h;
		}
		
		protected override void OnUnrealized ()
		{
			base.OnUnrealized ();
			lastAllocation = new Gdk.Rectangle ();
		}
		
		Gdk.Rectangle lastAllocation;
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			if (!lastAllocation.Equals (allocation))
				((IWidgetSurface)Frontend).Reallocate ();
			lastAllocation = allocation;
			foreach (var cr in children) {
				var r = cr.Value;
				cr.Key.SizeAllocate (new Gdk.Rectangle (allocation.X + (int)r.X, allocation.Y + (int)r.Y, (int)r.Width, (int)r.Height));
			}
		}
		
		protected override void ForAll (bool includeInternals, Gtk.Callback callback)
		{
			base.ForAll (includeInternals, callback);
			foreach (var c in children.Keys.ToArray ())
				callback (c);
		}
	}
}

