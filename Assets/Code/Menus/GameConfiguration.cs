using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Data Singleton
public class GameConfiguration : MonoBehaviour
{
    public static GameConfiguration Instance;

    // Parameters
    public float MapScale = 10;
    public float TileScale = 10;
    public float MapWidth = 3;
    public float MapDepth = 3;
    public float Goals = 4;
    public float Moles = 2;
    public float Entities = 2;
    public float Weapons = 6;

    // Sliders
    public GameObject mapScaleSlider;
    public GameObject tileScaleSlider;
    public GameObject mapWidthSlider;
    public GameObject mapDepthSlider;
    public GameObject goalsSlider;
    public GameObject molesSlider;
    public GameObject entitiesSlider;
    public GameObject weaponsSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MapScaleSlider ()
    {
		 MapScale = mapScaleSlider.GetComponent<Slider>().value;
	}

    public void TileScaleSlider ()
    {
		 TileScale = tileScaleSlider.GetComponent<Slider>().value;
	}

    public void MapWidthSlider ()
    {
		 MapWidth = mapWidthSlider.GetComponent<Slider>().value;
	}

    public void MapDepthSlider ()
    {
		 MapDepth = mapDepthSlider.GetComponent<Slider>().value;
	}

    public void GoalsSlider ()
    {
		 Goals = goalsSlider.GetComponent<Slider>().value;
	}

    public void MolesSlider ()
    {
		 Moles = molesSlider.GetComponent<Slider>().value;
	}

    public void EntitiesSlider ()
    {
		 Entities = entitiesSlider.GetComponent<Slider>().value;
	}

    public void WeaponsSlider ()
    {
		 Weapons = weaponsSlider.GetComponent<Slider>().value;
	}
}
