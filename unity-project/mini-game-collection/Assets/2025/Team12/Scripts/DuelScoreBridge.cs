using MiniGameCollection.Games2025.Team00;
using UnityEngine;

namespace MiniGameCollection.Games2025.Team12
{
    /// Bridges DuelController results into ScoreKeeper and loops rounds while manager is running.
    public class DuelScoreBridge : MiniGameBehaviour
    {
        [Header("Refs")]
        [SerializeField] private DuelController duel;
        [SerializeField] private DuelRandomizer randomizer;
        [SerializeField] private ScoreKeeper scoreKeeper;

        [Header("Points")]
        [SerializeField] private int winPoints = 1;
        [SerializeField] private int drawPoints = 0;

        private void OnEnable()
        {
            duel.OnRoundEnd += HandleRoundEnd;
        }
        private void OnDisable()
        {
            duel.OnRoundEnd -= HandleRoundEnd;
        }

        protected override void OnGameStart()
        {
            // Manager finished 3-2-1; arm the first round so the prompts are ready when inputs unlock.
            if (randomizer) randomizer.ArmNextRound();
        }

        private void HandleRoundEnd(DuelController.RoundResult result)
        {
            switch (result)
            {
                case DuelController.RoundResult.P1:
                    scoreKeeper.AddScore(PlayerID.Player1, winPoints);
                    break;
                case DuelController.RoundResult.P2:
                    scoreKeeper.AddScore(PlayerID.Player2, winPoints);
                    break;
                case DuelController.RoundResult.Draw:
                    if (drawPoints != 0)
                    {
                        scoreKeeper.AddScore(PlayerID.Player1, drawPoints);
                        scoreKeeper.AddScore(PlayerID.Player2, drawPoints);
                    }
                    break;
            }

            // Prepare next round immediately (manager timer keeps running in background).
            if (randomizer) randomizer.ArmNextRound();
        }
    }
}
