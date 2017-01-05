using System;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Plugin.Fingerprint.Abstractions;

namespace SMS.Fingerprint.Sample.Mac
{
	public partial class ViewController : NSViewController
	{
		private CancellationTokenSource _cancel;

		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		private async void OnAuthenticate(object sender, EventArgs e)
		{
			_cancel = swAutoCancel.State == NSCellStateValue.On ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
			lblStatus.StringValue = "";
			var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync("Prove you have fingers!", _cancel.Token);

			SetResult(result);
		}

		private async void OnAuthenticateLocalized(object sender, EventArgs e)
		{
			_cancel = swAutoCancel.State == NSCellStateValue.On ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
			lblStatus.StringValue = "";

			var dialogConfig = new AuthenticationRequestConfiguration("Beweise, dass du Finger hast!")
			{
				CancelTitle = "Abbrechen",
				FallbackTitle = "Anders!"
			};

			var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);
			SetResult(result);
		}

		private void SetResult(FingerprintAuthenticationResult result)
		{
			if (result.Authenticated)
			{
				var alert = new NSAlert
				{
					AlertStyle = NSAlertStyle.Informational,
					MessageText = "Success",
					InformativeText = "You have Fingers!"
				};

				alert.BeginSheet(this.View.Window);
			}
			else
			{
				lblStatus.StringValue = $"{result.Status}: {result.ErrorMessage}";
			}
		}
	}
}