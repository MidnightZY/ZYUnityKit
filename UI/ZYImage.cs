using UnityEngine;
using UnityEngine.UI;

namespace ZYUnityKit
{
  [AddComponentMenu("ZYUnityKit/UI/ZYImage")]
  public class ZYImage : Image
  {
    public void UpdateImageDirectly(Sprite sprite)
    {
      this.sprite = sprite;
    }
  }
}