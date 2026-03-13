namespace CahtFramework
{
    using UnityEngine;
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; private set; }

        public ShowIfAttribute(string conditionFieldName) { this.ConditionFieldName = conditionFieldName; }
    }
}