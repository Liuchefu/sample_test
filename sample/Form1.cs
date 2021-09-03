using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
namespace sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            StringBuilder Setuzoku = new StringBuilder();
            Setuzoku.Append("Server=localhost;");      //接続先のIPアドレスを設定します。
            Setuzoku.Append("Port=5432;");             //接続先のポート番号を設定します。
            Setuzoku.Append("User Id=postgres;");      //DBに接続するためのユーザーIDを設定します。
            Setuzoku.Append("Password=test;");         //DBに接続するためのパスワードを設定します。
            Setuzoku.Append("Database=postgres;");     //接続するデータベース名を設定します。
            string connString = Setuzoku.ToString();
            var buzai_name_list = new List<string>();
            int buzai_syubetsu_num = 0;

            using (var con = new NpgsqlConnection(connString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand(@"SELECT * FROM 部材種別マスター", con))
                {
                    using (var Reader = cmd.ExecuteReader())
                    { //取得処理実施

                        while (Reader.Read())
                        {
                            //取得結果を出力します。
                            Console.WriteLine("{0}", Reader["buzai_syubetsu_sum"]);
                            comboBox1.Items.Add((string)Reader["buzai_syubetsu_sum"]);
                            buzai_name_list.Add((string)Reader["buzai_syubetsu_sum"]);
                        }
                        //comboBox.Textと部材種別マスターidの番号紐づけ
                        buzai_syubetsu_num = buzai_name_list.IndexOf(comboBox1.Text) + 1;
                    }
                }
            }
        }





        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder Setuzoku = new StringBuilder();
            Setuzoku.Append("Server=localhost;");     //接続先のIPアドレスを設定します。
            Setuzoku.Append("Port=5432;");            //接続先のポート番号を設定します。
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
                            buzai_name_list.Add((string)Reader["buzai_syubetsu_sum"]);
                        }
                        buzai_syubetsu_num = buzai_name_list.IndexOf(comboBox1.Text) + 1;
                    }
                }

                string sql = @"SELECT field_title FROM 項目マスター INNER JOIN 部材種別マスター ON 部材種別マスター.id = 項目マスター.buzai_syubetsu_id AND 項目マスター.buzai_syubetsu_id = :buzai_syubetsu_num";
                //string sql = $@"select column_name from information_schema.columns WHERE table_name = ':comboBox1.Text'";

                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    //sql文に変数を埋め込む方法
                    cmd.Parameters.Add(new NpgsqlParameter("buzai_syubetsu_num", buzai_syubetsu_num));
                    //cmd.Parameters.Add(new NpgsqlParameter("comboBox1.Text", comboBox1.Text));

                    using (var dataReader = cmd.ExecuteReader())
                    { //取得処理実施

                        while (dataReader.Read())
                        {
                            Column_name_list.Add((string)dataReader["field_title"]);
                        }

                        dataGridView1.ColumnCount = Column_name_list.Count();

                        for (int i = 0; i < Column_name_list.Count(); i++)
                        {
                            // カラム名を指定
                            dataGridView1.Columns[i].HeaderText = Column_name_list[i];

                        }
                    }
                }

                //コンボボックスのテキストで取得するテーブル変更
                if (comboBox1.Text == "EBPP")
                {
                    sql = @"SELECT * FROM EBPP";
                }
                else if (comboBox1.Text == "ESPP")
                {
                    sql = @"SELECT * FROM ESPP";
                }
                else if (comboBox1.Text == "LPRM")
                {
                    sql = @"SELECT * FROM LPRM";
                }

                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            if (comboBox1.Text == "EBPP" || comboBox1.Text == "ESPP")
                            {
                                Column_list.AddRange(new List<string>() { dataReader["nominal_diameter"].ToString(), dataReader["parts_series"].ToString(), dataReader["SCH"].ToString(), dataReader["material_type"].ToString(), dataReader["seam"].ToString(), dataReader["piping_length"].ToString() });
                            }

                            else if (comboBox1.Text == "LPRM")
                            {
                                Column_list.AddRange(new List<string>() { dataReader["nominal_diameter"].ToString(), dataReader["parts_series"].ToString(), dataReader["SCH"].ToString(), dataReader["material_type"].ToString(), dataReader["small_caliber"].ToString() });
                            }
                        }

                        dataGridView1.ColumnCount = Column_name_list.Count();
                        dataGridView1.RowCount = Column_list.Count() / Column_name_list.Count();

                        int Column_list_num = 0;

                        for (int i = 0; i < Column_list.Count() / Column_name_list.Count(); i++)
                        {
                            for (int j = 0; j < Column_name_list.Count(); j++)
                            {
                                dataGridView1.Rows[i].Cells[j].Value = Column_list[Column_list_num];
                                Column_list_num += 1;
                            }
                        }
                    }
                }
            }
        }


    }
}
