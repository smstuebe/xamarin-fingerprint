// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SMS.Fingerprint.Sample.Mac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnAuthenticate { get; set; }

		[Outlet]
		AppKit.NSButton btnAuthenticateLocalized { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSButton swAutoCancel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (swAutoCancel != null) {
				swAutoCancel.Dispose ();
				swAutoCancel = null;
			}

			if (btnAuthenticate != null) {
				btnAuthenticate.Dispose ();
				btnAuthenticate = null;
			}

			if (btnAuthenticateLocalized != null) {
				btnAuthenticateLocalized.Dispose ();
				btnAuthenticateLocalized = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}
		}
	}
}
