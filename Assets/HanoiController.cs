using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class HanoiController : MonoBehaviour
{
    public GameObject[] disks; // disks in size order, higher index, larger disk
    public Vector3[] towers; // towers position: left to right, index 0 to 2
    public Stack<int>[] hanoi;
    // Use this for initialization
    public bool selected = false;
    public int selectedTower;
    void Start()
    {
        //left = new int[3];
        //middle = new int[3];
        //right = new int[3];
        //selectedArray = new int[3];
        hanoi = new Stack<int>[3];
        for(int i = 0; i < 3; i++)
        {
            hanoi[i] = new Stack<int>();
            disks[i].transform.position = new Vector3(towers[0].x, towers[0].y + 2-i, towers[0].z);
            hanoi[0].Push(2-i);
        }
        //for(int i = 0; i < hanoi.Length; i++) { hanoi[i] = []; }
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Move(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Move(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Move(3);
        }
    }

    //first press is to select the tower that want to move disk from
    //second press is to select the tower that want to move disk to
    private void Move(int v)
    {
        var current_tower = v - 1;
        if (!selected)
        {
            selected = true;
            selectedTower = current_tower;
            Debug.Log("Selected Tower " + selectedTower);
        }
        else
        {
            selected = false;
            Debug.Log("Move to Tower " + current_tower);
            //move to same tower
            if (selectedTower == current_tower)
            {
                selectedTower = -1;
                return;
            }
            var preSelectedTowerDiscNumber = hanoi[selectedTower].Count;

            //hanoi logic check
            if (preSelectedTowerDiscNumber > 0) 
            {
                var currentSelectedTowerDiscNumber = hanoi[v - 1].Count;
                if (currentSelectedTowerDiscNumber <= 0)
                {
                    var movedDisck = hanoi[selectedTower].Pop();
                    hanoi[current_tower].Push(movedDisck);
                    disks[movedDisck].transform.position = new Vector3 ( towers[current_tower].x, towers[current_tower].y + currentSelectedTowerDiscNumber, towers[current_tower].z );
                }
                else
                {
                    var botDisc = hanoi[current_tower].Pop();
                    var topDisc = hanoi[selectedTower].Pop();
                    hanoi[current_tower].Push(botDisc);
                    if (topDisc < botDisc)
                    {
                        hanoi[current_tower].Push(topDisc);
                        disks[topDisc].transform.position = new Vector3(towers[current_tower].x, towers[current_tower].y + currentSelectedTowerDiscNumber, towers[current_tower].z);
                    }
                    else
                    {
                        hanoi[selectedTower].Push(topDisc);

                    }
                }
            }
            selectedTower = -1;

        }
    }
}
