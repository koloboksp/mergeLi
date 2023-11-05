using System.Threading;
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public class Starter : MonoBehaviour
	{
		public Animation Logo;
		public AnimationClip LogoClip;

		public async void Start()
		{
            Logo.Play(LogoClip.name);
			var taskLogo = AsyncExtensions.WaitForSecondsAsync(Logo.clip.length, CancellationToken.None);
			var taskInitialization = ApplicationController.StartAsync();

            await Task.WhenAll(taskLogo, taskInitialization);

            ApplicationController.LoadGameScene();
		}
    }
}