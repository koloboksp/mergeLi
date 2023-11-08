
using UnityEngine;
using Core;

public class BlobFacer : MonoBehaviour
{
    [SerializeField] private DefaultBallSkin ballSkin;
    [SerializeField] private Transform bodyObject;
    
    private void Awake()
    {
        if (ballSkin == null)
            return;

        ballSkin.ChangeStateEvent += OnBallChangeState;
    }

    private void OnDestroy()
    {
        BlobFace.Hide();
        
        if (ballSkin != null)
            ballSkin.ChangeStateEvent -= OnBallChangeState;
    }

    private void OnBallChangeState(DefaultBallSkin.BallState state) => BlobFace.Show(bodyObject, state);
}
