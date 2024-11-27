using UnityEngine;
using UnityEngine.UI;

public class UnoUIManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text winnerNameText;

    public void ShowWinPanel(string winnerName)
    {
        winPanel.SetActive(true);
        winnerNameText.text = $"Winner: {winnerName}";
    }

    public void HideWinPanel()
    {
        winPanel.SetActive(false);
    }

    public void OnUnoButtonClicked()
    {
    }
}