using System;
using System.Collections;
using UnityEngine;
using TMPro;
using MiniGameCollection.Games2025.Team00;

namespace MiniGameCollection.Games2025.Team12
{
    public class DuelController : MiniGameBehaviour
    {
        public enum RoundResult { None, P1, P2, Draw }
        public enum DuelAction { Up, Down, Left, Right, Action1, Action2, None }
        [Header("UI (assign from scene)")]
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text p1PromptText;
        [SerializeField] private TMP_Text p2PromptText;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip startSound;      // plays during countdown & round
        [SerializeField] private AudioClip gameLoopSound;   // plays after round ends
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;

        [Header("Settings")]
        [SerializeField] private float simultaneousEpsilon = 0.06f;
        
        private bool inputsEnabled = false;
        private RoundResult firstWho = RoundResult.None;
        private float firstStamp = float.MaxValue;

        public event Action<RoundResult> OnRoundEnd;

        private bool firstRun = false;
        [SerializeField] private DuelAction[] p1Targets = {DuelAction.Up, DuelAction.Action1, DuelAction.Action2, DuelAction.Down, DuelAction.Left };
        [SerializeField] private DuelAction[] p2Targets = {DuelAction.Up, DuelAction.Action1, DuelAction.Action2, DuelAction.Down, DuelAction.Left };

        [SerializeField] private int p1count = 0;
        [SerializeField] private int p2count = 0;

        public void SetTargets(DuelAction[] p1, DuelAction[] p2)
        {
            string temp = "";
            p1Targets = p1;
            p2Targets = p2;
            foreach (var target in p1Targets)
            {
                temp += "\n" + Pretty(target);
            }
            if (p1PromptText) p1PromptText.text = $"Target: {temp}";
            temp = "";
            foreach (var target in p2Targets)
            {
                temp += "\n" + Pretty(target);
            }
            if (p2PromptText) p2PromptText.text = $"Target: {temp}";

        }

        // ========== EVENTS ==========
        protected override void OnCountDown(string message)
        {
            if (countdownText) countdownText.text = message;
            inputsEnabled = false;

            // play start sound (throughout the round)
            if (startSound != null)
            {
                audioSource.Stop();
                audioSource.loop = true;
                audioSource.clip = startSound;
                audioSource.Play();
            }
        }

        protected override void OnGameStart()
        {
            inputsEnabled = true;
            firstWho = RoundResult.None;
            firstStamp = float.MaxValue;
        }


        protected override void OnTimerUpdate(float _)
        {
            
            if (!inputsEnabled) return;
            if (!firstRun)
            {
                this.GetComponent<DuelRandomizer>().ArmNextRound();
                firstRun = true;
            }
            var p1Pressed = ReadPressed(PlayerID.Player1);
            var p2Pressed = ReadPressed(PlayerID.Player2);
            
            bool p1Ok = (p1Pressed == p1Targets[p1count]);
            bool p2Ok = (p2Pressed == p2Targets[p2count]);
            //Debug.Log(p1Ok);
            if (p1Ok && p1count < 4)
            { // goes through the combo
                p1count++;
                p1Ok = false;
                
            }
            if (p2Ok && p2count < 4)
            { // goes through the combo
                p2count++;
                p2Ok = false;
            }

            if (p1Ok) RegisterPress(RoundResult.P1);
            if (p2Ok) RegisterPress(RoundResult.P2);

            if (firstWho == RoundResult.P1 || firstWho == RoundResult.P2 || firstWho == RoundResult.Draw)
                EndRound(firstWho);
        }

        protected override void OnGameEnd()
        {
            inputsEnabled = false;

            // round finished: play gameLoop sound
            if (gameLoopSound != null)
            {
                audioSource.Stop();
                audioSource.loop = true;
                audioSource.clip = gameLoopSound;
                audioSource.Play();
            }
        }

        // ========== ROUND LOGIC ==========
        private void RegisterPress(RoundResult who)
        {
            float now = Time.time;
            if (now < firstStamp)
            {
                if (firstWho != RoundResult.None && (now - firstStamp) <= simultaneousEpsilon)
                    firstWho = RoundResult.Draw;
                else
                    firstWho = who;
                firstStamp = now;
            }
            else if ((now - firstStamp) <= simultaneousEpsilon && firstWho != who && firstWho != RoundResult.Draw)
            {
                firstWho = RoundResult.Draw;
            }
        }

        private void EndRound(RoundResult result)
        {
            if (!inputsEnabled) return;
            //inputsEnabled = false;
            p1count = 0; // resets combo count
            p2count = 0; // resets combo count

            // play win/lose sound ON TOP of current start sound
            if (result == RoundResult.P1 || result == RoundResult.P2)
                if (winSound != null) audioSource.PlayOneShot(winSound);
            if (result == RoundResult.Draw)
                if (loseSound != null) audioSource.PlayOneShot(loseSound);
            firstWho = RoundResult.None;
            firstStamp = float.MaxValue;
            OnRoundEnd?.Invoke(result);
        }

        // ========== INPUT LOGIC ==========
        private DuelAction ReadPressed(PlayerID pid)
        {
            var p = ArcadeInput.Players[(int)pid];
            if (p.Action1.Pressed) return DuelAction.Action1;
            if (p.Action2.Pressed) return DuelAction.Action2;

            const float dead = 0.6f;
            float x = p.AxisX;
            float y = p.AxisY;
            
            if (Mathf.Abs(y) < dead)
            {
                if (x <= -dead) return DuelAction.Left;
                if (x >= dead) return DuelAction.Right;
            }
            if (Mathf.Abs(x) < dead)
            {
                if (y <= -dead) return DuelAction.Down;
                if (y >= dead) return DuelAction.Up;
            }

            return DuelAction.None;
        }

        private static string Pretty(DuelAction a)
        {
            return a switch
            {
                DuelAction.Up => "↑",
                DuelAction.Down => "↓",
                DuelAction.Left => "←",
                DuelAction.Right => "→",
                DuelAction.Action1 => "Action1",
                DuelAction.Action2 => "Action2",
                _ => "-"
            };
        }
    }
}
