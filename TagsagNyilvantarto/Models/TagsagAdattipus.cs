using Caliburn.Micro;
using Dapper.Contrib.Extensions;

namespace TagsagNyilvantarto.Models
{
    [Table("tagsagi_adattipusok")]
    class TagsagAdattipus : PropertyChangedBase
    {
        int _id;
        string _tipus;
        [Key]
        public int Id { get => _id; set => _ = Set(ref _id, value); }
        public string Tipus { get => _tipus; set => _ = Set(ref _tipus, value); }
    }
}
