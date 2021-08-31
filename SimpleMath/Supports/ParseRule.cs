using System;
using System.Collections.Generic;

namespace SimpleMath.Supports
{
    public abstract class ParseRule<T1, T2>
    {
        private readonly Dictionary<Type, Func<T1, T2>> rules = new();
        protected Func<T1, T2> defaultRule;
        
        public Func<T1, T2> this[Type type]
        {
            get
            {
                if (rules.TryGetValue(type, out var func))
                    return func;
                else if (defaultRule != null)
                    return defaultRule;
                else
                    throw new KeyNotFoundException($"Parse rule for type '{type}' not found.");
            }
        }

        public void Add(Type type, Func<T1, T2> func)
            => rules.Add(type, func);
    }

    public class ParseToString : ParseRule<object, string>
    {
        public ParseToString()
        {
            defaultRule = obj => obj.ToString();

            Add(typeof(string), item => item as string);
        }
    }

    public class ParseFromString : ParseRule<string, object>
    {
        public ParseFromString()
        {
            defaultRule = str => null;

            Add(typeof(string), item => item);
        }
    }
}
