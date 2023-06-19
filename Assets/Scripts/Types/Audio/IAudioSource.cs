﻿using UnityEngine;

namespace Types.Audio {
    public interface IAudioSource {
    
        AudioSource Source { get; }
    
        void Play(AudioClip clip);
    }
}