using System;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto.Interfaces
{
    internal interface ITag
    {
        TagsagAdattipus AdatokTipusa { get; set; }
        bool Admin { get; set; }
        string Email { get; set; }
        string Nev { get; set; }
        DateTime? SzuletesiDatum { get; set; }
        int TagId { get; set; }
        TagsagAllapot TagsagAllapot { get; set; }
        DateTime? TagsagKezdete { get; set; }
        string Telefon { get; set; }
        string Tisztseg { get; set; }
        bool Kepviselo { get; set; }
    }
}