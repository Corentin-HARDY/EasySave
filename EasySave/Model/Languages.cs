using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System;

namespace EasySave.Model
{
    public class Languages
    {


        private List<Phrase> phrases;
        string jsonFilePath = "";

        ChooseLangue currentLangue = ChooseLangue.Fr;

        public Languages()
        {
            // Charger les phrases à partir du fichier JSON
            LoadPhrases();
        }


        public void LoadPhrases()
        {
            // Charger les phrases à partir du fichier JSON
            switch (currentLangue)
            {
                case ChooseLangue.En:
                    if (phrases != null)
                    {
                        phrases.Clear();
                    }
                    jsonFilePath = "../../../Data/en-US.json";
                    break;
                case ChooseLangue.Fr:
                    if (phrases != null)
                    {
                        phrases.Clear();
                    }
                    jsonFilePath = "../../../Data/fr-FR.json";
                    break;
                //default:
                //    Console.WriteLine(GetMessage("Warning7"));
                //    break;
            }


            var jsonContent = File.ReadAllText(jsonFilePath);
            phrases = JsonSerializer.Deserialize<List<Phrase>>(jsonContent);
        }
        public ChooseLangue CurrentLangue { get => currentLangue; set => currentLangue = value; }

        public string GetMessage(string keyword)
        {
            
                // Rechercher la phrase correspondante au mot-clé
                var phrase = phrases.FirstOrDefault(p => p.Keyword == keyword);

                // Si la phrase est trouvée, retourner la valeur de la propriété "sentence", sinon retourner une chaîne vide
                return phrase != null ? phrase.Sentence : string.Empty;
            
        }

    }

}

