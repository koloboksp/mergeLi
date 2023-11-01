
using Core;
using System.Collections.Generic;
using UnityEngine;

public class BlobFace : MonoBehaviour
{
    [System.Serializable]
    private class Face
    {
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
    }

    [SerializeField] private List<Face> faces;

    private static BlobFace s_instance;

    private void Awake()
    {
        foreach (var face in faces)
            face.Init();

        gameObject.SetActive(false);

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
        s_instance.transform.SetParent(null);
        s_instance.gameObject.SetActive(false);
    }

    public void ShowLocal(Transform root, DefaultBallSkin.BallState state)
    {
        bool doShow = false;

        foreach (var face in faces)
            doShow |= face.Show(state);

        gameObject.SetActive(doShow);

        if (!doShow)
            return;

        transform.SetParent(root);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = new Vector3(Random.Range(0, 1f) > .5f ? -1f : 1f, 1f, 1f);
    }
}
