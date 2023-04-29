using LlamAcademy.Guns.Modifiers;
using System.Collections.Generic;

namespace LlamAcademy.Guns
{
    public class Attachment 
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<IModifier> Modifiers { get; set; }
        public int Cost { get; set; }
        public int UnlockLevel { get; set; }

        public bool IsSelected { get; set; }

        public Attachment() { }

        public Attachment(string name, string description, List<IModifier> modifiers, int cost, int unlockLevel)
        {
            Name = name;
            Description = description;
            Modifiers = modifiers;
            Cost = cost;
            UnlockLevel = unlockLevel;
        }

        public void AddModifier(IModifier Modifier)
        {
            Modifiers.Add(Modifier);
        }
    }
}
