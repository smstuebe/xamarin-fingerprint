using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace Plugin.Fingerprint.Test.UI
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
                .Android
                .ApkFile(@"..\..\..\Plugin.Fingerprint.Test.UI.Android\bin\Debug\Plugin.Fingerprint.Test.UI.Android-Signed.apk")
                .StartApp();
        }

        [Test]
        public void ShouldNotCrash()
        {
            app.WaitForElement(q => q.Id("txtSuccess"));
            app.Screenshot("Success");
        }
    }
}

