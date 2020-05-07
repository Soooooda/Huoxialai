using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCMovement : MonoBehaviour
{
    public float walkingSpeed = 0.06f;
    public GameObject currentWeapon = null;
    public GameObject currentShield = null;
    public GameObject healthBar;
    public float health = 1f;
    public float maxHealth = 1f;

    private GameObject player;
    private Vector3 playerPos;
    private Vector3 NPCPos;
    private bool playerIsNear;
    private bool attacking;
    private Animator m_animator;

    private int numWeaponNPCHave;
    private int maxWeaponNum;
    private int nearByWeaponIndex;
    private int countFrame;
    private int changeDirectionDurtion;
    private GameObject thisNPC;
    private float fixRotationY = 0;
    private Camera cam;
    //private int currentState = 1;


    // Start is called before the first frame update
    public void increaseWeaponNum() {
        if(numWeaponNPCHave < maxWeaponNum) {
            numWeaponNPCHave += 1;
        }
        // Debug.Log("Now this NPC has one more weapon");
    }

    public void decreaseWeaponNum() {
        if(numWeaponNPCHave >= 1) {
            numWeaponNPCHave -= 1;
        }
        Debug.Log("Now this NPC has used one weapon");
    }

    private int findNearByWeaponIndex() {
        int i = 0;
        int res = -1;
        for(i = 0; i < Environment.weaponPositionList.Count; i++) {
            if(Environment.weaponPositionList[i] != null && Vector3.Distance(NPCPos, Environment.weaponPositionList[i].transform.position) <= 15) {
                if(Environment.weaponPositionList[i].transform.position.y >= 2)
                return i;
            }
        }
        return res;
    }

    private Vector3 getNearByWeaponByIndex(int index) {
        GameObject nearByWeapon = Environment.weaponPositionList[index];
        return nearByWeapon.transform.position;
    }


    void Start()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_animator.SetFloat("transit", 0.3f);
        StartCoroutine("RandomDirectionChanged");
        player = GameObject.Find("Player");
        numWeaponNPCHave = 0;
        maxWeaponNum = 1;
        nearByWeaponIndex = -1;
        countFrame = 0;
        changeDirectionDurtion = 60;
        health = 1f;
        thisNPC = gameObject;
        cam = GameObject.Find("Player/MainCamera").GetComponent<Camera>();
        healthBar = Instantiate(healthBar, GameObject.Find("Canvas").transform);

        StartCoroutine("CheckInSafeZone");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Environment.weaponPositionList[0].printPos());
        countFrame += 1;
        transform.Translate(Vector3.forward * walkingSpeed);
        attacking = false;

        // Configurate the health bar
        if (Vector3.Distance(player.transform.position, transform.position) < 15) {
            healthBar.SetActive(true);
            updateHealthBar();
        }
        else { healthBar.SetActive(false); }

        // Check death
        if (health <= 0)
        {
            DeathEffect();
        }


        playerPos = player.transform.position;
        NPCPos = transform.position;
        float distanceToPlayer = Vector3.Distance(playerPos, NPCPos);
        Vector3 weaponPos;
        Transform t;
        t = transform;
        if(numWeaponNPCHave == 0) {
            // if(nearByWeaponIndex == -1) {
            nearByWeaponIndex = findNearByWeaponIndex();
            // }
            if(nearByWeaponIndex != -1) {
                weaponPos = getNearByWeaponByIndex(nearByWeaponIndex);
                Vector3 dx = (weaponPos - NPCPos) / Vector3.Distance(weaponPos, NPCPos);
                dx.y = 0;
                transform.position += dx * 0.05f * Time.deltaTime;
                transform.LookAt(weaponPos);
            } else {
                if(distanceToPlayer <= 15) {
                    playerIsNear = true;
                    Vector3 dx = (playerPos - NPCPos) / Vector3.Distance(playerPos, NPCPos);
                    dx.y = 0;
                    transform.position += dx * 0.05f * Time.deltaTime;
                    transform.LookAt(player.transform); 
                } else {
                    if(countFrame % changeDirectionDurtion == 0) {
                        float y = Random.Range(0, 360);
                        transform.Rotate(0, y, 0);
                    }
                    transform.position += Vector3.forward * Time.deltaTime;
                }
            }
            // transform.Rotate(transform.localRotation.x, 0, transform.localRotation.z); 
            // transform.eulerAngles = new Vector3 (t.eulerAngles.x, fixRotationY, t.eulerAngles.z);
        } else {
            // Debug.Log("NPC already has weapon");
            if(distanceToPlayer <= 15) {
                if(distanceToPlayer <= 3) {
                    attacking = true;
                    // StartCoroutine("NPCAttackPlayer");
                } else {
                    playerIsNear = true;
                    Vector3 dx = (playerPos - NPCPos) / Vector3.Distance(playerPos, NPCPos);
                    dx.y = 0;
                    transform.position += dx * 0.05f * Time.deltaTime;
                    transform.LookAt(player.transform);
                    attacking = false;
                }
            } else {
                if(countFrame % changeDirectionDurtion == 0) {
                    float y = Random.Range(0, 360);
                    transform.Rotate(0, y, 0);
                }
                transform.position += Vector3.forward * Time.deltaTime;
                playerIsNear = false;
                attacking = false;
            }
            // transform.eulerAngles = new Vector3 (t.eulerAngles.x, fixRotationY, t.eulerAngles.z);
            
        }

        // transform.Rotate(transform.localRotation.x, 0, transform.localRotation.z);          


        // if (3 < distanceToPlayer && distanceToPlayer < 100 )
        // {
        //     playerIsNear = true;
        //     attacking = false;
        //     //Debug.Log(gameObject.name);
        //     //Debug.Log("distanceToPlayer!"+ distanceToPlayer);
        //     Vector3 dx = (playerPos - NPCPos) / Vector3.Distance(playerPos, NPCPos);
        //     transform.position += dx * 0.05f * Time.deltaTime;
        //     transform.LookAt(player.transform);
        // }
        // else if (distanceToPlayer <= 3)
        // {
        //     attacking = true;
        // }
        // else
        // {
        //     playerIsNear = false;
        //     attacking = false;
        // }

    }

    // private void NPCAttackPlayer() {
    //     if(thisNPC.GetComponent<Weapon>() == null) {
    //         Debug.Log("1");
    //     } else {
    //         Debug.Log("don't fuck 1");
    //     }
    //     // if(thisNPC.Find)
    //     // if(thisNPC.Find("Player/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Weapon") != null) {
    //     //     Debug.Log("hahahahah");
    //     // }
    //     if(thisNPC.GetComponent<Wrench>() == null) {
    //         Debug.Log("fuck 2");
    //     } else {
    //         Debug.Log("don't fuck 2");
    //     }
    //     // thisNPC.GetComponent<Wrench>().PlayAnimation();
    // }

    public void Throw()
    {
        currentWeapon.SendMessage("ThrowMe");
    }

    public void getHurt(float ATK)
    {
        health -= ATK;
        StartCoroutine("hurt");
    }

    public IEnumerator hurt()
    {
        m_animator.SetBool("Hurt", true);
        yield return new WaitForSeconds(Time.deltaTime * 10);
        m_animator.SetBool("Hurt", false);

    }

    private IEnumerator RandomDirectionChanged()
    {
        if (!playerIsNear)
        {
            yield return new WaitForSecondsRealtime(1f);

            float y = Random.Range(0, 360);// Randomly pick a rotation direction

            transform.Rotate(0, y, 0);
        }
    }

    public void DeathEffect()
    {

        GameObject bone = GameObject.Find("EnvironmentManager").GetComponent<Environment>().bone;
        GameObject deathBone = Instantiate(bone, transform.position, Quaternion.identity);
        deathBone.AddComponent<Rigidbody>().useGravity = true;
        deathBone.AddComponent<BoxCollider>();
        deathBone.transform.localScale *= 5;

        Destroy(gameObject);
        Destroy(healthBar);
    }


    private void updateHealthBar()
    {
        Vector3 barWorldPosition = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head").transform.position;
        //Vector3 barWorldPosition = transform.position;
        Vector3 screenPos = cam.WorldToScreenPoint(barWorldPosition);
        screenPos.y += 20;
        healthBar.transform.position = screenPos;

        //healthBar.SetActive(true);

        healthBar.GetComponent<Scrollbar>().size = health / maxHealth;
        if (health < 0.5f)
        {
            ColorBlock cb = healthBar.GetComponent<Scrollbar>().colors;
            cb.disabledColor = new Color(1.0f, 0.0f, 0.0f);
            healthBar.GetComponent<Scrollbar>().colors = cb;
        }
        else
        {
            ColorBlock cb = healthBar.GetComponent<Scrollbar>().colors;
            cb.disabledColor = new Color(0.0f, 1.0f, 0.25f);
            healthBar.GetComponent<Scrollbar>().colors = cb;
        }
        //healthBar.SetActive(false);
    }

    private IEnumerator CheckInSafeZone()
    {
        while (true)
        {
            GameObject sphere = GameObject.Find("Sphere");
            Vector3 SafeZoneCenter = sphere.transform.position;
            float SafeZoneRadius = sphere.transform.localScale.x * sphere.GetComponent<SphereCollider>().radius;
            if (Vector3.Distance(transform.position, SafeZoneCenter) > SafeZoneRadius)
            {
                health -= 0.1f;
                Debug.Log(transform.name + " is outside the safe zone!");
            }
            yield return new WaitForSecondsRealtime(1f);
        }

    }
}
