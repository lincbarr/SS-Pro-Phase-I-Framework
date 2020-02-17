using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _minePowerUp;
    [SerializeField] private GameObject[] _powerUps;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnSpaceMineRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        while(true)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 posToSpawn = new Vector3(randomX, 7.0f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (true)
        {
            int randomPowerUp = Random.Range(0, _powerUps.Length);
            float randomSeconds = Random.Range(3, 8);
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 posToSpawn = new Vector3(randomX, 7.0f, 0);
            yield return new WaitForSeconds(randomSeconds);
            Instantiate(_powerUps[randomPowerUp], posToSpawn, Quaternion.identity);
        }
    }

    private IEnumerator SpawnSpaceMineRoutine()
    {
        float startRandomSeconds = Random.Range(10, 18);
        yield return new WaitForSeconds(startRandomSeconds);

        while (true)
        {
            float randomSeconds = Random.Range(18, 25);
            float randomX = Random.Range(-8.0f, 8.0f);
            Vector3 posToSpawn = new Vector3(randomX, 7.0f, 0);
            yield return new WaitForSeconds(randomSeconds);
            Instantiate(_minePowerUp, posToSpawn, Quaternion.identity);
        }
    }


    public void OnPlayerDeath()
    {
        Destroy(this.gameObject);  // Workaround for not being able to stop SpawnEnemyRoutine.
    }
}
