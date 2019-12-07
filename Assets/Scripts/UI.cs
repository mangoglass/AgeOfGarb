using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text scoreElement;
    [SerializeField] private Text flavourText;

    public void SetScore(int score) 
    {
        scoreElement.text = "Trash: " + score + " Kg";

        if (score >= 140000) {
            flavourText.text = "The weight of dropped trash is more than an adult blue whale!";
        }

        else if (score >= 6350) {
            flavourText.text = "The weight of dropped trash is more than an adult African bush elephant!";
        }

        else if (score >= 2964) {
            flavourText.text = "The weight of dropped trash is more than the average weight of trash one American family generates per year";
        } //https://archive.epa.gov/epawaste/nonhaz/municipal/web/html/

        else if (score >= 1200) {
            flavourText.text = "The weight of dropped trash is more than an Nissan Micra";
        }

        else if (score >= 726) {
            flavourText.text = "The weight of dropped trash is more than the average weight of trash one American person generates per year";
        } //https://archive.epa.gov/epawaste/nonhaz/municipal/web/html/

        else if (score >= 466) {
            flavourText.text = "The weight of dropped trash is more than the average weight of trash one Swedish person generates per year";
        } // https://www.sopor.nu/fakta-om-sopor/statistik/hushaallsavfall/

        else if (score >= 70) {
            flavourText.text = "The weight of dropped trash is more than the average weight of an European adult human";
        } // https://bmcpublichealth.biomedcentral.com/track/pdf/10.1186/1471-2458-12-439

        else if (score >= 39) {
            flavourText.text = "The weight of dropped trash is more than the average weight of trash one Swedish person generates per month";
        } // https://www.sopor.nu/fakta-om-sopor/statistik/hushaallsavfall/

        else if (score >= 13) {
            flavourText.text = "The weight of dropped trash is more than the average weight of trash one American person generates per week";
        } //https://archive.epa.gov/epawaste/nonhaz/municipal/web/html/

        else if (score >= 5) {
            flavourText.text = "The weight of dropped trash is more than a newborn human baby";
        }
    }
}
