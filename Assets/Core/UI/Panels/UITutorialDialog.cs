using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UITutorialDialog : MonoBehaviour
    {
        [SerializeField] private RectTransform _areaRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Text _dialogText;
        [SerializeField] private int _showingSpeed = 30;
        
        public async Task Show(string textKey)
        {
            gameObject.SetActive(true);
            await ShowEffect(textKey);
        }
        
        public void Move(Rect rect)
        {
           // _root.anchoredPosition = _areaRoot.InverseTransformPoint(new Vector2(0, rect.position.y) + new Vector2(0, 0));
        }
        
        async Task ShowEffect(string text)
        {
            int textLenght = text.Length;
            float showTime = (float)text.Length / _showingSpeed;
            float showTimer = 0;
            while (showTimer < showTime)
            {
                showTimer += Time.deltaTime;
                if (showTimer > showTime)
                    showTimer = showTime;
                
                var nTimer = showTimer / showTime;
                var charNum = (int)Mathf.Lerp(0, textLenght, nTimer);
                var visiblePart = text.Substring(0, charNum);
                var unvisible = $"<color=#00000000>{text.Substring(charNum, textLenght - charNum)}</color>";
                _dialogText.text = $"{visiblePart}{unvisible}";
                await Task.Yield();
            }
        }

       
    }
}