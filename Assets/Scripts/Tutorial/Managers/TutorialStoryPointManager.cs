using StoryPoints;
using System;
using System.Threading.Tasks;

namespace Tutorial.Managers {

    public class TutorialStoryPointManager : StoryPointManager {

        protected override void OnEnable() {
        }


        protected override void OnDisable() {
        }

        public async Task ShowTutorialSP() {
            await NextStoryPoint();
        }
    }

}