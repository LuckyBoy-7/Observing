using System.Collections;

namespace Lucky.Framework
{
    public class SwapImmediately : IEnumerator
    {
        public IEnumerator Inner;
        public object Current => Inner.Current;

        public SwapImmediately(IEnumerator inner)
        {
            Inner = inner;
        }

        public bool MoveNext()
        {
            return Inner.MoveNext();
        }

        public void Reset()
        {
            Inner.Reset();
        }

    }
}