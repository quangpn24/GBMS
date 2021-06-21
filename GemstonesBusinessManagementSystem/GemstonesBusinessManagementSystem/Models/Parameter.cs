using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Parameter
    {
        private int idParameter;
        private string parameterName;
        private string value;

        public int IdParameter { get => idParameter; set => idParameter = value; }
        public string ParameterName { get => parameterName; set => parameterName = value; }
        public string Value { get => value; set => this.value = value; }
        public Parameter(int id, string name, string value)
        {
            this.idParameter = id;
            this.parameterName = name;
            this.value = value;
        }
        public Parameter()
        {

        }
    }
}
