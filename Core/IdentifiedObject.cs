namespace CahtFramework
{
    using System;
    using UnityEngine;

    [CreateAssetMenu]
    public class IdentifiedObject : ScriptableObject, ICloneable
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private int    id = -1;
        [SerializeField] private string codeName;
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public         Sprite Icon        => this.icon;
        public         int    ID          => this.id;
        public         string CodeName    => this.codeName;
        public         string DisplayName => this.displayName;
        public virtual string Description => this.description;

        public virtual object Clone() { return Instantiate(this); }
    }
}