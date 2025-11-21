using System;
using System.Collections.Generic;

namespace TextEditor.Observers
{
    public class TextEventManager : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();
        private string _textContent;

        public string TextContent
        {
            get { return _textContent; }
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                }
            }
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(_textContent);
            }
        }
    }
}
