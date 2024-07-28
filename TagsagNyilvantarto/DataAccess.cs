using Caliburn.Micro;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagsagNyilvantarto.Extensions;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto
{
    internal sealed class DataAccess : PropertyChangedBase
    {
        private readonly string _connectionString;
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
            _connectionString = $"Server=localhost; database=tagsag; UID=root; convert zero datetime=True; CharSet=utf8";
        }

        private async Task<int> ExecuteCommandAsync(CommandDefinition command)
        {
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    return await connection.ExecuteAsync(command).ConfigureAwait(false);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
        private int ExecuteCommand(CommandDefinition command)
        {
            using (IDbConnection connection = CreateConnection())
            {
                try
                {
                    return connection.Execute(command);
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private T GetById<T>(int id) where T : class
        {
            if (id < 1)
                return null;

            using (IDbConnection connection = CreateConnection())
            {
                return connection.Get<T>(id);
            }
        }

        private async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            if (id < 1)
                return null;

            using (IDbConnection connection = CreateConnection())
            {
                T objectToReturn = await connection.GetAsync<T>(id).ConfigureAwait(false);
                return objectToReturn;
            }
        }

        public async Task<DataTable> FillTagokDataTableAsync()
        {
            using (IDbConnection connection = CreateConnection())
            {
                //string sql = "SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, " +
                //    "DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, DATE_FORMAT(Tagdijfizetesek.Fizetve, '%Y.%m.%d.') As Fizetve, tagsag_allapotok.allapot As Jogállás, tagsagi_adattipusok.tipus As AdatokTípusa, kepviselo As Képviselő, " +
                //    "admin As Admin FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id INNER JOIN tagsagi_adattipusok ON tagok.adatok_tipusa = tagsagi_adattipusok.id " +
                //    "LEFT JOIN (SELECT tag_id, MAX(fizetve) As Fizetve FROM tagdij_fizetesek GROUP BY tag_id) As Tagdijfizetesek ON tagok.tag_id=Tagdijfizetesek.tag_id;";

                var queryBuilder = new StringBuilder()
                   .Append("SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, ")
                   .Append("DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, DATE_FORMAT(Tagdijfizetesek.Fizetve, '%Y.%m.%d.') As Fizetve, ")
                   .Append("tagsag_allapotok.allapot As Jogállás, tagsagi_adattipusok.tipus As AdatokTípusa, kepviselo As Képviselő, ")
                   .Append("admin As Admin FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id INNER JOIN tagsagi_adattipusok ON tagok.adatok_tipusa = tagsagi_adattipusok.id ")
                   .Append("LEFT JOIN (SELECT tag_id, MAX(fizetve) As Fizetve FROM tagdij_fizetesek GROUP BY tag_id) As Tagdijfizetesek ON tagok.tag_id=Tagdijfizetesek.tag_id;");

                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(queryBuilder.ToString(), (MySqlConnection)connection))
                {
                    var tagokDT = new DataTable();
                    _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
                    UpdateFilterLists(tagokDT);
                    return tagokDT;
                }
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

        public async Task<Tag> GetTagByIdAsync(int Id)
        {
            Tag tag;
            using (IDbConnection connection = CreateConnection())
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

        public async Task<IEnumerable<TagsagAllapot>> GetAllTagsagAllapotAsync()
        {
            IEnumerable<TagsagAllapot> tagsagallapotok;
            using (IDbConnection connection = CreateConnection())
            {
                tagsagallapotok = await connection.GetAllAsync<TagsagAllapot>().ConfigureAwait(false);
                return tagsagallapotok;
            }
        }

        public async Task<IEnumerable<TagsagAdattipus>> GetAllTagsagAdattipusAsync()
        {
            IEnumerable<TagsagAdattipus> tagsagadattipus;
            using (IDbConnection connection = CreateConnection())
            {
                tagsagadattipus = await connection.GetAllAsync<TagsagAdattipus>().ConfigureAwait(false);
                return tagsagadattipus;
            }
        }

        public async Task<DataTable> GetLejaroTagsagokAsync(bool lejart)
        {
            using (IDbConnection connection = CreateConnection())
            {
                int kulonbseg = (lejart) ? 10000 : 9973; //true : false

                var queryBuilder = new StringBuilder()
                   .Append("SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, ")
                   .Append("DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, tagsag_allapotok.allapot As Jogállás, adatok_tipusa As AdatokTípusa, kepviselo As Képviselő, admin As Admin ")
                   .Append("FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id ")
                   .Append("INNER JOIN (SELECT id, tag_id, MAX(fizetve) As utolso_fizetes FROM tagdij_fizetesek GROUP BY tag_id) As utolso_fizetesek ")
                   .Append("ON tagok.tag_id = utolso_fizetesek.tag_id ")
                   .Append("WHERE date(now())-utolso_fizetes >= ")
                   .Append(kulonbseg); // Február miatt 9973. 31 napos hónap esetén 9970 a két dátum közti különbség. 10000 vagy nagyobb, ha lejárt a tagság

                using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(queryBuilder.ToString(), (MySqlConnection)connection))
                {
                    var tagokDT = new DataTable();
                    _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
                    return tagokDT;
                }
            }
        }

        public async Task<int> InsertTag(Tag tag)
        {
            using (IDbConnection connection = CreateConnection())
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
                //if (res == 1) // Ha sikerült beilleszteni a tagot az adatbázisba
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
            using (IDbConnection connection = CreateConnection())
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
                    Id = tag.TagId
                };
                CommandDefinition command = new CommandDefinition(sql, parameteres, commandType: CommandType.Text);
                int res = await connection.ExecuteAsync(command).ConfigureAwait(false);
                return res;
            }
        }

        public async Task<IList<string>> GetTagdijFizetesekAsync(int tagid)
        {
            using (IDbConnection connection = CreateConnection())
            {
                string sql = "SELECT DATE_FORMAT(fizetve, '%Y.%m.%d.') FROM tagdij_fizetesek WHERE tag_id=@tagid ORDER BY fizetve;";
                IList<string> datumok = (await connection.QueryAsync<string>(sql, new { tagid }).ConfigureAwait(false)).ToList();
                return datumok;
            }
        }

        public async Task<int> InsertTagdijFizetesAsync(int tagId, string fizetesDatuma)
        {
            using (IDbConnection connection = CreateConnection())
            {
                string sql = "INSERT INTO tagdij_fizetesek (tag_id, fizetve) VALUES (@Tagid, @Datum);";
                object parameteres = new
                {
                    @Tagid = tagId,
                    @Datum = fizetesDatuma
                };
                CommandDefinition command = new CommandDefinition(sql, parameteres, commandType: CommandType.Text);
                int res = await connection.ExecuteAsync(command).ConfigureAwait(false);
                return res;
            }
        }

        public async Task<int> DeleteTagAsync(int tag_id)
        {
            //tag törlésével tagdij_fizetesek táblából is törlődnek a hozzátartozó sorok
            using (IDbConnection connection = CreateConnection())
            {
                string sql = "DELETE FROM tagok WHERE tag_id=@tag_id;";
                int res = await connection.ExecuteAsync(sql, new
                {
                    tag_id
                }).ConfigureAwait(false);
                return res;
            }
        }

        public async Task<IEnumerable<TagokEmail>> GetAllEmailAddressAsync()
        {
            using (IDbConnection connection = CreateConnection())
            {
                string sql = "SELECT nev As Nev, email As Email FROM tagok WHERE email <> \"\";";
                return await connection.QueryAsync<TagokEmail>(sql).ConfigureAwait(false);
            }
        }
    }
}
