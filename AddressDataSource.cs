using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebYandexMaps
{
    public class AddressDataSource
    {        
        public string Address { get; set; }

        public AddressDataSource(string Address = "")
        {
            this.Address = Address;
        }
    }
}
