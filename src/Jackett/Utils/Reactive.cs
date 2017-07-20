using System;
using System.Collections.Generic;

namespace Jackett.Utils
{
    public class NewValueObserver<T> : IObserver<T>
    {
        public NewValueObserver(Action<T> action)
        {
            this.action = action;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            action(value);
        }

        private Action<T> action;
    }

    public class Unsubscriber<T> : IDisposable where T : Observable<T>
    {
        public Unsubscriber(Observable<T> observable, IObserver<T> observer)
        {
            this.observable = observable;
            this.observer = observer;
        }

        public void Dispose()
        {
            observable.Unsubscribe(observer);
        }

        private Observable<T> observable;
        private IObserver<T> observer;
    }

    public abstract class Observable<T> : IObservable<T> where T : Observable<T>
    {
        public IDisposable Subscribe(Action<T> observer)
        {
            var obs = new NewValueObserver<T>(observer);
            var ret = Subscribe(obs);
            return ret;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var unsub = new Unsubscriber<T>(this, observer);
            observers.Add(observer);

            return unsub;
        }

        public void Unsubscribe(IObserver<T> subscriber)
        {
            observers.Remove(subscriber);
        }

        protected void Notify()
        {
            foreach (var observer in observers)
            {
                observer.OnNext((T)this);
            }
        }

        private ICollection<IObserver<T>> observers = new List<IObserver<T>>();
    }

    public abstract class Observing
    {
        protected void Observe<T>(Observable<T> observable, Action<T> action) where T : Observable<T>
        {
            var disp = observable.Subscribe(action);
            observers.Add(disp);
        }

        private ICollection<IDisposable> observers = new List<IDisposable>();
    }
}
