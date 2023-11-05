using System;

namespace Atom.Variant
{
    public class VarEventArgs : EventArgs
    {
        public VarEventArgs(string sender, string name, Var newValue, Var oldValue)
        {
            Sender = sender;
            Name = name;
            NewValue = newValue;
            OldValue = oldValue;
        }

        public string Name { get; }
        public Var NewValue { get; }
        public Var OldValue { get; }
        public string Sender { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}