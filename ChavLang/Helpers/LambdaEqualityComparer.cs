using System;
using System.Collections.Generic;
using System.Text;

namespace ChavLang.Helpers
{
    public class LambdaEqualityComparer<Tc>
    {
        public static IEqualityComparer<Tc> Create(Func<Tc, Tc, bool> equalityComparer)
        {
            return new Comparer<Tc>(equalityComparer);
        }

        private class Comparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equalsLambda;
            private readonly Func<T, int> _getHashCodeLambda;

            public Comparer(Func<T, T, bool> equalsLambda, Func<T, int> getHashCodeLambda)
            {
                _equalsLambda = equalsLambda;
                _getHashCodeLambda = getHashCodeLambda;
            }

            public Comparer(Func<T, T, bool> equalsLambda) : this(equalsLambda, null)
            {

            }

            public bool Equals(T x, T y)
            {
                return _equalsLambda(x, y);
            }

            public int GetHashCode(T obj)
            {
                if (_getHashCodeLambda == null)
                    return obj.GetHashCode();

                return _getHashCodeLambda(obj);
            }
        }
    }
}
