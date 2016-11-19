using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkSchelude.Types
{
    /// <summary>
    /// Класс с динамическими полями. В основном для работы с базой данных.
    /// </summary>
    public class DCustom
    {
        private dynamic Instance = new ExpandoObject();
        public void AddProperty(string name, object value)
        {
            ((IDictionary<string, object>)this.Instance).Add(name, value);
        }
        public int Id { get; set; }
        public dynamic GetProperty(string name)
        {
            if (((IDictionary<string, object>)this.Instance).ContainsKey(name))
                return ((IDictionary<string, object>)this.Instance)[name];
            else
                return null;
        }
        public string[] GetFields()
        {
            return ((IDictionary<string, object>)this.Instance).Select(i => i.Key).Cast<string>().ToArray();
        }
        public object this[string parametr]
        {
            get
            {
                if (parametr.Equals("Id"))
                    return this.Id;
                if (((IDictionary<string, object>)this.Instance).ContainsKey(parametr))
                    return ((IDictionary<string, object>)this.Instance).Single(i => i.Key == parametr).Value;
                else
                    return null;
            }
            set
            {
                if (parametr.Equals("Id"))
                    this.Id = (int)value;
                if (((IDictionary<string, object>)this.Instance).ContainsKey(parametr))
                    ((IDictionary<string, object>)this.Instance)[parametr] = value;
                else
                    AddProperty(parametr, value);
            }
        }
    }
}
