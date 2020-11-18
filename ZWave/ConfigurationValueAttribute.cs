using System;

namespace ZWave
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ConfigurationValueAttribute : Attribute
    {
        public ConfigurationValueAttribute(int value, object discriminator = null)
        {
            Value = value;
            Discriminator = discriminator;
        }

        public int Value { get; }
        public object Discriminator { get; }

        public override string ToString()
        {
            return $"Value: {Value}, Discriminator: {Discriminator}";
        }
    }
}
