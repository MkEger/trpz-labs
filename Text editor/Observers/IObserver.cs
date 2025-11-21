using System;

namespace TextEditor.Observers
{
    public interface IObserver
    {
        void Update(string textContent);
    }
}
