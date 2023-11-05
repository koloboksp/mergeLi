
using Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BlobFace : MonoBehaviour
{
    [System.Serializable]
    private class Face
    {
        private const float BLINK_SCALE = .3f;
        
        public DefaultBallSkin.BallState state;
        public Transform eyesRoot;
        public Transform mouthRoot;

        private List<GameObject> eyes;
        private List<GameObject> mouths;

        public void Init()
        {
            eyes = new List<GameObject>();
            for (int i = 0; i < eyesRoot.childCount; i++)
                eyes.Add(eyesRoot.GetChild(i).gameObject);

            mouths = new List<GameObject>();
            for (int i = 0; i < mouthRoot.childCount; i++)
                mouths.Add(mouthRoot.GetChild(i).gameObject);
        }

        public bool Show(DefaultBallSkin.BallState state)
        {
            bool doShow = state == this.state;

            ShowItems(eyes, doShow);
            ShowItems(mouths, doShow);

            return doShow;
        }

        private void ShowItems(List<GameObject> items, bool doShow)
        {
            int ind = doShow ? Random.Range(0, items.Count) : -1;

            for (int i = 0; i < items.Count; i++)
                items[i].SetActive(i == ind);
        }

        public void TurnFace()
        {
            var scale = eyesRoot.localScale;
            scale.x *= -1f;
            eyesRoot.localScale = scale;
            mouthRoot.localScale = scale;
        }

        public void BlinkEyes(bool doOpen)
        {
            var scale = eyesRoot.localScale;
            scale.y = doOpen ? 1f : BLINK_SCALE;
            eyesRoot.localScale = scale;
        }
    }

    [SerializeField] private float flipTime = 5f;
    [SerializeField] private float blinkWait = 5f;
    [SerializeField] private float blinkStep = .1f;
    [SerializeField] private int blinkCount = 2;
    private Face lastFace;

    [SerializeField] private List<Face> faces;

    private static BlobFace s_instance;

    private void Awake()
    {
        foreach (var face in faces)
            face.Init();

        gameObject.SetActive(false);

        TurnFace();
        BlinkEyes();

        s_instance = this;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }

    public static void Show(Transform root, DefaultBallSkin.BallState state)
    {
        if (s_instance != null)
            s_instance.ShowLocal(root, state);
    }

    public static void Hide()
    {
        if (s_instance == null)
            return;

        s_instance.transform.SetParent(null);
        s_instance.gameObject.SetActive(false);
    }

    public void ShowLocal(Transform root, DefaultBallSkin.BallState state)
    {
        bool doShow = false;
        bool isSnow = false;

        foreach (var face in faces)
        {
            isSnow = face.Show(state);
            doShow |= isSnow;

            if (isSnow)
                lastFace = face;
        }

        gameObject.SetActive(doShow);

        if (!doShow)
            return;

        transform.SetParent(root);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = new Vector3(Random.Range(0, 1f) > .5f ? -1f : 1f, 1f, 1f);
    }

    private async void TurnFace()
    {
        while (true)
        {
            await Task.Delay(RandomTime(flipTime));

            if (!Application.isPlaying)
                return;

            lastFace.TurnFace();
        }
    }

    private async void BlinkEyes()
    {
        while (true)
        {
            await Task.Delay(RandomTime(blinkWait));

            if (!Application.isPlaying)
                return;

            bool isOpen = true;

            for (int i = 0; i < blinkCount * 2; i++)
            {
                isOpen = !isOpen;
                lastFace.BlinkEyes(isOpen);
                await Task.Delay((int)(1000 * blinkStep));

                if (!Application.isPlaying)
                    return;
            }
        }
    }

    private int RandomTime(float time) => (int)(1000 * Random.Range(time * .8f, time * 1.2f));
}
