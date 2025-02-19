using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberDiscordMod.UI
{
    public class MyCustomCell : CustomCellTableCell
    {

        public TextMeshProUGUI label;

        public void Setup(string text)
        {
            if (label == null)
            {
                label = gameObject.AddComponent<TextMeshProUGUI>();
                label.fontSize = 4;
                label.alignment = TextAlignmentOptions.Center;
            }

            label.text = text; // Set the text in the cell
        }

    }
}
