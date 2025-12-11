using System.Collections.Generic;
using UnityEngine;
using ForestGame;
namespace IACGGames
{
    public enum UIState
    {
        MainMenu,
        GameHUD
    }
    public class UIManager : Singleton<UIManager>
    {
        [Header("Panels (assign inspector)")]
        public GameHUD gameHUD;
        public MainMenu mainMenu;
        private Dictionary<UIState, UIPanelBase> panels;
        private UIState currentState;
      //  [SerializeField] TutorialSkipButton tutorialSkipButton;
        protected override void Awake()
        {
            base.Awake();

            panels = new Dictionary<UIState, UIPanelBase>()
        {
            { UIState.MainMenu, mainMenu },
            { UIState.GameHUD, gameHUD },
        };

            foreach (var item in panels)
            {
                item.Value.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            //tutorialSkipButton.Init();

        }
        public void Show(UIState state,float animTime)
        {
          
            // Hide currently active
            if (panels.ContainsKey(currentState))
                panels[currentState].Hide();

            // Show new
           // Debug.Log("Current UI " + state);
            panels[state].Show(animTime);
            currentState = state;
        }
        public void Init()
        {
      
            Show(UIState.MainMenu, 0); //show level selection first
     
        }

        

    }
}

