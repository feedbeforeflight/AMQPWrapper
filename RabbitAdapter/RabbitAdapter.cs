using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RabbitAdapter
{
    [Guid("DE08E197-26FA-4A0B-8542-08E7D336BC9E")]

    internal interface IRabbitAdapter

    {

        [DispId(1)]

        void message(string msg);

    }



    [Guid("80B67123-1C81-45F1-9B98-B233C0C6DFBE"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]

    public interface IRabbitAdapterEvents

    {

    }



    [Guid("BEB7F94E-522F-488C-9927-8395AFDF51AC"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IRabbitAdapterEvents))]

    public class RabbitAdapter : IRabbitAdapter

    {

        public void message(string msg)

        {

            MessageBox.Show(msg, "Cообщение компоненты RabbitAdapter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

    }
}
