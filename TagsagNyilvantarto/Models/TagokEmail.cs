﻿using Caliburn.Micro;

namespace TagsagNyilvantarto.Models
{
    internal sealed class TagokEmail : PropertyChangedBase
    {
        private string _nev;
        private string _email;

        public string Nev { get => _nev; set => _ = Set(ref _nev, value); }
        public string Email { get => _email; set => _ = Set(ref _email, value); }

        public string DisplayFormat { get => this.ToString(); }
        public override string ToString()
        {
            return $"{Nev} - {Email}";
        }
    }
}
