using Caliburn.Micro;
using Dapper.Contrib.Extensions;
using System;

namespace TagsagNyilvantarto.Models
{
    [Table("tagokok")]
    class Tag : PropertyChangedBase, ITag
    {
        int tag_id;
        string nev = string.Empty;
        DateTime? szuletesi_datum;
        string email=string.Empty;
        string telefon = string.Empty;
        string tisztseg = string.Empty;
        DateTime? tagsga_kezdete;
        TagsagAdattipus adatok_tipusa;
        bool admin;
        bool kepviselo;
        TagsagAllapot tagsag_allapot;
        //DateTime _tagdij_fizetve;

        [Key]
        public int Tag_id { get => tag_id; set => _ = Set(ref tag_id, value); }
        public string Nev { get => nev; set => _ = Set(ref nev, value); }
        public DateTime? SzuletesiDatum { get => szuletesi_datum; set => _ = Set(ref szuletesi_datum, value); }
        public string Email { get => email; set => _ = Set(ref email, value); }
        public string Telefon { get => telefon; set => _ = Set(ref telefon, value); }
        public string Tisztseg { get => tisztseg; set => _ = Set(ref tisztseg, value); }
        public DateTime? TagsagKezdete { get => tagsga_kezdete; set => _ = Set(ref tagsga_kezdete, value); }
        [Computed]
        public TagsagAdattipus AdatokTipusa { get => adatok_tipusa; set => _ = Set(ref adatok_tipusa, value); }
        public bool Admin { get => admin; set => _ = Set(ref admin, value); }
        [Computed]
        public TagsagAllapot TagsagAllapot { get => tagsag_allapot; set => _ = Set(ref tagsag_allapot, value); }
        public bool Kepviselo { get => kepviselo; set => _ = Set(ref kepviselo, value); }
        //public DateTime TagdijFizetve { get => _tagdij_fizetve; set => _ = Set(ref _tagdij_fizetve, value); }
    }
}
