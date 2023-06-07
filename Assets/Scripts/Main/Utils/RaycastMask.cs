using UnityEngine;
using UnityEngine.UI;

namespace Main.Utils {
    
    // NOT IN USE
    
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
    {
        private Sprite _sprite;
 
        void Start ()
        {
            _sprite = GetComponent<Image>().sprite;
        }
   
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var rectTransform = (RectTransform) transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out var local);
        
            // normalize local coordinates
            var transformRect = rectTransform.rect;
            var transformPivot = rectTransform.pivot;
            var normalized = new Vector2(
                (local.x + transformPivot.x * transformRect.width) / transformRect.width,
                (local.y + transformPivot.y * transformRect.height) / transformRect.width);
            // convert to texture space
            var spriteRect = _sprite.textureRect;
            var x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * normalized.x);
            var y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * normalized.y);
        
            // destroy component if texture import settings are wrong
            try
            {
                return _sprite.texture.GetPixel(x,y).a > 0;
            }
            catch (UnityException e)
            {
                Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
                Destroy(this);
                return false;
            }
        }
    }
}