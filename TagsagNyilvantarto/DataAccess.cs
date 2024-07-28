using Caliburn.Micro;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TagsagNyilvantarto.Extensions;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto
{
    internal class DataAccess : PropertyChangedBase
    {
        private IDbConnection _mysqlConnection;
        private string connectionString = String.Empty;
        #region Fields
        private IEnumerable<string> _idk;
        private IEnumerable<string> _nevek;
        private IEnumerable<string> _szuletesek;
        private IEnumerable<string> _emailek;
        private IEnumerable<string> _telefonok;
        private IEnumerable<string> _tisztsegek;
        private IEnumerable<string> _tagsagkezdetek;
        private IEnumerable<string> _tagsagjogallasok;
        private IEnumerable<string> _adattipusok;
        private string[] _kepviselo;
        private string[] _admin;
        #endregion

        #region Properties
        public IEnumerable<string> Idk { get => _idk; set => _ = Set(ref _idk, value); }
        public IEnumerable<string> Nevek { get => _nevek; set => _ = Set(ref _nevek, value); }
        public IEnumerable<string> Szuletesek { get => _szuletesek; set => _ = Set(ref _szuletesek, value); }
        public IEnumerable<string> Emailek { get => _emailek; set => _ = Set(ref _emailek, value); }
        public IEnumerable<string> Telefonok { get => _telefonok; set => _ = Set(ref _telefonok, value); }
        public IEnumerable<string> Tisztsegek { get => _tisztsegek; set => _ = Set(ref _tisztsegek, value); }
        public IEnumerable<string> Tagsagkezdetek { get => _tagsagkezdetek; set => _ = Set(ref _tagsagkezdetek, value); }
        public IEnumerable<string> Tagsagjogallasok { get => _tagsagjogallasok; set => _ = Set(ref _tagsagjogallasok, value); }
        public IEnumerable<string> Adattipusok { get => _adattipusok; set => _ = Set(ref _adattipusok, value); }
        public string[] Kepviselo { get => _kepviselo; set => _ = Set(ref _kepviselo, value); }
        public string[] Admin { get => _admin; set => _ = Set(ref _admin, value); }
        #endregion
        public DataAccess()
        {
            connectionString = $"Server=localhost; database=tagsag; UID=root; convert zero datetime=True; CharSet=utf8";
        }

        [DllImport("User32.dll")]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        private async Task<bool> ExecuteCommandAsync(CommandDefinition command)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                bool success = false;
                try
                {
                    int res = await connection.ExecuteAsync(command).ConfigureAwait(false);
                    success = true;
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _ = MessageBox(new IntPtr(0), ex.Message, "Hiba", 0);
                }

                return success;
            }
        }

        private IDbConnection GetDbConnectionInstance()
        {
            return new MySqlConnection(connectionString);
        }
        private bool ExecuteCommand(CommandDefinition command)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                bool success = false;
                try
                {
                    int res = connection.Execute(command);
                    success = true;
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _ = MessageBox(new IntPtr(0), ex.Message, "Hiba", 0);
                }

                return success;
            }
        }

        private T Get<T>(int id) where T : class
        {
            if (id < 1)
                return null;
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                T objectToReturn = connection.Get<T>(id);
                return objectToReturn;
            }
        }
        private async Task<T> GetAsync<T>(int id) where T : class
        {
            if (id < 1)
                return null;

            using (IDbConnection connection = GetDbConnectionInstance())
            {
                T objectToReturn = await connection.GetAsync<T>(id).ConfigureAwait(false);
                return objectToReturn;
            }
        }

        public async Task<DataTable> FillTagokDTAsync()
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, " +
                    "DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, DATE_FORMAT(Tagdijfizetesek.Fizetve, '%Y.%m.%d.') As Fizetve, tagsag_allapotok.allapot As Jogállás, tagsagi_adattipusok.tipus As AdatokTípusa, kepviselo As Képviselő, " +
                    "admin As Admin FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id INNER JOIN tagsagi_adattipusok ON tagok.adatok_tipusa = tagsagi_adattipusok.id " +
                    "LEFT JOIN (SELECT tag_id, MAX(fizetve) As Fizetve FROM tagdij_fizetesek GROUP BY tag_id) As Tagdijfizetesek ON tagok.tag_id=Tagdijfizetesek.tag_id;";
                DataTable tagokDT = new DataTable();
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, (MySqlConnection)connection))
                {
                    _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
                }
                UpdateFilterLists(tagokDT);
                return tagokDT;
            }
        }
        private void UpdateFilterLists(DataTable tagokDT)
        {
            IEnumerable<DataRow> igenyeEnum = tagokDT.AsEnumerable();
            Idk = tagokDT.GetDistinctValues<int>("Id").Select(i => i.ToString());
            Nevek = tagokDT.GetDistinctValues<string>("Név");
            Szuletesek = tagokDT.GetDistinctValues<string>("Születés");
            Emailek = tagokDT.GetDistinctValues<string>("Email");
            Telefonok = tagokDT.GetDistinctValues<string>("Telefon");
            Tisztsegek = tagokDT.GetDistinctValues<string>("Tisztség");
            Tagsagkezdetek = tagokDT.GetDistinctValues<string>("TagságKezdete");
            Tagsagjogallasok = tagokDT.GetDistinctValues<string>("Jogállás");
            Adattipusok = tagokDT.GetDistinctValues<string>("AdatokTípusa");
            Kepviselo = new string[] { "Képviselő", "Nem képviselő" };
            Admin = new string[] { "Admin", "Nem admin" };
        }
        //public async Task<TagsagAllapot> GetTagsagByTagId(int tagid)
        //{
        //    TagsagAllapot tagsagAllapot;
        //    using (IDbConnection connection = GetDbConnectionInstance())
        //    {
        //        string sql = "SELECT id As Id, allapot As Allapot FROM tagsag_allapotok WHERE id=(SELECT tagsag_allapot FROM tagok WHERE tag_id=@tagid);";
        //        tagsagAllapot = (await connection.QueryAsync(sql, new { tagid }).ConfigureAwait(false)).SingleOrDefault();
        //        return tagsagAllapot;
        //    }
        //}

        public async Task<Tag> GetTag(int Id)
        {
            Tag tag;
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "SELECT tag_id As Tag_id, nev As Nev, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As SzuletesiDatum, email As Email, telefon As Telefon, tisztseg As Tisztseg, DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagsagKezdete, kepviselo As Kepviselo, admin As Admin FROM tagok WHERE tag_id=@Id;";
                tag = await connection.QueryFirstAsync<Tag>(sql, new { Id }).ConfigureAwait(false);
                sql = "SELECT id As Id, allapot As Allapot FROM tagsag_allapotok WHERE id=(SELECT tagsag_allapot FROM tagok WHERE tag_id=@Id);";
                tag.TagsagAllapot = await connection.QueryFirstAsync<TagsagAllapot>(sql, new { Id }).ConfigureAwait(false);
                sql = "SELECT id, tipus FROM tagsagi_adattipusok WHERE id=(SELECT adatok_tipusa FROM tagok WHERE tag_id=@Id);";
                tag.AdatokTipusa = await connection.QueryFirstAsync<TagsagAdattipus>(sql, new { Id }).ConfigureAwait(false);
                return tag;
            }
        }

        public async Task<IEnumerable<TagsagAllapot>> GetAllTagsagAllapot()
        {
            IEnumerable<TagsagAllapot> tagsagallapotok;
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                tagsagallapotok = await connection.GetAllAsync<TagsagAllapot>().ConfigureAwait(false);
                return tagsagallapotok;
            }
        }

        public async Task<IEnumerable<TagsagAdattipus>> GetAllTagsagAdattipus()
        {
            IEnumerable<TagsagAdattipus> tagsagadattipus;
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                tagsagadattipus = await connection.GetAllAsync<TagsagAdattipus>().ConfigureAwait(false);
                return tagsagadattipus;
            }
        }

        //INSERT tag: INSERT INTO `tagok` (`tag_id`, `nev`, `szuletes_datuma`, `email`, `telefon`, `tisztseg`, `tagsag_kezdete`, `adatok_tipusa`, `kepviselo`, `admin`, `tagsag_allapot`) VALUES (NULL, 'Nagy Elemér', '1990.01.29', 'mail@random.com', '06203040506', 'tag', '2020.01.28', '1', '0', '0', '1');

        public async Task<DataTable> LejaroTagsagok(bool lejart)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                int kulonbseg = (lejart) ? 10000 : 9973; //true : false

                string sql = "SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, " +
                    "DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, tagsag_allapotok.allapot As Jogállás, adatok_tipusa As AdatokTípusa, kepviselo As Képviselő, admin As Admin " +
                    "FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id " +
                    "INNER JOIN (SELECT id, tag_id, MAX(fizetve) As utolso_fizetes FROM tagdij_fizetesek GROUP BY tag_id) As utolso_fizetesek " +
                    "ON tagok.tag_id = utolso_fizetesek.tag_id " +
                    "WHERE date(now())-utolso_fizetes >= " + kulonbseg; //Február miatt 9973. 31 napos hónap esetén 9970 a két dátum közti különbség. 10000 vagy nagyobb ha lejárt a tagság
                DataTable tagokDT = new DataTable();
                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, (MySqlConnection)connection))
                {
                    _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
                }
                return tagokDT;
            }
        }

        public async Task<int> InsertTag(Tag tag)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "INSERT INTO tagok (nev, szuletes_datuma, email, telefon, tisztseg, tagsag_kezdete, adatok_tipusa, kepviselo, admin, tagsag_allapot) " +
                    "VALUES (@Nev, @SzuletesiDatum, @Email, @Telefon, @Tisztseg, @TagsagKezdete, @AdatokTipusa, @Kepviselo, @Admin, @TagsagAllapot);";
                object parameters = new
                {
                    tag.Nev,
                    tag.SzuletesiDatum,
                    tag.Email,
                    tag.Telefon,
                    tag.Tisztseg,
                    tag.TagsagKezdete,
                    AdatokTipusa = tag.AdatokTipusa.Id,
                    tag.Kepviselo,
                    tag.Admin,
                    TagsagAllapot = tag.TagsagAllapot.Id
                };
                CommandDefinition command = new CommandDefinition(sql, parameters);
                int res = await connection.ExecuteAsync(command).ConfigureAwait(false);
                //if (res == 1)//Ha sikerült beilleszteni a tagot az adatbázisba
                //{
                //    sql = "SELECT tag_id FROM tagok WHERE nev='@Nev' ORDER BY tag_id DESC LIMIT 1;";
                //    dynamic a = (await connection.QueryAsync<int>(sql, new { tag.Nev }, commandType: CommandType.Text)).SingleOrDefault();//Nem működik, adatbázisabn triggerrel helyettesítve
                //}

                //MySQL tirgger
                //CREATE TRIGGER after_tag_insert AFTER INSERT
                //ON tagok
                //FOR EACH ROW
                //INSERT INTO tagdij_fizetesek(tag_id, fizetve) VALUES(NEW.tag_id, NOW());

                return res;
            }
        }

        public async Task<int> UpdateTag(Tag tag)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "UPDATE tagok SET nev=@Nev, szuletes_datuma=@SzuletesiDatum, email=@Email, telefon=@Telefon, tisztseg=@Tisztseg, tagsag_kezdete=@TagsagKezdete, " +
                    "adatok_tipusa=@AdatokTipusa, kepviselo=@Kepviselo, admin=@Admin, tagsag_allapot=@TagsagAllapota WHERE tag_id=@Id;";
                object parameteres = new
                {
                    tag.Nev,
                    tag.SzuletesiDatum,
                    tag.Email,
                    tag.Telefon,
                    tag.Tisztseg,
                    tag.TagsagKezdete,
                    AdatokTipusa = tag.AdatokTipusa.Id,
                    tag.Kepviselo,
                    tag.Admin,
                    TagsagAllapota = tag.TagsagAllapot.Id,
                    Id = tag.Tag_id
                };
                CommandDefinition command = new CommandDefinition(sql, parameteres, commandType: CommandType.Text);
                int res = await connection.ExecuteAsync(command);
                return res;
            }
        }

        public async Task<IList<string>> TagdijFizetesek(int tagid)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "SELECT DATE_FORMAT(fizetve, '%Y.%m.%d.') FROM tagdij_fizetesek WHERE tag_id=@tagid ORDER BY fizetve;";
                IList<string> datumok = (await connection.QueryAsync<string>(sql, new { tagid })).ToList();
                return datumok;
            }
        }

        public async Task<int> TagdijFizetve(int tagid, string fizetesdatuma)
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "INSERT INTO tagdij_fizetesek (tag_id, fizetve) VALUES (@Tagid, @Datum);";
                object parameteres = new
                {
                    @Tagid = tagid,
                    @Datum = fizetesdatuma
                };
                CommandDefinition command = new CommandDefinition(sql, parameteres, commandType: CommandType.Text);
                int res = await connection.ExecuteAsync(command);
                return res;
            }
        }

        public async Task<int> DeleteTag(int tag_id)
        {
            //tag törlésével tagdij_fizetesek táblából is törlődnek a hozzátartozó sorok
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "DELETE FROM tagok WHERE tag_id=@tag_id;";
                int res = await connection.ExecuteAsync(sql, new { tag_id });
                return res;
            }
        }

        public async Task<List<TagokEmail>> GetAllEmailAddress()
        {
            using (IDbConnection connection = GetDbConnectionInstance())
            {
                string sql = "SELECT nev As Nev, email As Email FROM tagok WHERE email <> \"\";";
                List<TagokEmail> tagokEmails = (await connection.QueryAsync<TagokEmail>(sql)).ToList();
                return tagokEmails;
            }
        }
    }
}
