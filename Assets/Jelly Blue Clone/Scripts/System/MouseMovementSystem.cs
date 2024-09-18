using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using SnakeGame;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public interface IMouseMovementSystem : ISystem
{
    public void MouseDrag(Transform dropBubble, Vector3 offset, Vector3 extents, Transform leftBorder, Transform rightBorder);
    public void MouseClick(Transform dropBubble, Vector3 offset, Vector3 extents, Transform leftBorder, Transform rightBorder);
}

public class MouseMovementSystem : AbstractSystem, IMouseMovementSystem
{
    bool gameStarted = true;
    protected override void OnInit()
    {
        this.RegisterEvent<OnGameStartEvent>(e =>
        {
            gameStarted = true;
        });
        this.RegisterEvent<GameOverEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<OnLevelWinEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<RestartLevelEvent>(e =>
        {
            gameStarted = true;
        });
        this.RegisterEvent<PlayOnEvent>(e =>
        {
            gameStarted = true;
        });
        this.RegisterEvent<GamePauseEvent>(e =>
        {
            gameStarted = !e.gameIsPaused;
        });
        this.RegisterEvent<BoosterActivateEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<BoosterInactivateEvent>(e =>
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitForSeconds(3);
                gameStarted = true;
            });
        });
        this.RegisterEvent<BoosterPopupActivateEvent>(e =>
        {
            gameStarted = false;
        });
    }

    public void MouseDrag(Transform dropBubble, Vector3 offset, Vector3 extents, Transform leftBorder, Transform rightBorder)
    {
        if (dropBubble == null || !gameStarted) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, 0));
        mousePos.y = dropBubble.transform.position.y;
        dropBubble.position = mousePos;
        offset = dropBubble.position - Camera.main.ScreenToWorldPoint(mousePos);
        extents = dropBubble.GetComponent<BubbleController>().extents;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos) + offset;
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        Vector3 botLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);

        pos.x = Mathf.Clamp(pos.x, leftBorder.position.x + extents.x, rightBorder.position.x - extents.x);
        pos.y = Mathf.Clamp(pos.y, botLeft.y + extents.y, topRight.y - extents.y);

        dropBubble.position = pos;
    }

    public void MouseClick(Transform dropBubble, Vector3 offset, Vector3 extents, Transform leftBorder, Transform rightBorder)
    {
        if (dropBubble == null || !gameStarted) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, 0));
        mousePos.y = dropBubble.transform.position.y;
        dropBubble.position = mousePos;
        offset = dropBubble.position - Camera.main.ScreenToWorldPoint(mousePos);
        extents = dropBubble.GetComponent<BubbleController>().extents;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos) + offset;
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        Vector3 botLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);

        pos.x = Mathf.Clamp(pos.x, leftBorder.position.x + extents.x, rightBorder.position.x - extents.x);
        pos.y = Mathf.Clamp(pos.y, botLeft.y + extents.y, topRight.y - extents.y);
        //// Dừng mọi DOTween đang hoạt động trên đối tượng này trước khi khởi động một tween mới
        //dropBubble.DOKill();

        //// Thêm tween với easing để làm cho chuyển động mượt mà hơn
        //dropBubble.DOMove(pos, 0.2f).SetEase(Ease.OutSine);
        dropBubble.position = pos;
    }
}
