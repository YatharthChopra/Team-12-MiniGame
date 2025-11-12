using MiniGameCollection.Games2025.Team00;
using System;
using TMPro;
using UnityEngine;

namespace MiniGameCollection.Games2025.Team12
{
    /// Wild West Duel: waits for MiniGameManager 3-2-1 countdown, unlocks at GO!,
    /// first player to press their target input wins the round.
    public class DuelController : MiniGameBehaviour
    {
        public enum RoundResult { None, P1, P2, Draw }
        public enum DuelAction { Up, Down, Left, Right, Action1, Action2, None }

        [Header("UI (assign from scene)")]
        [SerializeField] private TMP_Text countdownText;   // Hook to the existing message UI or separate text
        [SerializeField] private TMP_Text p1PromptText;    // Optional: "Target: ↑"
        [SerializeField] private TMP_Text p2PromptText;    // Optional: "Target: Action1"

        [Header("Tuning")]
        [Tooltip("Treat presses within this (seconds) as a draw.")]
        [SerializeField] private float simultaneousEpsilon = 0.06f;

        [Header("State (read-only)")]
        [SerializeField] private bool inputsEnabled = false;

        // Targets for the current round (set by randomizer BEFORE the round starts)
        [SerializeField] private DuelAction p1Target = DuelAction.Action1;
        [SerializeField] private DuelAction p2Target = DuelAction.Action1;

        public event Action OnRoundArmed;                // Countdown began (inputs locked)
        public event Action OnRoundFire;                 // Inputs just unlocked (GO!)
        public event Action<RoundResult> OnRoundEnd;     // Winner/Draw for the round

        /// Set the required inputs this round (call before OnGameStart / StartRound).
        public void SetTargets(DuelAction p1, DuelAction p2)
        {
            p1Target = p1;
            p2Target = p2;
            if (p1PromptText) p1PromptText.text = $"Target: {Pretty(p1Target)}";
            if (p2PromptText) p2PromptText.text = $"Target: {Pretty(p2Target)}";
        }

        // ===== MiniGameBehaviour hooks =====

        protected override void OnCountDown(string message)
        {
            // The manager drives "3", "2", "1", "GO!", then clears.
            if (countdownText) countdownText.text = message;
            inputsEnabled = false;            // lock during countdown
            OnRoundArmed?.Invoke();
        }

        protected override void OnGameStart()
        {
            // Manager finished "GO!" and started the timer => unlock inputs now.
            inputsEnabled = true;
            OnRoundFire?.Invoke();
            // Begin listening immediately.
            firstWho = RoundResult.None;
            firstStamp = float.MaxValue;
        }

        protected override void OnTimerUpdate(float _)
        {
            // While the manager timer runs, listen for the first correct press.
            if (!inputsEnabled) return;

            var p1Pressed = ReadPressed(PlayerID.Player1);
            var p2Pressed = ReadPressed(PlayerID.Player2);

            bool p1Ok = (p1Pressed == p1Target);
            bool p2Ok = (p2Pressed == p2Target);

            if (p1Ok) RegisterPress(RoundResult.P1);
            if (p2Ok) RegisterPress(RoundResult.P2);

            // Resolve early if we already have a winner/draw
            if (firstWho == RoundResult.P1 || firstWho == RoundResult.P2 || firstWho == RoundResult.Draw)
                EndRound(firstWho);
        }

        protected override void OnGameEnd()
        {
            // Manager ended the mini-game (time up or forced stop) → lock inputs & clear UI.
            inputsEnabled = false;
            if (countdownText) countdownText.text = "";
        }

        // ===== Internal round detection =====

        private RoundResult firstWho = RoundResult.None;
        private float firstStamp = float.MaxValue;

        private void RegisterPress(RoundResult who)
        {
            // First valid press wins; near-simultaneous counts as draw.
            float now = Time.time;

            if (now < firstStamp)
            {
                // New earliest
                if (firstWho != RoundResult.None && (now - firstStamp) <= simultaneousEpsilon)
                {
                    firstWho = RoundResult.Draw;
                }
                else
                {
                    firstWho = who;
                }
                firstStamp = now;
            }
            else
            {
                // Someone already pressed first; check for near-simultaneous draw
                if ((now - firstStamp) <= simultaneousEpsilon && firstWho != who && firstWho != RoundResult.Draw)
                {
                    firstWho = RoundResult.Draw;
                }
            }
        }

        private void EndRound(RoundResult result)
        {
            inputsEnabled = false;
            if (countdownText) countdownText.text = "";
            OnRoundEnd?.Invoke(result);
        }

        // Read one logical action from ArcadeInput (same pattern as PlayerController)
        private DuelAction ReadPressed(PlayerID pid)
        {
            var p = ArcadeInput.Players[(int)pid];

            if (p.Action1.Pressed) return DuelAction.Action1;
            if (p.Action2.Pressed) return DuelAction.Action2;

            // Treat stick as digital 4-way like the debug scene
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
            switch (a)
            {
                case DuelAction.Up: return "↑";
                case DuelAction.Down: return "↓";
                case DuelAction.Left: return "←";
                case DuelAction.Right: return "→";
                case DuelAction.Action1: return "Action1";
                case DuelAction.Action2: return "Action2";
                default: return "-";
            }
        }
    }
}
