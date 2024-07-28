using Caliburn.Micro;

namespace TagsagNyilvantarto.Models
{
    internal class FilterValues : PropertyChangedBase
    {
        private string _id;
        private string _nev;
        private string _szuletes;
        private string _email;
        private string _telefon;
        private string _tisztseg;
        private string _tagsagkezdete;
        private string _jogallas;
        private string _adatoktipusa;
        private string _kepviselo;
        private string _admin;


        public string Id { get => _id; set => _ = Set(ref _id, value); }
        public string Nev { get => _nev; set => _ = Set(ref _nev, value); }
        public string Email { get => _email; set => _ = Set(ref _email, value); }
        public string Telefon { get => _telefon; set => _ = Set(ref _telefon, value); }
        public string Tisztseg { get => _tisztseg; set => _ = Set(ref _tisztseg, value); }
        public string Jogallas { get => _jogallas; set => _ = Set(ref _jogallas, value); }
        public string AdatokTipusa { get => _adatoktipusa; set => _ = Set(ref _adatoktipusa, value); }
        public string Kepviselo { get => _kepviselo; set => _ = Set(ref _kepviselo, value); }
        public string Admin { get => _admin; set => _ = Set(ref _admin, value); }
        public string Szuletes { get => _szuletes; set => _ = Set(ref _szuletes, value); }
        public string TagsagKezdete { get => _tagsagkezdete; set => _ = Set(ref _tagsagkezdete, value); }

    }
}
