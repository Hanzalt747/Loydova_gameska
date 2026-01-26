using MVVMPexeso.Model;
using MVVMProject.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.ViewModel
{
    internal class CardViewModel : ViewModelBase
    {
        public Card Model { get; }
        public CardViewModel(Card card)
        {
            Model = card;
        }

        // databindingované vlastnosti:

        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set { _isFlipped = value; OnPropertyChanged(); }
        }

        private bool _isMatched;
        public bool IsMatched
        {
            get => _isMatched;
            set { _isMatched = value; OnPropertyChanged(); }
        }
        public int Id => Model.Id;


    }
}
