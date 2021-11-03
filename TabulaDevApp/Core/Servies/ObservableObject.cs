using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TabulaDevApp.Core.Servies
{
    public class ObservableObject : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void Dispose() { }
    }
}
