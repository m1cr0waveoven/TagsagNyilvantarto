using System;

namespace TagsagNyilvantarto.Models
{
    internal interface ITag
    {
        TagsagAdattipus AdatokTipusa { get; set; }
        bool Admin { get; set; }
        string Email { get; set; }
        string Nev { get; set; }
        DateTime? SzuletesiDatum { get; set; }
        int Tag_id { get; set; }
        TagsagAllapot TagsagAllapot { get; set; }
        DateTime? TagsagKezdete { get; set; }
        string Telefon { get; set; }
        string Tisztseg { get; set; }
        bool Kepviselo { get; set; }
    }
}