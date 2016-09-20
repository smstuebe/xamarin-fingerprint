using System.Threading.Tasks;
using Android.Animation;

namespace Plugin.Fingerprint.Utils
{
    public class TaskAnimationListener : Java.Lang.Object, Animator.IAnimatorListener
    {
        private readonly TaskCompletionSource<int> _tcs;

        public Task Task => _tcs.Task;

        public TaskAnimationListener()
        {
            _tcs = new TaskCompletionSource<int>();
        }

        public void OnAnimationCancel(Animator animation)
        {
            _tcs.TrySetCanceled();
        }

        public void OnAnimationEnd(Animator animation)
        {
            _tcs.TrySetResult(0);
        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationStart(Animator animation)
        {
        }
    }
}