using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class Jabatan : Departemen
    {
        private int _id_jabatan;
        private string _nama_jabatan;
        private decimal _salary;

        public int Id_Jabatan
        {
            get => _id_jabatan;
            set => _id_jabatan = value;
        }

        public string Nama_Jabatan
        {
            get => _nama_jabatan;
            set => _nama_jabatan = value;
        }

        public decimal Salary
        {
            get => _salary;
            set => _salary = value;
        }
    }
}
