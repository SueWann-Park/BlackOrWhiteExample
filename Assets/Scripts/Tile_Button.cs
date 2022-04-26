using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Button : MonoBehaviour
{
    bool isPushed = false;
    Vector3 originalScale;

    public static int numLeft = 3;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (TestMousePosition() == false)
        {
            if (isPushed == true)
            {
                isPushed = false;
                transform.localScale = originalScale;
            }

            return;
        }

        if (Input.GetMouseButtonDown(0) == true)
        {
            isPushed = true;
            transform.localScale = originalScale * 0.95f;
        }
        else if (Input.GetMouseButtonUp(0) == true && isPushed == true)
        {
            isPushed = false;
            transform.localScale = originalScale;

            OnClick();
        }
    }

    public void UnselectTile()
    {
        isSelected = false;
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }

        Quaternion q = transform.rotation;
        q.eulerAngles = Vector3.zero;
        transform.rotation = q;
    }

    bool isSelected = false;
    Coroutine lastRoutine = null;
    public void OnClick()
    {
        string[] data = gameObject.name.Split('_');
        int.TryParse(data[1], out int row);
        int.TryParse(data[2], out int col);

        GameMaster.script.SelectTile(row * 3 + col);

        if (isSelected == false)
        {
            if (numLeft == 0)
                return;

            numLeft--;
            isSelected = true;
            lastRoutine = StartCoroutine(Twiggle());
        }
        else
        {
            numLeft++;
            UnselectTile();
        }
    }

    private IEnumerator Twiggle()
    {
        Quaternion q = transform.rotation;
        for (; ;)
        {
            q.eulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 8) * 3 - 1.5f);
            transform.rotation = q;
            yield return new WaitForFixedUpdate();
        }
    }

    private bool TestMousePosition()
    {
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mp.x > transform.position.x - 2f &&
            mp.x < transform.position.x + 2f &&
            mp.y > transform.position.y - 2f &&
            mp.y < transform.position.y + 2f)
            return true;

        return false;
    }
}