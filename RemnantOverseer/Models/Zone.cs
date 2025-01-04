using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemnantOverseer.Models;
public class Zone
{
    public string Name { get; set; }
    public string Story { get; set; }
    public List<Location> Locations { get; set; }

    public Zone ShallowCopy()
    {
        return (Zone)MemberwiseClone();
    }
}
