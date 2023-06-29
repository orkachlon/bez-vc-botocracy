using Assets.Scripts.Tutorial.StoryPoints;
using StoryPoints;
using System;
using System.Threading.Tasks;

namespace Tutorial.Managers {

    public class TutorialStoryPointManager : StoryPointManager {

        private bool _isSPEnabled;
        public bool IsSPEnabled {
            get {
                return _isSPEnabled;
            }
            set {
                _currentSP.IsSPEnabled = value;
                _isSPEnabled = value;
            }
        }
        private MTutorialStoryPoint _currentSP => _currentStory as MTutorialStoryPoint;

        protected override void OnEnable() {
        }


        protected override void OnDisable() {
        }

        public async Task ShowTutorialSP() {
            await NextStoryPoint();
            _currentSP.IsSPEnabled = IsSPEnabled;
        }
    }

}