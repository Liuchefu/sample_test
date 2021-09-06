using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;  //C#とポスグレを接続するため NuGetパッケージ管理からDLしてください。


namespace sample
{

    public partial class Init : Form
    {
        public Init()
        {
            InitializeComponent();
        }

        //初期設定---コンボボックスに部材種別マスターの部材種別名を格納する
        private void Init_Load(object sender, EventArgs e)
        {
            StringBuilder Setuzoku = new StringBuilder();
            Setuzoku.Append("Server=localhost;");      //接続先のIPアドレスを設定します。
            Setuzoku.Append("Port=5432;");             //接続先のポート番号を設定します。
            Setuzoku.Append("User Id=postgres;");      //DBに接続するためのユーザーIDを設定します。
            Setuzoku.Append("Password=test;");         //DBに接続するためのパスワードを設定します。
            Setuzoku.Append("Database=postgres;");     //接続するデータベース名を設定します。
            string connString = Setuzoku.ToString();
            var buzai_name_list = new List<string>();
            int buzai_syubetsu_num;

            using (var con = new NpgsqlConnection(connString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(@"SELECT * FROM 部材種別マスター", con))
                {
                    using (var Reader = cmd.ExecuteReader())
                    { //取得処理実施

                        while (Reader.Read())
                        {
                            //取得結果をコンボボックスと部材名リストに格納する
                            comboBox1.Items.Add(Reader["buzai_syubetsu_sum"].ToString());
                            buzai_name_list.Add(Reader["buzai_syubetsu_sum"].ToString());
                        }
                        //comboBox.Textと部材種別マスターidの番号紐づけ
                        buzai_syubetsu_num = buzai_name_list.IndexOf(comboBox1.GetValue()) + 1;
                    }
                }
                con.Close();
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder Setuzoku = new StringBuilder();
            Setuzoku.Append("Server=localhost;");      //接続先のIPアドレスを設定します。
            Setuzoku.Append("Port=5432;");             //接続先のポート番号を設定します。
            Setuzoku.Append("User Id=postgres;");      //DBに接続するためのユーザーIDを設定します。
            Setuzoku.Append("Password=test;");         //DBに接続するためのパスワードを設定します。
            Setuzoku.Append("Database=postgres;");     //接続するデータベース名を設定します。

            string connString = Setuzoku.ToString();
            var buzai_name_list = new List<string>();
            var Column_list = new List<string>();
            var Column_name_list = new List<string>();
            int buzai_syubetsu_num = 0;

            using (var con = new NpgsqlConnection(connString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(@"SELECT buzai_syubetsu_sum FROM 部材種別マスター", con))
                {
                    using (var Reader = cmd.ExecuteReader())
                    { //取得処理実施

                        while (Reader.Read())
                        {
                            buzai_name_list.Add(Reader["buzai_syubetsu_sum"].ToString());
                        }
                        buzai_syubetsu_num = buzai_name_list.IndexOf(comboBox1.GetValue()) + 1;
                    }
                }

                //string sql = @"SELECT field_title FROM 項目マスター INNER JOIN 部材種別マスター ON 部材種別マスター.id = 項目マスター.buzai_syubetsu_id AND 項目マスター.buzai_syubetsu_id = :buzai_syubetsu_num";
                string sql = $"SELECT field_title FROM 項目マスター INNER JOIN 部材種別マスター ON 部材種別マスター.id = 項目マスター.buzai_syubetsu_id AND 項目マスター.buzai_syubetsu_id = {buzai_syubetsu_num}";

                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    //sql文に変数を埋め込む方法
                    //cmd.Parameters.Add(new NpgsqlParameter("buzai_syubetsu_num", buzai_syubetsu_num));
                    using (var dataReader = cmd.ExecuteReader())
                    { //取得処理実施

                        while (dataReader.Read())
                        {
                            Column_name_list.Add(dataReader["field_title"].ToString());
                        }

                        dataGridView1.ColumnCount = Column_name_list.Count();

                        for (int i = 0; i < Column_name_list.Count(); i++)
                        {
                            // カラム名を指定
                            dataGridView1.Columns[i].HeaderText = Column_name_list[i];

                        }
                    }
                }

                //テーブルのカラム名をリストに格納する
                var colum_name_lists = new List<string>();
                sql = $"select column_name from information_schema.columns WHERE table_name = '{comboBox1.GetValue().ToLower()}' ORDER BY ordinal_position";
                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    { //取得処理実施
                        while (dataReader.Read())
                        {
                            colum_name_lists.AddRange(new List<string>() { (dataReader["column_name"].ToString()) });
                        }
                    }                    
                }

                sql = $"SELECT * FROM {comboBox1.GetValue()}";
                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {

                        while (dataReader.Read())
                        {
                            foreach(string i in colum_name_lists )
                            {
                                Column_list.AddRange(new List<string>() { dataReader[i].ToString() });
                            }
                        }

                        dataGridView1.ColumnCount = Column_name_list.Count();
                        dataGridView1.RowCount = Column_list.Count() / Column_name_list.Count();
                        int table_records_number = Column_list.Count() / Column_name_list.Count();
                        int column_list_num = 0;

                        for (int i = 0; i < table_records_number; i++)
                        {
                            for (int j = 0; j < Column_name_list.Count(); j++)
                            {
                                dataGridView1.Rows[i].Cells[j].Value = Column_list[column_list_num];
                                column_list_num += 1;
                            }
                        }
                    }
                }
                con.Close();
            }   
        }
        
    }


    //拡張メソッド---コンボボックスからテキスト取得を優先する
    //この設定を行わないと、前のテキストを取ってくるかもしれない
    public static class ComboBoxExtention
    {
        public static string GetValue(this ComboBox cmb)
        {
            return cmb.SelectedItem?.ToString() ?? cmb.Text;
        }

    }
}
