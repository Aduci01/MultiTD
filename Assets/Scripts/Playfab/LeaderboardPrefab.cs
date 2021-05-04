using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LeaderboardPrefab : MonoBehaviour {
    public string nickName;
    public int rank;
    public int trophies;

    [SerializeField] TMP_Text nameText, rankText, trophyText;
    [SerializeField] GameObject[] rankImages;

    public void InitData(string name, int rank, int trophies) {
        this.nickName = name;
        this.rank = rank + 1;
        this.trophies = trophies;

        nameText.text = nickName;
        if (name == null) nameText.text = "Player";

        rankText.text = this.rank + "";
        trophyText.text = trophies + "";

        SetRankImage();

        gameObject.SetActive(true);
    }

    void SetRankImage() {
        if (rank < 4) {
            rankImages[rank - 1].SetActive(true);
            rankText.text = "";
        }
    }
}

