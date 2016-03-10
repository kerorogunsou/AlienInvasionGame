using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpawnTanks : MonoBehaviour {
	#region variables
	public GameObject[] backgrounds;
	Animator[] backgroundAnimators;
	bool urban = false;
	public Sprite buildingUrbanSprite;

    control_stuff alienControlScript;
    public Camera globalCamera;
	const int bonuseFlavours = 3;
	int[] bonuseProbabilities = new int[bonuseFlavours] {25, 15, 5};
	public Sprite[] bonuseSprites;
	string[] bonuseNames = new string[bonuseFlavours] {"health", "shield", "autofire"};
	enum BonuseIndexes : int { health , shield, autofire };

    //counters determining delay between waves
    int delay = 200;
    int minDelay = 20;
    int decDelayMax = 20; // determines when to decrement delay between spawning enemies
    int decDelayCounter = 0;
    int tankSpawnCounter = 0; // determines if we have to spawn tank in this frame update
    int planeSpawnCounter = 0; // determines if we have to spawn plane in this frame update
    int bomberSpawnCounter = 0; // determines if we have to spawn bomber in this frame update
    int buildingSpawnCounter = 0;
    //counters determining when it's time to add new type of enemies
    int counterForAdditionalEnemies = 0; // enables additional types of enemies to spawn
    int planeLimit = 100; // value after which planes spawn
    int bomberLimit = 200; // value after which bombers spawn
    int buildingLimit = 200;
    bool planeLimitReached = false;
    bool bomberLimitReached = false;
    bool buildingLimitReached = false;

    //prefabs
	public GameObject tankPrefab;
	public GameObject planePrefab;
	public GameObject bomberPrefab;
    public GameObject buildingPrefab;

	public GameObject bulletPrefab;
    public GameObject grenadePrefab;
	public GameObject explosionPrefab;
    public GameObject bonusePrefab;

	//text
	public Text killedText;
	public Text deadText;
	public Text totalText;
	string killedCore = "kills: ";
	string deadCore = "deads: ";
	string totalCore = "total: ";

    //pools
    int tankQuantity = 10;
    int planeQuantity = 10;
    int bomberQuantity = 10;
    int buildingQuality = 10;
    int bonusQuantity = 5;
	List<GameObject> tanks = new List<GameObject>();
	List<GameObject> planes = new List<GameObject>();
	List<GameObject> bombers = new List<GameObject>();
    List<GameObject> buildings = new List<GameObject>();
	List<GameObject> bullets = new List<GameObject>();
    List<GameObject> grenades = new List<GameObject>();
    List<GameObject> explosions = new List<GameObject>();
    List<GameObject> bonuses = new List<GameObject>();
	tank_script tempTankScr;
    building_ai tempBuildingScr;
    bonus_ai tempBonuseScr;
    #endregion
	 void ShuffleSpriteColors (List<GameObject> pool) {
		for(int i = 0; i < pool.Count; i++) {
			pool[i].GetComponent<SpriteRenderer>().color = new Color(Random.Range (0.8f, 1f), Random.Range (0.8f, 1f), Random.Range (0.8f, 1f));
		}
	}
	void ShootBullet(Vector3 myPos, float angle) {
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0);
        float len;
        if (tanks.Count > 0) {
            len = tanks[0].GetComponent<CircleCollider2D>().radius + bullets[0].GetComponent<CircleCollider2D>().radius;
        }
        else {
            len = planes[0].GetComponent<CircleCollider2D>().radius + bullets[0].GetComponent<CircleCollider2D>().radius;
        }
        len += 0.1f; // len = size of 2 colliders + small value
        int i = InstantiateBulletIfFree(bullets, "shot", myPos + direction * len);
        //instantiate_bullet_if_free(explosions, "explosion", my_pos);
        if (i >= 0) {
			bullets[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			bullets[i].transform.position = myPos + direction * len;
		}
	}
    void DropBomb(Vector3 myPos, float angle) {
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0);
        float len = bombers[0].GetComponent<CircleCollider2D>().radius + grenades[0].GetComponent<CircleCollider2D>().radius + 0.1f;
        int i = InstantiateBulletIfFree(grenades, "shot", myPos + direction * len);
        if (i >= 0) {
            grenades[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            grenades[i].transform.position = myPos + direction * len;
        }
    }
    // Use this for initialization
    void Start () {
		backgroundAnimators = new Animator[backgrounds.Length];
		for(int i = 0; i < backgrounds.Length; i++) {
			backgroundAnimators[i] = backgrounds[i].GetComponent<Animator>();
		}

        alienControlScript = GetComponent<control_stuff>();

        int maxExplosions = tankQuantity + planeQuantity + bomberQuantity;
        int maxBullets = (tankQuantity + planeQuantity) * 50;
        int maxGrenades = bomberQuantity * 50;

        InitPool(tanks, tankPrefab, tankQuantity);
		ShuffleSpriteColors(tanks);
		InitPool(planes, planePrefab, planeQuantity);
		ShuffleSpriteColors(planes);
		InitPool(bombers, bomberPrefab, bomberQuantity, 5);
		ShuffleSpriteColors(bombers);
        InitBuildingPool(buildings, buildingPrefab, buildingQuality);

        InitBulletPool(explosions, explosionPrefab, maxExplosions);
        InitBulletPool(bullets, bulletPrefab, maxBullets);
        InitBulletPool(grenades, grenadePrefab, maxGrenades);
        InitBulletPool(bonuses, bonusePrefab, bonusQuantity);
    }
    void InitBulletPool(List<GameObject> pool, GameObject prefab, int max) {
        for(int i = 0; i < max; i++) {
            pool.Add(Instantiate(prefab, transform.position, Quaternion.identity) as GameObject);
            Deactivate(pool[i]);
        }
    }
    void InitBuildingPool(List<GameObject> pool, GameObject prefab, int max) {
        for (int i = 0; i < max; i++) {
            pool.Add(Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity) as GameObject);
            Deactivate(pool[i]);
        }
    }
	void InitPool(List<GameObject> pool, GameObject prefab, int max, int height = 0) {
		for(int i = 0; i < max; i++) {
			pool.Add(Instantiate(prefab, transform.position + Vector3.up * height, Quaternion.identity)as GameObject);
			tempTankScr = pool[i].GetComponent<tank_script>();
            if (pool[i].tag == "bomber") {
                tempTankScr.shoot = DropBomb;
            }
            else {
                tempTankScr.shoot = ShootBullet;
            }
			tempTankScr.id = i;
			Deactivate(pool[i], height);
		}
	}
	void Deactivate(GameObject g, int height = 0) {
		if(g.activeSelf) {
			g.SetActive(false);
		}
		if(g.tag == "killed" || g.tag == "dead") {
			InstantiateBulletIfFree (explosions, "explosion", g.transform.position);
		}
        g.transform.position = transform.position;// + Vector3.up * height;
		g.transform.rotation = Quaternion.identity;
		g.tag = "free";
	}
	
	void IncCount(Text text, string core) {
		int temp = 0;
		if(int.TryParse(text.text.Split(new char[] {':'})[1].Trim (), out temp)) {
			text.text = core + (temp + 1).ToString();
		}
	}
	void SpawnBonuse(Vector3 pos) {
		//nothing = 10/health = 5/shield = 3
		//|nothing|health|shield|other|other|etc|
		//|         random^ => bonus is shield  |
		//              v-here ends health
		//012345678901234
		//         ^-here ends nothing
		int nothing  = 20;
		int curSum = nothing;
		for(int i = 0; i < bonuseFlavours; i++) {
			curSum += bonuseProbabilities[i];
		}
		int bonusInd = 0;
		int ran = Random.Range(0, curSum);
		if (ran < nothing) {
			return;
		}
		curSum = nothing;
		for(int i = 0; i < bonuseFlavours; i++) {
			curSum += bonuseProbabilities[i];
			// we have > because we got random = 0, bonuse_probability[0] = 1, sum = 0, sum + 1 = 1, => must be >
			if(curSum > ran) {
				//Debug.Log (ran);
				bonusInd = i;
				break;
			}
			else if(i == bonuseFlavours - 1) {
				Debug.Log ("bonuse choosing algorythm is bugged");
			}
		}
		int bonuse = InstantiateBulletIfFree(bonuses, bonuseNames[bonusInd] + "_bonus", pos);
		if (bonuse >= 0) {
			tempBonuseScr = bonuses[bonuse].GetComponent<bonus_ai>();
			tempBonuseScr.target = alienControlScript.ChooseRandomAlien();
			bonuses[bonuse].gameObject.GetComponent<SpriteRenderer>().sprite = bonuseSprites[bonusInd];
		}
    }
	void CheckDeadsInPool(List<GameObject> pool, int height = 0) {
		for(int i = 0; i < pool.Count; i++) {
			if(pool[i].tag == "killed") {
				IncCount(killedText, killedCore);
				IncCount(deadText, deadCore);
                SpawnBonuse(pool[i].transform.position);
				Deactivate(pool[i], height);
			}
			else if(pool[i].tag == "dead") {
				IncCount(deadText, deadCore);
				Deactivate(pool[i], height);
			}
			else if(pool[i].tag == "out") {
				Deactivate(pool[i], height);
			}
		}
	}
	int InstantiateBulletIfFree(List<GameObject> pool, string tag, Vector3 pos) {
		for(int i = 0; i < pool.Count; i++) {
			if(pool[i].tag == "free") {
				pool[i].transform.position = pos;
				pool[i].SetActive (true);
				pool[i].tag = tag;
				return i;
			}
		}
		return -1;
	}
	int InstantiateIfFree(List<GameObject> pool, string tag, int hp, Vector3 position) {
		for(int i = 0; i < pool.Count; i++) {
			if(pool[i].tag == "free") {
                pool[i].transform.position = position;
				pool[i].SetActive (true);
				pool[i].tag = tag;
                if (tag == "building") {
                    tempBuildingScr = pool[i].GetComponent<building_ai>();
                    tempBuildingScr.hp = hp;
                }
                else {
                    tempTankScr = pool[i].GetComponent<tank_script>();
                    tempTankScr.hp = hp;
                }
				IncCount(totalText, totalCore);
				return i;
			}
		}
		return -1;
	}
	void RepaintAllBuildings(List<GameObject> pool, Sprite sprite) {
		for(int i = 0; i < pool.Count; i++) {
			pool[i].GetComponent<SpriteRenderer>().sprite = buildingUrbanSprite;
		}
	}
	// deals with all counters
    void FixedUpdate() {
		if (Time.timeScale == 0) {
			return;
		}
        if (!buildingLimitReached) {
            counterForAdditionalEnemies++;
            if (counterForAdditionalEnemies > planeLimit) {
                planeLimitReached = true;
            }
            if(counterForAdditionalEnemies > bomberLimit) {
                bomberLimitReached = true;
            }
            if(counterForAdditionalEnemies > buildingLimit) {
                buildingLimitReached = true;
            }
        }
        tankSpawnCounter += Random.Range(-5, 10);
        if (planeLimitReached) {
            planeSpawnCounter += Random.Range(-5, 10);
        }
        if (bomberLimitReached) {
            bomberSpawnCounter += Random.Range(-5, 10);
        }
        if (buildingLimitReached) {
            buildingSpawnCounter += Random.Range(-5, 10);
        }
        if (delay > minDelay) {
            decDelayCounter++;
            if (decDelayCounter > decDelayMax) {
                decDelayCounter = 0;
                delay--;
            }
        }
    }
    // Update is called once per frame
    void LateUpdate() {
        if (Time.timeScale == 0) {
            return;
        }
		if(!urban) {
			if(backgroundAnimators[0].GetCurrentAnimatorStateInfo (0).IsTag ("urban") || 
			   backgroundAnimators[1].GetCurrentAnimatorStateInfo (0).IsTag ("urban")) {
				RepaintAllBuildings (buildings, buildingUrbanSprite);
				urban = true;
			}
		}
        Vector3 tankPos = globalCamera.ViewportToWorldPoint(new Vector3(1f, 0.1f, globalCamera.nearClipPlane));
        Vector3 planePos = globalCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, globalCamera.nearClipPlane));
        Vector3 bomberPos = globalCamera.ViewportToWorldPoint(new Vector3(1f, 0.8f, globalCamera.nearClipPlane));
        Vector3 buildingPos = globalCamera.ViewportToWorldPoint(new Vector3(1f, 0.1f + Random.Range(0.04f, 0.1f) , globalCamera.nearClipPlane));

        tankPos.z = 0;
        planePos.z = 0;
        bomberPos.z = 0;

        CheckDeadsInPool(tanks);
        CheckDeadsInPool(planes);
        CheckDeadsInPool(bombers, 5);
        CheckDeadsInPool(buildings);
        CheckDeadsInPool(bullets);
        CheckDeadsInPool(grenades);
        CheckDeadsInPool(explosions);
        CheckDeadsInPool(bonuses);

        if (tankSpawnCounter >= delay) {
            InstantiateIfFree(tanks, "tank", 100, tankPos);
			tankSpawnCounter = 0;
        }
        if (planeSpawnCounter >= delay) {
            InstantiateIfFree(planes, "plane", 100, planePos);
            planeSpawnCounter = 0;
        }
        if (bomberSpawnCounter >= delay) {
            InstantiateIfFree(bombers, "bomber", 100, bomberPos);
            bomberSpawnCounter = 0;
        }
        if (buildingSpawnCounter >= delay) {
            InstantiateIfFree(buildings, "building", 100, buildingPos);
            buildingSpawnCounter = 0;
        }
    }
}
