using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Xml.Linq;

public interface IUISystem : ISystem
{
    public RectTransform MainCanvas { get; set; }
    public RectTransform GoldUIPos { get; set; }

    public RectTransform StartPoint { get; set; }
    public RectTransform EndPoint { get; set; }
    public void OnBubbleDone(Vector3 jelliesPos, int type);
    public Vector2 ConVertUIToWorld(RectTransform uiPoint);

    public void CameraResizer(Camera camera);
}

public class UIEventSystem : AbstractSystem, IUISystem
{
    public RectTransform MainCanvas { get; set; }

    public RectTransform GoldUIPos { get; set; }

    public RectTransform StartPoint { get; set; }
    public RectTransform EndPoint { get; set; }
    protected override void OnInit()
    {

    }

    public void OnBubbleDone(Vector3 jelliesPos, int type)
    {
        Vector3 newScreenPoint;
        // Lấy tọa độ màn hình của RectTransform
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(jelliesPos);
        // Chuyển từ Screen Space sang World Space
        RectTransformUtility.ScreenPointToWorldPointInRectangle(MainCanvas, screenPoint, Camera.main, out newScreenPoint);

        this.SendEvent(new OnJellyCollectEvent
        {
            jelliesScreenPos = newScreenPoint,
            type = type
        });
    }

    public Vector2 ConVertUIToWorld(RectTransform uiPoint)
    {
        Vector2 worldPos = RectTransformUtility.WorldToScreenPoint(Camera.main, uiPoint.position);
        // Lấy tọa độ màn hình của RectTransform
        Vector2 screenPoint = Camera.main.ScreenToWorldPoint(worldPos);
        return screenPoint;
    }

    public void CameraResizer(Camera camera)
    {
        float baseWidth = 1080f;
        float baseHeight = 1920f;

        // Calculate the base aspect ratio
        float baseAspectRatio = baseWidth / baseHeight;

        // Calculate the current aspect ratio
        float currentAspectRatio = (float)Screen.width / Screen.height;
        // Calculate the scaling factor to maintain the aspect ratio
        float scale = baseAspectRatio / currentAspectRatio;
        // Adjust the camera's orthographic size based on the scaling factor
        camera.orthographicSize = baseHeight / 200f * scale;
        // Divide by 200 because orthographicSize represents half the height in world units.
    }
}
