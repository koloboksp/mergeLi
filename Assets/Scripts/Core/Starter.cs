using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
	public class Starter : MonoBehaviour
	{
		public Animation Logo;
		public AnimationClip LogoClip;

		public async void Start()
		{
            Logo.Play(LogoClip.name);
			var taskLogo = AsyncExtensions.WaitForSecondsAsync(Logo.clip.length, CancellationToken.None);
			
            await Task.WhenAll(taskLogo, ApplicationController.Instance.WaitForInitializationAsync(CancellationToken.None));

            ApplicationController.LoadGameScene();
		}
    }
}