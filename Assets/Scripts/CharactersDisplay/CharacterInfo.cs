

namespace CharactersDisplay
{
    [System.Serializable]
    public class CharacterInfo
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Culture { get; set; }

        public CharacterInfo(string name, string gender, string culture)
        {
            Name = name;
            Gender = gender;
            Culture = culture;
        }

        public override string ToString()
        {
            string nameDisplay = string.IsNullOrEmpty(Name) ? "[No name specified]" : Name;
            string genderDisplay = string.IsNullOrEmpty(Gender) ? "[No gender specified]" : Gender;
            string cultureDisplay = string.IsNullOrEmpty(Culture) ? "[No culture specified]" : Culture;

            return $"Character Info:\n" +
                   $"• Name: {nameDisplay}\n" +
                   $"• Gender: {genderDisplay}\n" +
                   $"• Culture: {cultureDisplay}";
        }
    }
}
