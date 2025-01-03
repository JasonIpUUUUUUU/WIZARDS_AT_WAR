using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class edges : MonoBehaviour
{
    [SerializeField]
    private bool golden, redFire, blueFire, electro;

    [SerializeField]
    private GameObject fireEffects, currentBlueFire, currentRedFire, fireParticles, line2, showingLineRed, showingLineBlue, sparkle, electroboom;

    [SerializeField]
    private float redFireCounter, blueFireCounter;

    [SerializeField]
    private int distance;

    [SerializeField]
    private Color electroColor, electroColor2, goldColor, defaultColor;

    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        defaultColor = lr.endColor;
    }

    private void Update()
    {
        if(redFireCounter > 0)
        {
            redFireCounter -= Time.deltaTime;
        }
        else
        {
            Destroy(currentRedFire);
            Destroy(showingLineRed);
            redFire = false;
        }
        if (blueFireCounter > 0)
        {
            blueFireCounter -= Time.deltaTime;
        }
        else
        {
            Destroy(currentBlueFire);
            Destroy(showingLineBlue);
            blueFire = false;
        }
    }

    public void displayParticles()
    {
        ParticleSystem particles = Instantiate(fireEffects, transform).GetComponent<ParticleSystem>();
        Vector3 startPoint = lr.GetPosition(0);
        Vector3 endPoint = lr.GetPosition(1);
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.radius = 0.5f; 
        shape.position = Vector3.zero; 
        shape.scale = new Vector3((endPoint - startPoint).magnitude, 0.1f, 0.5f); // set length of particles to length of the line
        Vector3 direction = endPoint - startPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shape.rotation = new Vector3(0, 360 - angle, 0);
        if(redFire)
        {
            Debug.Log("redFire");
            showingLineRed = Instantiate(line2);
            showingLineRed.transform.position = transform.position;
            LineRenderer showLR = showingLineRed.GetComponent<LineRenderer>();
            showLR.SetPosition(0, startPoint);
            showLR.SetPosition(1, endPoint);
            showLR.SetColors(Color.red, Color.red);
            currentRedFire = particles.gameObject;
            particles.startColor = Color.red;
        }
        else if (blueFire)
        {
            showingLineBlue = Instantiate(line2);
            showingLineBlue.transform.position = transform.position;
            LineRenderer showLR = showingLineRed.GetComponent<LineRenderer>();
            showLR.SetPosition(0, startPoint);
            showLR.SetPosition(1, endPoint);
            showLR.SetColors(Color.blue, Color.blue);
            particles.startColor = Color.blue;
            currentBlueFire = particles.gameObject;
        }
        particles.transform.position = returnMidPoint();
        particles.Play();
    }

    public void setFire(bool team, float fireTime)
    {
        if (team)
        {
            redFire = true;
            redFireCounter = fireTime;
            if (!currentRedFire)
            {
                displayParticles();
            }
        }
        else
        {
            blueFire = true;
            blueFireCounter = fireTime;
            if (!currentBlueFire)
            {
                displayParticles();
            }
        }
    }

    public bool hasFire(bool team)
    {
        if (team)
        {
            return blueFire == true;
        }
        else
        {
            return redFire == true;
        }
    }

    public void turnElectro(bool infinite)
    {
        StartCoroutine(electroCoroutine(infinite));
    }

    private IEnumerator electroCoroutine(bool infinite)
    {
        GameObject b = Instantiate(electroboom);
        b.transform.position = returnMidPoint();
        Destroy(b, 0.5f);
        electro = true;
        lr.SetColors(electroColor, electroColor2);
        if (!infinite)
        {
            yield return new WaitForSeconds(9);
            electro = false;
            lr.SetColors(defaultColor, defaultColor);
        }
    }

    public bool returnElectro()
    {
        return electro;
    }

    public void setDistance(int distanceP)
    {
        distance = distanceP;
    }

    public int returnDistance()
    {
        int tempDist = distance;
        if (electro)
        {
            tempDist /= 2;
        }
        return tempDist;
    }

    public void setLine(Vector2 startPoint, Vector2 endPoint)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }

    public void turnGold()
    {
        StartCoroutine(goldCoroutine());
    }

    private IEnumerator goldCoroutine()
    {
        GameObject b = Instantiate(sparkle);
        b.transform.position = returnMidPoint();
        Destroy(b, 8);
        golden = true;
        lr.SetColors(goldColor, goldColor);
        yield return new WaitForSeconds(8);
        golden = false;
        lr.SetColors(defaultColor, defaultColor);
    }

    public bool returnGold()
    {
        return golden;
    }

    public Vector3 returnMidPoint()
    {
        Vector3 startPoint = lr.GetPosition(0);
        Vector3 endPoint = lr.GetPosition(1);
        return (startPoint + endPoint) / 2;
    }
}
