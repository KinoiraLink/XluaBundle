using System;
using UnityEngine.UI;

// ReSharper disable All

namespace LuaBehaviour
{
    public class TestAudioUI : LuaBehaviour
    {
        private Action _luaOnOpen;
        private Action _luaOnClose;


        private Button btn_play_music;
        private Button btn_pause_music;
        private Button btn_unpause_music;
        private Button btn_stop_music;
        private Button btn_play_sound;

        private Slider slider_music_volume;
        private Slider slider_sound_volume;

        public override void Init(string luaName)
        {
            base.Init(luaName);
            _luaOnOpen = _scriptEnv.Get<Action>("OnOpen");
            _luaOnClose = _scriptEnv.Get<Action>("OnClose");
        }
        
        public void OnOpen()
        {
            _luaOnOpen?.Invoke();
        }

        public void OnClose()
        {
            _luaOnClose?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            _luaOnOpen = null;
            _luaOnClose = null;
        }
    }
}