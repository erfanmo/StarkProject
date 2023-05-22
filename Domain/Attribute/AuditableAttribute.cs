using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attribute
{

    [AttributeUsage(AttributeTargets.Class)]
    public class AuditableAttribute : System.Attribute
    {

    }
}
