using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour {

    public Canvas BossCanvas;
    public GameObject HealthPrefab;
    public float HealthPanelOffset = 0.35f;

    private GameObject HealthPanel;
    private EnemyBase enemyScript;
    private Slider healthSlider;
    private float xAxisOffset;

    void Start ()
    {
        enemyScript = GetComponent<EnemyBase>();
        HealthPanel = Instantiate(HealthPrefab) as GameObject;
        HealthPanel.transform.SetParent(BossCanvas.transform, false);
        healthSlider = HealthPanel.GetComponentInChildren<Slider>();
        xAxisOffset = GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        HealthPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!HealthPanel.gameObject.activeInHierarchy && enemyScript.Health < enemyScript.StartingHealth)
        {
            HealthPanel.gameObject.SetActive(true);
        }

        healthSlider.value = enemyScript.Health / (float)enemyScript.StartingHealth;

        if (healthSlider.value <= .01)
        {
            HealthPanel.gameObject.SetActive(false);
        }

        Vector3 worldPos = new Vector3(transform.position.x - xAxisOffset, transform.position.y + HealthPanelOffset, transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        HealthPanel.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
    }
}
