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

        public Attachment() 
        {
            Modifiers = new();
        }

        public Attachment(
            string Name, 
            string Description, 
            List<IModifier> Modifiers, 
            int Cost, 
            int UnlockLevel, 
            bool IsSelected = false) : base()
        {
            this.Name = Name;
            this.Description = Description;
            this.Modifiers = Modifiers;
            this.Cost = Cost;
            this.UnlockLevel = UnlockLevel;
            this.IsSelected = IsSelected;
        }

        public void AddModifier(IModifier Modifier)
        {
            Modifiers.Add(Modifier);
        }
    }
}
