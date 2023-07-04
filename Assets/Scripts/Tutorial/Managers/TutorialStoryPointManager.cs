using StoryPoints;
using System;
using System.Threading.Tasks;
using Tutorial.StoryPoints;

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
        private MTutorialStoryPoint _currentSP => CurrentStory as MTutorialStoryPoint;

        protected override void OnEnable() {
        }


        protected override void OnDisable() {
        }

        public void InitTutorialSP() {
            var sp = SPProvider.Next();
            if (!sp.HasValue) {
                DispatchNoMoreSPs();
                return;
            }
            InitNewSP(sp.Value);
            HideTutorialSP();
        }

        public void HideTutorialSP() {
            _currentSP.AwaitHideAnimation();
        }
        
        public async Task ShowTutorialSP() {
            await _currentSP.AwaitInitAnimation();
            _currentSP.IsSPEnabled = IsSPEnabled;
        }
    }

}