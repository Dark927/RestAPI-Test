
using System.Collections.Generic;

namespace CharactersDisplay
{
    [System.Serializable]
    public class CharacterApiResponse
    {
        public string url;
        public string name;
        public string gender;
        public string culture;
        public string born;
        public string died;
        public List<string> titles = new List<string>();
        public List<string> aliases = new List<string>();
        public string father;
        public string mother;
        public string spouse;
        public List<string> allegiances = new List<string>();
        public List<string> books = new List<string>();
        public List<string> povBooks = new List<string>();
        public List<string> tvSeries = new List<string>();
        public List<string> playedBy = new List<string>();
    }
}