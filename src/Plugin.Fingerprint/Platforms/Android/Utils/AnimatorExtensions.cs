using System.Threading.Tasks;
using Android.Animation;

namespace Plugin.Fingerprint.Utils
{
    public static class AnimatorExtensions
    {
        public static Task StartAsync(this Animator animator)
        {
            var listener = new TaskAnimationListener();
            animator.AddListener(listener);
            animator.Start();
            return listener.Task;
        }
    }
}