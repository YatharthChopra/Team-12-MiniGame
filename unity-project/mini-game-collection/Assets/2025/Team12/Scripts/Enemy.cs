using UnityEngine;

namespace MiniGameCollection.Games2025.Team00
{
    public class Enemy : MiniGameBehaviour
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        // These should probably be in a lookup table
        [field: SerializeField] public int PointsAwarded { get; private set; } = 100;
        [field: SerializeField] public int PointPenalty { get; private set; } = -100;
        [field: SerializeField] public float ShootInterval { get; private set; } = 1;
        [field: SerializeField] public float BulletSpeed { get; private set; } = 4; // units per second
        [field: SerializeField] public GameObject BulletPrefab { get; private set; }
        [field: SerializeField] public ScoreKeeper ScoreKeeper { get; private set; }

        private float timePassed = 0;

        private void Update()
        {
            switch (EnemyType)
            {
                case EnemyType.Level1_Green:
                    //LogicLevel1();
                    break;
                case EnemyType.Level2_Orange:
                    //LogicLevel2();
                    break;
                case EnemyType.Level3_Black:
                    //LogicLevel3();
                    break;
                default: throw new System.NotImplementedException();
            }
        }

        private void LogicLevel1()
        {
            // Nothing :)
        }

        private void LogicLevel2()
        {
            // hive mind random shooting ... ?
        }

        private void LogicLevel3()
        {
            timePassed += Time.deltaTime;
            bool doShoot = timePassed >= ShootInterval;
            if (!doShoot)
                return;

            timePassed -= ShootInterval;
            ShootBullets();
        }

        public void DestroyEnemy()
        {
            Destroy(this.gameObject);
        }

        private void ShootBullet(Vector3 direction)
        {
            Vector3 position = transform.position + direction;
            GameObject prefab = Instantiate(BulletPrefab, position, Quaternion.Euler(0, 0, 90));
            Bullet bullet = prefab.GetComponent<Bullet>();
            bullet.Shoot(BulletOwner.Enemy, direction, BulletSpeed, ScoreKeeper, MiniGameManager);
            bullet.PointPenalty = PointPenalty; // gave up and made this property mutable...
        }

        private void ShootBullets()
        {
            ShootBullet(Vector3.left);
            ShootBullet(Vector3.right);
        }

    }
}