using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameLineTrigger : MonoBehaviour, IBeginDragHandler {
    //Folder string can be passed down through MenuTreeGenerator in separate monobehaviour
    public string Folder = "Testing";
    public SerializationManager.SavePathType PathType = SerializationManager.SavePathType.Streaming;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Dragged downwards
        if(eventData.delta.y < 0)
        {
            //Create data passer for new line scene
            GameObject go = new GameObject();
            LineGamePasser passer = go.AddComponent<LineGamePasser>();
            passer.Setup(GetComponent<LargeCard>().GetCardData(), Folder, PathType);
            SceneManager.LoadScene("LineGame");
        }
    }

    class LineGamePasser : SceneDataPasser
    {
        public SerializationManager.SavePathType PathType = SerializationManager.SavePathType.Streaming;
        public string LanguageFolder = "";
        public string Line = "";
        public bool ReloadCards = true;
        public bool UnloadCards = true;
        public CardManager.Direction Direction = CardManager.Direction.To;

        public void Setup(CardData card, string folder, SerializationManager.SavePathType PathType)
        {
            this.PathType = PathType;
            LanguageFolder = folder;
            
            //To Broken up
            //Direction = CardManager.Direction.To;
            //Line = card.BrokenUpTo;

            //To with phrases
            //Direction = CardManager.Direction.To;
            //Line = card.To;
            
            //From
            Direction = CardManager.Direction.From;
            Line = card.From;
        }

        protected override void DoAfterLoad()
        {
            base.DoAfterLoad();
            LineManager lineM = GameObject.FindObjectOfType<LineManager>();
            lineM.LineString = Line;
            lineM.PathType = PathType;
            lineM.LanguageFolder = LanguageFolder;
            lineM.ReloadCards = ReloadCards;
            lineM.UnloadCards = UnloadCards;
            lineM.Direction = Direction;
        }
    }
}
