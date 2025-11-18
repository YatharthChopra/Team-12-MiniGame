using UnityEngine;

namespace MiniGameCollection.Games2025.Team12
{
    public class DuelRandomizer : MonoBehaviour
    {
        [SerializeField] private DuelController duel;

        public void ArmNextRound()
        {

            var p1 = Pick();
            var p2 = Pick();
            duel.SetTargets(p1, p2);
        }

        private DuelController.DuelAction RandomInput()
        {
            int r = Random.Range(0, 6); // Up, Down, Left, Right, Action1, Action2
            switch (r)
            {
                case 0: return DuelController.DuelAction.Up;
                case 1: return DuelController.DuelAction.Down;
                case 2: return DuelController.DuelAction.Left;
                case 3: return DuelController.DuelAction.Right;
                case 4: return DuelController.DuelAction.Action1;
                case 5: return DuelController.DuelAction.Action2;
            }
            return DuelController.DuelAction.Action1;
        }

        private DuelController.DuelAction[] Pick()
        { // this chooses the combo
            DuelController.DuelAction[] combo = new DuelController.DuelAction[5];
            for (int i = 0; i < 5; i++)
            {
                combo[i] = RandomInput();
            }
            return (combo);
        }
    }
}
