using UnityEngine;

    [CreateAssetMenu(menuName = "DataContainer/DialogueDataConfig")]
    public class DialogueSpriteDataConfig : ScriptableObject
    {
        public Sprite Default;
        public Sprite Angry;
        public Sprite Flustred;
        public Sprite Happy;
        public Sprite Serious;
        public Sprite Surprised;
        public Sprite Pleased;
        public Sprite Yawn;

        public Sprite GetCharacterSprite(eEmotionType emotion)
        {
            switch (emotion)
            {
                case eEmotionType.Angry:
                    return Angry;
                case eEmotionType.Flustred:
                    return Flustred;
                case eEmotionType.Happy:
                    return Happy;
                case eEmotionType.Serious:
                    return Serious;
                case eEmotionType.Surprised:
                    return Surprised;
                case eEmotionType.Pleased:
                    return Pleased;
                case eEmotionType.Yawn:
                    return Yawn;
            }
            return Default;
        }
    }
