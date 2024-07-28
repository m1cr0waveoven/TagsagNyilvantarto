using Caliburn.Micro;
using Dapper.Contrib.Extensions;
using TagsagNyilvantarto.Interfaces;

namespace TagsagNyilvantarto.Models;

[Table("tagsag_allapotok")]
internal sealed class TagsagAllapot : PropertyChangedBase, ITagsagAllapot
{
    private int _id;
    private string _allapot;

    [Key]
    public int Id { get => _id; set => _ = Set(ref _id, value); }
    public string Allapot { get => _allapot; set => _ = Set(ref _allapot, value); }
}
