using Core.EventSystem;
using Types.StoryPoint;
using UnityEngine;

namespace Core.Types.EventChannels {
    [CreateAssetMenu(fileName = "SPEventChannel", menuName = "SP Event Channel")]
    public class SSPEventChannel : SEventChannel<IStoryPoint> {
    }
}
