using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnSetting
{
    [Tooltip("이 설정으로 생성할 적 프리팹")]
    public GameObject enemyPrefab;

    [Tooltip("보스 HUD에 표시할 이름. 비워 두면 프리팹 이름을 사용합니다.")]
    public string displayName;

    [Min(0)]
    [Tooltip("생성할 적 수. 지정한 스폰 지점 수보다 많으면 지점 수에 맞춰 줄어듭니다.")]
    public int count = 1;

    [Tooltip("적이 생성될 위치들. 스폰 지점 오브젝트는 Monster Container 밖에 둡니다.")]
    public EnemySpawnPoint[] spawnPoints = new EnemySpawnPoint[0];
}

public class RoomManager : MonoBehaviour
{
    public enum RoomType { NormalCombat, Shop, Treasure, Boss }

    [Header("방 설정")]
    [SerializeField] private RoomType roomType = RoomType.NormalCombat;
    [SerializeField] private RoomMoveTrigger exitDoor;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private bool isInitialRoom;

    [Header("전투방 전용 (몬스터 부모)")]
    [SerializeField] private Transform monsterContainer;

    [Header("전투방 적 생성 설정")]
    [SerializeField] private List<EnemySpawnSetting> enemySpawnSettings = new List<EnemySpawnSetting>();
    [SerializeField] private bool replaceExistingMonstersWhenSpawning = true;

    [Header("보스 UI")]
    [SerializeField] private BossHealthBarUI bossHealthBar;

    private bool isCleared = false;
    private bool hasSpawnedConfiguredEnemies = false;

    void Start()
    {
        if (roomType == RoomType.Shop || roomType == RoomType.Treasure)
        {
            ClearRoom();
        }
        else if (exitDoor != null)
        {
            exitDoor.SetDoorOpen(false);
        }

        // 최초 방만 게임 시작 시 플레이어를 배치하고 적을 생성합니다.
        if (isInitialRoom)
            EnterRoom(GameObject.FindGameObjectWithTag("Player"));
    }

    public void EnterRoom(GameObject player)
    {
        if (player == null)
        {
            Debug.LogWarning($"[{name}] 입장할 플레이어를 찾지 못했습니다.", this);
            return;
        }

        if (playerSpawnPoint != null)
            player.transform.position = playerSpawnPoint.position;

        if ((roomType == RoomType.NormalCombat || roomType == RoomType.Boss) && !hasSpawnedConfiguredEnemies)
        {
            hasSpawnedConfiguredEnemies = true;
            SpawnConfiguredEnemies();
        }
    }

    void Update()
    {
        if (isCleared) return;

        // Boss rooms are inactive until EnterRoom spawns their configured boss.
        if (roomType == RoomType.Boss && !hasSpawnedConfiguredEnemies)
            return;

        if (roomType == RoomType.NormalCombat || roomType == RoomType.Boss)
            CheckMonsters();
    }

    private void CheckMonsters()
    {
        if (monsterContainer != null && monsterContainer.childCount == 0)
            ClearRoom();
    }

    private void SpawnConfiguredEnemies()
    {
        bool hasConfiguredEnemies = false;
        foreach (EnemySpawnSetting setting in enemySpawnSettings)
        {
            if (setting == null || setting.enemyPrefab == null || setting.count <= 0 || setting.spawnPoints == null)
                continue;

            foreach (EnemySpawnPoint point in setting.spawnPoints)
            {
                if (point != null)
                {
                    hasConfiguredEnemies = true;
                    break;
                }
            }

            if (hasConfiguredEnemies) break;
        }

        if (!hasConfiguredEnemies) return;

        if (monsterContainer == null)
        {
            Debug.LogError($"[{name}] 적 생성 설정이 있지만 Monster Container가 지정되지 않았습니다.", this);
            return;
        }

        if (replaceExistingMonstersWhenSpawning)
        {
            for (int i = monsterContainer.childCount - 1; i >= 0; i--)
                Destroy(monsterContainer.GetChild(i).gameObject);
        }

        foreach (EnemySpawnSetting setting in enemySpawnSettings)
        {
            if (setting == null || setting.enemyPrefab == null || setting.count <= 0) continue;

            List<EnemySpawnPoint> validPoints = new List<EnemySpawnPoint>();
            if (setting.spawnPoints != null)
            {
                foreach (EnemySpawnPoint point in setting.spawnPoints)
                    if (point != null) validPoints.Add(point);
            }

            if (validPoints.Count == 0)
            {
                Debug.LogWarning($"[{name}] {setting.enemyPrefab.name} 스폰 지점이 없어 생성하지 않았습니다.", this);
                continue;
            }

            int spawnCount = Mathf.Min(setting.count, validPoints.Count);
            if (spawnCount < setting.count)
                Debug.LogWarning($"[{name}] {setting.enemyPrefab.name} 수량({setting.count})이 스폰 지점 수({validPoints.Count})보다 많아 {spawnCount}마리만 생성합니다.", this);

            for (int i = 0; i < spawnCount; i++)
            {
                int selectedIndex = Random.Range(i, validPoints.Count);
                EnemySpawnPoint temp = validPoints[i];
                validPoints[i] = validPoints[selectedIndex];
                validPoints[selectedIndex] = temp;

                Transform spawn = validPoints[i].transform;
                GameObject spawnedEnemy = Instantiate(setting.enemyPrefab, spawn.position,
                    setting.enemyPrefab.transform.rotation, monsterContainer);

                if (roomType == RoomType.Boss)
                {
                    if (bossHealthBar == null)
                        bossHealthBar = FindObjectOfType<BossHealthBarUI>();

                    CharacterStats bossStats = spawnedEnemy.GetComponent<CharacterStats>();
                    if (bossHealthBar != null && bossStats != null)
                    {
                        string displayName = string.IsNullOrWhiteSpace(setting.displayName)
                            ? setting.enemyPrefab.name
                            : setting.displayName;
                        bossHealthBar.ShowBoss(bossStats, displayName);
                    }
                }
            }
        }
    }

    private void ClearRoom()
    {
        isCleared = true;
        Debug.Log("방 클리어 완료! 출구 문이 열렸습니다.");

        if (roomType == RoomType.Boss && bossHealthBar != null)
            bossHealthBar.HideBoss();

        if (exitDoor != null)
            exitDoor.SetDoorOpen(true);
    }
}
