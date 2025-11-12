using System;
using UnityEngine;

namespace MiniGameCollection.Games2025.Team00
{
    public class ScoreKeeper : MiniGameBehaviour
    {
        [field: SerializeField] public MiniGameScoreUI MiniGameScoreUI { get; private set; }
        [field: SerializeField] public int P1Score { get; private set; }
        [field: SerializeField] public int P2Score { get; private set; }
        
        public delegate void ScoreUpdate(PlayerID playerID, int points);
        public event ScoreUpdate OnScoreUpdate;

        protected override void OnGameEnd()
        {
            if (P1Score == P2Score)
                MiniGameManager.Winner = MiniGameWinner.Draw;
            else if (P1Score > P2Score)
                MiniGameManager.Winner = MiniGameWinner.Player1;
            else if (P1Score < P2Score)
                MiniGameManager.Winner = MiniGameWinner.Player2;
            else
                throw new NotImplementedException();
        }

        private void UpdateScores()
        {
            MiniGameScoreUI.SetPlayerScore(1, P1Score);
            MiniGameScoreUI.SetPlayerScore(2, P2Score);
        }

        public void AddScore(PlayerID playerID, int points)
        {
            switch (playerID)
            {
                case PlayerID.Player1: P1Score += points; break;
                case PlayerID.Player2: P2Score += points; break;
                default: throw new NotImplementedException();
            }
            UpdateScores();
            OnScoreUpdate?.Invoke(playerID, points);
        }
    }
}
