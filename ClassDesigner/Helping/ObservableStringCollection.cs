using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public class ObservableStringCollection : IEnumerable, INotifyCollectionChanged
    {
        StringCollection _collection;
        
        public ObservableStringCollection(StringCollection collection)
        {
            _collection = collection;
        }
        public new void Add(string value)
        {
            _collection.Add(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }
         public new void Remove(string value)
        {
            _collection.Remove(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        public IEnumerator GetEnumerator() => new ObservableStringEnumerator(_collection);
    }
    
    public class ObservableStringEnumerator: IEnumerator
    {
        StringCollection strings;
        int position = -1;
        public ObservableStringEnumerator(StringCollection strings) => this.strings = strings;
        public object Current
        {
            get
            {
                if (position == -1 || position >= strings.Count)
                    throw new ArgumentException();
                return strings[position];
            }
        }
        public bool MoveNext()
        {
            if (position < strings.Count - 1)
            {
                position++;
                return true;
            }
            else
                return false;
        }
        public void Reset() => position = -1;
    }
}