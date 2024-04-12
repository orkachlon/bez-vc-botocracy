using Core.EventSystem;
using UnityEngine;

namespace Core.Types.EventChannels {

    [CreateAssetMenu(fileName = "EmptyEventChannel", menuName = "Empty Event Channel")]
    public class SEmptyEventChannel : SEventChannel<Empty> { }

    public class Empty { }
}
