using Caliburn.Micro;
using Dapper.Contrib.Extensions;
using System;
using TagsagNyilvantarto.Interfaces;

namespace TagsagNyilvantarto.Models
{
    [Table("tagokok")]
    internal sealed class Tag : PropertyChangedBase, ITag
    {
        private int _tagId;
        private string _nev = string.Empty;
        private DateTime? _szuletesi_datum;
        private string _email = string.Empty;
        private string _telefon = string.Empty;
        private string _tisztseg = string.Empty;
        private DateTime? _tagsga_kezdete;
        private TagsagAdattipus _adatok_tipusa;
        private bool _admin;
        private bool _kepviselo;
        private TagsagAllapot _tagsag_allapot;

        [Key]
        public int TagId { get => _tagId; set => _ = Set(ref _tagId, value); }
        public string Nev { get => _nev; set => _ = Set(ref _nev, value); }
        public DateTime? SzuletesiDatum { get => _szuletesi_datum; set => _ = Set(ref _szuletesi_datum, value); }
        public string Email { get => _email; set => _ = Set(ref _email, value); }
        public string Telefon { get => _telefon; set => _ = Set(ref _telefon, value); }
        public string Tisztseg { get => _tisztseg; set => _ = Set(ref _tisztseg, value); }
        public DateTime? TagsagKezdete { get => _tagsga_kezdete; set => _ = Set(ref _tagsga_kezdete, value); }
        [Computed]
        public TagsagAdattipus AdatokTipusa { get => _adatok_tipusa; set => _ = Set(ref _adatok_tipusa, value); }
        public bool Admin { get => _admin; set => _ = Set(ref _admin, value); }
        [Computed]
        public TagsagAllapot TagsagAllapot { get => _tagsag_allapot; set => _ = Set(ref _tagsag_allapot, value); }
        public bool Kepviselo { get => _kepviselo; set => _ = Set(ref _kepviselo, value); }
    }
}
