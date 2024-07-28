using Caliburn.Micro;
using Dapper.Contrib.Extensions;

namespace TagsagNyilvantarto.Models
{
    [Table("tagsagi_adattipusok")]
    internal sealed class TagsagAdattipus : PropertyChangedBase
    {
        private int _id;
        private string _tipus;

        [Key]
        public int Id { get => _id; set => _ = Set(ref _id, value); }
        public string Tipus { get => _tipus; set => _ = Set(ref _tipus, value); }
    }
}
