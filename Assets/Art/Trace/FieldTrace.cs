using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldTrace : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image image;
    [SerializeField] private Vector2Int texSize;
    [SerializeField] private int dampSpeed = 8;

    private Transform target;

    private bool move;

    private byte[] map;
    private Texture2D tex;
    public Vector2 pos;
    private int posx, posy;
    private int texSquare;
    private Sprite sprite;

    private void Awake()
    {
        Core.Ball.OnMovingStateChangedGlobal += Ball_OnMovingStateChangedGlobal;

        texSquare = texSize.x * texSize.y;
        map = new byte[texSquare];
        for (int i = 0; i < texSquare; i++)
            map[i] = 0;

        tex = new Texture2D(texSize.x, texSize.y, TextureFormat.Alpha8, false, true);

        sprite = Sprite.Create(tex, new Rect(0, 0, texSize.x, texSize.y), Vector2.one / 2f);
        image.sprite = sprite;
        image.color = Color.white;
    }

    private void OnDestroy()
    {
        Core.Ball.OnMovingStateChangedGlobal -= Ball_OnMovingStateChangedGlobal;
    }

    private void Ball_OnMovingStateChangedGlobal(Core.Ball ball, bool move)
    {
        this.move = move;

        if (move)
        {
            target = ball.transform;
            image.color = ball.View.MainColor;
        }
    }

    private void Update()
    {
        if (move && target != null)
        {
            pos = rect.InverseTransformPoint(target.position);
            posx = (int)(Mathf.Clamp01(pos.x / rect.sizeDelta.x) * texSize.x);
            posy = (int)(Mathf.Clamp01(pos.y / rect.sizeDelta.y) * texSize.y);
            map[posy * texSize.x + posx] = 255;
        }

        for (int i = 0; i < texSquare; i++)
            if (map[i] > 0)
                map[i] = (byte)Mathf.Max(0, (map[i] - dampSpeed));


        tex.LoadRawTextureData(map);
        tex.Apply();
    }
}
