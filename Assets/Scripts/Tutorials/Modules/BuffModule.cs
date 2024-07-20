using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Tutorials
{
    public class BuffModule : ModuleTutorialStep
    {
        [SerializeField] private Buff _target;

        public override void OnBeginUpdate(TutorialStep step)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                var uiGameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
                var uiBuff = uiGameScreen.UIBuffs.Find(i => i.Model == _target);
                var uiDragDropBuff = uiBuff as UIDragDropBuff;
                var pointerEventData = new PointerEventData(EventSystem.current);
                var screensCanvas = step.Tutorial.Controller.GameProcessor.UIScreensCanvas;
                pointerEventData.position = screensCanvas.worldCamera.WorldToScreenPoint(focusedOnSomething.GetFocusedRect().center);

                uiDragDropBuff.OnBeginDrag(pointerEventData);
            }

            base.OnBeginUpdate(step);
        }

        public override void OnUpdate(TutorialStep step)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                var uiGameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
                
                var uiBuff = uiGameScreen.UIBuffs.Find(i => i.Model == _target);
                var uiDragDropBuff = uiBuff as UIDragDropBuff;
                var pointerEventData = new PointerEventData(EventSystem.current);
                var screensCanvas = step.Tutorial.Controller.GameProcessor.UIScreensCanvas;
                pointerEventData.position = screensCanvas.worldCamera.WorldToScreenPoint(focusedOnSomething.GetFocusedRect().center);
                uiDragDropBuff.OnDrag(pointerEventData);
            }
        }

        public override void OnEndUpdate(TutorialStep step)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                var uiGameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
                var uiBuff = uiGameScreen.UIBuffs.Find(i => i.Model == _target);
                var uiDragDropBuff = uiBuff as UIDragDropBuff;
                var pointerEventData = new PointerEventData(EventSystem.current);
                var screensCanvas = step.Tutorial.Controller.GameProcessor.UIScreensCanvas;
                pointerEventData.position = screensCanvas.worldCamera.WorldToScreenPoint(focusedOnSomething.GetFocusedRect().center);
                uiDragDropBuff.OnEndDrag(pointerEventData);
            }
        }
    }
}