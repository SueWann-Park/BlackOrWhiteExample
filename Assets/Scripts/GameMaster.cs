using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public static GameMaster script;
    private static int score = 0;

    Transform rt;
    Transform bt;
    int[][][] info;
    Color[] colorTable;

    // invariant: element of blacks should be int[3]
    List<int[]> blacks;
    List<int> selected;

    Text scoreText; 

    // Start is called before the first frame update
    void Start()
    {
        if(script == null)
        {
            script = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    private void Init()
    {
        bt = GameObject.Find("Board").transform;
        rt = GameObject.Find("Resources").transform;
        colorTable = new Color[6] { new Color(0.6f, 0.2f, 0.2f), new Color(0.2f, 0.6f, 0.2f), new Color(0.2f, 0.2f, 0.6f),
                                    new Color(0.8f, 0.4f, 0.4f), new Color(0.4f, 0.8f, 0.4f), new Color(0.4f, 0.4f, 0.8f),};
        SetBoard();
        CalBlacks();
        selected = new List<int>();
        scoreText = GameObject.Find("Canvas/Score/Text").GetComponent<Text>();
        scoreText.text = score.ToString();
    }


    bool sema = false;
    public bool SelectTile(int n)
    {
        if (sema == true)
            return false;

        if(selected.Contains(n) == true)
        {
            selected.Remove(n);
            return true;
        }

        selected.Add(n);
        if (selected.Count != 3)
            return true;

        selected.Sort();
        for(int i = 0; i < blacks.Count; i++)
        {
            if(blacks[i][0] == selected[0] && blacks[i][1] == selected[1] && blacks[i][2] == selected[2])
            {
                blacks.RemoveAt(i);
                StartCoroutine(ResetBoard(1));
                return true;
            }
        }
        StartCoroutine(ResetBoard(-1));
        return true;
    }

    private IEnumerator ResetBoard(int s)
    {
        sema = true;
        yield return new WaitForSeconds(0.5f);
        
        AddScore(s);

        for(int i = 0;  i < selected.Count; i++)
        {
            bt.GetChild(selected[i]).GetComponent<Tile_Button>().UnselectTile();
        }
        selected.Clear();
        Tile_Button.numLeft = 3;

        sema = false;
    }

    public void OnClickWhite()
    {
        if(blacks.Count == 0)
        {
            AddScore(3);
            StartCoroutine(DelayedReload(1));
        }
        else
        {
            AddScore(-1);
        }
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }

    private IEnumerator DelayedReload(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(gameObject.scene.name);
    }

    private void AddScore(int s)
    {
        score += s;
        scoreText.text = score.ToString();
    }

    private void CalBlacks()
    {
        blacks = new List<int[]>();
        for(int i = 0; i < 7; i++)
        {
            for (int j = i+1; j < 8; j++)
            {
                for (int k = j+1; k < 9; k++)
                {
                    int[] a1 = info[i / 3][i % 3];
                    int[] a2 = info[j / 3][j % 3];
                    int[] a3 = info[k / 3][k % 3];

                    bool isBlack = true;
                    for(int z = 0; z < 3; z++)
                    {
                        if ((a1[z] == a2[z] && a2[z] == a3[z]) ||
                            (a1[z] != a2[z] && a2[z] != a3[z] && a3[z] != a1[z]))
                            continue;
                        else
                            isBlack = false;
                    }

                    if(isBlack == true)
                    {
                        blacks.Add(new int[3] { i, j, k });
                        Debug.Log(i.ToString() + " " + j.ToString() + " " + k.ToString());
                    }
                }
            }
        }
    }

    private void SetBoard()
    {
        info = new int[3][][];
        for (int i = 0; i <3; i++)
        {
            info[i] = new int[3][];
            for(int j= 0; j <3; j++)
            {
                info[i][j] = new int[3];
                for(int k = 0; k < 3; k++)
                {
                    info[i][j][k] = Random.Range(0, 3);
                }

                SetTile(i, j);
            }
        }
    }

    private void SetTile(int i, int j)
    {
        Transform tt = Instantiate(rt.GetChild(0).gameObject).transform;
        tt.parent = bt;
        tt.name = "Tile" + "_" + i.ToString() + "_" + j.ToString();
        tt.localPosition = new Vector3(4.1f * i, 4.1f * j);
        tt.GetComponent<SpriteRenderer>().color = colorTable[info[i][j][0]];

        const int childCoff = 1;
        Transform ct = Instantiate(rt.GetChild(info[i][j][1] + childCoff).gameObject).transform;
        ct.parent = tt;
        ct.localPosition = Vector3.zero;

        const int colorCoff = 3;
        ct.GetComponent<SpriteRenderer>().color = colorTable[info[i][j][2] + colorCoff];
    }
}
