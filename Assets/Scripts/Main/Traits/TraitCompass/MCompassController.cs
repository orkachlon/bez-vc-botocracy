using UnityEngine;

namespace Main.Traits.TraitCompass {
    public class MCompassController : MonoBehaviour {

        [SerializeField] private MTraitCompass traitCompass;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                traitCompass.gameObject.SetActive(!traitCompass.gameObject.activeInHierarchy);
                traitCompass.RectTransform.position = Input.mousePosition;
            }
        }
    }
}