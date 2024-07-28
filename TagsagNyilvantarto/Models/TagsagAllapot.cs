using Caliburn.Micro;
using Dapper.Contrib.Extensions;

namespace TagsagNyilvantarto.Models
{
    [Table("tagsag_allapotok")]
    class TagsagAllapot : PropertyChangedBase, ITagsagAllapot
    {
        int id;
        string allapot;

        [Key]
        public int Id { get => id; set => _ = Set(ref id, value); }
        public string Allapot { get => allapot; set => _ = Set(ref allapot, value); }
    }
}
