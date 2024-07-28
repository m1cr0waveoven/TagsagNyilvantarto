using Caliburn.Micro;
using System;

namespace TagsagNyilvantarto.Models
{
    class TagdijFizetes : PropertyChangedBase
    {
        int _id;
        int _tag_id;
        DateTime _fizetve;

        public int Id { get => _id; set => _ = Set(ref _id, value); }
        public int Tag_id { get => _tag_id; set => _ = Set(ref _tag_id, value); }
        public DateTime Fizetve { get => _fizetve; set => _ = Set(ref _fizetve, value); }
    }
}
