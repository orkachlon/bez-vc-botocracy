using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using DG.Tweening;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;

namespace Neurons.Rewarder {
    public class MRewardEffect : MonoBehaviour {
        [Header("Animation"), SerializeField] private SpriteRenderer rewardSpritePrefab;
        [SerializeField] private ParticleSystem idleParticles;
        [SerializeField] private ParticleSystem rewardParticles;
        [SerializeField] private float idleAnimationDuration;
        [SerializeField] private float rewardAnimationHeight;
        [SerializeField] private float rewardAnimationDuration;
        // [SerializeField] private TileBase rewardTile;

        [Header("Sound"), SerializeField] private AudioClip rewardSound;
        [SerializeField, Range(0, 1)] private float rewardVolume;

        private ParticleSystem RewardParticles => rewardParticles;
        private ParticleSystem IdleParticles => idleParticles;

        private Tween _currentTween;
        private readonly ConcurrentDictionary<Hex, SpriteRenderer> _activeSprites = new();

        private void OnDestroy() {
            Kill();
            _currentTween = null;
        }

        public async void PlayIdle(INeuronBoardController boardController, Hex hex) {
            // start particles
            IdleParticles.Play();
            
            // enable tile
            // boardController.SetTile(hex, rewardTile, BoardConstants.RewardTilemapLayer);
            // boardController.SetColor(hex, new Color(1, 1, 1, 0f), BoardConstants.RewardTilemapLayer);

            await Task.Delay((int) (1000 * Random.value));
            // tween tile and neuron sprite
            var effectSprite = Instantiate(rewardSpritePrefab, boardController.HexToWorldPos(hex), Quaternion.identity,
                transform);
            _activeSprites[hex] = effectSprite;
            _currentTween = DOTween.Sequence()
                // sprite animation up-down with fade
                // .Append(effectSprite.transform.DOMoveY(effectSprite.transform.position.y + rewardAnimationHeight, rewardAnimationDuration))
                .Append(effectSprite.DOFade(0.3f, idleAnimationDuration))
                // tile animation fade color in-out
                // .Join(DOVirtual.Color(new Color(1, 1, 1, 0f), new Color(1, 1, 1, 0.7f), rewardAnimationDuration * 2,
                //     c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer)))
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);
        }
        
        public async Task PlayReward(INeuronBoardController boardController, Hex hex, int amount) {
            // kill any previous animation
            Kill();
            
            // play particles
            RewardParticles.Play();
            
            // play sound
            var s = AudioSpawner.GetAudioSource();
            s.Source.volume = rewardVolume;
            s.Source.PlayOneShot(rewardSound);
            AudioSpawner.ReleaseWhenDone(s);
            
            // fade tile to full white
            // var startColor = boardController.GetColor(hex, BoardConstants.RewardTilemapLayer);
            // DOVirtual.Color(startColor, Color.white, rewardAnimationDuration * 0.3f,
            //     c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer));
            
            // animate neuron sprites fading upwards
            var effectsTasks = new List<Task>();
            for (var i = 0; i < amount; i++) {
                var effectSprite = Instantiate(rewardSpritePrefab, boardController.HexToWorldPos(hex), Quaternion.identity,
                    transform);
                _activeSprites[hex] = effectSprite;
                var effectTask = DOTween.Sequence()
                    .AppendInterval(i * rewardAnimationDuration * 0.5f)
                    .Append(effectSprite.transform.DOMoveY(effectSprite.transform.position.y + rewardAnimationHeight, rewardAnimationDuration))
                    .Join(effectSprite.DOFade(0, rewardAnimationDuration))
                    .OnComplete(() => { 
                        Destroy(effectSprite.gameObject);
                        _activeSprites.TryRemove(hex, out _);
                    })
                    .AsyncWaitForCompletion();
                effectsTasks.Add(effectTask);
            }

            await Task.WhenAll(effectsTasks);

            // fade tile to clear
            // DOTween.Sequence()
            //     .Append(DOVirtual.Color(Color.white, new Color(1, 1, 1, 0), rewardAnimationDuration * 0.5f,
            //         c => boardController.SetColor(hex, c, BoardConstants.RewardTilemapLayer)))
            //     .OnComplete(() => boardController.SetTile(hex, null, BoardConstants.RewardTilemapLayer));
        }

        public void Complete() {
            _currentTween?.Complete();
            RewardParticles.Stop();
            IdleParticles.Stop();
        }

        public void Kill() {
            _currentTween?.Kill();
            _currentTween = null;
            RewardParticles.Stop();
            IdleParticles.Stop();
            foreach (var hex in _activeSprites.Keys) {
                Destroy(_activeSprites[hex].gameObject);
            }
            _activeSprites.Clear();
        }
    }
}