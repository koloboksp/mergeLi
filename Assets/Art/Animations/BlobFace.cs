
using System.Collections;
using Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skins.Custom;
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
            bool doShow = (this.state & state) == state;

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

    private const float BLINK_SCALE = .3f;

    [SerializeField] private float flipTime = 5f;
    [SerializeField] private float blinkWait = 5f;
    [SerializeField] private float blinkStep = .1f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private Transform eyesRoot;

    [Space(8)]
    [SerializeField] private DefaultBallSkin.BallState changeBlobStates;
    [SerializeField] private List<Face> faces;
    
    private void Awake()
    {
        foreach (var face in faces)
            face.Init();
       // gameObject.SetActive(false);
    }
    
    private void Start()
    {
        

        TurnFaceCycle();
        BlinkEyesCycle();
    }

   // private void OnDestroy()
   // {
   //     s_instance = null;
   // }
//
   // public static void Show(Transform root, DefaultBallSkin.BallState state)
   // {
   //     if (s_instance != null)
   //         s_instance.ShowLocal(root, state);
   // }

    public void ShowLocal(DefaultBallSkin.BallState state)
    {
       //if (root == null)
       //    return;

        if ((changeBlobStates & state) != state)
            return;

       // s_root = root;

        gameObject.SetActive(true);

        foreach (var face in faces)
            face.Show(state);
    }

    //private void Update()
    //{
    //    if (s_root == null)
    //    {
    //        gameObject.SetActive(false);
    //        return;
    //    }
//
    //    transform.SetPositionAndRotation(s_root.position, s_root.rotation);
    //    transform.localScale = s_root.localScale;
    //}

    private IEnumerator TurnFaceCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomTime(flipTime));
            transform.localScale = Vector3.Scale(new Vector3(-1f, 1f, 1f), transform.localScale);
        }
    }

    private IEnumerator BlinkEyesCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomTime(blinkWait));
           
            bool isOpen = true;

            for (int i = 0; i < blinkCount * 2; i++)
            {
                isOpen = !isOpen;

                eyesRoot.localScale = new Vector3(1f, isOpen ? 1f : BLINK_SCALE, 1f);

                yield return new WaitForSeconds((int)(1000 * blinkStep));
            }
        }
    }

    private int RandomTime(float time) => (int)(1000 * Random.Range(time * .8f, time * 1.2f));
}
