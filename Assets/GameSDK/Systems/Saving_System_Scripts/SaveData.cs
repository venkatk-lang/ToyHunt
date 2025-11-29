using System.Collections.Generic;
using UnityEngine;
namespace IACGGames {
    [System.Serializable]
    public class SaveData
    {
        [Header("Settings Save Data")]
        public bool vibrationOn;
        public bool inGameSoundFXOn;
        public bool bgSoundOn;
        public float bgSoundValue;
        public float inGameSoundFXValue;

        [Header("Level Save Data")]
  
        public bool tutorialCompleted;
        public SaveData(GameConfig gameConfig)
        {
            tutorialCompleted = false;
            vibrationOn = true;
            inGameSoundFXOn = true;
            bgSoundOn = true;
            bgSoundValue = 1f;
            inGameSoundFXValue = 1f;
   
        }
       
    }

}
